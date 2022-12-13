using SharpHook;
using S = SharpHook.Native;

namespace Phanes.Common;

public static class Input
{
	static Input()
	{
		_Input.Start();
	}
	
	public static void Start() {}
	
	public static event Action<MouseEventArgs>? MouseMove;
	public static event Action<MouseEventArgs>? MouseDrag;
	public static event Action<MouseEventArgs>? MouseDown;
	public static event Action<MouseEventArgs>? MouseUp;
	public static event Action<MouseEventArgs>? MouseClick;
	public static event Action<MouseWheelEventArgs>? MouseWheel;

	public static event Action<KeyEventArgs>? KeyDown;
	public static event Action<KeyEventArgs>? KeyUp;
	public static event Action<KeyEventArgs>? KeyTyped;

	public static vec2 MousePosition(WindowsElement? element = null)
		=> _Input.GetMousePostion(element);

	public static bool IsWithinBounds(WindowsElement element)
		=> _Input.IsWithinBounds(element, _Input.GetMousePostion());
	
	#region Invokes
	internal static void Invoke_MouseMove(MouseEventArgs e) => MouseMove?.Invoke(e);
	internal static void Invoke_MouseDrag(MouseEventArgs e) => MouseDrag?.Invoke(e);
	internal static void Invoke_MouseDown(MouseEventArgs e) => MouseDown?.Invoke(e);
	internal static void Invoke_MouseUp(MouseEventArgs e) => MouseUp?.Invoke(e);
	internal static void Invoke_MouseClick(MouseEventArgs e) => MouseClick?.Invoke(e);
	internal static void Invoke_MouseWheel(MouseWheelEventArgs e) => MouseWheel?.Invoke(e);
	internal static void Invoke_KeyDown(KeyEventArgs e) => KeyDown?.Invoke(e);
	internal static void Invoke_KeyUp(KeyEventArgs e) => KeyUp?.Invoke(e);
	internal static void Invoke_KeyTyped(KeyEventArgs e) => KeyTyped?.Invoke(e);
	#endregion
	
	public static bool HasControlFlag(this KeyModifiers modifiers)
		=> modifiers.HasFlag(KeyModifiers.LeftControl) || modifiers.HasFlag(KeyModifiers.RightControl);
}

internal static class _Input
{
	private static readonly Dictionary<KeyCode, bool> s_keys = new();
	private static readonly Dictionary<MouseButton, bool> s_mouseButtons = new();

	private static vec2 s_mousePosition;

	static _Input()
	{
		TaskPoolGlobalHook hook = new();

		hook.MouseMoved += Hook_MouseMove;
		hook.MouseDragged += Hook_MouseDrag;
		hook.MousePressed += Hook_MouseDown;
		hook.MouseReleased += Hook_MouseUp;
		hook.MouseClicked += Hook_MouseClick;
		hook.MouseWheel += Hook_MouseWheel;
		hook.KeyPressed += Hook_KeyDown;
		hook.KeyReleased += Hook_KeyUp;
		hook.KeyTyped += Hook_KeyTyped;

		hook.RunAsync();

		s_keys = new();
		
		KeyCode[] keys = Enum.GetValues<KeyCode>();
		foreach (KeyCode key in keys) 
			s_keys.Add(key, false);
		
		MouseButton[] buttons = Enum.GetValues<MouseButton>();
		foreach (MouseButton button in buttons) 
			s_mouseButtons.Add(button, false);
	}
	
	internal static void Start() {}
	
	#region Hooks

	private static void Hook_MouseMove(object? _, MouseHookEventArgs e)
	{
		vec2 pos = new(e.Data.X, e.Data.Y);
		MouseButton code = Convert_MouseCodes(e.Data.Button);

		s_mousePosition = pos;

		Input.Invoke_MouseMove(new(code, pos, e));
	}

	private static void Hook_MouseDrag(object? _, MouseHookEventArgs e)
	{
		vec2 pos = new(e.Data.X, e.Data.Y);
		MouseButton code = Convert_MouseCodes(e.Data.Button);

		Input.Invoke_MouseDrag(new(code, pos, e));
		Input.Invoke_MouseMove(new(code, pos, e)); 
		
		// Also invokes MouseMove as is otherwise not invoked
	}

