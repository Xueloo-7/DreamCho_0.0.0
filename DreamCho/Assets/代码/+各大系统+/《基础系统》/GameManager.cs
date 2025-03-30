using DG.Tweening;
using System;
using UnityEngine;

public class GameManager : PersistenceSingleton<GameManager>
{
    public static AppMode appMode { get; private set; }
    public static Action<AppMode> onAppModeChanged; // 平台切换事件
    [SerializeField] string forcePlatform; // debug 强制进入平台

    public static GameData gameData { get; private set; }
    public static GameSettings gameSettings { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        DOTween.SetTweensCapacity(500, 50); // 提前设置好tween的容量上限，优化性能

        InitialPlatform();

        LoadData();
    }
    private void Start()
    {
        Event.onNewSceneStart?.Invoke();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftShift))
        {
            EventDebugger.PrintAllEventListeners();
        }
    }

    #region Platform
    void InitialPlatform()
    {
        if (forcePlatform != "")
        {
            // 强制切换为对应平台
            switch (forcePlatform)
            {
                case "PC":
                    SetPlatform(AppMode.PC);
                    break;
                case "Android":
                    SetPlatform(AppMode.Android);
                    break;
            }
        }
        else
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    SetPlatform(AppMode.Android);
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    SetPlatform(AppMode.PC);
                    break;
            }
        }
    }
    public static void SetPlatform(AppMode current_appMode)
    {
        if (appMode == current_appMode)
        {
            Debug.LogWarning("尝试切换到相同平台，操作无效!");
            return;
        }
        appMode = current_appMode;
        //Debug.Log("切换至" + current_appMode + "平台");
        onAppModeChanged?.Invoke(appMode);
    }
    #endregion


    #region Data Save and Load

    private void LoadData()
    {
        if (Event.onLoadData == null)
            return;

        object obj = Event.onLoadData("/game_data.data");
        if (obj != null)
        {
            gameData = obj as GameData;
            if (gameData == null) // 首次进入游戏，使用默认数据
            {
                gameData = new GameData();
            }
        }
        obj = Event.onLoadData("/game_settings.data");
        if (obj != null)
        {
            gameSettings = obj as GameSettings;
            if (gameSettings == null) // 首次进入游戏，使用默认数据
            {
                gameSettings = new GameSettings();
            }
        }
    }
    private void SaveData()
    {
        Event.onSaveData?.Invoke("/game_data.data");
        Event.onSaveData?.Invoke("/game_settings.data");
    }
    #endregion



    public void QuitGame()
    {
        Event.onGameQuit?.Invoke();
        SaveData();
        Application.Quit();
    }
}

public enum AppMode
{
    Android, PC
}
public enum ScreenResolution
{
    SR3840_2160, SR2560_1440, SR1920_1080, SR1280_720, SR1024_768, SR800_600
}
public enum RenderingPrecision
{
    RP06, RP08, RP10, RP12
}

/// <summary>
/// 储存玩家的基本信息和游玩状态
/// </summary>
[Serializable]
public class GameData
{
    public AppMode appMode;
    //string playerName;
    //int playerLevel;

    public GameData() // 默认数据
    {
        //playerName = "Empty";
        //playerLevel = 1;
    }
}
/// <summary>
/// 储存游戏的设置参数
/// </summary>
[Serializable]
public class GameSettings
{
    //这里的默认是针对我这一类型的性能

    public bool postProcessing; // 开
    public float value_bloom; // 0 - 5, 默认2
    public float value_vignette; // 0 - 1, 默认0.2
    //public bool motionBlur; // 开
    public int fps; // 60
    public bool Vsync; // 开
    //public bool anti_aliasing; // 开
    public int screenResolution; // PC默认1080
    public bool fullScreen; // 开
    public int renderingPrecision; // 默认1.0

    // 0 - 1
    public float volume_main;
    public float volume_background;
    public float volume_sound;
    public float volume_fx;
    public bool mute; // 关

    public bool 死亡动画加速; // 关
    public bool 隐藏UI功能; // 关 
    public bool 显示血量详情; // 关
    public bool 相机智能跟随; // 开
    public float camera_Xlerp; // 1 - 10, 默认5
    public float camera_Ylerp; // 1 - 10, 默认1
    public bool 主页动态壁纸加载; // 开
    public bool 出界攻击自动瞄准; // 开
    public bool 显示UI提示; // 开，如果是PC
    public bool 开启菜单时关闭游戏画面; // 关
    public int 视野距离; // 50 - 100, 默认80

    public int language; // Chinese
    public int soundLanguage; // Chinese

    public GameSettings() // 默认数据，初次游玩时会进行性能测试动态调整初始设置
    {
        // 初始化默认设置
        postProcessing = true;
        value_bloom = 2.0f; 
        value_vignette = 0.2f;
        //motionBlur = true;
        fps = 2; //Fps.fps120
        Vsync = true; 
        //anti_aliasing = true;
        screenResolution = 2; //ScreenResolution.SR1920_1080
        fullScreen = true;
        renderingPrecision = 2; //RenderingPrecision.RP10

        // 音量设置
        volume_main = 1.0f; 
        volume_background = 1.0f;
        volume_sound = 1.0f;
        volume_fx = 1.0f;
        mute = false;

        // 游戏设置
        死亡动画加速 = false; 
        隐藏UI功能 = false; 
        显示血量详情 = false;
        相机智能跟随 = true;
        camera_Xlerp = 5.0f;
        camera_Ylerp = 1.0f; 
        主页动态壁纸加载 = true; 
        出界攻击自动瞄准 = true; 
        显示UI提示 = true;
        开启菜单时关闭游戏画面 = false;
        视野距离 = 80;

        language = 0; //Language.Chinese
        soundLanguage = 0; //Language.Chinese
    }
}