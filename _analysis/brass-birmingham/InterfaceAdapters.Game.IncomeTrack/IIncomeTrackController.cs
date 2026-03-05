using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Game.IncomeTrack;

public interface IIncomeTrackController
{
	void OnMouseEnter(string element, float x, float y, PlayerColor? playerColor = null);

	void OnMouseExit();
}
