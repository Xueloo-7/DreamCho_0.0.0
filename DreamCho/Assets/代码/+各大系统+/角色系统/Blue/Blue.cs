using DG.Tweening;
using DreamCho;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Blue Skill", menuName = "SO/Skill/Blue")]
public class Blue : Skill
{
    [SerializeField] bool isQuantum; // 是否处于量子状态
    [SerializeField] float quantumSpeed = 30; // 量子状态下的速度
    [SerializeField] float quantumMoveDeceleration = 60; // 量子状态下的减速度
    [SerializeField] float min_quantumSpeed = 12; // 初始加速度，用于定义色差效果的最小值
    [SerializeField] float continuousCold = 0.5f; // 技能松开后再接下一次的冷却
    [SerializeField] float penaltyCold = 3f; // 能量耗尽的惩罚冷却
    [SerializeField] int maxEnergy = 3;

    [Header("Dynamic")]
    [SerializeField] float debugEnergy;
    private float energy; // 能量，每秒消耗1能量

    private PlayerController control; // 控制
    private Player player;
    private ChromaticAberration effect;
    private Coroutine quantumCor;

    private float coldCounter;

    public override void Initialize()
    {
        // 确保以下三个都是唯一的
        if (control == null) control = FindFirstObjectByType<PlayerController>();
        if (player == null) player = FindFirstObjectByType<Player>();
        Volume volume = FindFirstObjectByType<Volume>();
        volume.profile.TryGet(out effect);
        if (effect == null) Debug.LogError("没有ChromaticAberration");

        if (Key.GetButton(Key.K.Power)) Use(); // 按下技能键，直接进入量子态

        player.StartCoroutine(RestoreEnergy());
    }

    public override void Stop() // 松开技能，退出量子态
    {
        if (isQuantum == false)
            return;

        Debug.Log("Stop");

        isQuantum = false;
        control.SetMove(); // 重置移动速度
        control.CanJump = true; // 恢复跳跃
        control.CanDash = true; // 恢复冲刺
        player.MainRenderer.ChangeAlpha(1f);
        if(quantumCor != null) player.StopCoroutine(quantumCor);
        FadeOutEffect(effect);

        coldCounter = energy > 0 ? continuousCold : penaltyCold;
    }

    public override void Use() // 使用技能，进入量子态
    {
        if (isQuantum == true || coldCounter > 0)
            return;

        isQuantum = true;
        control.SetMove(quantumSpeed, quantumMoveDeceleration); // 设置量子移动速度
        control.CanJump = false; // 不能跳跃
        control.CanDash = false; // 不能冲刺
        player.MainRenderer.ChangeAlpha(0.2f);
        quantumCor = player.StartCoroutine(Quantum());
    }

    IEnumerator Quantum()
    {
        while (true)
        {
            // intensity随玩家速度增高，玩家速度>=quantumSpeed, Intensity=1 ; 玩家速度<=min_quantumSpeed, Intensity=0
            float percent = Mathf.InverseLerp(min_quantumSpeed, quantumSpeed, Mathf.Abs(player.Rigibody.linearVelocity.x));
            effect.intensity.Override(percent);

            energy -= Time.deltaTime;
            if (energy <= 0) // 能量不足，直接退出
            {
                quantumCor = null;
                Stop();
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator RestoreEnergy()
    {
        while (true)
        {
            if (!isQuantum && energy < maxEnergy)
            {
                energy += Time.deltaTime;
                energy = Mathf.Clamp(energy, 0, maxEnergy);
            }

            debugEnergy = energy;

            if (coldCounter > 0)
            {
                coldCounter -= Time.deltaTime;
            }

            yield return null;
        }
    }

    void FadeOutEffect(ChromaticAberration chromaticAberration)
    {
        // 淡出effect效果
        DOVirtual.Float(chromaticAberration.intensity.value, 0, continuousCold, (float value) =>
        {
            chromaticAberration.intensity.value = value;
        });

    }
}