using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.FinishedGameReview;

public class FinishedGameReviewUseCase : IUseCase, IFinishedGameReviewInput, IReplayDelegate
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(FinishedGameReviewUseCase));

	private readonly IGameHistory _gameHistory;

	private readonly IFinishedGameReviewOutput _presenter;

	public FinishedGameReviewUseCase(IFinishedGameReviewOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void StartGameReview(PlayerMetadata localPlayerMetadata)
	{
		Logger.Debug("Starting Game Review");
		_presenter.ReplayUnseenActions(localPlayerMetadata, this);
	}

	public void FinishGameReview()
	{
		if (!_gameHistory.CurrentGame.Metadata.GameType.Equals(GameType.Network))
		{
			Logger.Debug("Deleting Game after review");
			_gameHistory.Repository.Delete(_gameHistory.CurrentGame.Metadata.GameId);
		}
	}

	public void ReplayStarted()
	{
		_presenter.ShowSkipToMyNextTurnView();
	}

	public void ShowGameSummaryTable()
	{
		Dictionary<PlayerColor, (int, int, int, int, int)> endGamePoints = _gameHistory.CurrentGame.Game.GameProgressInformation.EndGamePoints;
		bool isIntroductoryGame = _gameHistory.CurrentGame.Metadata.GameStartConfiguration.IsIntroductoryGame;
		Dictionary<PlayerColor, PlayerAvatar> dictionary = _gameHistory.CurrentGame.Metadata.PlayerMetadata.ToDictionary((PlayerMetadata p) => p.PlayerStartConfiguration.Color, (PlayerMetadata p) => p.PlayerAvatar);
		Dictionary<PlayerColor, string> dictionary2 = _gameHistory.CurrentGame.Metadata.PlayerMetadata.ToDictionary((PlayerMetadata p) => p.PlayerStartConfiguration.Color, (PlayerMetadata p) => p.NickName);
		if (isIntroductoryGame)
		{
			Dictionary<PlayerColor, (int, int, int)> introductoryPoints = _gameHistory.CurrentGame.Game.GameProgressInformation.IntroductoryPoints;
			List<(PlayerColor, PlayerAvatar, string, int, int, int, int, int, int, int, int)> list = new List<(PlayerColor, PlayerAvatar, string, int, int, int, int, int, int, int, int)>();
			foreach (Player player in _gameHistory.CurrentGame.Game.PlayersTrack.Players)
			{
				_ = player.VP;
				list.Add((player.Color, dictionary[player.Color], dictionary2[player.Color], endGamePoints[player.Color].Item1, endGamePoints[player.Color].Item2, endGamePoints[player.Color].Item3, player.VP, endGamePoints[player.Color].Item5, introductoryPoints[player.Color].Item1, introductoryPoints[player.Color].Item2, introductoryPoints[player.Color].Item3));
			}
			_presenter.ShowIntroductoryGameSummaryTable(list);
			return;
		}
		List<(PlayerColor, PlayerAvatar, string, int, int, int, int, int, int)> list2 = new List<(PlayerColor, PlayerAvatar, string, int, int, int, int, int, int)>();
		foreach (Player player2 in _gameHistory.CurrentGame.Game.PlayersTrack.Players)
		{
			int item = player2.VP - (endGamePoints[player2.Color].Item2 + endGamePoints[player2.Color].Item1 + endGamePoints[player2.Color].Item3);
			list2.Add((player2.Color, dictionary[player2.Color], dictionary2[player2.Color], item, endGamePoints[player2.Color].Item1, endGamePoints[player2.Color].Item2, endGamePoints[player2.Color].Item3, player2.VP, endGamePoints[player2.Color].Item5));
		}
		_presenter.ShowGameSummaryTable(list2);
	}

	public void ReplayCompleted()
	{
		_presenter.HidePlayerActionView();
		_presenter.ShowButtonToLaunchGameSummaryTable();
		ShowGameSummaryTable();
	}
}
