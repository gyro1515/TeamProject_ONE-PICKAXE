using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float MoveSpeed = 5f;
    public float JumpForce = 7f;

    [Header("Ground Check Settings")]
    public Transform GroundCheck;
    public float GroundCheckRadius = 0.2f;
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
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, GroundLayer);
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

    void OnDrawGizmosSelected()
    {
        if (GroundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
    }
}
