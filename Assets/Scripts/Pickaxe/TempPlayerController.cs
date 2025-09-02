using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float MoveSpeed = 5f;
    public float JumpForce = 7f;

    [Header("Ground Check Settings")]
    public float RayDistance = 0.2f;
    public LayerMask GroundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = 0f;

        if(Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }

        rb.velocity = new Vector2(horizontalInput * MoveSpeed, rb.velocity.y);

        // 입력 방향과 현재 보고 있는 방향이 다를 경우에만 Flip() 호출
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // y축 속도를 즉시 0으로 만든 후 점프하여 일관된 점프 높이 제공
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();
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

        // 결과를 반환합니다.
        return isGrounded;
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

    // 레이캐스트 디버깅
    private void OnDrawGizmos()
    {
        // Gizmos 색상 설정
        // 땅에 닿아있으면 녹색 아니면 빨간색으로 표시
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // 플레이어의 위치에서 아래로 레이캐스트의 거리를 시각적으로 그림
        Gizmos.DrawRay(transform.position, Vector2.down * RayDistance);
    }
}
