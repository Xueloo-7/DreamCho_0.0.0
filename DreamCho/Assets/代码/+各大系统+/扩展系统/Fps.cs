using UnityEngine;
using UnityEngine.UI;

public class Fps : MonoBehaviour
{
    [SerializeField] private Text fpsText;
    
    private void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";
    }
}