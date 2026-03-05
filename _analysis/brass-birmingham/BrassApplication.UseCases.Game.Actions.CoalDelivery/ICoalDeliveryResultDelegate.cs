using BoardGameRules.Entities.Common;

namespace BrassApplication.UseCases.Game.Actions.CoalDelivery;

public interface ICoalDeliveryResultDelegate
{
	void SelectCoalSource(ICoalSource coalSource, int amount = 1);
}
