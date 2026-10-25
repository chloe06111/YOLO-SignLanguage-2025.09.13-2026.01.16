using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float followSpeed = 12f;

    [SerializeField] private bool snapOnAcquire = true;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        AcquireTargetIfNeeded(true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 바뀌면 새 씬에서 Player를 다시 찾거나, 스포너가 SetTarget 해주길 기다림
        target = null;
        AcquireTargetIfNeeded(true);
    }

    private void LateUpdate()
    {
        if (!AcquireTargetIfNeeded(false))
            return;

        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (snapOnAcquire && target != null)
        {
            transform.position = target.position + offset;
        }
    }

    private bool AcquireTargetIfNeeded(bool allowSnap)
    {
        if (target != null) return true;

        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj == null) return false;

        target = playerObj.transform;

        if (snapOnAcquire && allowSnap)
        {
            transform.position = target.position + offset;
        }

        return true;
    }
}
