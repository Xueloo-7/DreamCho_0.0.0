using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] Vector2 _spawnPoint_debug;
    public static Vector2 spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _spawnPoint_debug = transform.position;
            spawnPoint = transform.position;
        }
    }
}
