using Godot;
using System.Collections.Generic;

public struct CardDataStruct
{
	public string Title;
	public string Description;
	public string BackgroundPath;
	public string IconPath;

	public CardDataStruct(string title, string description, string backgroundPath, string iconPath)
	{
		Title = title;
		Description = description;
		BackgroundPath = backgroundPath;
		IconPath = iconPath;
	}
}

public static class CardDatabase
{
	// Database mapping unique card IDs to their data structs
	public static readonly Dictionary<string, CardDataStruct> Cards = new()
	{
		{ "card_strike_1", new CardDataStruct("Strike", "Deal 6 damage.", "res://assets/card_texture.jpg", "res://assets/icon.svg") },
		{ "card_strike_2", new CardDataStruct("Strike", "Deal 6 damage.", "res://assets/card_texture.jpg", "res://assets/icon.svg") },
		{ "card_defend_1", new CardDataStruct("Defend", "Gain 5 block.", "res://assets/card_texture.jpg", "res://assets/icon.svg") },
		{ "card_defend_2", new CardDataStruct("Defend", "Gain 5 block.", "res://assets/card_texture.jpg", "res://assets/icon.svg") },
		{ "card_heal",     new CardDataStruct("Heal", "Restore 4 HP.", "res://assets/card_texture.jpg", "res://assets/icon.svg") },
		{ "card_fireball", new CardDataStruct("Fireball", "Deal 12 damage to all enemies.", "res://assets/card_texture.jpg", "res://assets/icon.svg") }
	};
}
