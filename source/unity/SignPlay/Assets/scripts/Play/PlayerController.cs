using System.Collections;
using UnityEngine;

/// <summary>
/// PlayerController (A 방식: 스크립트가 animator.Play로 방향 전환)
/// - 키 입력(WASD/Arrows) 또는 외부 SetAxis로 이동 (Rigidbody2D 기반)
/// - 애니메이션 전환(상/하/좌/우/정지)
/// - 텔레포트/현재 위치 저장/퀴즈 시작 요청 포함
///
/// B안 적용:
/// - 코스튬/스킨(Animator Override Controller) 관련 코드는 전부 제거됨
/// - 스킨 적용은 PlayerSpawnManager(또는 별도 스킨 매니저)가 담당
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3.0f;

    [Header("Animation Names (State names in Animator)")]
    public string stopAnime = "PlayerStop";
    public string upAnime = "PlayerUp";
    public string downAnime = "PlayerDown";
    public string leftAnime = "PlayerLeft";
    public string rightAnime = "PlayerRight";

    // 현재/이전 애니메이션 추적
    private string nowAnimation = "";
    private string oldAnimation = "";

    // 입력 축
    private float axisH = 0f;
    private float axisV = 0f;

    // 내부 상태
    private Rigidbody2D rbody;
    private Animator animator;
    private bool isMoving = false;
    private float angleZ = -1f; // -1 => 정지

    [Header("Teleport (Play features)")]
    public bool smoothTeleport = false;
    public float smoothTeleportDuration = 0.2f;
    private bool hasTeleportedThisStart = false;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {

        Debug.Log($"📂 데이터 저장 폴더 위치: {Application.persistentDataPath}");
        // 기본 애니메이션 설정(정지 하도록)
        oldAnimation = stopAnime;
        nowAnimation = stopAnime;

        // 시작 시 정지 애니메이션 한번 확실히 재생
        if (animator != null)
            animator.Play(nowAnimation);

        // 씬 시작 시 저장된 위치로 텔레포트 시도 (Play 규칙)
        // 충돌 일어나서 비활성 시킴
        // PlayTeleportToLastPositionIfExists();
    }

    private void Update()
    {
        // 입력을 외부 SetAxis로 제어할 수도 있으므로, isMoving 플래그로 덮어씀
        if (!isMoving)
        {
            axisH = Input.GetAxisRaw("Horizontal");
            axisV = Input.GetAxisRaw("Vertical");
        }

        // 방향 각도 계산 (정지면 -1)
        Vector2 fromPt = transform.position;
        Vector2 toPt = new Vector2(fromPt.x + axisH, fromPt.y + axisV);
        angleZ = GetAngle(fromPt, toPt);

        // 각도에 따라 애니메이션 결정
        if (angleZ == -1f)
        {
            nowAnimation = stopAnime;
        }
        else if (angleZ > -45f && angleZ < 45f)
        {
            nowAnimation = rightAnime;
        }
        else if (angleZ >= 45f && angleZ <= 135f)
        {
            nowAnimation = upAnime;
        }
        else if (angleZ >= -135f && angleZ <= -45f)
        {
            nowAnimation = downAnime;
        }
        else
        {
            nowAnimation = leftAnime;
        }

        // 애니메이션 변경 적용
        if (nowAnimation != oldAnimation)
        {
            oldAnimation = nowAnimation;
            if (animator != null)
            {
                animator.Play(nowAnimation);
            }
        }
    }

    private void FixedUpdate()
    {
        // Rigidbody2D를 사용한 이동 (physics-friendly)
        Vector2 vel = new Vector2(axisH, axisV).normalized * speed;
        rbody.velocity = vel;
    }

    /// <summary>
    /// 외부에서 입력을 줄 때 사용 (예: 가상 조이스틱)
    /// </summary>
    public void SetAxis(float h, float v)
    {
        axisH = h;
        axisV = v;
        isMoving = !(Mathf.Approximately(axisH, 0f) && Mathf.Approximately(axisV, 0f));
    }

    private float GetAngle(Vector2 p1, Vector2 p2)
    {
        if (!Mathf.Approximately(axisH, 0f) || !Mathf.Approximately(axisV, 0f))
        {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            float rad = Mathf.Atan2(dy, dx);
            float angle = rad * Mathf.Rad2Deg;
            return angle;
        }
        return -1f; // 정지 상태 표시
    }

    // -----------------------------
    // Play-prefixed features (teleport / save position)
    // -----------------------------

    public void PlayTeleportToLastPositionIfExists()
    {
        if (hasTeleportedThisStart) return;
        hasTeleportedThisStart = true;

        if (QuizGameManager.Instance == null) return;

        Vector3 lastPos;

        if (QuizGameManager.Instance.GetLastMapPosition(out lastPos))
        {
            if (!smoothTeleport || smoothTeleportDuration <= 0f)
            {
                transform.position = lastPos;
            }
            else
            {
                StartCoroutine(PlaySmoothTeleportRoutine(lastPos, smoothTeleportDuration));
            }
        }
    }

    private IEnumerator PlaySmoothTeleportRoutine(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        transform.position = target;
    }

    public void PlaySaveCurrentPosition()
    {
        if (QuizGameManager.Instance == null) return;
        QuizGameManager.Instance.SetLastPosition(transform.position);
    }

    public void PlayOnStartQuizRequested()
    {
        PlaySaveCurrentPosition();
        // QuizZone 또는 PlaySceneController가 스테이지 선택/씬 전환을 담당
    }
}
