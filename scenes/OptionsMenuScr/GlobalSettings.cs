using Godot;
using System.Collections.Generic;

public partial class GlobalSettings : Node
{
	public static GlobalSettings Instance { get; private set; }

	private const string SavePath = "user://settings.cfg";
	private readonly ConfigFile _config = new();

	// Define your default settings here
	public readonly Dictionary<string, Variant> Settings = new()
	{
		{ "Fullscreen", true },
		{ "Master Volume", 80.0f },
		{ "Player Name", "Player" }
	};

	public override void _Ready()
	{
		Instance = this;
		LoadSettings();
	}

	public void SetSetting(string key, Variant value)
	{
		if (Settings.ContainsKey(key))
		{
			// Handle string to float conversion (e.g., from the LineEdit for Master Volume)
			if (value.VariantType == Variant.Type.String && Settings[key].VariantType == Variant.Type.Float)
			{
				if (float.TryParse(value.AsString(), out float parsedFloat))
					value = parsedFloat;
			}

			Settings[key] = value;
			ApplySettings();
			SaveSettings();
		}
	}

	public void SaveSettings()
	{
		foreach (var (key, value) in Settings)
		{
			_config.SetValue("Settings", key, value);
		}
		_config.Save(SavePath);
	}

	public void LoadSettings()
	{
		Error err = _config.Load(SavePath);
		if (err == Error.Ok)
		{
			foreach (string key in Settings.Keys)
			{
				if (_config.HasSectionKey("Settings", key))
				{
					Settings[key] = _config.GetValue("Settings", key);
				}
			}
		}
		ApplySettings();
	}

	public void ApplySettings()
	{
		if (Settings["Fullscreen"].VariantType == Variant.Type.Bool)
		{
			bool isFullscreen = Settings["Fullscreen"].AsBool();
			DisplayServer.WindowSetMode(isFullscreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
		}

		if (Settings["Master Volume"].VariantType == Variant.Type.Float)
		{
			float volume = Settings["Master Volume"].AsSingle();
			int busIndex = AudioServer.GetBusIndex("Master");
			if (busIndex != -1)
			{
				float db = Mathf.LinearToDb(volume / 100.0f);
				AudioServer.SetBusVolumeDb(busIndex, db);
			}
		}
	}
}
