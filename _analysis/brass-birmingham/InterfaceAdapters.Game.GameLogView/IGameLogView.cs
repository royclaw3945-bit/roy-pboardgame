using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.GameLogView;

public interface IGameLogView : IBaseView
{
	void UpdateGameLogView(ActionLogViewModel actionLogViewModel);
}
