using UnityEngine;
using UnityEngine.UI;

public class CatapultForceUI : MonoBehaviour
{
    [SerializeField] Text number;

    public void UpdateUI(int forceLevel)
    {
        number.text = (forceLevel+1).ToString();
    }
}