	private static void Hook_MouseDown(object? _, MouseHookEventArgs e)
	{
		vec2 pos = new(e.Data.X, e.Data.Y);
		MouseButton code = Convert_MouseCodes(e.Data.Button);

		s_mouseButtons[code] = true;

		Input.Invoke_MouseDown(new(code, pos, e));
	}
	
	private static void Hook_MouseUp(object? _, MouseHookEventArgs e)
	{
		vec2 pos = new(e.Data.X, e.Data.Y);
		MouseButton code = Convert_MouseCodes(e.Data.Button);

		s_mouseButtons[code] = false;

		Input.Invoke_MouseUp(new(code, pos, e));
	}

	private static void Hook_MouseClick(object? _, MouseHookEventArgs e)
	{
		vec2 pos = new(e.Data.X, e.Data.Y);
		MouseButton code = Convert_MouseCodes(e.Data.Button);

		Input.Invoke_MouseClick(new(code, pos, e));
	}

	private static void Hook_MouseWheel(object? _, MouseWheelHookEventArgs e)
	{
		vec2 pos = new(e.Data.X, e.Data.Y);
		MouseWheelDirection dir = (e.Data.Rotation > 0 ? MouseWheelDirection.Down : MouseWheelDirection.Up);

		Input.Invoke_MouseWheel(new(dir, pos, e));
	}

	private static void Hook_KeyDown(object? _, KeyboardHookEventArgs e)
	{
		KeyCode code = Convert_KeyCodes(e.Data.KeyCode);

		s_keys[code] = true;

		Input.Invoke_KeyDown(new(code, e));
	}

	private static void Hook_KeyUp(object? _, KeyboardHookEventArgs e)
	{
		KeyCode code = Convert_KeyCodes(e.Data.KeyCode);

		s_keys[code] = false;

		Input.Invoke_KeyUp(new(code, e));
	}

	private static void Hook_KeyTyped(object? _, KeyboardHookEventArgs e)
	{
		KeyCode code = Convert_KeyCodes(e.Data.KeyCode);

		Input.Invoke_KeyTyped(new(code, e));
	}
	
	#endregion
	
