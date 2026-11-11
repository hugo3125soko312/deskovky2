using Godot;
using System;
using System.Collections.Generic;

// Static access so you can call: Globals.Input...
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
			// Resolve Key (handles physical fallback for international layouts)
			Key key = keyEvent.Keycode != Key.None ? keyEvent.Keycode : keyEvent.PhysicalKeycode;
			string keyName = OS.GetKeycodeString(key);

			ulong currentProcFrame = Engine.GetProcessFrames();
			ulong currentPhysFrame = Engine.GetPhysicsFrames();

			if (keyEvent.Pressed)
			{
				// KeyDown: starts the KeyHold state and triggers KeyPress for this frame
				_heldKeys.Add(key);
				_pressedProcessFrames[key] = currentProcFrame;
				_pressedPhysicsFrames[key] = currentPhysFrame;

				GD.Print($"[KeyListener] Key Down (Press & Hold Start): {key} ({keyName})");
			}
			else
			{
				// KeyUp: stops the KeyHold state and triggers KeyUp for this frame
				_heldKeys.Remove(key);
				_releasedProcessFrames[key] = currentProcFrame;
				_releasedPhysicsFrames[key] = currentPhysFrame;

				GD.Print($"[KeyListener] Key Up (Released): {key} ({keyName})");
			}
		}
	}

	// ==========================================
	// CALLABLE API (Use through Globals.Input)
	// ==========================================

	/// <summary>
	/// Returns true only on the initial frame the key was pressed down (Just Pressed).
	/// </summary>
	public bool KeyPress(Key key)
	{
		ulong proc = Engine.GetProcessFrames();
		ulong phys = Engine.GetPhysicsFrames();

		return (_pressedProcessFrames.TryGetValue(key, out ulong pFrame) && pFrame == proc)
			|| (_pressedPhysicsFrames.TryGetValue(key, out ulong phFrame) && phFrame == phys);
	}

	/// <summary>
	/// Alias for KeyPress.
	/// </summary>
	public bool KeyDown(Key key) => KeyPress(key);

	/// <summary>
	/// Returns true continuously every frame while the key is kept held down.
	/// </summary>
	public bool KeyHold(Key key)
	{
		return _heldKeys.Contains(key);
	}

	/// <summary>
	/// Returns true only on the frame the key was released (Just Released).
	/// </summary>
	public bool KeyUp(Key key)
	{
		ulong proc = Engine.GetProcessFrames();
		ulong phys = Engine.GetPhysicsFrames();

		return (_releasedProcessFrames.TryGetValue(key, out ulong rFrame) && rFrame == proc)
			|| (_releasedPhysicsFrames.TryGetValue(key, out ulong rhFrame) && rhFrame == phys);
	}
}
