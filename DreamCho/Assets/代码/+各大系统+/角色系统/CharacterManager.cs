using UnityEngine;
using DreamCho;
using System;

public class CharacterManager : Singleton<CharacterManager>
{
    [SerializeField] Character[] characters = new Character[3]; // 固定最多三个角色

    [Header("OnCharacterSwitch")]
    private SpriteRenderer updatePlayerSr;
    private Material dynamicColor;

    private int character_index = 0;
    private int prev_character_index = -1;
    private bool canUseSkill = true;
    private bool canSwitchCharacter = true; 

    public static Action<Character> onCharacterSwitch;

    private new void Awake()
    {
        base.Awake();

        if (characters.Length > 3)
        {
            Debug.LogError("角色数量超过三个，自动截取前三个！");
            Array.Resize(ref characters, 3);
        }
        dynamicColor = Resources.Load<Material>("Dynamic Color");
        if (dynamicColor == null) Debug.LogError("找不到 Dynamic Color 材质");

        Event.onNewSceneStart += OnNewSceneStart;
        KeyEvent.onNextCharacterDown += SwitchCharacter;
        KeyEvent.onPrevioousCharacterDown += SwitchCharacter;
        KeyEvent.onSkillDown += UseSkill;
        KeyEvent.onSkillUp += UseSkill;
        onCharacterSwitch += OnCharacterSwitch;
    }

    private void Start()
    {
        OnCharacterSwitch(characters[character_index]); // 初始化角色
    }
    private new void OnDestroy()
    {
        base.OnDestroy();
        Event.onNewSceneStart -= OnNewSceneStart;
        KeyEvent.onNextCharacterDown -= SwitchCharacter;
        KeyEvent.onPrevioousCharacterDown -= SwitchCharacter;
        KeyEvent.onSkillDown -= UseSkill;
        KeyEvent.onSkillUp -= UseSkill;
    }
    void OnNewSceneStart()
    {
        updatePlayerSr = FindFirstObjectByType<Player>().MainRenderer;
        canUseSkill = true;
        canSwitchCharacter = true;
    }

    public void SwitchCharacter(Character character)
    {
        if (!canSwitchCharacter) return;
        if (characters[character_index] == character) return; // 防止重复切换

        // 关闭上一个角色的技能
        characters[character_index].skill.Stop();
        prev_character_index = character_index;

        for (int i = 0; i < characters.Length; i++) // 更新索引
            if (characters[i] == character)
                character_index = i;

        onCharacterSwitch?.Invoke(character); // 切换为下一个角色
    }
    public void SwitchCharacter(int index_increase)
    {
        if (!canSwitchCharacter) return;
        if (index_increase == 0) return; // 防止重复切换

        // 关闭上一个角色的技能
        characters[character_index].skill.Stop();
        prev_character_index = character_index;

        // 索引调整
        character_index += index_increase;
        if(character_index < 0) character_index = characters.Length - 1;
        else if (character_index >= characters.Length) character_index = 0;

        onCharacterSwitch?.Invoke(characters[character_index]); // 切换为下一个角色
    }

    void UseSkill(bool use)
    {
        if (canUseSkill && use)
        {
            characters[character_index].Skill(); // 按下技能键，使用技能
        }
        else if(!use)
        {
            characters[character_index].skill.Stop(); // 松开技能键，停止技能
        }
    }
    public void SetSkillAvailable(bool canUse)
    {
        canUseSkill = canUse;
    }
    public void SetSwitchAvailable(bool canSwitch)
    {
        canSwitchCharacter = canSwitch;
    }

    void OnCharacterSwitch(Character character)
    {
        UpdatePlayerSr(character);
        character.InitializeSkill();
    }

    public void UpdatePlayerSr(Character character)
    {
        if (updatePlayerSr != null)
        {
            updatePlayerSr.color = character.themeColor;
            dynamicColor.SetColor("_Color", character.themeColor);
        }
    }

    public Character GetCharacter()
    {
        return characters[character_index];
    }
    public Character GetPreviousCharacter()
    {
        if(prev_character_index != -1)
            return characters[prev_character_index];
        else return null;
    }
}
