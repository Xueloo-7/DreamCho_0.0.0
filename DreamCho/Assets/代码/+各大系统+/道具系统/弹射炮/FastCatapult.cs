using DreamCho;
using System.Collections;
using UnityEngine;

public class FastCatapult : Catapult
{
    [SerializeField] float shootDelay = 0.2f;

    public override void EnterCatapult(Entity entity)
    {
        base.EnterCatapult(entity);

        // 第三种弹射炮允许所有实体进入
        EntityEnterCatapultInside(entity);

        // 在进入后立即发射
        StartCoroutine(ShootDelay());
    }
    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        Shoot();
    }
    public override void Shoot()
    {
        base.Shoot();
    }
    public override void OnInteract()
    {
        base.OnInteract();
    }
    public override void OnEscape()
    {
        base.OnEscape();
    }
}
