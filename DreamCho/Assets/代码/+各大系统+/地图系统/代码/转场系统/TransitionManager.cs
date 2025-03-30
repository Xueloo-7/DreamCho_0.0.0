using System;
using UnityEngine;

public class TransitionManager : Singleton<TransitionManager>
{
    [Serializable]
    class TransitionData
    {
        public Animator animator;
        public TransitionType transitionType;
    }

    [SerializeField] TransitionData[] transitions;

    private bool isTransitioning;
    private GameObject activeTransition;
    private Action endTranisitionAction;

    private void Start()
    {
        // 测试
        //CallTransition("左侧滑入", true);
    }

    public void CallTransition(TransitionType transitionType, bool open, Action action = null)
    {
        if (isTransitioning) // 确保只有一个转场在进行
            return;

        isTransitioning = true;
        endTranisitionAction = action; // 注册转场结束后的回调

        foreach (var transition in transitions)
        {
            if (transition.transitionType == transitionType)
            {
                // 检查 Animator 是否有 OnEndTransition 事件
                if (!HasEndTransitionEvent(transition.animator))
                {
                    Debug.LogWarning($"转场动画 {transitionType} 没有 OnEndTransition 事件！");
                    isTransitioning = false;
                    return;
                }

                if (open)
                {
                    activeTransition = transition.animator.gameObject;
                    transition.animator.gameObject.SetActive(true); // 激活转场动画
                }
                else
                    transition.animator.SetTrigger("Close"); // 关闭转场动画
                break;
            }
        }
    }

    public void CloseTransition()
    {
        if(activeTransition != null)
            activeTransition.SetActive(false);
    }

    public void OnEndTransition()
    {
        isTransitioning = false;
        endTranisitionAction?.Invoke();
    }

    private bool HasEndTransitionEvent(Animator animator)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return false;

        // 遍历 Animator 里的所有 AnimationClip
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in controller.animationClips)
        {
            if(clip.name.Contains("默认动画")) // 跳过默认动画
                continue;
            foreach (AnimationEvent animEvent in clip.events)
            {
                if (animEvent.functionName != "EndTransition")
                {
                    return false; // 没找到 OnEndTransition 事件
                }
            }
        }

        return true; // 所有动画都有 OnEndTransition 事件
    }
}
