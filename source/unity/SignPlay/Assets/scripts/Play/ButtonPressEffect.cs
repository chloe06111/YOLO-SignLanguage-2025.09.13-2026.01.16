using UnityEngine;
using UnityEngine.EventSystems; // 터치/클릭 감지를 위해 필수!

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    public Transform targetToMove; // 움직일 대상 (보통 텍스트)
    public Vector3 moveOffset = new Vector3(0, -5, 0); // 얼만큼 움직일지 (x, y, z)

    private Vector3 originalPosition; // 원래 위치 기억용

    void Awake()
    {
        // 만약 타겟을 안 넣었으면, 내 자식 중에 첫 번째 놈(보통 텍스트)을 자동으로 잡음
        if (targetToMove == null && transform.childCount > 0)
        {
            targetToMove = transform.GetChild(0);
        }
    }

    // 눌렀을 때 (Down)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetToMove != null)
        {
            originalPosition = targetToMove.localPosition; // 원래 위치 기억
            targetToMove.localPosition += moveOffset;      // 살짝 내리기
        }
    }

    // 뗐을 때 (Up)
    public void OnPointerUp(PointerEventData eventData)
    {
        if (targetToMove != null)
        {
            targetToMove.localPosition = originalPosition; // 원래 위치로 복구
        }
    }
}