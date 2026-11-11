using Godot;
using System;
using System.Collections.Generic;

public static class Globals
{
	public static KeyListener Input => KeyListener.Instance;
}

public partial class KeyListener : Node
{
	public static KeyListener Instance { get; private set; }

	// Internal state tracking
	private readonly HashSet<Key> _heldKeys = new();
	private readonly Dictionary<Key, ulong> _pressedProcessFrames = new();
	private readonly Dictionary<Key, ulong> _pressedPhysicsFrames = new();
	private readonly Dictionary<Key, ulong> _releasedProcessFrames = new();
	private readonly Dictionary<Key, ulong> _releasedPhysicsFrames = new();

	// Helper property to check if logging is enabled in GlobalSettings
	private bool IsLoggingEnabled =>
		GlobalSettings.Instance != null &&
		GlobalSettings.Instance.Settings.TryGetValue("Log Key Inputs", out Variant val) &&
		val.AsBool();

	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && !keyEvent.Echo)
		{
			Key key = keyEvent.Keycode != Key.None ? keyEvent.Keycode : keyEvent.PhysicalKeycode;
			string keyName = OS.GetKeycodeString(key);

			ulong currentProcFrame = Engine.GetProcessFrames();
			ulong currentPhysFrame = Engine.GetPhysicsFrames();

			if (keyEvent.Pressed)
			{
				_heldKeys.Add(key);
				_pressedProcessFrames[key] = currentProcFrame;
				_pressedPhysicsFrames[key] = currentPhysFrame;

				// Log only if enabled in settings
				if (IsLoggingEnabled)
					GD.Print($"[KeyListener] Key Down (Press & Hold Start): {key} ({keyName})");
			}
			else
			{
				_heldKeys.Remove(key);
				_releasedProcessFrames[key] = currentProcFrame;
				_releasedPhysicsFrames[key] = currentPhysFrame;

				// Log only if enabled in settings
				if (IsLoggingEnabled)
					GD.Print($"[KeyListener] Key Up (Released): {key} ({keyName})");
			}
		}
	}

	// ==========================================
	// CALLABLE API (Remains identical)
	// ==========================================

	public bool KeyPress(Key key)
	{
		ulong proc = Engine.GetProcessFrames();
		ulong phys = Engine.GetPhysicsFrames();

		return (_pressedProcessFrames.TryGetValue(key, out ulong pFrame) && pFrame == proc)
			|| (_pressedPhysicsFrames.TryGetValue(key, out ulong phFrame) && phFrame == phys);
	}

	public bool KeyDown(Key key) => KeyPress(key);

	public bool KeyHold(Key key)
	{
		return _heldKeys.Contains(key);
	}

	public bool KeyUp(Key key)
	{
		ulong proc = Engine.GetProcessFrames();
		ulong phys = Engine.GetPhysicsFrames();

		return (_releasedProcessFrames.TryGetValue(key, out ulong rFrame) && rFrame == proc)
			|| (_releasedPhysicsFrames.TryGetValue(key, out ulong rhFrame) && rhFrame == phys);
	}
}
