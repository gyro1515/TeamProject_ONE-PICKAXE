using System.Collections;
using UnityEngine;

public class Door : InteractableObject
{
    private bool isOpen = false;

    // Interaction() 메서드는 버튼이 아닌 다른 오브젝트와 닿았을 때 호출되므로,
    // 이 로직은 유지하거나 다른 상호작용 방식으로 변경해야 합니다.
    public override void Interaction(GameObject other)
    {
        // 여기서는 문에 닿는 것만으로는 아무 일도 일어나지 않도록 비워둡니다.
        // 버튼을 통해서만 문이 열리게 하기 위함입니다.
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            // 문을 열 때 파괴 로직을 코루틴으로 실행
            StartCoroutine(OpenAndDestroy());
        }
        else
        {
            Debug.Log("문이 닫혔습니다.");
        }
    }

    // 코루틴: 문을 열고 잠시 후 파괴
    private IEnumerator OpenAndDestroy()
    {
        Debug.Log("문이 열리는 애니메이션을 재생합니다.");
        // 문이 열리는 애니메이션을 재생하는 코드 (예: transform.position을 이동)
        // yield return new WaitForSeconds(1.0f); // 1초 대기

        // 애니메이션이 끝난 후 오브젝트 파괴
        Debug.Log("문 오브젝트를 파괴합니다.");
        Destroy(gameObject);

        yield return null;
    }
}