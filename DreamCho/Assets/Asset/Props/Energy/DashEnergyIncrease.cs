using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DashEnergyIncrease : MonoBehaviour
{
    [SerializeField] float increase;
    [SerializeField] float respawnTime;
    [SerializeField] Color color;

    SpriteRenderer sr;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enabled)
        {
            PlayerController controller = collision.GetComponent<PlayerController>();
            if (controller == null || controller.GetDashEnergy() >= controller.ControlParameter.maxDashEnergy)
                return;

            PlayerController.increaseEnergy?.Invoke(increase); // 恢复体力

            // 自己消失
            enabled = false;
            if(sr ==  null) sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
            sr.color = Color.clear;
            DashEnergyManager.Instance.PlayParticle(transform.position);

            // 震动效果
            SoundShake.Instance.Shake(transform.position, 4, 2, 0.2f);

            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        enabled = true;
        sr.color = color;
        sr.transform.localScale = Vector3.zero;
        sr.transform.DOScale(1, 0.3f).SetEase(Ease.OutCubic);
    }
}
