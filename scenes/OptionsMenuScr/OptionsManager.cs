using Godot;
using System;

public partial class OptionsManager : Control
{
	[Export] private VBoxContainer _settingsListContainer;
	[Export] private HBoxContainer _boolRowTemplate;
	[Export] private HBoxContainer _textRowTemplate;
	[Export] private Button _backButton;

	[Export(PropertyHint.File, "*.tscn")]
	private string _mainMenuScenePath = "res://scenes/MainMenuScr/MainMenuScene.tscn";

	public override void _Ready()
	{
		// 1. Setup Back Button
		if (_backButton == null) _backButton = GetNodeOrNull<Button>("VBoxContainer/BackButton");
		if (_backButton != null) _backButton.Pressed += OnBackButtonPressed;

		// 2. Validate templates
		if (_settingsListContainer == null || _boolRowTemplate == null || _textRowTemplate == null)
		{
			GD.PrintErr("[OptionsManager] Missing [Export] assignments! Assign them in the Inspector.");
			return;
		}

		_boolRowTemplate.Visible = false;
		_textRowTemplate.Visible = false;

		// 3. Build UI rows from Autoload
		foreach (var setting in GlobalSettings.Instance.Settings)
		{
			if (setting.Value.VariantType == Variant.Type.Bool)
				BuildBoolRow(setting.Key, setting.Value.AsBool());
			else
				BuildTextRow(setting.Key, setting.Value.ToString());
		}

		_boolRowTemplate.QueueFree();
		_textRowTemplate.QueueFree();
	}

	private void BuildBoolRow(string settingName, bool currentValue)
	{
		var row = (HBoxContainer)_boolRowTemplate.Duplicate();
		row.Visible = true;

		var label  = row.GetNode<Label>("SettingLabel");
		var toggle = row.GetNode<CheckBox>("CheckBox");

		label.Text = settingName;
		toggle.ButtonPressed = currentValue;

		toggle.Toggled += pressed => GlobalSettings.Instance.SetSetting(settingName, pressed);

		_settingsListContainer.AddChild(row);
	}

	private void BuildTextRow(string settingName, string currentValue)
	{
		var row = (HBoxContainer)_textRowTemplate.Duplicate();
		row.Visible = true;

		var label = row.GetNode<Label>("SettingLabel");
		var input = row.GetNode<LineEdit>("LineEdit");

		label.Text = settingName;
		input.Text = currentValue;

		input.TextChanged += newText => GlobalSettings.Instance.SetSetting(settingName, newText);

		_settingsListContainer.AddChild(row);
	}

	private void OnBackButtonPressed()
	{
		Error err = GetTree().ChangeSceneToFile(_mainMenuScenePath);
		if (err != Error.Ok) GD.PrintErr($"[OptionsManager] Failed to load Main Menu: {err}");
	}
}
