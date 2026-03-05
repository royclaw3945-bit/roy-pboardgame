using System.Linq;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.AI;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Network;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Common.SaveGame;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.Game.PlayGame;

public class PlayGameUseCase : IUseCase, IPlayGameInput, ISaveGameDelegate
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(PlayGameUseCase));

	private readonly IGameHistory _gameHistory;

	private readonly IPlayGameOutput _presenter;

	private readonly INetworkBackendClient _networkBackendClient;

	private readonly IAnalytics _analytics;

	public PlayGameUseCase(IPlayGameOutput presenter, IGameHistory gameHistory, INetworkBackendClient networkBackendClient, IAnalytics analytics)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
		_networkBackendClient = networkBackendClient;
		_analytics = analytics;
	}

	public void PlayGame()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Logger.Debug("PlayGameUseCase.PlayGame(): Current Round: {0}, Current Turn: {1}", currentGame.Game.GameProgressInformation.CurrentRoundNumber, currentGame.Game.GameProgressInformation.CurrentTurnNumber);
		if (currentGame.Game.GameProgressInformation.IsGameOver)
		{
			Logger.Debug("PlayGameUseCase: Game is over");
			PlayerMetadata localPlayerMetadata;
			if (currentGame.Metadata.GameType.Equals(GameType.Network))
			{
				string loggedInPlayerId = _networkBackendClient.UserManagement.UserId;
				localPlayerMetadata = currentGame.Metadata.PlayerMetadata.First((PlayerMetadata p) => p.PlayerNetworkId.Equals(loggedInPlayerId));
			}
			else
			{
				localPlayerMetadata = currentGame.Metadata.PlayerMetadata.First((PlayerMetadata p) => p.AiType.Equals(AIType.Human));
			}
			_presenter.StartGameReview(localPlayerMetadata);
			if (!currentGame.Metadata.GameType.Equals(GameType.Network))
			{
				_analytics.GameOver(currentGame.Metadata);
			}
			return;
		}
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMetadata playerMetadataByColor = currentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color);
		if (currentGame.Metadata.GameType.Equals(GameType.Network))
		{
			string userId = _networkBackendClient.UserManagement.UserId;
			if (playerMetadataByColor.PlayerNetworkId.Equals(userId))
			{
				Logger.Debug("PlayGameUseCase: Network game and local player turn. Launching Network Human Turn.");
				_presenter.StartNetworkHumanTurn();
			}
			else
			{
				Logger.Debug("PlayGameUseCase: Network game and remote player turn. Launching Network Spectator Turn");
				PlayerMetadata playerMetadataByNetworkPlayerId = currentGame.Metadata.GetPlayerMetadataByNetworkPlayerId(userId);
				_presenter.StartNetworkSpectatorTurn(playerMetadataByNetworkPlayerId);
			}
		}
		else if (playerMetadataByColor.AiType.Equals(AIType.Human))
		{
			Logger.Debug("PlayGameUseCase: Local game and human player turn. Launching Human Turn.");
			_presenter.StartHumanTurn();
		}
		else
		{
			Logger.Debug("PlayGameUseCase: Local game and AI turn. Launching AI Turn.");
			_presenter.StartAITurn(playerMetadataByColor);
		}
	}

	public void EndTurn()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Logger.Debug("PlayGameUseCase.EndTurn(): Current Round: {0}, Current Turn: {1}", currentGame.Game.GameProgressInformation.CurrentRoundNumber, currentGame.Game.GameProgressInformation.CurrentTurnNumber);
		GameFlow gameFlow = currentGame.Game.GameFlow;
		gameFlow.EndTurn();
		if (gameFlow.IsLastTurnOver())
		{
			if (gameFlow.IsSellIndustryNecessary())
			{
				_presenter.StartSellingIndustries();
			}
			gameFlow.EndRound();
		}
		_presenter.SaveGame(currentGame, this);
	}

	public void SaveGameComplete(bool isSuccessful)
	{
		Logger.Debug("PlayGameUseCase.SaveGameComplete()");
		PlayGame();
	}
}
