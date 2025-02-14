using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    public Sound[] musicSounds;
    public Sound[] fxSounds;
}
