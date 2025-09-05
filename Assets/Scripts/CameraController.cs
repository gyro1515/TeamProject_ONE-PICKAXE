using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 pos = Vector2.zero;
    public float cameraSpeed = 5.0f;

    public GameObject player;

    private void Start()
    {
        // GameManager에서 플레이어 오브젝트를 가져와 할당
        if (GameManager.Instance.Player != null)
        {
            player = GameManager.Instance.Player.gameObject;
        }
    }

    private void Update()
    {
        // player 변수가 할당되었는지 확인하는 null 체크
        if (player != null)
        {
            Vector3 dir = player.transform.position - (this.transform.position + pos);
            //dir.y = 0;
            Vector3 moveVector = new Vector3(dir.x * cameraSpeed * Time.deltaTime, dir.y * cameraSpeed * Time.deltaTime, 0.0f);
            this.transform.Translate(moveVector);
        }
        else
        {
            // 플레이어가 아직 할당되지 않았을 경우, 재시도
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                player = GameManager.Instance.Player.gameObject;
            }
        }
    }
}