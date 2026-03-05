using System;
using BoardGameRules.Entities.Board.BuildingSlots;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Market;

public interface IMarketView : IBaseView
{
	void UpdateMarket(MarketViewModel viewModel);

	void SetUpCoalMarketOnClickAction(int resource, int availableResource, Action actionOnClickCoalMarket);

	void SetUpIronMarketOnClickAction(int resource, int availableResource, Action actionOnClickIronMarket);

	void StopIronHighlight();

	void StopCoalHighlight();

	void TakeResource();

	IAnimation CreateAddResourcesToMarketAnimation(BuildingType type, int amount);
}
