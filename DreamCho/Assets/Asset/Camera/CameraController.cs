using Unity.Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using DreamCho;

public class CameraController : Singleton<CameraController>
{
    #region Offset Control
    [SerializeField] private Camera[] syncCams; // 场景中所有需要同步玩家画面的摄像机
    [SerializeField] private Camera synchronizedCam;
    [SerializeField] private CinemachineCamera highestCam; // 场景中所有需要同步玩家画面的摄像机
    [SerializeField] float defaultLens = 70;
    [SerializeField] float offsetLens = 1;
    [SerializeField] float lens_lerpSpeed = 1;
    [SerializeField] float X_offset = 5;
    [SerializeField] float Y_offset = 5;
    [SerializeField] float X_lerpSpeed = 5;
    [SerializeField] float Y_lerpSpeed = 5;
    //[SerializeField] float Y_clamp = 5;
    [SerializeField] float viewDistance = 5;
    [SerializeField] float viewLerpSpeed = 20;
    [SerializeField] float viewTime = 1;
    [SerializeField] float attackOffset = 1;
    [SerializeField] float attackLerpSpeed = 10;
    [SerializeField] float attacklimit = 500;
    [SerializeField] CinemachineCamera cam;

    public bool enableCamLerp = true;
    public bool enableCamShake = true;
    [SerializeField] bool canView = true;
    public bool CanView { private get { return canView; } set { canView = value; } }

    public CinemachinePositionComposer transposer;
    private float viewCounter;
    private Coroutine moveZCor;
    #endregion

    #region Shake Control
    private CinemachineBasicMultiChannelPerlin perlinNoise;
    private float duration;
    float amplitudeVelocity = 0f;
    float frequencyVelocity = 0f;
    #endregion

    [SerializeField] Rigidbody2D playerRb;

    void Start()
    {
        transposer = cam.GetComponent<CinemachinePositionComposer>();
        perlinNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        highestCam = cam;
    }

    void Update()
    {
        if (enableCamLerp)
        {

            float x = transposer.TargetOffset.x;
            float y = transposer.TargetOffset.y;
            Vector3 finalOffset = Vector3.zero;
            Vector3 lerpResult = Vector3.zero;

            if (playerRb.linearVelocity.sqrMagnitude > 0.1f) // 玩家移动时
            {
                viewCounter = 0;
                float y_offset = 0;
                if (playerRb.linearVelocity.y > 0.01f)
                    y_offset = Y_offset;
                else if (playerRb.linearVelocity.y < -0.01f)
                    y_offset = -Y_offset;

                lerpResult += new Vector3
                (Mathf.Lerp(x, playerRb.linearVelocity.x * X_offset, Time.fixedDeltaTime * X_lerpSpeed),
                Mathf.Lerp(y, y_offset, Time.deltaTime * Y_lerpSpeed), 0); // 动量偏移

                finalOffset = new Vector3(playerRb.linearVelocity.x * X_offset, y_offset, 0);
            }
            else // 玩家几乎静止时
            {
                lerpResult += new Vector3
                (Mathf.Lerp(x, 0, Time.deltaTime * X_lerpSpeed),
                Mathf.Lerp(y, ViewHandle() ? Key.rawAxisY * viewDistance : 0, Time.fixedDeltaTime * viewLerpSpeed), 0); // 动量偏移

                finalOffset = new Vector3(0, ViewHandle() ? Key.rawAxisY * viewDistance : 0, 0);
            }

            transposer.TargetOffset = lerpResult;

            DynamicLens(finalOffset);
        }

        if (enableCamShake && perlinNoise != null)
        {
            // 恢复shake
            perlinNoise.AmplitudeGain = Mathf.SmoothDamp(perlinNoise.AmplitudeGain, 0, ref amplitudeVelocity, duration);
            perlinNoise.FrequencyGain = Mathf.SmoothDamp(perlinNoise.FrequencyGain, 0, ref frequencyVelocity, duration);
        }
    }

    #region Lens and Offset
    private void DynamicLens(Vector3 finalOffset)
    {
        // 根据与实时调整Lens
        float offsetBetweenTarget = (transposer.TargetOffset - finalOffset).magnitude;
        float lens = Mathf.Lerp(cam.Lens.FieldOfView, defaultLens + (offsetBetweenTarget * offsetLens)
            , Time.deltaTime * lens_lerpSpeed);

        cam.Lens.FieldOfView = lens; // 同步当前摄像机的FOV
        for (int i = 0; i < syncCams.Length; i++) // 同步其他【玩家跟踪摄像机】的FOV
        {
            syncCams[i].fieldOfView = synchronizedCam.fieldOfView;
        }
    }

