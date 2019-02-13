using System;
using OpenTK;

namespace ImGuiOpenTK
{
    public class TKEvent : EventArgs
    {
        public enum Type
        {
            Window = 0,
            Keyboard,
            TextEditing,
            TextInput,
            MouseMotion,
            MouseButton,
            MouseWheel,
            JoyAxis,
            JoyBall,
            JoyHat,
            JoyButton,
            JoyDevice,
            ControllerAxis,
            ControllerButton,
            ControllerDevice,
            AudioDeviceEvent,
            Quit,
            User,
            SystemWindow,
            TouchFinger,
            MultiGesture,
            Finger,
            Drop
        }

        public Type EventType { get; protected set; }

    }

    public class KeyBoardEvent : TKEvent
    {
        public bool IsKeyDown { get; private set; }
        public OpenTK.Input.Key Key { get; private set; }
        public KeyBoardEvent(bool IsKeyDown, OpenTK.Input.Key Key)
        {
            EventType = Type.Keyboard;
            this.IsKeyDown = IsKeyDown;
            this.Key = Key;
        }
    }

    public class MouseButtonEvent : TKEvent
    {
        public bool IsButtonDown { get; private set; }
        public OpenTK.Input.MouseButton Button { get; private set; }
        public MouseButtonEvent(bool IsButtonDown, OpenTK.Input.MouseButton Button)
        {
            EventType = Type.MouseButton;
            this.IsButtonDown = IsButtonDown;
            this.Button = Button;
        }
    }

    public class MouseWheelEvent : TKEvent
    {
        public float Value { get; private set; }
        public MouseWheelEvent(float WheelValue)
        {
            EventType = Type.MouseWheel;
            Value = WheelValue;
        }
    }

    public class MouseMotionEvent : TKEvent
    {
        public Point Position { get; private set; }
        public MouseMotionEvent(Point newPosition)
        {
            EventType = Type.MouseMotion;
            Position = newPosition;
        }
    }

    public class TextInputEvent : TKEvent
    {
        public string Text { get; private set; }
        public TextInputEvent(string Text)
        {
            EventType = Type.TextInput;
            this.Text = Text;
        }
    }
}
