using BrassApplication.UseCases.Game.FinishedGameReview;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.FinishedGameReview;

public class FinishedGameReviewController : IController
{
	private readonly IFinishedGameReviewInput _useCase;

	public FinishedGameReviewController(IFinishedGameReviewInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
		_useCase.FinishGameReview();
	}

	public void OnShowGameSummaryTableClicked()
	{
		_useCase.ShowGameSummaryTable();
	}
}
