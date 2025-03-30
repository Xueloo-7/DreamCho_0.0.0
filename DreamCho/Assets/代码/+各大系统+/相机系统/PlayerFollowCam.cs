using Unity.Cinemachine;
using UnityEngine;

public class PlayerFollowCam : MonoBehaviour
{
    private CinemachineCamera cam;

    private void Awake()
    {
        cam = GetComponent<CinemachineCamera>();

        Event.onNewSceneStart += OnNewSceneStart;
    }
    private void OnDestroy()
    {
        Event.onNewSceneStart -= OnNewSceneStart;
    }
    void OnNewSceneStart()
    {
        if(gameObject == null) return;

        CameraManager.Instance.SetCameraPriority(cam, 1);

        cam.Follow = FindAnyObjectByType<Player>().transform;
    }
}
