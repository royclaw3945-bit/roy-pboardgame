using BoardGameRules.Entities.Board.Merchants;

namespace BrassApplication.UseCases.Game.Actions;

public interface ISelectMerchantSlotDelegate
{
	void SelectMerchantSlot(MerchantSlot merchantSlot);
}
