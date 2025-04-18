﻿using DG.Tweening;
using DreamCho;
using System.Collections;
using UnityEngine;

/// <summary>
/// 自定义方向弹射炮
/// 只允许玩家进入，进入后选择八个方向
/// 冲刺键发射，交互键调整弹射力，返回键取消弹射
/// 发射后恢复所有冲刺机会
/// </summary>
public abstract class Catapult : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] float[] dashForce = { 32, 48 };
    [SerializeField] float dashTime = 0.1f;

    [Header("Ref")]
    [SerializeField] CatapultForceUI ui;

    protected bool isEnter; // 实体是否在内部
    protected bool isExit; // 实体是否下来，直到离开侦测范围后设为false
    private float enterCold; // 进入冷却时间
    protected Entity enterEntity; // 进入的实体
    //private Transform entityOriginParent; // 实体在进入内部前的父对象
    private Vector2 shootDirection; // 发射角度
    private int forceLevel = 0; // 弹射力等级    

    protected void Start()
    {
        shootDirection = transform.up;
        enterCold = 0.2f;

        ui.UpdateUI(forceLevel);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExit) return;

        Entity entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            EnterCatapult(entity);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            /*if (isExit) // 确认实体离开检测区域，这样才能进行下一次进入检测
            {
                StartCoroutine(ExitCold());
            }*/
        }


    }

    #region Catapult Func
    protected void OnDirectionDown(Vector2 direction)
    {
        if (isEnter == false || direction == Vector2.zero) return;

        float angle = XMath.VecToA(direction.normalized);
        transform.DORotate(new Vector3(0, 0, -90 + angle), 0.3f, RotateMode.Fast);

        shootDirection = direction.normalized;
    }
    protected void EntityEnterCatapultInside(Entity entity) // 实体进入弹射炮内部
    {
        entity.OnEnterCatapult();
        entity.Hidden(false);
        //entityOriginParent = entity.transform.parent;
        entity.SetTransform(transform.position, Quaternion.identity, Vector3.one, entity.transform.parent);
        entity.SetRigibody(RigidbodyType2D.Static);

        enterEntity = entity;
    }
    protected void EntityExitCataput(Entity entity, Vector2 spawnPoint = default) // 实体离开弹射炮，在离开检测范围前都不能再次进入内部（避免频繁进退bug）
    {
        entity.Showing();
        //entity.transform.SetParent(entityOriginParent);
        entity.SetTransform(
            (Vector2)transform.position + spawnPoint,
            Quaternion.Euler(0, 0, -90 + XMath.VecToA(shootDirection)),
            Vector3.one, entity.transform.parent);

        //entityOriginParent = null;

        entity.SetRigibody(RigidbodyType2D.Dynamic);
        entity.ResetState(); // 重置一些属性以确保恢复到进入前的状态

        enterEntity = null;

        StartCoroutine(ExitCold());
    }
    #endregion

    #region Override Func
    public virtual void EnterCatapult(Entity entity)
    {
        if (isEnter) return;

        isEnter = true;
    }

    #region Shoot
    public virtual void Shoot()
    {
        if(!isEnter) return;

        Entity dashEntity = enterEntity;

        OnEscape();

        StartCoroutine(Dash(dashEntity, shootDirection.normalized));
    }
    IEnumerator Dash(Entity entity, Vector2 direction)
    {
        entity.OnCatapultShoot_Start(direction);

        float _dashTime = dashTime;

        Vector2 dashDir = direction * dashForce[forceLevel];
        if (direction.x != 0)
        {
            _dashTime *= 1.25f; // 横向斜向的弹射距离稍微长一些
            if (direction.y == 0)
                dashDir.y = 0.5f * dashForce[forceLevel]; // 横向弹射偏上一点
            else
                dashDir.y = 0.75f * dashForce[forceLevel]; // 斜向弹射偏上一点
        }

        entity.SetRigibody(entity.Rigibody.gravityScale, Vector2.zero); // 设置重力，0速度
        entity.Rigibody.AddForce(dashDir, ForceMode2D.Impulse); // 发射

        yield return new WaitForSeconds(_dashTime);

        // 冲刺结束
        entity.OnCatapultShoot_End(direction);
    }

    #endregion

    public virtual void OnInteract()
    {
        if (isEnter == false) return;

        // 切换弹射力等级
        if(forceLevel == dashForce.Length - 1)
            forceLevel = 0;
        else
            forceLevel++;

        ui.UpdateUI(forceLevel);
    }
    public virtual void OnEscape()
    {
        if (isEnter == false) return;

        isEnter = false;
        isExit = true;

        EntityExitCataput(enterEntity, shootDirection * 1.5f);
    }

    IEnumerator ExitCold()
    {
        yield return new WaitForSeconds(enterCold);
        isExit = false;
    }
    #endregion
}
