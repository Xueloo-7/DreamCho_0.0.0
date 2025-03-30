using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SoundShake : Singleton<SoundShake>
{
    [SerializeField] CinemachineCamera cam; // 震动摄像机

    private CinemachineCameraOffset camOffset;
    private Coroutine coroutine;
    private Tween tween;

    private Vector2 shakeDirection; // 实时更新，受多个震源影响
    private float currentStrength;

    public static bool isShaking = false;


    void Start()
    {
        SetNewCam(cam);
    }

    #region Shake
    /*IEnumerator LoopShake() // 无限震动测试用
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            Vector2 randomPos = player.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Debug.Log(player.position + "  " + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            Shake(randomPos, Random.Range(1, 3), 2, 0.15f);
        }
    }*/

    // 根据震源与玩家的距离，震源方向，震源强度决定摄像机震动
    public void Shake(Vector2 position, float strength, float frequency, float duration)
    {
        Transform player = FindAnyObjectByType<Player>().transform;

        float distanceWithPlayer = (position - (Vector2)player.position).sqrMagnitude;
        strength = Mathf.Clamp(strength / (distanceWithPlayer + 1f), 0, strength * 10);

        currentStrength = strength;

        // 决定震动方向
        Vector2 direction = ((Vector2)player.position - position).normalized;
        shakeDirection = direction;

        // 开始震动
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ShakeCamera(frequency, duration));
    }

    IEnumerator ShakeCamera(float frequency, float duration) // 一个frequency代表一次弹跳和一次回弹和收回
    {
        if (tween != null) tween.Kill();

        // 震动
        float interval = duration / frequency;
        Vector2 originTargetOffset = Vector2.zero;

        for (int i = 0; i < frequency; i++)
        {
            tween = DOVirtual.Vector2(camOffset.Offset, originTargetOffset + shakeDirection * currentStrength / (i + 1), interval / 3, DoCamera)
            .SetEase(Ease.OutExpo) // 弹出去，快速减速
            .OnComplete(() =>
            {
                tween = DOVirtual.Vector2(camOffset.Offset, originTargetOffset - shakeDirection * currentStrength / 2 / (i + 1), interval / 3, DoCamera)
                .SetEase(Ease.InOutSine) // 回弹，柔和过渡
                .OnComplete(() =>
                {
                    tween = DOVirtual.Vector2(camOffset.Offset, originTargetOffset, interval / 3, DoCamera)
                    .SetEase(Ease.InCubic); // 收回，缓慢回稳
                });
            });

            yield return new WaitForSeconds(interval);
        }
    }

    private void DoCamera(Vector2 move)
    {
        camOffset.Offset = move;
    }
    #endregion

    #region Camera
    public void SetNewCam(CinemachineCamera newCamera)
    {
        cam = newCamera;
        camOffset = cam.GetComponent<CinemachineCameraOffset>();
        if(camOffset == null)
        {
            Debug.LogWarning("No CinemachineCameraOffset found, adding a new one.");
            camOffset = cam.gameObject.AddComponent<CinemachineCameraOffset>();
            camOffset.ApplyAfter = CinemachineCore.Stage.Body;
        }
    }

    #endregion
}