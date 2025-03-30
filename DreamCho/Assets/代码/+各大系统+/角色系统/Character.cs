using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "SO/Character")]
public class Character : ScriptableObject
{
    public Skill skill;
    public Color themeColor;

    public void InitializeSkill()
    {
        skill.Initialize();
    }
    public void Skill()
    {
        skill.Use();
    }
    public void StopSkill()
    {
        skill.Stop();
    }
}