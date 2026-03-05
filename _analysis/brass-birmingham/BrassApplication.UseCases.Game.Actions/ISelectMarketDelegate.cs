using BoardGameRules.Entities.Markets;

namespace BrassApplication.UseCases.Game.Actions;

public interface ISelectMarketDelegate
{
	void SelectMarket(BaseMarket market);
}
