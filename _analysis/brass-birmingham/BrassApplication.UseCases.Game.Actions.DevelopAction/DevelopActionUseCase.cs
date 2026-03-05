using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.IronDelivery;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.DevelopAction;

public class DevelopActionUseCase : BaseActionUseCase, IDevelopActionInput, IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate, ISelectIndustryOfTypeDelegate, IIronDeliveryResultDelegate, IActionInProgressFinishDelegate
{
	protected static readonly ILog Logger = LogFactory.BuildLogger(typeof(DevelopActionUseCase));

	private readonly IDevelopActionOutput _presenter;

	protected Player _currentPlayer;

	protected BoardGameRules.Entities.Actions.DevelopAction _developAction;

	protected DevelopActionUseCaseState _developActionUseCaseState;

	public DevelopActionUseCase(IDevelopActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public void StartDevelopAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		_developAction = currentGame.Game.TurnFlow.StartDevelopAction();
		_presenter.ShowCardsToDiscard(this);
		_presenter.ShowPlayerInfoToSelectCardToDiscard(this, this, currentGame.Game.TurnFlow.GetCurrentActionNumber(), currentGame.Game.TurnFlow.GetMaxActionsCount());
		_developActionUseCaseState = DevelopActionUseCaseState.SelectingCard;
	}

	public void SelectCard(BaseCard card)
	{
		_developAction.DiscardCard(card, _currentPlayer);
		PresentEvents();
	}

	public void SelectIndustryOfType(BuildingType buildingType)
	{
		_developAction.Develop(buildingType);
		PresentEvents();
	}

	public async void SelectIronSource(IIronSource ironSource, int amount = 1)
	{
		_developAction.SupplyIron(ironSource, amount);
		_presenter.HideIronDelivery();
		await _presenter.WaitForTweensEnd();
		PresentEvents();
	}

	public override void ReplayCompleted()
	{
		Logger.Debug("DevelopActionUseCase.ReplayCompleted");
		if (_developActionUseCaseState == DevelopActionUseCaseState.SelectingCard)
		{
			_presenter.HideCards();
			_presenter.ShowIndustriesDetailsView(_gameHistory.CurrentGame, this);
			_presenter.ShowPlayerInfoToSelectIndustry();
			_developActionUseCaseState = DevelopActionUseCaseState.SelectingIndustry;
		}
		if (_developActionUseCaseState == DevelopActionUseCaseState.SelectingIndustry)
		{
			if (_developAction.DevelopCounter == 1)
			{
				_presenter.UpdateIndustriesDetailsView(_gameHistory.CurrentGame);
				_presenter.ActivateFinishButton();
				if (_developAction.HasPlayerIronForSecondDevelop(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer))
				{
					_presenter.ShowPlayerInfoToSelectIndustry();
				}
				else
				{
					_presenter.ShowNotEnoughResourcesForSecondDevelopPopup();
				}
			}
			else if (_developAction.DevelopCounter == 2)
			{
				FinishAction();
			}
		}
		else if (_developActionUseCaseState == DevelopActionUseCaseState.IronDelivery)
		{
			if (_developAction.NeededIron == 0)
			{
				_developActionUseCaseState = DevelopActionUseCaseState.Done;
				ConfirmAction();
			}
			else
			{
				_presenter.StartIronDeliveryUseCase(_developAction.NeededIron, this);
				_presenter.HideReplayView();
			}
		}
		else
		{
			base.ReplayCompleted();
		}
	}

	public override void CancelAction()
	{
		if (_developActionUseCaseState == DevelopActionUseCaseState.SelectingIndustry)
		{
			_presenter.HideIndustriesDetailsView();
		}
		if (_developActionUseCaseState == DevelopActionUseCaseState.IronDelivery)
		{
			_presenter.HideIronMarketHighlight();
		}
		base.CancelAction();
	}

	public virtual void FinishAction()
	{
		_presenter.HideFinishButton();
		_presenter.HideIndustriesDetailsView();
		_presenter.StartIronDeliveryUseCase(_developAction.NeededIron, this);
		_presenter.HideReplayView();
		_developActionUseCaseState = DevelopActionUseCaseState.IronDelivery;
	}

	public void ConfirmNotEnoughResourcesForSecondDevelop()
	{
		_presenter.HideNotEnoughResourcesForSecondDevelopPopup();
		FinishAction();
	}

	protected override void ConfirmAction()
	{
		if (_developActionUseCaseState == DevelopActionUseCaseState.IronDelivery)
		{
			_presenter.DeliverIronAndHide();
		}
		base.ConfirmAction();
	}
}
