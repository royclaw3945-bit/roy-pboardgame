using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.FinishedGameReview;

public interface IFinishedGameReviewInput : IReplayDelegate
{
	void ShowGameSummaryTable();

	void FinishGameReview();
}
