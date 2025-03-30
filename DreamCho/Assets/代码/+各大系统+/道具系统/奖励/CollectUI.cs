using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Text text;

    private int collectCount = -1;

    private void Start()
    {
        Event.onCollect += OnCollect;

        OnCollect();
    }
    private void OnDisable()
    {
        Event.onCollect -= OnCollect;
    }

    private void OnCollect()
    {
        collectCount++;

        text.text = "x " + collectCount.ToString();
        canvasGroup.alpha = 1;

        StartCoroutine(CloseUI());
    }

    IEnumerator CloseUI()
    {
        yield return new WaitForSeconds(5);
        canvasGroup.alpha = 0;
    }
}
