using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Game.Actions;

namespace BrassApplication.UseCases.Game.HandOfCards;

public class HandOfCardsUseCase : IUseCase, IHandOfCardsInput
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(HandOfCardsUseCase));

	private readonly IHandOfCardsOutput _presenter;

	protected readonly IGameHistory _gameHistory;

	protected ISelectCardDelegate _selectCardDelegate;

	public HandOfCardsUseCase(IHandOfCardsOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void UpdateCards(PlayerMetadata replayWatchingPlayerMetadata)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<BaseCard> hand = currentGame.Game.GameProgressInformation.CurrentPlayer.Hand;
		bool shouldHideCards = !currentGame.Game.GameProgressInformation.CurrentPlayer.Color.Equals(replayWatchingPlayerMetadata.PlayerStartConfiguration.Color);
		if (currentGame.Game.TurnFlow.CurrentAction != currentGame.Game.TurnFlow.ScoutAction)
		{
			_presenter.UpdateHandCardsView(hand, shouldHideCards);
		}
	}

	public void UpdateCardsForPlayer(PlayerMetadata playerMetadata)
	{
		List<BaseCard> hand = _gameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(playerMetadata.PlayerStartConfiguration.Color).Hand;
		_presenter.UpdateHandCardsView(hand, shouldHideCards: false);
	}

	public void OnCardSelected(BaseCard card)
	{
		if (_selectCardDelegate != null)
		{
			_selectCardDelegate.SelectCard(card);
		}
		else if (card is LocationCard locationCard)
		{
			List<BuildingSlot> buildingSlotsWhereBuildIsPossible = BuildAction.GetBuildingSlotsWhereBuildingIsPossibleStatic(_gameHistory.CurrentGame.Game.Board, _gameHistory.CurrentGame.Game.GameProgressInformation, (BuildingType t) => true).ToList();
			_presenter.OnLocationCardClicked(locationCard.Region, buildingSlotsWhereBuildIsPossible, _gameHistory.CurrentGame.Game.Board);
		}
	}

	public void SetOnCardClickActionDelegate(ISelectCardDelegate selectCardDelegate)
	{
		_selectCardDelegate = selectCardDelegate;
	}

	public string GetRegionFriendlyName(string regionName)
	{
		return _presenter.GetRegionFriendlyName(regionName);
	}
}
