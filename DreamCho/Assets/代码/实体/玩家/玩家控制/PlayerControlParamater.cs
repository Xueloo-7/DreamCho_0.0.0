using UnityEngine;

[CreateAssetMenu(fileName = "Control Parameter", menuName = "Player/Control_Par")]
public class PlayerControlParamater : ScriptableObject
{
    #region Move
    [Header("Move"), Space]
    public float moveSpeed = 12;
    public float move_deceleration = 80;
    public float fallAcceleration = 100;
    #endregion



    #region Jump
    [Header("Jump"), Space]
    public float jumpForce = 23;
    public float bounceJumpForce = 30; // 下冲弹跳高度
    public float hyperJumpForce = 15; // 冲刺期间跳跃有衰减
    public float superJumpTimeForDash = 0.05f; // 冲刺期间前多少时间内跳跃有加成
    public float hyperJump_moveForce = 50; // 冲刺期间跳跃有移速加成
    public float maxFallSpeed = -20;
    [Tooltip("松开跳跃后下落的速度")] public float fallMultiplier = 2;
    [Tooltip("松开跳跃后过渡到下落的速度")] public float lowJumpMultiplier = 20;
    [Tooltip("土狼时间")] public float coyoteTime = .1f;
    [Tooltip("跳跃预输入时间")] public float jumpBufferTime = .15f;
    public float bounceBufferTime = .15f;
    #endregion



    #region Dash
    [Header("Dash"), Space]
    [Tooltip("冲刺预输入时间")] public float dashBufferTime = .15f;
    public float dashForce = 1500;
    public AnimationCurve dashForceCurve;
    public float dashShakeStrength = 0.5f;
    public float dashShakeFrequency = 2;
    public float dashShakeDuration = .2f;
    public float dashTime = .15f;
    public float dashFrameStop = 0.03f;
    public float dashCold = 0f; // 空中冲刺是没有冷却的
    public float dashGroundCold = 0.3f; // 地面冲刺冷却
    public float maxDashEnergy = 1;
    public float defMaxDash = 1;
    public float dashEnergyRestoreSpeed = .01f;
    public AnimationCurve dashRotateCurve;
    public int ghostCount = 3;
    public float ghostInterval = 0.08f;
    public float ghostFadeOut = 0.3f;
    #endregion
}
