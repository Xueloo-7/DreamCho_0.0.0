using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// 统一管理相机相关脚本
/// </summary>
public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] SoundShake soundShakeCam;
    [SerializeField] PlayerFollowCam playerFollowCam;

    private CinemachineCamera highest;
    int highestPriority = 1;

    /// <summary>
    /// Priority 规则：
    /// 0 = defualt
    /// 1 = high
    /// 10 = very high
    /// 如果优先级一样，则新设置的cam会替换掉旧的
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="priority"></param>
    public void SetCameraPriority(CinemachineCamera camera, int priority)
    {
        camera.Priority = highestPriority;
        highest = camera;
        highestPriority++;

        OnSetNewCamera();
    }

    void OnSetNewCamera() // 更新其他相机组件
    {
        soundShakeCam.SetNewCam(highest);
    }
}
