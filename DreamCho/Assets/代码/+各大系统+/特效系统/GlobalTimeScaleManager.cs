using UnityEngine;

public class GlobalTimeScaleManager : Singleton<GlobalTimeScaleManager>
{
    [SerializeField] ParticleSystem[] particle;

    private void Start()
    {
        Event.globalTimeScale += GlobalTimeScale;
    }
    private new void OnDestroy()
    {
        base.OnDestroy();
        Event.globalTimeScale -= GlobalTimeScale;
    }

    void GlobalTimeScale(float timeScale)
    {
        foreach (var par in particle)
        {
            var main = par.main;
            main.simulationSpeed = timeScale;
        }
    }
}
