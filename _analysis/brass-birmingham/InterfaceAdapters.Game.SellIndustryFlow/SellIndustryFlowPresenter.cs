using System;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.SellIndustry.Events;
using BrassApplication.UseCases.Game.SellIndustryFlow;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Game.SellIndustryFlow;

public class SellIndustryFlowPresenter : IPresenter, ISellIndustryFlowOutput
{
	private readonly IRouter _router;

	public SellIndustryFlowPresenter(IRouter router)
	{
		_router = router;
	}

	public void ShowIndustrySellingSummary(IEvent industrySellingEndedEvent, Action onDismiss)
	{
		IndustrySellingEnded industrySellingEnded = (IndustrySellingEnded)industrySellingEndedEvent;
		_router.ShowView<ISellIndustryFlowView>();
		_router.GetView<ISellIndustryFlowView>().UpdateView(industrySellingEnded.SoldInLastRound, industrySellingEnded.LostVpsInLastRound, delegate
		{
			onDismiss?.Invoke();
			_router.HideView<ISellIndustryFlowView>();
		});
	}
}
