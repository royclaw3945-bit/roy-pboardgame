using System.Collections.Generic;
using BoardGameRules.Entities.Industries;

namespace BrassApplication.UseCases.Game.Actions.CoalDelivery;

public interface ICoalDeliveryInput : ISelectIndustryDelegate, ISelectMarketDelegate
{
	void StartCoalDelivery(int coalToDeliver, IEnumerable<CoalMine> possibleCoalMinesSources, ICoalDeliveryResultDelegate coalDeliveryResultDelegate);
}
