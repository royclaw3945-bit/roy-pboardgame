namespace BrassApplication.UseCases.Game.Market;

public interface IMarketInput
{
	void UpdateMarket();

	void ShowTooltip(string element, float x, float y);

	void HideTooltip();
}
