using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.ActionInProgressView;

public class ActionInProgressPresenter : IPresenter, IActionInProgressOutput
{
	private IRouter _router;

	private readonly ILocalization _localization;

	public ActionInProgressPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
		_localization.GetTranslation("UI/GAME_NAME");
	}
}
