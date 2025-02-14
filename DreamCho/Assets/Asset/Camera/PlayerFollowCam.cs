using Unity.Cinemachine;
using UnityEngine;

public class PlayerFollowCam : MonoBehaviour
{
    private CinemachineCamera cam;

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        CameraManager.Instance.SetCameraPriority(cam, 1);
    }
}
