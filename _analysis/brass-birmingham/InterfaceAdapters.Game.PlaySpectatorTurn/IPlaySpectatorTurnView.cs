using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlaySpectatorTurn;

public interface IPlaySpectatorTurnView : IBaseView
{
	void SetCurrentPlayerName(string playerName);
}
