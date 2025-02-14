using Unity.Cinemachine;
using UnityEngine;

public class RoomCam : MonoBehaviour
{
    [SerializeField] CinemachineCamera cam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraManager.Instance.SetCameraPriority(cam, 1);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 即使对象变为static也不会影响退出
        if (collision.CompareTag("Player") && collision.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
        {
            CameraManager.Instance.RollBack();
        }
    }
}
