using System;
using BoardGameRules.Entities.EventSourcing.Events;

namespace BrassApplication.UseCases.Game.SellIndustryFlow;

public interface ISellIndustryFlowOutput
{
	void ShowIndustrySellingSummary(IEvent industrySellingEndedEvent, Action onDismiss);
}
