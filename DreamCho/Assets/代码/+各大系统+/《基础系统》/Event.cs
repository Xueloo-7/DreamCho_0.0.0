using DreamCho;
using System;
using System.Reflection;
using UnityEngine;



public static class Event
{
    #region Game
    public static Action onGameQuit;
    public static Action<string> onSaveData;
    public static Func<string, object> onLoadData;
    public static Action onLoadScene; // 当场景开始加载时
    public static Action onNewSceneStart; // 当新场景加载完成时

    // 全局时间（部分对象不受影响，例如UI和玩家）
    // 受影响对象需要注册这个事件
    public static Action<float> globalTimeScale;

    #endregion

    #region Player
    public static Action<Vector2> playerTeleport;
    public static Action<int> onEnergyCountUpdate;

    #endregion

    #region Skill
    public static Action<Color> onBlackWhiteSwitch; // 黑白切换

    #endregion

    #region Object Function
    public static Action onCollect;
    public static Action onEnterBlackWhitePlatform;
    public static Action onExitBlackWhitePlatform;

    #endregion

    #region Settings
    public static Action<bool> onPostProcessingChanged;
    public static Action<float> onValueBloomChanged;
    public static Action<float> onValueVignetteChanged;
    //public static Action<bool> onMotionBlurChanged;
    public static Action<int> onFpsChanged;
    public static Action<bool> onVsyncChanged;
    //public static Action<bool> onAntiAliasingChanged;
    public static Action<int> onScreenResolutionChanged;
    public static Action<bool> onFullScreenChanged;
    public static Action<int> onRenderingPrecisionChanged;

    [Space(30)]

    // Volume settings
    public static Action<float> onVolumeMainChanged;
    public static Action<float> onVolumeBackgroundChanged;
    public static Action<float> onVolumeSoundChanged;
    public static Action<float> onVolumeFxChanged;
    public static Action<bool> onMuteChanged;

    [Space(30)]

    // Gameplay settings (中文变量名)
    public static Action<bool> on死亡动画加速Changed;
    public static Action<bool> on隐藏UI功能Changed;
    public static Action<bool> on显示血量详情Changed;
    public static Action<bool> on相机智能跟随Changed;
    public static Action<float> onCameraXlerpChanged;
    public static Action<float> onCameraYlerpChanged;
    public static Action<bool> on主页动态壁纸加载Changed;
    public static Action<bool> on出界攻击自动瞄准Changed;
    public static Action<bool> on显示UI提示Changed;
    public static Action<bool> on开启菜单时关闭游戏画面Changed;
    public static Action<int> on视野距离Changed;

    [Space(30)]

    // Language setting
    public static Action<int> onLanguageChanged;
    public static Action<int> onSoundLanguageChanged;

    #endregion

    #region KeyCode
    public static Action onLeftDown;
    public static Action onRightDown;
    public static Action onUpDown;
    public static Action onDownDown;
    public static Action onLeftUp;
    public static Action onRightUp;
    public static Action onUpUp;
    public static Action onDownUp;
    public static Action onPowerDown;
    public static Action onPowerUp;
    public static Action onCharacterDown;
    public static Action onCharacterUp;
    public static Action onAttackDown;
    public static Action onAttack;
    public static Action onAttackUp;
    public static Action<int> onSwitchHarmonizer;
    public static Action onTabDown;
    public static Action onTabUp;
    public static Action onMouseDown;
    public static Action onMouse;
    public static Action onMouseUp;
    public static Action onR_funcDown;

    #endregion
}


public static class EventDebugger
{
    public static void PrintAllEventListeners()
    {
        int totalSubscriptions = 0;

        Debug.Log("=== 事件监听器列表 ===");
        totalSubscriptions += PrintEventListeners(typeof(Event));
        totalSubscriptions += PrintEventListeners(typeof(DreamCho.KeyEvent));

        Debug.Log($"=======================");
        Debug.Log($"总共注册的事件监听数量: {totalSubscriptions}");
    }

    private static int PrintEventListeners(Type eventType)
    {
        FieldInfo[] fields = eventType.GetFields(BindingFlags.Public | BindingFlags.Static);
        int totalCount = 0;

        Debug.Log($"== {eventType.Name} 事件 ==");
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType.IsSubclassOf(typeof(Delegate)) || field.FieldType == typeof(Delegate))
            {
                Delegate del = field.GetValue(null) as Delegate;
                if (del != null)
                {
                    Debug.Log($"事件: {field.Name}");
                    int count = 0;

                    foreach (var method in del.GetInvocationList())
                    {
                        Debug.Log($"  -> 监听方法: {method.Method.DeclaringType}.{method.Method.Name}");
                        count++;
                    }

                    totalCount += count; // 累计事件订阅数
                }
                else
                {
                    Debug.Log($"事件: {field.Name}（无监听器）");
                }
            }
        }

        return totalCount;
    }
}
