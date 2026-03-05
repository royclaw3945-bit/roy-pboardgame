using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.GameProgressSummary;

public interface IGameProgressSummaryView : IBaseView
{
	void UpdateGameProgressSummaryView(GameProgressSummaryViewModel gameProgressDetailsViewModel);
}
