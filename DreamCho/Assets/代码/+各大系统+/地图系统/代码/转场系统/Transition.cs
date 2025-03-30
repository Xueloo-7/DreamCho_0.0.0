using UnityEngine;

public class Transition : MonoBehaviour
{
    public void EndTransition()
    {
        TransitionManager.Instance.OnEndTransition();
    }
}
