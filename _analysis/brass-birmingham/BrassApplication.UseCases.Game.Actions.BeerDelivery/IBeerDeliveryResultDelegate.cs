using BoardGameRules.Entities.Common;

namespace BrassApplication.UseCases.Game.Actions.BeerDelivery;

public interface IBeerDeliveryResultDelegate
{
	void SelectBeerSource(IBeerSource beerSource, int amount = 1);
}
