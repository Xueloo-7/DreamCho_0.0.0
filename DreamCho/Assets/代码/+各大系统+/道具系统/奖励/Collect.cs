using UnityEngine;

public class Collect : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Event.onCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}
