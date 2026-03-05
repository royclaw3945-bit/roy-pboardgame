using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlayAiTurn;

public interface IPlayAiTurnView : IBaseView
{
	void SetCurrentPlayerName(string playerName);
}
