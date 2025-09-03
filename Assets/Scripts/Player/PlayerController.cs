using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
    [Header("Ground Check Settings")]
    [SerializeField] float RayDistance = 1f;
    [SerializeField] LayerMask GroundLayer;

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
        rb = GetComponent<Rigidbody2D>();

        playerAnimationData = GetComponent<PlayerAnimationData>();
    }

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void FixedUpdate()
    {
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
}
