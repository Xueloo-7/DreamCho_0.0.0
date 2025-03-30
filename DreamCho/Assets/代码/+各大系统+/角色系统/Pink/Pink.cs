using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pink Skill", menuName = "SO/Skill/Pink")]
public class Pink : Skill
{
    [SerializeField] Character me;
    [SerializeField] ParticleSystem floralParticle_prefab;
    [SerializeField] bool isFloral; // 花造域开启
    [SerializeField] float domainTime = 5; // 花造域的最高维持时间
    [SerializeField] float remainTime = 3; // 复现完成后的停留时间

    private Player player;
    private List<Position> positions;
    private bool isInterrupted;
    private Coroutine floralCor;
    private SpriteRenderer ghost; // 替身，用于显示留在原地的黑白蓝，和复现移动轨迹的粉
    private Character nextSwitchCharacter; // 花造域结束后切换到的角色
    private ParticleSystem floralParticle;

    public override void Initialize()
    {
        if (player == null) player = FindFirstObjectByType<Player>();
        if (ghost == null)
        {
            ghost = GameObject.Find("Ghost").transform.Find("Floral Replay Ghost").GetComponent<SpriteRenderer>();
            ghost.gameObject.SetActive(false);
        }
        if (floralParticle == null)
        {
            floralParticle = Instantiate(floralParticle_prefab, GameObject.Find("UI 界面").transform);
            floralParticle.gameObject.SetActive(false);
        }
        isFloral = false; // 使得下一次Use的时候isFloral为true
    }

    public override void Stop()
    {
    }

    void OnCharacterSwitch(Character character)
    {
        if (isFloral == true) // 花造域期间，切换角色会直接结束花造域并切换为指定角色
        {
            nextSwitchCharacter = character;
            Use();
        }
    }

    public override void Use()
    {
        isFloral = !isFloral;

        if (isFloral == true)
        {
            nextSwitchCharacter = null;
            floralCor = player.StartCoroutine(FloralDomain());

            // 留下原本角色在原地
            ghost.gameObject.SetActive(true);
            ghost.transform.position = player.transform.position;
            ghost.color = CharacterManager.Instance.GetPreviousCharacter().themeColor;

            // 全局时停
            Event.globalTimeScale.Invoke(0);

            // 滤镜效果
            floralParticle.gameObject.SetActive(true);
            floralParticle.Play();
        }
        else // 提前结束领域或者领域抵达维持上限
        {
            if (floralCor != null) player.StopCoroutine(floralCor);
            player.StartCoroutine(ReplayMovement());

            // ghost替换为粉
            ghost.color = me.themeColor;

            // 恢复时间
            Event.globalTimeScale.Invoke(1);

            // 玩家回到初始位置
            player.transform.position = ghost.transform.position;

            // 玩家切换为上个角色（按技能键结束）or 玩家点按切换角色键切换为指定角色
            CharacterManager.Instance.SwitchCharacter(nextSwitchCharacter == null ?
                CharacterManager.Instance.GetPreviousCharacter() : nextSwitchCharacter);

            // 关闭滤镜
            floralParticle.gameObject.SetActive(false);
        }
    }

    IEnumerator FloralDomain()
    {
        positions = new List<Position>();
        Transform playerSr = player.MainRenderer.transform;

        float recordInterval = 0.05f;  // 每 0.05 秒记录一次
        float timer = 0f;
        float _domainTime = domainTime;
        while (_domainTime > 0)
        {
            timer += Time.deltaTime;
            if (timer >= recordInterval)
            {
                timer = 0f;

                // 记录位置和旋转，以及交互行为
                positions.Add(new Position(playerSr.position, playerSr.rotation));
            }

            _domainTime -= Time.deltaTime;
            yield return null;
        }
        // 技能结束
        Use();
    }
    IEnumerator ReplayMovement()
    {
        if (positions == null || positions.Count == 0)
        {
            yield break;
        }

        int index = 0;

        while (index < positions.Count - 1) // 避免数组越界
        {
            Position startPos = positions[index];
            Position endPos = positions[index + 1];

            float elapsedTime = 0f;
            float moveDuration = 0.05f;  // 让移动与记录间隔匹配

            while (elapsedTime < moveDuration)
            {
                float t = elapsedTime / moveDuration;
                ghost.transform.position = Vector3.Lerp(startPos.position, endPos.position, t);
                ghost.transform.rotation = Quaternion.Slerp(startPos.rotation, endPos.rotation, t);

                // 逐帧推进
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            index++;

            // 检测是否被攻击导致复现失败
            if (isInterrupted)
            {
                Debug.Log("复现被中断");
                EndReplay();
                yield break;
            }
        }

        ghost.transform.SnapRotationTo90(); // 调整最后的旋转角度

        yield return new WaitForSeconds(remainTime);
        EndReplay();
    }

    void EndReplay()
    {
        ghost.gameObject.SetActive(false);

        if (positions != null)
        {
            Debug.Log("Clear: "+positions.Count);
            positions.Clear();
        }
    }

    private void OnEnable() // 初次初始化
    {
        CharacterManager.onCharacterSwitch += OnCharacterSwitch;
        Application.quitting += OnGameQuit;
    }

    void OnGameQuit()
    {
        if (positions != null) positions.Clear();
        CharacterManager.onCharacterSwitch -= OnCharacterSwitch;
    }
}

public class Position
{
    public Vector3 position;
    public Quaternion rotation;
    public Position(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

