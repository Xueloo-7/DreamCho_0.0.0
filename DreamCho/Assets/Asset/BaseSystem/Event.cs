using System;
using UnityEngine;
using UnityEngine.Events;



public static class Event
{
    #region Game
    public static Action onGameQuit;
    public static Action<string> onSaveData;
    public static Func<string, object> onLoadData;
    #endregion



    #region Player
    public static Action<Vector2> playerTeleport;
    public static Action<int> onEnergyCountUpdate;

    #endregion



    #region Object Function

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



    #region Touch
    public static Action<int> onTouchDown;
    public static Action<int> onTouch;
    public static Action<int> onTouchUp;
    public static Action onTouchMove;
    public static Action onTouchStay;
    public static Action<int> onTouchCancel;
    #endregion



    #region 开发者

    public static Action<bool> godModeOpen;
    public static Action<float> godMoveSpeedChange;
    public static Action<int> platformChange;
    public static Action<float> timescaleChange;

    #endregion
}
