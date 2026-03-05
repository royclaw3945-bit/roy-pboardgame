using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.SellIndustryFlow;

public interface ISellIndustryFlowView : IBaseView
{
	void UpdateView(Dictionary<PlayerColor, List<BaseIndustry>> industries, Dictionary<PlayerColor, int> lostVPs, Action onConfirmAction);
}
