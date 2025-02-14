using UnityEngine;

[CreateAssetMenu(fileName = "Control Parameter", menuName = "Player/Control_Par")]
public class PlayerControlParamater : ScriptableObject
{
    #region Move
    [Header("Move"), Space]
    public float moveSpeed = 8;
    public float move_deceleration = 50;
    public float fallAcceleration = 100;
    #endregion



    #region Jump
    [Header("Jump"), Space]
    public float jumpForce = 20;
    public float maxFallSpeed = -20;
    [Tooltip("松开跳跃后下落的速度")] public float fallMultiplier = 2;
    [Tooltip("松开跳跃后过渡到下落的速度")] public float lowJumpMultiplier = 50;
    [Tooltip("土狼时间")] public float coyoteTime = .15f;
    [Tooltip("跳跃预输入时间")] public float jumpBufferTime = .15f;
    #endregion



    #region Dash
    [Header("Dash"), Space]
    [Tooltip("冲刺预输入时间")] public float dashBufferTime = .15f;
    public float dashForce = 20;
    public AnimationCurve dashForceCurve;
    public float dashShakeStrength = 5;
    public float dashShakeFrequency = 2;
    public float dashShakeDuration = .2f;
    public float dashTime = .05f;
    public float dashFrameStop = 0.05f;
    public float dashCold = 0.1f;
    public float maxDashEnergy = 3;
    public float defMaxDash = 3;
    public float dashEnergyRestoreSpeed = .75f;
    public AnimationCurve dashRotateCurve;
    public int ghostCount = 3;
    public float ghostInterval = 0.05f;
    public float ghostFadeOut = 0.3f;
    #endregion
}
