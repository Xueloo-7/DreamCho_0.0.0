using UnityEngine;

public class Deadline : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Event.playerTeleport?.Invoke(Spawnpoint.spawnPoint);
        }
    }
}
