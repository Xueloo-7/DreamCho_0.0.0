using UnityEngine;

public class Spike : MonoBehaviour
{
    Vector2 spawnPoint;
    private void Start()
    {
        spawnPoint = FindAnyObjectByType<PlayerController>().transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Event.playerTeleport?.Invoke(spawnPoint);
        }
    }
}
