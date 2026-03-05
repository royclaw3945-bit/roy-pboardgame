using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.PlayerActions;

namespace BrassApplication.UseCases.Tutorials.PlayerActions;

public class TutorialPlayerActionsUseCase : PlayerActionsUseCase, ITutorialPlayerActionsInput, IPlayerActionsInput
{
	public TutorialPlayerActionsUseCase(IPlayerActionsOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
	}
}
