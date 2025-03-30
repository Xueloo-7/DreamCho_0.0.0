using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    [SerializeField] float fallGravity = 3; // 坠落速度
    [SerializeField] float mass = 1000; // 质量
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D detectCollider; // 检测玩家进入的碰撞体

    private void Start()
    {
        if (rb.bodyType != RigidbodyType2D.Static)
        {
            Debug.LogWarning("Rigidbody2D should be set to Static in the inspector.");
            //rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Fall();
        }
    }

    void Fall()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
        rb.mass = mass;
        detectCollider.enabled = false;
    }
}
