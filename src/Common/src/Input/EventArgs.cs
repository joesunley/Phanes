using SharpHook;

namespace Phanes.Common;

public interface IInputEventArgs { }

#region Mouse

public sealed class MouseEventArgs : IInputEventArgs
{
    public readonly vec2 Position;

    public readonly MouseButton MouseButton;
    public readonly KeyModifiers Modifiers;

    public readonly DateTime TimeStamp;

    public readonly MouseHookEventArgs? OriginalArgs;

    public MouseEventArgs(MouseButton mouseButton, vec2 position, MouseHookEventArgs? originalEventArgs = null)
    {
        Position = position;

        MouseButton = (mouseButton == MouseButton.NoButton) ? 
            _Input.GetPressedMouseButtons().FirstOrDefault(MouseButton.NoButton) : 
            mouseButton;
        
        Modifiers = _Input.GetKeyModifiers();

        TimeStamp = DateTime.UtcNow;

        OriginalArgs = originalEventArgs;
    }
}

public sealed class MouseWheelEventArgs : IInputEventArgs
{
    public readonly vec2 Position;

    public readonly MouseWheelDirection Direction;
    public readonly KeyModifiers Modifiers;

    public readonly DateTime TimeStamp;

    public readonly MouseWheelHookEventArgs? OriginalArgs;

    public MouseWheelEventArgs(MouseWheelDirection direction, vec2 position, MouseWheelHookEventArgs? originalEventArgs = null)
    {
        Position = position;

        Direction = direction;
        Modifiers = _Input.GetKeyModifiers();

        TimeStamp = DateTime.UtcNow;

        OriginalArgs = originalEventArgs;
    }
}

#endregion

#region Keyboard

public sealed class KeyEventArgs : IInputEventArgs
{
    public readonly KeyCode KeyCode;
    public readonly KeyModifiers Modifiers;

    public readonly DateTime TimeStamp;

    public readonly KeyboardHookEventArgs? OriginalArgs;

    public KeyEventArgs(KeyCode keyCode, KeyboardHookEventArgs? originalEventArgs = null)
    {
        KeyCode = keyCode;
        Modifiers = _Input.GetKeyModifiers();

        TimeStamp = DateTime.UtcNow;

        OriginalArgs = originalEventArgs;
    }
}

#endregion