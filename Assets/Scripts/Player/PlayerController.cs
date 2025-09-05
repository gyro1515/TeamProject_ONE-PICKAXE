using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    Normal,
    Dashing,
    Hanging,
    Dead
}

public class PlayerController : BaseController
{
    [Header("Player State")]
    [SerializeField] private PlayerState currentState = PlayerState.Normal; // 현재 플레이어 상태

    [Header("Ground Check Settings")]
    [SerializeField] private float RayDistance = 1f;
    [SerializeField] private LayerMask GroundLayer;

    [Header("Dash Settings")]
    [SerializeField] private float dashRange = 10f; // 대쉬 가능 범위
    [SerializeField] private float dashSpeed = 20f; // 대쉬 속도
    [SerializeField] private float dashLineOffsetRadius = 0.5f; // 대쉬 라인 시작 지점 반경
    [SerializeField] private LayerMask obstacleLayerMask; // 장애물 레이어 (지형: Cave)
    [SerializeField] private Color dashLineColor = new Color(1, 1, 1, 0.8f); // 대쉬 라인 색상 및 투명도
    [SerializeField] private LineRenderer dashLineRenderer; // 대쉬 경로를 표시할 LineRenderer
    [SerializeField] private GameObject dashVFXPrefab; // 대쉬 시작 시 생성될 VFX 프리팹

    [Header("Hanging Settings")]
    [SerializeField] private GameObject hangJumpVFXPrefab; // 매달리기 점프 VFX 프리팹

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 2f; // 무적 지속 시간
    [SerializeField] private Color damageColor = new Color(1f, 0.6f, 0.6f, 1f);
    private bool isInvincible = false; // 현재 무적 상태인지 확인

    [Header("Sound Settings")]
    [SerializeField] private float footstepInterval = 0.4f; // 발소리 재생 간격
    private float footstepTimer; // 발소리 타이머
    private bool wasGrounded; // 이전 프레임의 지면 접지 상태를 저장

    private EquippedPickaxeController equippedPickaxe; // 장착된 곡괭이 참조
    private ThrownPickaxeController stuckPickaxe; // 현재 박혀있는 곡괭이 참조
    private bool isDashAvailable = false; // 대쉬가 가능한 상태인지
    private float originalGravityScale; // 원래 중력 값을 저장할 변수

    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer equippedPickaxeSpriteRenderer;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = true;
    float horizontalInput = 0f;
    private int jumpCount = 0; // Jump 관련
    bool isPressedJumpButton = false;

    private PlayerAnimationData playerAnimationData;

    Player player;
    public PlayerInput PlayerInput { get; private set; }
    public PlayerInput.PlayerActions PlayerActions { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        PlayerInput = new PlayerInput();
        PlayerActions = PlayerInput.Player;
        PlayerActions.Jump.started += OnJump;
        PlayerActions.Jump.canceled += OnJumpCanceled;
        PlayerActions.OpenPauseMenu.started += OnOpenPauseMenu;
        PlayerActions.Dash.started += OnDash;
        rb = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerAnimationData = GetComponent<PlayerAnimationData>();
        equippedPickaxe = GetComponentInChildren<EquippedPickaxeController>();
        equippedPickaxeSpriteRenderer = equippedPickaxe.GetComponentInChildren<SpriteRenderer>();

        originalGravityScale = rb.gravityScale; // 기존 중력 값 저장
        dashLineRenderer.enabled = false; // 시작 시 라인 끄기
    }

    protected override void Start()
    {
        base.Start();
        
    }

