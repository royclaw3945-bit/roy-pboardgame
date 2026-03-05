using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.AI;
using BrassApplication.AI.AiBehaviors;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.PlayAiTurn;

public class PlayAiTurnUseCase : IUseCase, IPlayAITurnInput
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(PlayAiTurnUseCase));

	private readonly IGameHistory _gameHistory;

	private readonly IPlayAITurnOutput _presenter;

	public PlayAiTurnUseCase(IPlayAITurnOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void PlayAITurn(bool endTurn = true)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		IAiBehavior aIBehaviour = currentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color).AIBehaviour;
		Logger.Debug("Starting AI Turn");
		while (currentGame.Game.TurnFlow.GetRemainingActionsCount() != 0)
		{
			aIBehaviour.GetNextAIMove(currentGame)();
			currentGame.Game.TurnFlow.ConfirmCurrentAction();
		}
		if (endTurn)
		{
			_presenter.EndTurn();
		}
	}

	public void PrintAITurn()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMetadata playerMetadataByColor = currentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color);
		PrintAITurn(playerMetadataByColor.AiType);
	}

	public void PrintAITurn(AIType aiType)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		IAiBehavior aiBehavior = AiBehaviorFactory.CreateAiBehavior(new PlayerMetadata
		{
			AiType = aiType
		});
		Logger.Debug("\r\n AI suggestion");
		aiBehavior.GetNextAIMove(currentGame);
		aiBehavior.PrintNextAIMove(currentGame, Logger);
		currentGame.Game.TurnFlow.CancelCurrentAction();
	}
}
