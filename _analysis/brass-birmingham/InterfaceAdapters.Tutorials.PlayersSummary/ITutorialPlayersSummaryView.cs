using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayersSummary;

namespace InterfaceAdapters.Tutorials.PlayersSummary;

public interface ITutorialPlayersSummaryView : IPlayersSummaryView, IBaseView
{
	void HideOtherPlayers();

	void ShowAllSetPlayers();

	void GrowYellowPlayerPortrait();

	void ShrinkYellowPlayerPortrait();

	void ShowAllPlayersAfterNextEndTurnAnimation();
}