    // Update에서 대쉬 가능 여부 체크
    protected override void Update()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                // 일반 상태일 때만 대쉬 관련 로직 처리
                FindStuckPickaxe();
                CheckDashAvailability();
                UpdateDashVisuals();
                //CheckJumpPressedDuration();
                break;
            case PlayerState.Hanging:
                // 매달리기 상태일 때의 입력 처리
                HandleHangingInput();
                //CheckJumpPressedDuration();
                break;
            case PlayerState.Dashing:
                // 대쉬 중에는 입력을 받지 않음
                break;
        }
    }

    protected override void FixedUpdate()
    {
        if (currentState == PlayerState.Normal)
        {
            base.FixedUpdate();

            isGrounded = CheckGround();
        }
    }

    private void OnEnable()
    {
        PlayerInput.Enable();
    }

    private void OnDisable()
    {
        PlayerActions.Jump.started -= OnJump;
        PlayerActions.Jump.canceled -= OnJumpCanceled;
        PlayerActions.OpenPauseMenu.started -= OnOpenPauseMenu;
        PlayerActions.Dash.started -= OnDash;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log($"OnJump, JumpCnt: {jumpCount}");
        if (currentState == PlayerState.Dashing) return;
        if (jumpCount == 0 || isPressedJumpButton) return;
        isPressedJumpButton = true;
        
        Jump();

        if (player.JumpSFX)
        {
            SoundManager.PlayClip(player.JumpSFX);
        }
    }

    void OnJumpCanceled(InputAction.CallbackContext context)
    {
        Debug.Log($"OnJumpCanceled, JumpCnt: {jumpCount}");
        if (!isPressedJumpButton) return;
        isPressedJumpButton = false;
        if (currentState == PlayerState.Dashing) return; // 대시 중이라면 리턴
        // 떨어질 때만 중력 없애기
        if (rb.velocity.y < 0f) return;
        rb.totalForce = Vector2.zero;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
    }

    void OnOpenPauseMenu(InputAction.CallbackContext context)
    {
        player.UIPause.OpenUI();
    }
    
    void Jump()
    {
        if (jumpCount == 0)
            return;

        rb.velocity = new Vector2(rb.velocity.x, 0f);

        // 바로 최대 높이로 점프 -> 단, 점프키 캔슬시 바로 떨어지도록 세팅
        rb.AddForce(Vector2.up * player.MaxJumpForce, ForceMode2D.Impulse);

        switch (currentState)
        {
            case PlayerState.Normal:
                //Debug.Log($"NormalJump, PressTime: {pressDurationTimer}, Force: {tmpForce}");
                // 이펙트
                break;
            case PlayerState.Hanging:
                ExitHangingState();
                //Debug.Log($"HangingJump, PressTime: {pressDurationTimer}, Force: {tmpForce}");
                if (hangJumpVFXPrefab != null)
                    Instantiate(hangJumpVFXPrefab, transform.position, Quaternion.identity);
                break;
        }
        jumpCount = 0;
        isGrounded = false;
    }
    protected override void Move()
    {
        base.Move();

        horizontalInput = PlayerActions.Move.ReadValue<float>();

        // horizontalInput의 절대값이 0보다 크면(입력이 있으면) true 아니면 false 전달
        animator.SetBool(playerAnimationData.RunParameterHash, Mathf.Abs(horizontalInput) > 0);

        rb.velocity = new Vector2(horizontalInput * player.MoveSpeed, rb.velocity.y);

        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    public void FlipTowardsMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 마우스가 플레이어의 오른쪽에 있다면
        if (mousePos.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        // 마우스가 플레이어의 왼쪽에 있다면
        else if (mousePos.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        // 현재 방향 반전
        isFacingRight = !isFacingRight;

        // 플레이어 localScale의 x 값을 반전 -> 방향 전환
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool CheckGround()
    {
        if (isGrounded == true || rb.velocity.y > 0) return isGrounded = false; // 낙하 중일때만 레이캐스트 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, RayDistance, GroundLayer);

        // 레이캐스트가 어떤 물체와 충돌했는지 확인
        if (hit.collider != null)
        {
            isGrounded = true;
            jumpCount = 1;
        }
        else
        {
            isGrounded = false;
        }

        return isGrounded;
    }

    // 레이캐스트 디버깅
    private void OnDrawGizmos()
    {
        // Gizmos 색상 설정
        // 땅에 닿아있으면 녹색 아니면 빨간색으로 표시
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // 플레이어의 위치에서 아래로 레이캐스트의 거리를 시각적으로 그림
        Gizmos.DrawRay(transform.position, Vector2.down * RayDistance);
    }

    public void SetPlayerInput(bool active)
    {
        if (active) PlayerInput.Enable();
        else PlayerInput.Disable();
    }

    public override void TakeDamage(int damage)
    {
        // 무적 상태이거나 이미 죽었다면 데미지를 받지 않음
        if (isInvincible || player.IsDead)
        {
            return;
        }

        if (player.HitSFX)
        {
            SoundManager.PlayClip(player.HitSFX);
        }

        player.CurrentHP -= damage;

        if (player.CurrentHP <= 0)
        {
            Dead();
        }
        else
        {
            // 아직 살아있다면 무적 코루틴 시작
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float elapsedTime = 0f;
        Color playerOriginalColor = playerSpriteRenderer.color;
        Color pickaxeOriginalColor = equippedPickaxeSpriteRenderer.color;

        // 무적 시간 동안 깜빡임
        while (elapsedTime < invincibilityDuration)
        {
            // 알파 값을 0.5 (반투명)와 1 (불투명) 사이에서 반복
            playerSpriteRenderer.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0.5f);
            equippedPickaxeSpriteRenderer.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0.5f);
            yield return new WaitForSeconds(0.1f);

            playerSpriteRenderer.color = new Color(damageColor.r, damageColor.g, damageColor.b, 1f);
            equippedPickaxeSpriteRenderer.color = new Color(damageColor.r, damageColor.g, damageColor.b, 1f);
            yield return new WaitForSeconds(0.1f);

            elapsedTime += 0.2f;
        }

        // 무적이 끝나면 원래 색상으로 복구하고 무적 상태 해제
        playerSpriteRenderer.color = playerOriginalColor;
        equippedPickaxeSpriteRenderer.color = pickaxeOriginalColor;

        isInvincible = false;
    }

    protected override void Dead()
    {
        // currentState를 Dead로 변경하여 다른 행동(이동, 점프 등)을 못하게 막음
        if (currentState == PlayerState.Dead)
        {
            return; // 중복 실행 방지
        }

        currentState = PlayerState.Dead;
        base.Dead();

        if (player.DeathSFX)
        {
            SoundManager.PlayClip(player.DeathSFX);
        }

        // 플레이어 입력 비활성화?
        SetPlayerInput(false);

        // 장착 곡괭이 비활성화
        equippedPickaxe.gameObject.SetActive(false);

        // 사망 애니메이션 재생
        animator.SetBool(playerAnimationData.DieParameterHash, true);

        // 사망 처리 코루틴 시작
        StartCoroutine(DeadCoroutine());
    }

    private IEnumerator DeadCoroutine()
    {
        // 애니메이터가 상태 전환할 시간을 주기 위해 한 프레임 대기
        yield return null;

        // 현재 애니메이터의 상태 정보 가져옴(0은 Base Layer)
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 현재 재생 중인 애니메이션의 길이 가져옴
        float animationLength = stateInfo.length;

        // 가져온 애니메이션 길이만큼 기다림
        yield return new WaitForSeconds(animationLength);

        // 현재 스테이지 로드
        SceneLoader.Instance.StartLoadScene(SceneLoader.Instance.CurrentSceneState);
    }

    // 박힌 곡괭이 찾는 메소드
    private void FindStuckPickaxe()
    {
        stuckPickaxe = ThrownPickaxeController.ThrownPickaxeInstance;
    }

    // 대쉬 가능 조건을 확인하는 메소드
    private void CheckDashAvailability()
    {
        isDashAvailable = false; // 매번 초기화
        if (stuckPickaxe == null || stuckPickaxe.CurrentState != stuckPickaxe.StateMachine.StuckState)
        {
            return; // 박힌 곡괭이가 없으면 대쉬 불가
        }

        Vector2 playerPosition = transform.position;
        Vector2 pickaxePosition = stuckPickaxe.transform.position;
        float distance = Vector2.Distance(playerPosition, pickaxePosition);

        if (distance > dashRange)
        {
            return; // 범위 밖이면 대쉬 불가
        }

        // 플레이어와 곡괭이 사이에 장애물이 있는지 확인 (Raycast)
        RaycastHit2D hit = Physics2D.Linecast(playerPosition, pickaxePosition, obstacleLayerMask);
        if (hit.collider != null)
        {
            // 무언가에 막혔다면 대쉬 불가
            return;
        }

        // 모든 조건을 통과하면 대쉬 가능
        isDashAvailable = true;
    }

    // 대쉬 가능 시각 효과(점선)를 업데이트하는 메소드
    private void UpdateDashVisuals()
    {
        if (isDashAvailable && stuckPickaxe != null)
        {
            dashLineRenderer.enabled = true;

            // Inspector에서 설정한 색상과 투명도 적용
            dashLineRenderer.startColor = dashLineColor;
            dashLineRenderer.endColor = dashLineColor;

            Vector2 playerPosition = transform.position;
            Vector2 pickaxePosition = stuckPickaxe.transform.position;

            // 플레이어에서 곡괭이로 향하는 방향 벡터 계산
            Vector2 direction = (pickaxePosition - playerPosition).normalized;

            // 플레이어 중심에서 반경(offset)만큼 떨어진 지점을 시작점으로 설정
            Vector2 lineStartPoint = playerPosition + (direction * dashLineOffsetRadius);

            dashLineRenderer.SetPosition(0, lineStartPoint);
            dashLineRenderer.SetPosition(1, stuckPickaxe.transform.position);
        }
        else
        {
            dashLineRenderer.enabled = false;
        }
    }

    // 대쉬 입력 처리
    private void OnDash(InputAction.CallbackContext context)
    {
        if (isDashAvailable && currentState == PlayerState.Normal)
        {
            if(player.DashSFX)
            {
                SoundManager.PlayClip(player.DashSFX);
            }

            StartCoroutine(DashCoroutine(stuckPickaxe.transform.position));
        }
    }

    // 실제 대쉬 이동을 처리하는 코루틴
    private IEnumerator DashCoroutine(Vector3 targetPosition)
    {
        currentState = PlayerState.Dashing;
        isDashAvailable = false; // 대쉬 시작과 함께 대쉬 가능 상태 해제
        UpdateDashVisuals(); // 라인 끄기

        // 대쉬 방향으로 플레이어 바라보게 하기
        // 기존 방향 상태를 가져옴
        bool originallyFacingRight = isFacingRight;

        // 대쉬 목표 방향 결정
        bool shouldFaceRight = targetPosition.x > transform.position.x;

        // 현재 방향과 목표 방향이 다를 경우에만 Flip
        if (originallyFacingRight != shouldFaceRight)
        {
            Flip();
        }

        // 물리 효과 및 충돌 처리 변경
        rb.velocity = Vector2.zero; // 기존 속도 제거
        rb.gravityScale = 0f; // 중력 비활성화

        // 대쉬 대상이 되는 곡괭이의 참조를 코루틴 내에서 안전하게 보관
        var targetPickaxe = stuckPickaxe;
        var pickaxeCollider = targetPickaxe.GetComponent<Collider2D>();

        // 도착 직전 자동 회수를 막기 위해 곡괭이의 트리거 잠시 비활성화
        if (pickaxeCollider != null)
        {
            pickaxeCollider.enabled = false;
        }

        // TODO: 기획에 따라 특정 레이어와의 충돌을 무시하는 코드 추가
        // Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        // DashVFX 재생
        if (dashVFXPrefab != null)
        {
            dashVFXPrefab.gameObject.SetActive(true);
        }

        // 목표 지점에 도달할 때까지 이동
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }

        transform.position = targetPosition; // 정확한 위치로 보정

        if (targetPickaxe != null)
        {
            if (CheckGround())
            {
                stuckPickaxe.Owner.RetrievePickaxe(false);
                Destroy(stuckPickaxe.gameObject);
                currentState = PlayerState.Normal;
                rb.gravityScale = originalGravityScale; // 땅에서는 중력 즉시 복원
            }
            else
            {
                currentState = PlayerState.Hanging;
                transform.SetParent(stuckPickaxe.transform); // 플레이어를 곡괭이의 자식으로 만들어 위치 고정
                jumpCount = 1;
                Debug.Log("매달리기 성공! 점프 카운트 = 1");
            }
        }
        else
        {
            currentState = PlayerState.Normal;
            rb.gravityScale = originalGravityScale;
        }

        dashVFXPrefab.gameObject.SetActive(false);
    }

    private void HandleHangingInput()
    {
        // 회수 UI 닫기
        UIRecallPickaxe uIRecallPickaxe = GameManager.Instance.Player.UIRecallPickaxe;
        uIRecallPickaxe?.CloseUI();

        // 떨어지기 (S 키 또는 아래 방향키)
        if (PlayerActions.Drop.WasPressedThisFrame())
        {
            ExitHangingState();
        }
        // 매달리기 점프 (점프 키)
        /*else if (PlayerActions.Jump.WasPressedThisFrame())
        {
            // 점프 카운트가 있어야 점프 가능
            if (jumpCount > 0)
            {
                jumpCount--;
                ExitHangingState();
                Debug.Log("HandleHangingJump");
                rb.velocity = new Vector2(rb.velocity.x, player.JumpForce);

                if (hangJumpVFXPrefab != null)
                {
                    //Instantiate(hangJumpVFXPrefab, transform.position, Quaternion.identity);
                }
            }
        }*/
    }

    private void ExitHangingState()
    {
        transform.SetParent(null);

        currentState = PlayerState.Normal;
        rb.gravityScale = originalGravityScale; // 중력 복원

        if (stuckPickaxe != null)
        {
            stuckPickaxe.Owner.RetrievePickaxe(false);
            Destroy(stuckPickaxe.gameObject);
            stuckPickaxe = null;
        }

        Debug.Log("매달리기 해제!");
    }
}
