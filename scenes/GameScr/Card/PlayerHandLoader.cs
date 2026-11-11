using Godot;
using System.Collections.Generic;

public partial class PlayerHandLoader : Node2D
{
	[ExportCategory("Card Setup")]
	[Export] public PackedScene CardTemplateScene;

	[ExportCategory("Arc & Hand Layout")]
	[Export] public float CardSpacing { get; set; } = 140.0f;
	[Export] public float MaxHandAngle { get; set; } = 16.0f;
	[Export] public float ArcHeight { get; set; } = 30.0f;

	private readonly List<Node2D> _handCards = new();

	public override void _Ready()
	{
		// 1. Scene setup validation
		if (CardTemplateScene == null)
		{
			GD.PushWarning($"[PlayerHandLoader WARNING] 'CardTemplateScene' is NOT assigned in the Inspector on node '{Name}'! Please assign Card.tscn.");
			return;
		}

		// 2. Database validation check
		if (CardDatabase.Cards == null || CardDatabase.Cards.Count == 0)
		{
			GD.PushWarning($"[PlayerHandLoader WARNING] 'CardDatabase.Cards' is empty or null! No cards will be loaded.");
			return;
		}

		InitializeHand(CardDatabase.Cards);
	}

	public void InitializeHand(Dictionary<string, CardDataStruct> initialDeckDict)
	{
		if (initialDeckDict == null)
		{
			GD.PushWarning("[PlayerHandLoader WARNING] Passed dictionary to InitializeHand() is null!");
			return;
		}

		ClearHand();

		foreach (var entry in initialDeckDict)
		{
			InstantiateAndAddCard(entry.Key, entry.Value);
		}

		UpdateHandLayout();
	}

	public void AddCard(string cardId, CardDataStruct data)
	{
		InstantiateAndAddCard(cardId, data);
		UpdateHandLayout();
	}

	public void RemoveCardAt(int index)
	{
		if (index < 0 || index >= _handCards.Count)
		{
			GD.PushWarning($"[PlayerHandLoader WARNING] Cannot remove card: Index {index} is out of bounds (Hand count: {_handCards.Count}).");
			return;
		}

		Node2D cardToRemove = _handCards[index];
		_handCards.RemoveAt(index);
		cardToRemove.QueueFree();

		UpdateHandLayout();
	}

	private void InstantiateAndAddCard(string cardId, CardDataStruct data)
	{
		if (CardTemplateScene == null)
		{
			GD.PushError($"[PlayerHandLoader ERROR] Cannot instantiate card '{cardId}'. CardTemplateScene is unassigned!");
			return;
		}

		Node2D cardInstance = CardTemplateScene.Instantiate<Node2D>();
		AddChild(cardInstance);

		// Subnode verification with safety fallbacks
		TextureRect bgTexture = cardInstance.GetNodeOrNull<TextureRect>("CardTemplate/BackgroundTexture");
		RichTextLabel titleLabel = cardInstance.GetNodeOrNull<RichTextLabel>("CardTemplate/BackgroundTexture/CardLayout/TitleMargin/TitleLabel");
		RichTextLabel descLabel = cardInstance.GetNodeOrNull<RichTextLabel>("CardTemplate/BackgroundTexture/CardLayout/DescMargin/DescLabel");
		TextureRect iconTexture = cardInstance.GetNodeOrNull<TextureRect>("CardTemplate/BackgroundTexture/CardLayout/IconMargin/HBoxContainer/IconTexture");

		if (bgTexture == null)
			GD.PushWarning($"[PlayerHandLoader WARNING] 'BackgroundTexture' node missing in card template for '{cardId}'.");
		else if (!string.IsNullOrEmpty(data.BackgroundPath))
			bgTexture.Texture = ResourceLoader.Exists(data.BackgroundPath) ? GD.Load<Texture2D>(data.BackgroundPath) : null;

		if (iconTexture == null)
			GD.PushWarning($"[PlayerHandLoader WARNING] 'IconTexture' node missing in card template for '{cardId}'.");
		else if (!string.IsNullOrEmpty(data.IconPath))
			iconTexture.Texture = ResourceLoader.Exists(data.IconPath) ? GD.Load<Texture2D>(data.IconPath) : null;

		if (titleLabel != null)
			titleLabel.Text = data.Title;
		else
			GD.PushWarning($"[PlayerHandLoader WARNING] 'TitleLabel' node missing in card template for '{cardId}'.");

		if (descLabel != null)
			descLabel.Text = data.Description;
		else
			GD.PushWarning($"[PlayerHandLoader WARNING] 'DescLabel' node missing in card template for '{cardId}'.");

		_handCards.Add(cardInstance);
	}

	private void UpdateHandLayout()
	{
		int count = _handCards.Count;
		if (count == 0) return;

		if (count == 1)
		{
			_handCards[0].Position = Vector2.Zero;
			_handCards[0].RotationDegrees = 0.0f;
			_handCards[0].ZIndex = 0;
			return;
		}

		float totalWidth = (count - 1) * CardSpacing;
		float startX = -totalWidth / 2.0f;

		for (int i = 0; i < count; i++)
		{
			float t = (float)i / (count - 1);
			float normalizedIndex = (t * 2.0f) - 1.0f;

			float xPos = startX + (i * CardSpacing);
			float yPos = (normalizedIndex * normalizedIndex) * ArcHeight;
			float rotationAngle = normalizedIndex * MaxHandAngle;

			_handCards[i].Position = new Vector2(xPos, yPos);
			_handCards[i].RotationDegrees = rotationAngle;
			_handCards[i].ZIndex = i;
		}
	}

	private void ClearHand()
	{
		foreach (var card in _handCards)
		{
			if (IsInstanceValid(card))
				card.QueueFree();
		}
		_handCards.Clear();
	}
}