    public void ResetY_Offset()
    {
        transposer.TargetOffset = new Vector3(transposer.TargetOffset.x, 0, transposer.TargetOffset.z);
    }
    public void ResetX_Offset()
    {
        transposer.TargetOffset = new Vector3(0, transposer.TargetOffset.y, transposer.TargetOffset.z);
    }
    public void ResetBoth_Offset()
    {
        ResetX_Offset(); ResetY_Offset();
    }
    public void ResetTransform()
    {
        transposer.Damping.x = 0;
        transposer.Damping.y = 0;
        StartCoroutine(_ResetTransform());
        IEnumerator _ResetTransform()
        {
            yield return null;
            transposer.Damping.x = 1;
            transposer.Damping.y = 1;
        }
    }

    private bool ViewHandle()
    {
        if (canView)
        {
            if (Key.rawAxisY != 0)
            {
                viewCounter += Time.deltaTime;

                if (viewCounter >= viewTime)
                {
                    return true;
                }

                return false;
            }

            viewCounter = 0;

            return false;

        }
        else return false;
    }

    public void SetCamera_Z(float target_Z, float lerpSpeed)
    {
        if (moveZCor != null) StopCoroutine(moveZCor);
        moveZCor = StartCoroutine(MoveToword_Z(target_Z, lerpSpeed));
    }

    public static void SetZ(float target_Z, float lerpSpeed)
    {
        if (Instance != null)
        {
            if (Instance.moveZCor != null) Instance.StopCoroutine(Instance.moveZCor);
            Instance.moveZCor = Instance.StartCoroutine(Instance.MoveToword_Z(target_Z, lerpSpeed));
        }
    }

    IEnumerator MoveToword_Z(float target_Z, float lerpSpeed)
    {
        float z = cam.Lens.FieldOfView;
        while (z != target_Z)
        {
            cam.Lens.FieldOfView = Mathf.MoveTowards(z, target_Z, Time.deltaTime * lerpSpeed);
            z = cam.Lens.FieldOfView;

            yield return null;
        }
    }

    public static float GetZ()
    {
        if (Instance != null)
        {
            return Instance.cam.Lens.FieldOfView;
        }
        else
        {
            return 0;
        }
    }

    #endregion

    #region Shake
    private float shakeTimer = 0f;
    //private bool isShaking = false;
    private Coroutine shakeCoroutine;
    public static void ShakeCamera(float amplitude, float frequency, float duration, Ease ease = Ease.Linear)
    {
        if (Instance != null) Instance._ShakeCamera(amplitude, frequency, duration, ease);
    }

    public void _ShakeCamera(float _amplitude, float _frequency, float _duration, Ease ease = Ease.Linear)
    {
        /*// 如果正在震动，停止当前恢复协程
        if (shakeCoroutine != null)
        {
            //StopCoroutine(shakeCoroutine);
        }*/
        if (enableCamShake && perlinNoise != null)
        {
            // 更新振幅和频率
            perlinNoise.AmplitudeGain = Mathf.Max(perlinNoise.AmplitudeGain, _amplitude);
            perlinNoise.FrequencyGain = Mathf.Max(perlinNoise.FrequencyGain, _frequency);

            // 更新计时器和持续时间
            if (_duration > shakeTimer)
            {
                shakeTimer = _duration;
                shakeCoroutine = StartCoroutine(RestoreShake(ease));
            }
            //shakeTimer = Mathf.Max(shakeTimer, _duration);

            // 开始新的恢复协程
        }

    }

    private IEnumerator RestoreShake(Ease ease)
    {
        //isShaking = true;
        float elapsed = 0f;
        float initialAmplitude = perlinNoise.AmplitudeGain;
        float initialFrequency = perlinNoise.FrequencyGain;

        while (elapsed < shakeTimer)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shakeTimer;

            // 使用缓动效果平滑恢复
            perlinNoise.AmplitudeGain = Mathf.Lerp(initialAmplitude, 0, t);
            perlinNoise.FrequencyGain = Mathf.Lerp(initialFrequency, 0, t);

            yield return null;
        }

        // 确保完全恢复到 0
        perlinNoise.AmplitudeGain = 0;
        perlinNoise.FrequencyGain = 0;
        shakeTimer = 0;
        //isShaking = false;


    }

    #endregion

    #region Cinemachine Priority Manage
    public void SetHighestCam(CinemachineCamera camera)
    {
        if (highestCam != null) highestCam.Priority = -100;
        if (camera == null)
        {
            camera = cam;
            perlinNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
        else
        {
            perlinNoise = camera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
        camera.Priority = 100;
        highestCam = camera;
    }

    #endregion
}