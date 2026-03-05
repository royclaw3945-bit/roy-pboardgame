using System;
using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Menu.Online.CreateOnlineGame;

public class PlayerOnlineGameViewModel
{
	public string Nickname { get; set; }

	public string AiTypeText { get; set; }

	public PlayerColor PlayerColor { get; set; }

	public PlayerAvatar PlayerAvatar { get; set; }

	public int PlayerNumber { get; set; }

	public int CurrentPlayerCount { get; set; }

	public Action<PlayerColor, PlayerAvatar> OnEditPortraitAndColorAction { get; set; }

	public Action OnRemovePlayerClickedAction { get; set; }

	public Action OnAddPlayerClickedAction { get; set; }

	public bool HasJoined { get; set; }
}
