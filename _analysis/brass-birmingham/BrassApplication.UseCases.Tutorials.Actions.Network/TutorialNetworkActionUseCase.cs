using System.Collections.Generic;
using BoardGameRules.Entities.Board.Links;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.NetworkAction;

namespace BrassApplication.UseCases.Tutorials.Actions.Network;

public class TutorialNetworkActionUseCase : NetworkActionUseCase
{
	private List<Link> _linksToHighlight;

	public TutorialNetworkActionUseCase(INetworkActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
	}

	public void StartNetworkActionWithSpecifiedLinks(List<Link> linksToHighlight)
	{
		_linksToHighlight = linksToHighlight;
		StartNetworkAction();
	}

	public override void ReplayCompleted()
	{
		NetworkActionUseCase.Logger.Debug("TutorialNetworkActionUseCase.ReplayCompleted");
		if (_linksToHighlight.Count != 0 && _networkActionUseCaseState == NetworkActionUseCaseState.SelectingCard)
		{
			_presenter.ShowPlayerInfoToSelectLinks();
			BrassApplicationGame currentGame = _gameHistory.CurrentGame;
			_presenter.HighlightValidConnections(currentGame, _linksToHighlight, this);
			_networkActionUseCaseState = NetworkActionUseCaseState.SelectLink;
			BaseReplayCompleted();
		}
		else
		{
			base.ReplayCompleted();
		}
	}
}
