using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.Market;

public interface IMarketOutput
{
	void UpdateMarket(BrassApplicationGame game);

	void ShowTooltip(string element, float x, float y, int resourceCost);

	void HideTooltip();
}
