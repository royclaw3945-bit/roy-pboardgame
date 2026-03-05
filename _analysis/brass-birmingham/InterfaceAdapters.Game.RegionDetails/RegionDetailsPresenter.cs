using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.RegionDetails;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Game.RegionDetails;

public class RegionDetailsPresenter : IPresenter, IRegionDetailsOutput
{
	private readonly IRouter _router;

	public RegionDetailsPresenter(IRouter router)
	{
		_router = router;
	}

	public void BackToBoard()
	{
		_router.HideView<IRegionDetailsView>();
	}

	public void CancelAction()
	{
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().CancelAction();
	}
}
