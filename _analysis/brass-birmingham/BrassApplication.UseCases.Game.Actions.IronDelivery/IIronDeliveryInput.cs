namespace BrassApplication.UseCases.Game.Actions.IronDelivery;

public interface IIronDeliveryInput : ISelectIndustryDelegate, ISelectMarketDelegate
{
	void StartIronDelivery(int ironToDeliver, IIronDeliveryResultDelegate ironDeliveryResultDelegate);

	void HideIronDelivery();
}
