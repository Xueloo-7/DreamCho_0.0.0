using System;
using UnityEngine;

namespace DreamCho
{
    public static class KeyEvent
    {
        public static Action onJump_Down;
        public static Action onJump_Up;
        public static Action<bool> onJump;

        public static Action onDash_Down;
        public static Action onDash_Up;
        public static Action<bool> onDash;

        public static Action onEscape_Down; // 返回键
        public static Action onInteract_Down; // 交互键

        public static Action<Vector2> onDirection_Down; // 方向键

        public static Action<bool> onSkillDown;
        public static Action<bool> onSkillUp;

        public static Action<int> onNextCharacterDown; // 下一个角色切换
        public static Action<int> onPrevioousCharacterDown; // 上一个角色切换

        public static Action onAttackDown;
        public static Action onAttackUp;
        public static Action onAttack;

        public static Action onMouseDown;
        public static Action onMouseUp;
        public static Action onMouse;
    }
}
