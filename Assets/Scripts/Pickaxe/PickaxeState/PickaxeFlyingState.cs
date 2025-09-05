using UnityEngine;

public class PickaxeFlyingState : PickaxeBaseState<ThrownPickaxeStateMachine>
{
    private Rigidbody2D rb;
    private ThrownPickaxeController thrownPickaxeController;
    private ContactFilter2D contactFilter;
    private RaycastHit2D[] hits = new RaycastHit2D[1]; // 충돌 결과를 담을 배열(하나만 감지하면 되므로 크기는 1)

    public override void EnterState(ThrownPickaxeStateMachine stateMachine)
    {
        Debug.Log("곡괭이 상태: 날아가는 중");

        if (stateMachine.ThrownPickaxeController.FlyingSFX)
        {
            SoundManager.PlayClip(stateMachine.ThrownPickaxeController.FlyingSFX);
        }

        // 필요한 컴포넌트와 설정 정보를 가져와서 초기화
        rb = stateMachine.ThrownPickaxeController.Rb2D;
        thrownPickaxeController = stateMachine.ThrownPickaxeController;

        var groundLayer = thrownPickaxeController.GroundLayerMask;
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(groundLayer); // ThrownPickaxeController에서 설정한 레이어만 감지
        contactFilter.useTriggers = false; // 물리 충돌만 감지하기 위해 트리거는 제외
    }

    public override void FixedUpdateState(ThrownPickaxeStateMachine stateMachine)
    {
        // 다음 물리 프레임까지 이동할 예상 거리
        float distance = rb.velocity.magnitude * Time.fixedDeltaTime;

        // 이동 방향으로 Rigidbody의 콜라이더 모양 그대로 투사(Cast)하여 충돌 예측
        int hitCount = rb.Cast(rb.velocity.normalized, contactFilter, hits, distance);

        if (hitCount > 0)
        {
            // 충돌이 예측되었다면 가장 먼저 부딪힌 객체의 정보를 가져옴
            RaycastHit2D firstHit = hits[0];
            Collider2D other = firstHit.collider; // 충돌한 객체의 콜라이더

            // 박히는 타일과 충돌했는지 확인
            if (other.CompareTag("CanStuck"))
            {
                // 예측된 충돌 정보를 컨트롤러에 저장
                thrownPickaxeController.SetLastHitInfo(firstHit);

                // 박힘 상태로 전환
                stateMachine.ChangeState(stateMachine.StuckState);
            }
        }
    }

    // FixedUpdateState에서 예측하지 못한 충돌이 발생했을 때 처리(튕기기는 트리거로 처리)
    public override void HandleTrigger(ThrownPickaxeStateMachine stateMachine, Collider2D other)
    {
        // 박히는 타일과 충돌했는지 확인
        if (other.CompareTag("CanStuck"))
        {
            // 곡괭이의 속도 방향으로 레이캐스트를 쏴서 충돌 정보를 얻습니다.
            Vector2 rayDirection = stateMachine.ThrownPickaxeController.Rb2D.velocity.normalized;
            float rayDistance = 0.5f; // 곡괭이 크기에 맞춰 적절한 값 설정

            var lastHitInfo = Physics2D.Raycast(stateMachine.ThrownPickaxeController.transform.position, rayDirection, rayDistance, other.gameObject.layer);
            stateMachine.ThrownPickaxeController.SetLastHitInfo(lastHitInfo);

            stateMachine.ChangeState(stateMachine.StuckState);
        }
        // 박히는 지형이 아니면 모두 튕기는 오브젝트로 간주(적, 함정 제외)
        else
        {
            // 데미지 처리
            if (!other.CompareTag("Player"))
            {
                var damageable = other.GetComponent<IDamageable>();
                Debug.Log(other.name);
                if (damageable != null)
                {
                    damageable.TakeDamage(stateMachine.ThrownPickaxeController.Damage);
                    Debug.Log(damageable + " 데미지 입음: " + stateMachine.ThrownPickaxeController.Damage);
                }
            }

            // 충돌 지점을 추정하여 BounceState로 전달
            Vector2 hitPoint = stateMachine.ThrownPickaxeController.transform.position;
            stateMachine.ThrownPickaxeController.Bounce(hitPoint, stateMachine.ThrownPickaxeController.PlayerTransform);
        }
    }

    public override void HandleCollision(ThrownPickaxeStateMachine stateMachine, Collision2D collision) { }
    public override void ExitState(ThrownPickaxeStateMachine stateMachine) { }
    public override void HandleInput(ThrownPickaxeStateMachine stateMachine) { }
    public override void UpdateState(ThrownPickaxeStateMachine stateMachine) { }
}
