using UnityEngine;

public class BlackWhitePlatformDetect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "-黑平台-" || collision.gameObject.name == "-白平台-")
        {
            Event.onEnterBlackWhitePlatform?.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "-黑平台-" || collision.gameObject.name == "-白平台-")
        {
            Event.onExitBlackWhitePlatform?.Invoke();
        }
    }
}
