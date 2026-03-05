using System;
using BoardGameRules.Entities.Players;
using BrassApplication.AI;
using BrassApplication.Game;

namespace InterfaceAdapters.Common.CreateGame;

public class PlayerCreateGameViewModel
{
	public PlayerMetadata PlayerMetadata { get; set; }

	public string Nickname { get; set; }

	public AIType AiType { get; set; }

	public string AiTypeText { get; set; }

	public PlayerColor PlayerColor { get; set; }

	public int PlayerNumber { get; set; }

	public int CurrentPlayerCount { get; set; }

	public bool Editable { get; set; }

	public Action<PlayerMetadata> OnNextDifficultyClickedAction { get; set; }

	public Action<PlayerMetadata> OnPreviousDifficultyClickedAction { get; set; }

	public Action<PlayerMetadata> OnEditPortraitAndColorAction { get; set; }

	public Action<PlayerColor, string> OnPlayerNameChangedAction { get; set; }

	public Action OnRemovePlayerClickedAction { get; set; }

	public Action OnAddPlayerClickedAction { get; set; }
}