    private static KeyCode Convert_KeyCodes(S.KeyCode keyCode)
    {
        int val = keyCode switch
        {
            S.KeyCode.VcSpace => 32,
            S.KeyCode.VcBackquote => 39,
            S.KeyCode.VcComma => 44,
            S.KeyCode.VcMinus => 45,
            S.KeyCode.VcPeriod => 46,
            S.KeyCode.VcSlash => 47,

            S.KeyCode.Vc0 => 48,
            S.KeyCode.Vc1 => 49,
            S.KeyCode.Vc2 => 50,
            S.KeyCode.Vc3 => 51,
            S.KeyCode.Vc4 => 52,
            S.KeyCode.Vc5 => 53,
            S.KeyCode.Vc6 => 54,
            S.KeyCode.Vc7 => 55,
            S.KeyCode.Vc8 => 56,
            S.KeyCode.Vc9 => 57,

            S.KeyCode.VcSemicolon => 59,
            S.KeyCode.VcEquals => 61,

            S.KeyCode.VcA => 65,
            S.KeyCode.VcB => 66,
            S.KeyCode.VcC => 67,
            S.KeyCode.VcD => 68,
            S.KeyCode.VcE => 69,
            S.KeyCode.VcF => 70,
            S.KeyCode.VcG => 71,
            S.KeyCode.VcH => 72,
            S.KeyCode.VcI => 73,
            S.KeyCode.VcJ => 74,
            S.KeyCode.VcK => 75,
            S.KeyCode.VcL => 76,
            S.KeyCode.VcM => 77,
            S.KeyCode.VcN => 78,
            S.KeyCode.VcO => 79,
            S.KeyCode.VcP => 80,
            S.KeyCode.VcQ => 81,
            S.KeyCode.VcR => 82,
            S.KeyCode.VcS => 83,
            S.KeyCode.VcT => 84,
            S.KeyCode.VcU => 85,
            S.KeyCode.VcV => 86,
            S.KeyCode.VcW => 87,
            S.KeyCode.VcX => 88,
            S.KeyCode.VcY => 89,
            S.KeyCode.VcZ => 90,

            S.KeyCode.VcOpenBracket => 91,
            S.KeyCode.VcBackSlash => 92,
            S.KeyCode.VcCloseBracket => 93,
            S.KeyCode.VcYen => 96,

            S.KeyCode.VcEscape => 256,
            S.KeyCode.VcEnter => 257,
            S.KeyCode.VcTab => 258,
            S.KeyCode.VcBackspace => 259,
            S.KeyCode.VcInsert => 260,
            S.KeyCode.VcDelete => 261,

            S.KeyCode.VcRight => 262,
            S.KeyCode.VcLeft => 263,
            S.KeyCode.VcDown => 264,
            S.KeyCode.VcUp => 265,

            S.KeyCode.VcPageUp => 266,
            S.KeyCode.VcPageDown => 267,
            S.KeyCode.VcHome => 268,
            S.KeyCode.VcEnd => 269,

            S.KeyCode.VcCapsLock => 280,
            S.KeyCode.VcScrollLock => 281,
            S.KeyCode.VcNumLock => 282,
            S.KeyCode.VcPrintScreen => 283,
            S.KeyCode.VcPause => 284,

            S.KeyCode.VcF1 => 290,
            S.KeyCode.VcF2 => 291,
            S.KeyCode.VcF3 => 292,
            S.KeyCode.VcF4 => 293,
            S.KeyCode.VcF5 => 294,
            S.KeyCode.VcF6 => 295,
            S.KeyCode.VcF7 => 296,
            S.KeyCode.VcF8 => 297,
            S.KeyCode.VcF9 => 298,
            S.KeyCode.VcF10 => 299,
            S.KeyCode.VcF11 => 300,
            S.KeyCode.VcF12 => 301,
            S.KeyCode.VcF13 => 302,
            S.KeyCode.VcF14 => 303,
            S.KeyCode.VcF15 => 304,
            S.KeyCode.VcF16 => 305,
            S.KeyCode.VcF17 => 306,
            S.KeyCode.VcF18 => 307,
            S.KeyCode.VcF19 => 308,
            S.KeyCode.VcF20 => 309,
            S.KeyCode.VcF21 => 310,
            S.KeyCode.VcF22 => 311,
            S.KeyCode.VcF23 => 312,
            S.KeyCode.VcF24 => 313,

            S.KeyCode.VcNumPad0 => 320,
            S.KeyCode.VcNumPad1 => 321,
            S.KeyCode.VcNumPad2 => 322,
            S.KeyCode.VcNumPad3 => 323,
            S.KeyCode.VcNumPad4 => 324,
            S.KeyCode.VcNumPad5 => 325,
            S.KeyCode.VcNumPad6 => 326,
            S.KeyCode.VcNumPad7 => 327,
            S.KeyCode.VcNumPad8 => 328,
            S.KeyCode.VcNumPad9 => 329,
            S.KeyCode.VcNumPadSeparator => 330,
            S.KeyCode.VcNumPadDivide => 331,
            S.KeyCode.VcNumPadMultiply => 332,
            S.KeyCode.VcNumPadSubtract => 333,
            S.KeyCode.VcNumPadAdd => 334,
            S.KeyCode.VcNumPadEnter => 335,
            S.KeyCode.VcNumPadEquals => 336,

            S.KeyCode.VcLeftShift => 340,
            S.KeyCode.VcLeftControl => 341,
            S.KeyCode.VcLeftAlt => 342,
            S.KeyCode.VcLeftMeta => 343,
            S.KeyCode.VcRightShift => 344,
            S.KeyCode.VcRightControl => 345,
            S.KeyCode.VcRightMeta => 346,
            S.KeyCode.VcContextMenu => 347,

            _ => -1,
        };

        return (KeyCode)val;
    }

