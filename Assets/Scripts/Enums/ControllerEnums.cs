using System;

using UnityEngine;

namespace Enum
{
    public class ControllerEnums : MonoBehaviour
    {
        [Flags]
        public enum Actuation
        {
            None = 0,
            Trigger = 1,
            Grip = 2,
            Button_AX = 4,
            Touch_AX = 8,
            Button_BY = 16,
            Touch_BY = 32,
            Thumbstick_Left = 64,
            Thumbstick_Right = 128,
            Thumbstick_Up = 256,
            Thumbstick_Down = 512,
            Thumbstick_Click = 1024,
            Menu_Oculus = 2048
        }

        public enum State
        {
            Hovering,
            Holding
        }
    }
}