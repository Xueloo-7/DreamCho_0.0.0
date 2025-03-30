using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashEnergyUI : MonoBehaviour
{
    [SerializeField] private Image iconPrefab; // 预制的能量格（作为高亮层）
    [SerializeField] private PlayerController controller; // 角色控制器
    [SerializeField] private Color activeColor = Color.white; // 高亮颜色
    [SerializeField] private Color fillColor = new Color(.75f, .75f, .75f, 1); // 高亮颜色
    [SerializeField] private Color inactiveColor = new Color(1, 1, 1, 0.3f); // 低亮颜色

    private List<Image> activeIcons = new List<Image>(); // 存储高亮层
    private List<Image> inactiveIcons = new List<Image>(); // 存储低亮层
    private int maxEnergy;

    private void Start()
    {
        if(controller == null)
            controller = FindFirstObjectByType<PlayerController>();

        maxEnergy = (int)controller.ControlParameter.maxDashEnergy;
        InitializeIcons();

        // 订阅体力上限变化事件
        Event.onEnergyCountUpdate += OnEnergyCountUpdate;
    }

    private void OnDestroy()
    {
        // 取消订阅，防止内存泄露
        Event.onEnergyCountUpdate -= OnEnergyCountUpdate;
    }

    private void InitializeIcons()
    {
        // 先清空现有的图标，防止重复创建
        foreach (var icon in activeIcons) Destroy(icon.gameObject);
        foreach (var icon in inactiveIcons) Destroy(icon.gameObject);
        activeIcons.Clear();
        inactiveIcons.Clear();

        for (int i = 0; i < maxEnergy; i++)
        {
            // 生成高亮层（动态变化）
            Image activeIcon = Instantiate(iconPrefab, transform);
            //activeIcon.color = activeColor;
            activeIcons.Add(activeIcon);

            // 生成低亮层（始终存在）
            Image inactiveIcon = Instantiate(iconPrefab, activeIcon.transform);
            inactiveIcon.transform.localScale = Vector3.one;
            inactiveIcon.transform.position = activeIcon.transform.position;
            inactiveIcon.color = inactiveColor;
            inactiveIcons.Add(inactiveIcon);
        }

        UpdateIcons();
    }

    private void Update()
    {
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        float dashEnergy = controller.GetDashEnergy();
        int fullIcons = Mathf.FloorToInt(dashEnergy); // 完全填充的图标数量
        float partialFill = dashEnergy - fullIcons; // 最后一个填充部分

        for (int i = 0; i < maxEnergy; i++)
        {
            if (i < fullIcons)
            {
                activeIcons[i].color = activeColor;
                activeIcons[i].fillAmount = 1f; // 完全填充
            }
            else if (i == fullIcons)
            {
                activeIcons[i].color = fillColor;
                activeIcons[i].fillAmount = partialFill; // 只填充部分
            }
            else
                activeIcons[i].fillAmount = 0f; // 为空
        }
    }

    // 当体力上限改变时更新图标
    private void OnEnergyCountUpdate(int newMaxEnergy)
    {
        maxEnergy = newMaxEnergy;
        InitializeIcons();
    }
}
