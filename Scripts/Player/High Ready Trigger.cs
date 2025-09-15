using UnityEngine;

public class HighReadyTrigger : MonoBehaviour
{
    // 현재 이 트리거가 벽에 닿아있는지 여부를 추적
    private bool isTouchingWall = false;

    // 벽으로 사용할 레이어를 유니티 인스펙터에서 설정
    [SerializeField] private LayerMask wallLayer;

    // 외부 스크립트에서 벽 감지 상태를 확인할 수 있도록 하는 함수
    public bool GetIsTouchingWall()
    {
        return isTouchingWall;
    }

    // 트리거에 다른 콜라이더가 들어왔을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거 엔터");

        // 들어온 오브젝트가 wallLayer에 속해있는지 비트 연산으로 확인
        if ((wallLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Debug.Log("벽이 맞음");
            // 벽에 닿았다고 상태만 변경
            isTouchingWall = true;
        }
    }

    // 트리거에서 다른 콜라이더가 나갔을 때 호출
    private void OnTriggerExit(Collider other)
    {
        // 나간 오브젝트가 wallLayer에 속해있는지 비트 연산으로 확인
        if ((wallLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // 벽에서 떨어졌다고 상태만 변경
            isTouchingWall = false;
        }
    }
}