	private static MouseButton Convert_MouseCodes(S.MouseButton mouseButton)
		=> (MouseButton)mouseButton;

	internal static KeyModifiers GetKeyModifiers()
	{
		KeyModifiers mod = 0;

		if (s_keys[KeyCode.LeftShift])
			mod |= KeyModifiers.LeftShift;
		if (s_keys[KeyCode.LeftControl])
			mod |= KeyModifiers.LeftControl;
		if (s_keys[KeyCode.LeftAlt])
			mod |= KeyModifiers.LeftAlt;
		if (s_keys[KeyCode.LeftSuper])
			mod |= KeyModifiers.LeftSuper;

		if (s_keys[KeyCode.RightShift])
			mod |= KeyModifiers.RightShift;
		if (s_keys[KeyCode.RightControl])
			mod |= KeyModifiers.RightControl;
		if (s_keys[KeyCode.RightAlt])
			mod |= KeyModifiers.RightAlt;
		if (s_keys[KeyCode.RightSuper])
			mod |= KeyModifiers.RightSuper;

		return mod;
	}

	internal static IEnumerable<MouseButton> GetPressedMouseButtons()
		=> s_mouseButtons.Where(x => x.Value).Select(x => x.Key);

	internal static IEnumerable<KeyCode> GetPressedKeys()
		=> s_keys.Where(x => x.Value).Select(x => x.Key);

	internal static bool IsMouseButtonPressed(MouseButton button)
		=> s_mouseButtons[button];
	
	internal static bool IsKeyPressed(KeyCode code)
		=> s_keys[code];

	internal static vec2 GetMousePostion(WindowsElement? element = null)
	{
		if (element is null)
			return s_mousePosition;
		
		vec2 globalDelta = s_mousePosition - element.GlobalPosition;
		
		if (element.LocalEndPosition == vec2.Zero || element.LocalPosition == vec2.Zero)
			return globalDelta / (ViewManager.Zoom * element.Scaling);
		
		return globalDelta / (ViewManager.Zoom * element.Scaling) + element.LocalPosition;
	}

	internal static bool IsWithinBounds(WindowsElement element, vec2 pos)
	{
		return pos.X > element.GlobalPosition.X && pos.Y > element.GlobalPosition.Y &&
			   pos.X < element.GlobalEndPosition.X && pos.Y < element.GlobalEndPosition.Y;
	}
}

public sealed class WindowsElement
{
	public vec2 GlobalPosition { get; set; }
	public vec2 LocalPosition { get; set; }

	public vec2 GlobalEndPosition { get; set; }
	public vec2 LocalEndPosition { get; set; }
    
	public float Scaling { get; set; }

	public WindowsElement()
	{
		GlobalPosition = vec2.Zero;
		GlobalEndPosition = vec2.Zero;
        
		LocalPosition = vec2.Zero;
		LocalEndPosition = vec2.Zero;

		Scaling = 1f;
	}

	public WindowsElement(vec2 globalPosition, vec2 localPosition,
						  vec2 globalEndPosition, vec2 localEndPosition,
						  float scaling)
	{
		GlobalPosition = globalPosition;
		GlobalEndPosition = globalEndPosition;
        
		LocalPosition = localPosition;
		LocalEndPosition = localEndPosition;

		Scaling = scaling;
	}
	public WindowsElement(vec2 globalPosition, vec2 globalEndPosition, float scaling)
	{
		GlobalPosition = globalPosition;
		GlobalEndPosition = globalEndPosition;

		LocalPosition = vec2.Zero;
		LocalEndPosition = globalEndPosition - globalPosition;

		Scaling = scaling;
	}
	public WindowsElement(vec2 globalPosition, vec2 size, float scaling, bool a)
	{
		GlobalPosition = globalPosition;
		GlobalEndPosition = globalPosition + size;
        
		LocalPosition = vec2.Zero;
		LocalEndPosition = size;

		Scaling = scaling;
	}
}