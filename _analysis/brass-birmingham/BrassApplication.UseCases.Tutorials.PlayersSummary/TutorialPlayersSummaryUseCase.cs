using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.PlayersSummary;

namespace BrassApplication.UseCases.Tutorials.PlayersSummary;

public class TutorialPlayersSummaryUseCase : PlayersSummaryUseCase, ITutorialPlayersSummaryInput, IPlayersSummaryInput
{
	private readonly ITutorialPlayersSummaryOutput _presenter;

	public TutorialPlayersSummaryUseCase(ITutorialPlayersSummaryOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}
}
