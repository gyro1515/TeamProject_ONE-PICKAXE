using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerController : BaseController
{
    [Header("Ground Check Settings")]
    [SerializeField] float RayDistance = 1f;
    [SerializeField] LayerMask GroundLayer;

    [Header("Dash Settings")]
    [SerializeField] private float dashRange = 10f; // 대쉬 가능 범위
    [SerializeField] private float dashSpeed = 20f; // 대쉬 속도
    [SerializeField] private float dashLineOffsetRadius = 0.5f; // 대쉬 라인 시작 지점 반경
    [SerializeField] private LayerMask obstacleLayerMask; // 장애물 레이어 (지형: Cave)
    [SerializeField] private Color dashLineColor = new Color(1, 1, 1, 0.8f); // 대쉬 라인 색상 및 투명도
    [SerializeField] private LineRenderer dashLineRenderer; // 대쉬 경로를 표시할 LineRenderer
    [SerializeField] private GameObject dashVFXPrefab; // 대쉬 시작 시 생성될 VFX 프리팹

    private ThrownPickaxeController stuckPickaxe; // 현재 박혀있는 곡괭이 참조
    private bool isDashAvailable = false; // 대쉬가 가능한 상태인지
    private bool isDashing = false; // 현재 대쉬 중인지
    private float originalGravityScale; // 원래 중력 값을 저장할 변수

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = true;
    float horizontalInput = 0f;

    private PlayerAnimationData playerAnimationData;

    Player player;
    public PlayerInput Input { get; private set; }
    public PlayerInput.PlayerActions PlayerActions { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        Input = new PlayerInput();
        PlayerActions = Input.Player;
        PlayerActions.Jump.started += OnJump;
        PlayerActions.OpenPauseMenu.started += OnOpenPauseMenu;
        PlayerActions.Dash.started += OnDash;
        rb = GetComponent<Rigidbody2D>();

        playerAnimationData = GetComponent<PlayerAnimationData>();

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
        if (isDashing)
        {
            return; // 대쉬 중에는 아무것도 하지 않음
        }

        FindStuckPickaxe();
        CheckDashAvailability();
        UpdateDashVisuals();
    }

    // FixedUpdate에서 대쉬 중 조작 방지
    protected override void FixedUpdate()
    {
        // 대쉬 중에는 물리 움직임 처리 안 함
        if (isDashing)
        {
            return;
        }

        base.FixedUpdate();

        isGrounded = CheckGround();
    }

    private void OnEnable()
    {
        Input.Enable();
    }

    private void OnDisable()
    {
        PlayerActions.Jump.started -= OnJump;
        PlayerActions.OpenPauseMenu.started -= OnOpenPauseMenu;
        PlayerActions.Dash.started -= OnDash;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if(isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * player.JumpForce, ForceMode2D.Impulse);
        }
    }

    void OnOpenPauseMenu(InputAction.CallbackContext context)
    {
        player.UIPause.OpenUI();
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, RayDistance, GroundLayer);

        // 레이캐스트가 어떤 물체와 충돌했는지 확인
        if (hit.collider != null)
        {
            isGrounded = true;
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
        if (active) Input.Enable();
        else Input.Disable();
    }

    // 박힌 곡괭이 찾는 메소드
    private void FindStuckPickaxe()
    {
        // ThrownPickaxeController는 하나만 존재한다고 가정
        stuckPickaxe = FindObjectOfType<ThrownPickaxeController>();
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
        if (isDashAvailable && !isDashing)
        {
            StartCoroutine(DashCoroutine(stuckPickaxe.transform.position));
        }
    }

    // 실제 대쉬 이동을 처리하는 코루틴
    private IEnumerator DashCoroutine(Vector3 targetPosition)
    {
        isDashing = true;
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

        // 대쉬 종료 후 처리
        if (stuckPickaxe != null)
        {
            // 곡괭이 즉시 회수
            stuckPickaxe.Owner.RetrievePickaxe(false); // 일반 회수로 처리
            Destroy(stuckPickaxe.gameObject);
        }

        // 플레이어 상태 원상복구
        rb.gravityScale = originalGravityScale; // 중력 복원

        // TODO: 무시했던 레이어와의 충돌을 다시 활성화
        // Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
        isDashing = false;

        dashVFXPrefab.gameObject.SetActive(false);
    }
}
