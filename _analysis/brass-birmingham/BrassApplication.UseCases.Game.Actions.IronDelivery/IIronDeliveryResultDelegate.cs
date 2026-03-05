using BoardGameRules.Entities.Common;

namespace BrassApplication.UseCases.Game.Actions.IronDelivery;

public interface IIronDeliveryResultDelegate
{
	void SelectIronSource(IIronSource ironSource, int amount = 1);
}
