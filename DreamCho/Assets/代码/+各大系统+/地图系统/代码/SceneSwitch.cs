using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum TransitionType
{
    左侧滑入
}
public class SceneSwitch : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] TransitionType transitionType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchScene();
        }
    }

    public void SwitchScene()
    {
        // 加载转场
        TransitionManager.Instance.CallTransition(transitionType, true, () =>
        {
            // 转场结束后加载场景
            TransitionManager.Instance.StartCoroutine(LoadScene());
        });
    }

    private IEnumerator LoadScene()
    {
        DOTween.KillAll(); // 停止所有 DOTween 动画
        Event.onLoadScene?.Invoke(); // 通知开始加载新场景

        var async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            yield return null;
        }

        Debug.Log($"场景 {sceneName} 加载完成！");
        Event.onNewSceneStart?.Invoke(); // 通知新场景加载完成
        TransitionManager.Instance.CallTransition(transitionType, false, ()=> TransitionManager.Instance.CloseTransition());
    }
}
