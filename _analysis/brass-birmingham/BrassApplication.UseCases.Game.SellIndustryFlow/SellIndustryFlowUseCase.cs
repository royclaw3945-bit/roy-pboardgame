using System;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Utils.Logging;
using BrassApplication.AI.RuleBasedSystem;
using BrassApplication.AI.RuleBasedSystem.RuleSelectors;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.SellIndustryFlow;

public class SellIndustryFlowUseCase : IUseCase, ISellIndustryFlowInput
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(SellIndustryFlowUseCase));

	private readonly IGameHistory _gameHistory;

	private readonly ISellIndustryFlowOutput _presenter;

	private readonly RuleSelector _sellIndustryRuleSelector;

	public SellIndustryFlowUseCase(ISellIndustryFlowOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
		RuleSet ruleSet = RuleSetFactory.CreateSellIndustryRuleSet();
		_sellIndustryRuleSelector = RuleSelectorFactory.CreatePrioritySelector(ruleSet);
	}

	public void StartSellIndustriesFlow()
	{
		Logger.Debug("SellIndustryFlowUseCase.StartSellIndustriesFlow()");
		GetSellIndustryAction()();
	}

	private Action GetSellIndustryAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		return _sellIndustryRuleSelector.Select(currentGame).MakeAction(currentGame);
	}

	public void ShowIndustrySellingSummary(IEvent industrySellingEndedEvent, Action onDismiss)
	{
		_presenter.ShowIndustrySellingSummary(industrySellingEndedEvent, onDismiss);
	}
}
