using DreamCho;
using UnityEngine;

public class CustomCatapult : Catapult
{
    private new void Start()
    {
        base.Start();

        KeyEvent.onDirection_Down += OnDirectionDown;
        KeyEvent.onDash_Down += Shoot;
        KeyEvent.onInteract_Down += OnInteract;
        KeyEvent.onEscape_Down += OnEscape;
    }
    private void OnDisable()
    {
        KeyEvent.onDirection_Down -= OnDirectionDown;
        KeyEvent.onDash_Down -= Shoot;
        KeyEvent.onInteract_Down -= OnInteract;
        KeyEvent.onEscape_Down -= OnEscape;
    }

    public override void EnterCatapult(Entity entity)
    {
        base.EnterCatapult(entity);

        if (entity.CompareTag("Player")) // 第一种弹射炮仅允许玩家进入
        {
            EntityEnterCatapultInside(entity);
        }
    }
    public override void Shoot()
    {
        base.Shoot();

        // 发射和冲刺相等距离

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
