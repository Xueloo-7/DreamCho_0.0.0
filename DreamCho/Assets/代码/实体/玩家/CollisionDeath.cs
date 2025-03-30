using UnityEngine;

public class CollisionDeath : MonoBehaviour
{
    [SerializeField] GameObject[] ignoreObj;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !ColliderWithIgnoreObj(collision))
        {
            // 被其他非玩家物体挤压，玩家死亡
            Event.playerTeleport?.Invoke(Spawnpoint.spawnPoint);
        }
    }

    bool ColliderWithIgnoreObj(Collision2D collision)
    {
        foreach (var obj in ignoreObj)
        {
            if (collision.gameObject == obj)
            {
                return true;
            }
        }
        return false;
    }
}
