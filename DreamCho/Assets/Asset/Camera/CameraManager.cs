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
    private CinemachineCamera previous; // 上一个被换下去的Camera，记录以便换回来

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
        // 优先级替换
        camera.Priority = priority; // 上去
        if (highest == null)
        {
            highest = camera;
            return;
        }
        if (highest.Priority <= priority)
        {
            highest.Priority = 0; // 下去
            previous = highest; // 记录
            highest = camera; // 替换
        }

        OnSetNewCamera();
    }

    /// <summary>
    /// 换回上一个被替换掉的Camera
    /// </summary>
    public void RollBack()
    {
        if(previous != null)
        {
            highest.Priority = 0; // 下去
            previous.Priority = 1; // 回来
            highest = previous; // 替换回来
            previous = null;

            OnSetNewCamera();
        }
    }

    void OnSetNewCamera() // 更新其他相机组件
    {
        soundShakeCam.SetNewCam(highest);
    }
}
