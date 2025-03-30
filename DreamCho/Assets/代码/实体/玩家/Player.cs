using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] GameObject player;
    [SerializeField] PlayerController controller;
    [SerializeField] TrailRenderer trail;
    [SerializeField] ParticleSystem[] particles;
    [SerializeField] Collider2D deathCollider; // 挤压死亡检测的碰撞体
    [SerializeField] SpriteRenderer edgeLineVisual; // 边缘线

    private void Start()
    {
        Event.playerTeleport += Teleport;
        Event.onEnterBlackWhitePlatform += OnEnterBlackWhitePlatform;
        Event.onExitBlackWhitePlatform += OnExitBlackWhitePlatform;
    }
    private void OnDestroy()
    {
        Event.playerTeleport -= Teleport;
        Event.onEnterBlackWhitePlatform -= OnEnterBlackWhitePlatform;
        Event.onExitBlackWhitePlatform -= OnExitBlackWhitePlatform;

        StopAllCoroutines();
    }

    #region Function
    public void Teleport(Vector2 position)
    {
        trail.emitting = false;
        StartCoroutine(T(position));
    }
    IEnumerator T(Vector2 position)
    {
        yield return new WaitForEndOfFrame();
        player.transform.position = position;
    }

    private void OnEnterBlackWhitePlatform()
    {
        //controller.SetDashGhostColor(Color.grey);
        //edgeLineVisual.gameObject.SetActive(true);
    }
    private void OnExitBlackWhitePlatform()
    {
        //controller.SetDashGhostColor(MainRenderer.color);
        //edgeLineVisual.gameObject.SetActive(false);
    }

    #endregion

    #region Override
    #region Transform
    public override void SetTransform(Vector2 position, Quaternion rotation, Vector3 localScale, Transform parent)
    {
        // base.SetTransform(position, rotation, localScale);
        transform.rotation = Quaternion.identity;
        MainRenderer.transform.rotation = rotation;
        transform.position = position;
        transform.localScale = localScale;
        transform.SetParent(parent);
    }

    #endregion

    #region Sr
    public override void Hidden(bool onlyBody)
    {
        base.Hidden(onlyBody);

        foreach (var particle in particles)
        {
            var main = particle.main;
            main.startColor = Color.clear;
        }

        trail.emitting = false;
        controller.HiddenDirHint = true;
    }
    public override void Showing()
    {
        base.Showing();

        foreach (var particle in particles)
        {
            var main = particle.main;
            main.startColor = Color.white;
        }

        trail.emitting = true;
        controller.HiddenDirHint = false;
    }
    #endregion

    #region Rb
    public override void SetRigibody(RigidbodyType2D bodyType)
    {
        base.SetRigibody(bodyType);
    }
    public override void SetRigibody(float gravityScale = 1, Vector2 velocity = default)
    {
        base.SetRigibody(gravityScale, velocity);
    }
    #endregion

    #region State
    public override void ResetState()
    {
        base.ResetState();

        controller.ResetState();
    }
    #endregion

    #region Props Function
    public override void OnEnterCatapult()
    {
        base.OnEnterCatapult();
        CharacterManager.Instance.SetSkillAvailable(false);
        CharacterManager.Instance.SetSwitchAvailable(false);
        deathCollider.enabled = false;
        controller.OnEnterCatapult();
    }
    public override void OnCatapultShoot_Start(Vector2 direction)
    {
        base.OnCatapultShoot_Start(direction);
        CharacterManager.Instance.SetSkillAvailable(true);
        CharacterManager.Instance.SetSwitchAvailable(true);
        deathCollider.enabled = true;
        controller.OnCatapultShoot_Start(direction);
    }
    public override void OnCatapultShoot_End(Vector2 direction)
    {
        base.OnCatapultShoot_End(direction);
        controller.OnCatapultShoot_End(direction);
    }

    #endregion

    #endregion
}
