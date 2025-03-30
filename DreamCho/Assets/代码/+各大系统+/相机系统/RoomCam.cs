using Unity.Cinemachine;
using UnityEngine;

public class RoomCam : MonoBehaviour
{
    [SerializeField] bool followPlayer;
    [SerializeField] CinemachineCamera cam;

    private void Start()
    {
        if (followPlayer)
            cam.Follow = FindAnyObjectByType<Player>().transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraManager.Instance.SetCameraPriority(cam, 1);
        }
    }
}
