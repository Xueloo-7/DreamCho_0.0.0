using DG.Tweening;
using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr; // 单体
    [SerializeField] SpriteRenderer[] srs; // 所有sr
    [SerializeField] Rigidbody2D rb;

    public SpriteRenderer MainRenderer { get => sr; }
    public Rigidbody2D Rigibody { get => rb; }

    private Color[] originColor;
    private bool isHidden; // 是否处于隐藏自己的状态

    #region Transform
    public virtual void SetTransform(Vector2 position, Quaternion rotation, Vector3 localScale, Transform parent)
    {
        Debug.Log("Parent");
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = localScale;
        transform.SetParent(parent);
    }

    #endregion

    #region Sr
    public virtual void Hidden(bool onlyBody)
    {
        if (isHidden) return;

        if (onlyBody) // 隐藏本体
        {
            originColor = new Color[1];
            originColor[0] = sr.color;
            sr.DOKill();
            sr.color = Color.clear;
        }
        else // 隐藏所有Sr
        {
            originColor = new Color[srs.Length];
            for (int i = 0; i < srs.Length; i++)
            {
                originColor[i] = srs[i].color;
                srs[i].DOKill();
                srs[i].color = Color.clear;
            }
        }
        isHidden = true;
    }
    public virtual void Showing()
    {
        if (!isHidden) return;
        if (originColor.Length == 1)
        {
            sr.color = originColor[0];
        }
        else
        {
            for (int i = 0; i < srs.Length; i++)
            {
                srs[i].color = originColor[i];
            }
        }
        originColor = null;
        isHidden = false;
    }
    #endregion

    #region Rb
    public virtual void SetRigibody(RigidbodyType2D bodyType = RigidbodyType2D.Dynamic)
    {
        rb.bodyType = bodyType;
    }
    public virtual void SetRigibody(float gravityScale = 1, Vector2 velocity = default)
    {
        rb.gravityScale = gravityScale;
        rb.linearVelocity = velocity;
    }

    #endregion

    #region State
    public virtual void ResetState()
    {
        // 重置一些基础属性
    }

    #endregion

    #region Props Function
    public virtual void OnEnterCatapult() // 当进入弹射炮内部
    {}
    public virtual void OnCatapultShoot_Start(Vector2 direction) // 当被弹射炮发射
    {}
    public virtual void OnCatapultShoot_End(Vector2 direction) // 当弹射炮发射结束
    {}
    #endregion
}
