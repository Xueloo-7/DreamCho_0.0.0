using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DreamCho
{
    /// <summary>
    /// 专门处理玩家的自定义输入，需要判断当前平台，然后更新玩家输入
    /// </summary>
    public class Key : Singleton<Key>
    {
        [SerializeField] KeySettings k;
        //[SerializeField] Joystick joystick;
        public static KeySettings K { get { return Instance.k; } }
        public static bool canGetInput = true;
        public static List<string> remainInput = new List<string>(); //预留输入[额外功能]，将一个输入作为事件保留在数据中，在需要的时候使用

        private bool enterDirection; // 按下方向键

        private void Start()
        {
            //if (joystick == null) Debug.LogWarning("缺少joystick组件，无法支持joystick操控");
            if (GameManager.Instance == null)
                Debug.LogWarning("GameManager不存在，无法确定当前所处平台!");
        }


        #region 更新按键信息
        private void Update()
        {
            switch (GameManager.appMode)
            {
                case AppMode.Android:
                    /*if (joystick != null)
                    {
                        rawAxisX = Mathf.RoundToInt(joystick.Horizontal); rawAxisY = Mathf.RoundToInt(joystick.Vertical);
                        axisX = joystick.Horizontal; axisY = joystick.Vertical;
                    }*/
                    break;
                case AppMode.PC:
                    GetAxis(InputDir.Horizontal); GetAxis(InputDir.Vertical);
                    GetAxisRaw(InputDir.Horizontal); GetAxisRaw(InputDir.Vertical);
                    break;
                default: break;
            }

            KeyEvent.onJump?.Invoke(GetButton(K.Jump));
            if (GetButtonDown(K.Jump)) KeyEvent.onJump_Down?.Invoke();
            if (GetButtonUp(K.Jump)) KeyEvent.onJump_Up?.Invoke();

            KeyEvent.onDash?.Invoke(GetButton(K.Dash));
            if (GetButtonDown(K.Dash)) KeyEvent.onDash_Down?.Invoke();
            if (GetButtonUp(K.Dash)) KeyEvent.onDash_Up?.Invoke();

            if (GetButtonDown(K.Escape)) KeyEvent.onEscape_Down?.Invoke();
            if (GetButtonDown(K.Interact)) KeyEvent.onInteract_Down?.Invoke();

            if (GetButtonDown(K.Down) || GetButtonDown(K.Right) || GetButtonDown(K.Up) || GetButtonDown(K.Left))
            {
                KeyEvent.onDirection_Down?.Invoke(new Vector2(rawAxisX, rawAxisY));
            }
            if (GetButtonUp(K.Down) || GetButtonUp(K.Right) || GetButtonUp(K.Up) || GetButtonUp(K.Left))
            {
                //KeyEvent.onDirection_Up?.Invoke(new Vector2(rawAxisX, rawAxisY));
            }

            if(GetButtonDown(K.NextCharacter)) KeyEvent.onNextCharacterDown?.Invoke(1);
            if (GetButtonDown(K.PreviousCharacter)) KeyEvent.onPrevioousCharacterDown?.Invoke(-1);

            if(GetButtonDown(K.Power)) KeyEvent.onSkillDown?.Invoke(true);
            if(GetButtonUp(K.Power)) KeyEvent.onSkillUp?.Invoke(false);

        }

        private void LateUpdate()
        {
            RemainInputHandle();
        }

        /// <summary>
        /// 处理预输入的Down,Up
        /// </summary>
        private void RemainInputHandle()
        {
            for (int i = remainInput.Count - 1; i >= 0; i--)
            {
                if (remainInput[i].Contains("Down") || remainInput[i].Contains("Up"))
                {
                    remainInput.RemoveAt(i);
                }
            }
        }
        public static bool CheckRemainInput(string inputName)
        {
            return remainInput.Contains(inputName);
        }

        /// <summary>
        /// 检测按键的点按状况
        /// </summary>
        public static bool CheckKey(IEnumerable<KeyCode> keys, Func<KeyCode, bool> inputMethod)
        {
            foreach (KeyCode key in keys)
            {
                if (inputMethod(key))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 将GameKey转换成Keys以便在GetButtonCommon中使用
        /// </summary>
        private IEnumerable<KeyCode> KeyToKeys(GameKey gameKey)
        {
            switch (gameKey)
            {
                case GameKey.Talk: return K.Interact;
                case GameKey.Jump: return K.Jump;
                case GameKey.Dash: return K.Dash;
                case GameKey.Power: return K.Power;
                case GameKey.Character: return K.PreviousCharacter;
                case GameKey.Attack: return K.Attack;

                default: return null;
            }
        }
        #endregion



        #region GetAxis
        public static float rawAxisX;
        public static float rawAxisY;
        static bool pressLeft;
        static bool pressRight;
        static bool pressUp;
        static bool pressDown;
        /// <summary>
        /// 返回-1, 0, 1的轴值
        /// </summary>
        /// <param name="inputDir"></param>
        /// <returns></returns>
        public float GetAxisRaw(InputDir inputDir)
        {
            pressLeft = pressRight = pressUp = pressDown = false; //重置

            if (inputDir == InputDir.Horizontal)
            {
                if (CheckKey(K.Left, Input.GetKey)) pressLeft = true;
                if (CheckKey(K.Right, Input.GetKey)) pressRight = true;

                if (pressLeft && !pressRight)
                {
                    rawAxisX = -1;
                }
                else if (pressRight && !pressLeft)
                {
                    rawAxisX = 1;
                }
                else
                {
                    rawAxisX = 0;
                }
                return rawAxisX;
            }
            else if (inputDir == InputDir.Vertical)
            {
                if (CheckKey(K.Up, Input.GetKey)) pressUp = true;
                if (CheckKey(K.Down, Input.GetKey)) pressDown = true;

                if (pressDown && !pressUp)
                {
                    rawAxisY = -1;
                }
                else if (pressUp && !pressDown)
                {
                    rawAxisY = 1;
                }
                else
                {
                    rawAxisY = 0;
                }
                return rawAxisY;
            }
            else return 0;
        }



        public static float axisX;
        public static float axisY;
        static bool pressAxisLeft;
        static bool pressAxisRight;
        static bool pressAxisUp;
        static bool pressAxisDown;
        /// <summary>
        /// 返回-1到1之间的轴值
        /// </summary>
        /// <param name="inputDir"></param>
        /// <returns></returns>
        public float GetAxis(InputDir inputDir)
        {
            if (inputDir == InputDir.Horizontal)
            {
                if (CheckKey(K.Left, Input.GetKey)) pressAxisLeft = true;
                if (CheckKey(K.Right, Input.GetKey)) pressAxisRight = true;

                if (pressAxisLeft && !pressAxisRight)
                {
                    if (axisX > -1)
                        axisX -= Time.deltaTime;
                    else axisX = -1;
                }
                else if (pressAxisRight && !pressAxisLeft)
                {
                    if (axisX < 1)
                        axisX += Time.deltaTime;
                    else axisX = 1;
                }
                else
                {
                    axisX = Mathf.MoveTowards(axisX, 0, Time.deltaTime);
                }
                return axisX;
            }
            else if (inputDir == InputDir.Vertical)
            {
                if (CheckKey(K.Up, Input.GetKey)) pressAxisUp = true;
                if (CheckKey(K.Down, Input.GetKey)) pressAxisDown = true;

                if (pressAxisDown && !pressAxisUp)
                {
                    if (axisY > -1)
                        axisY -= Time.deltaTime;
                    else axisY = -1;
                }
                else if (pressAxisUp && !pressAxisDown)
                {
                    if (axisY < 1)
                        axisY += Time.deltaTime;
                    else axisY = 1;
                }
                else
                {
                    if (axisY < 0)
                        axisY += Time.deltaTime;
                    else if (axisY > 0)
                        axisY -= Time.deltaTime;
                    else axisY = 0;
                }
                return axisY;
            }
            else return 0;
        }
        #endregion



        #region GetButton

        /// <summary>
        /// 根据给定的键，获取当前持有按键的检测状态
        /// </summary>
        /// <param name="inputMethod">检测方式</param>
        /// <param name="keys">给定按键</param>
        /// <param name="gameKey">给定按键</param>
        private bool GetButtonCommon(Func<KeyCode, bool> inputMethod, IEnumerable<KeyCode> keys, GameKey gameKey = GameKey.None)
        {
            if (keys == null)
            {
                keys = KeyToKeys(gameKey);
            }
            return CheckKey(keys, inputMethod);
        }

        #region Up,Down,Press判断,同时划分了GameKey和Keys的参数选择
        public static bool GetButtonDown(IEnumerable<KeyCode> keys)
        {
            return Instance.GetButtonCommon(Input.GetKeyDown, keys);
        }
        public static bool GetButton(IEnumerable<KeyCode> keys)
        {
            return Instance.GetButtonCommon(Input.GetKey, keys);
        }
        public static bool GetButtonUp(IEnumerable<KeyCode> keys)
        {
            return Instance.GetButtonCommon(Input.GetKeyUp, keys);
        }
        public static bool GetButtonDown(GameKey gameKey)
        {
            return Instance.GetButtonCommon(Input.GetKeyDown, null, gameKey);
        }
        public static bool GetButton(GameKey gameKey)
        {
            return Instance.GetButtonCommon(Input.GetKey, null, gameKey);
        }
        public static bool GetButtonUp(GameKey gameKey)
        {
            return Instance.GetButtonCommon(Input.GetKeyUp, null, gameKey);
        }
        #endregion

        #endregion
    }
    public enum GameKey
    {
        None, Talk, Jump, Dash, Grab, Power, Character, H1, H2, H3, H0, Attack
    }
    public enum InputDir
    {
        Horizontal,
        Vertical
    }
}