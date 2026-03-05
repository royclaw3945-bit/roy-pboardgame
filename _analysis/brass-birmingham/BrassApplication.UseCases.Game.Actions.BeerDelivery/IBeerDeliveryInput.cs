using System.Collections.Generic;
using BoardGameRules.Entities.Common;

namespace BrassApplication.UseCases.Game.Actions.BeerDelivery;

public interface IBeerDeliveryInput : ISelectIndustryDelegate, ISelectMerchantSlotDelegate
{
	void StartBeerDelivery(int beerToDeliver, IEnumerable<IBeerSource> possibleBeerSources, IBeerDeliveryResultDelegate beerDeliveryResultDelegate);
}
