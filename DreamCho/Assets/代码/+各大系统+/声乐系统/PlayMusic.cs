using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(clip == null)
            {
                Debug.LogWarning("No AudioClip Assigned");
                return;
            }
            AudioManager.PlayMusic(clip);
        }
    }
}
