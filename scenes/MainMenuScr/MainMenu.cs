using Godot;
using System;

// Make sure this matches the filename: MainMenu.cs
public partial class MainMenu : Control
{
	[Export] private Button _playButton;
	[Export] private Button _optionsButton;
	[Export] private Button _quitButton;

	[Export(PropertyHint.File, "*.tscn")] 
	private string _optionsScenePath = "res://scenes/OptionsMenuScr/OptionsMenuScene.tscn";

	[Export(PropertyHint.File, "*.tscn")] 
	private string _gameScenePath = "res://scenes/GameScr/Game.tscn";

	public override void _Ready()
	{
		// --- PLAY BUTTON ---
		if (_playButton == null) 
			_playButton = GetNodeOrNull<Button>("VBoxContainer/PlayButton");
			
		if (_playButton != null)
			_playButton.Pressed += OnPlayButtonPressed;
		else
			GD.PrintErr("[MainMenu] PlayButton not found!");

		// --- OPTIONS BUTTON ---
		if (_optionsButton == null) 
			_optionsButton = GetNodeOrNull<Button>("VBoxContainer/OptionsButton");
			
		if (_optionsButton != null)
			_optionsButton.Pressed += OnOptionsButtonPressed;
		else
			GD.PrintErr("[MainMenu] OptionsButton not found!");

		// --- QUIT BUTTON ---
		if (_quitButton == null) 
			_quitButton = GetNodeOrNull<Button>("VBoxContainer/QuitButton");
			
		if (_quitButton != null)
			_quitButton.Pressed += OnQuitButtonPressed;
		else
			GD.PrintErr("[MainMenu] QuitButton not found!");
	}

	private void OnPlayButtonPressed()
	{
		GD.Print($"[MainMenu] Loading Game Scene: {_gameScenePath}");
		Error error = GetTree().ChangeSceneToFile(_gameScenePath);
		
		if (error != Error.Ok)
			GD.PrintErr($"[MainMenu -> ERROR] Failed to load game scene. Error Code: {error}");
	}

	private void OnOptionsButtonPressed()
	{
		GD.Print($"[MainMenu] Loading Options Scene: {_optionsScenePath}");
		Error error = GetTree().ChangeSceneToFile(_optionsScenePath);
		
		if (error != Error.Ok)
			GD.PrintErr($"[MainMenu -> ERROR] Failed to load options scene. Error Code: {error}");
	}

	private void OnQuitButtonPressed()
	{
		GD.Print("[MainMenu] Quitting game...");
		GetTree().Quit(); 
	}
}
