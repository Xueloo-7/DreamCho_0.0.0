using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public abstract void Initialize();  
    public abstract void Use();
    public abstract void Stop();
}
