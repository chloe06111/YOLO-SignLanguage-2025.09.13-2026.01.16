using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleButtonEffect : MonoBehaviour, IPointerDownHandler
{
    [Header("Settings")]
    public Transform targetToMove; // 움직일 대상
    public Vector3 moveOffset = new Vector3(0, -5, 0); // 눌렸을 때 이동할 거리

    [Header("Status")]
    public bool isPressed = false; // 현재 눌려있는지 여부 (인스펙터에서 확인 가능)

    private Vector3 originalPosition; // 원래 위치 기억용

    private void Awake()
    {
        // 타겟 없으면 첫 번째 자식 자동 할당
        if (targetToMove == null && transform.childCount > 0)
        {
            targetToMove = transform.GetChild(0);
        }
    }

    private void Start()
    {
        if (targetToMove != null)
        {
            originalPosition = targetToMove.localPosition;
            
            // 시작할 때 이미 isPressed가 체크되어 있다면 눌린 상태로 시작
            if (isPressed) UpdatePosition();
        }
    }

    // 터치/클릭 하는 순간 실행
    public void OnPointerDown(PointerEventData eventData)
    {
        // 상태를 반대로 뒤집음 (꺼져있으면 켜고, 켜져있으면 끔)
        isPressed = !isPressed;
        
        // 위치 업데이트
        UpdatePosition();
    }

    // 현재 상태(isPressed)에 따라 위치를 이동시키는 함수
    private void UpdatePosition()
    {
        if (targetToMove == null) return;

        if (isPressed)
        {
            // 눌린 상태: 원래 위치 + 오프셋
            targetToMove.localPosition = originalPosition + moveOffset;
        }
        else
        {
            // 안 눌린 상태: 원래 위치 복구
            targetToMove.localPosition = originalPosition;
        }
    }

    // [중요] 외부(다른 스크립트)에서 강제로 버튼을 튀어오르게 할 때 사용
    // 예: 다른 버튼을 눌러서 이 버튼 선택을 해제해야 할 때 -> SetPressed(false) 호출
    public void SetPressed(bool pressed)
    {
        isPressed = pressed;
        UpdatePosition();
    }
}