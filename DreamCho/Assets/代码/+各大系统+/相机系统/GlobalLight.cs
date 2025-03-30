using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] Volume volume;

    private void Awake()
    {
        Event.onLoadScene += OnLoadScene;
        Event.onNewSceneStart += OnNewSceneStart;
    }
    private void OnDestroy()
    {
        Event.onLoadScene -= OnLoadScene;
        Event.onNewSceneStart -= OnNewSceneStart;
    }

    void OnLoadScene()
    {
        light2D.enabled = false;
        volume.enabled = false;
    }

    void OnNewSceneStart()
    {
        light2D.enabled = true;
        volume.enabled = true;
    }
}
