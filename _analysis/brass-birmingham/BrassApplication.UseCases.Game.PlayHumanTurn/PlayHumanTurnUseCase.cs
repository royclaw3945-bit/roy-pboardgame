using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.PlayHumanTurn;

public class PlayHumanTurnUseCase : IUseCase, IPlayHumanTurnInput, IReplayDelegate
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(PlayHumanTurnUseCase));

	private readonly IGameHistory _gameHistory;

	private readonly IPlayHumanTurnOutput _presenter;

	public PlayHumanTurnUseCase(IPlayHumanTurnOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void StartHumanTurn()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMetadata playerMetadataByColor = currentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color);
		Logger.Debug("Starting Human Turn");
		_presenter.ReplayUnseenActions(playerMetadataByColor, this);
	}

	public void ReplayStarted()
	{
		_presenter.ShowSkipToMyNextTurnView();
	}

	public void ReplayCompleted()
	{
		_presenter.RestorePlayerActionsView();
	}
}
