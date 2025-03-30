using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "SO/Skill/Black And White")]
public class BlackAndWhite : Skill
{
    [SerializeField] ColorType colorType;

    public override void Initialize()
    {
        BlackWhiteSwitch(colorType); // 初始化黑白
    }

    public override void Stop()
    {

    }

    public override void Use()
    {
        colorType = colorType == ColorType.Black ? ColorType.White : ColorType.Black; // 切换黑白

        BlackWhiteSwitch(colorType); 
    }

    void BlackWhiteSwitch(ColorType colorType) // 黑白切换效果
    {
        Character character = CharacterManager.Instance.GetCharacter(); // 获取当前角色

        character.themeColor = colorType == ColorType.Black ? Color.black : Color.white; // 切换主题颜色

        CharacterManager.Instance.UpdatePlayerSr(character); // 更新玩家颜色

        Event.onBlackWhiteSwitch?.Invoke(character.themeColor); // 触发黑白切换事件
    }

    enum ColorType
    {
        Black,
        White
    }
}