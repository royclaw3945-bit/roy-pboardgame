using BrassApplication.UseCases.Game.PlayerActions;
using BrassApplication.UseCases.Tutorials.PlayerActions;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Tutorials.PlayerActions;

public class TutorialPlayerActionsPresenter : PlayerActionsPresenter, ITutorialPlayerActionsOutput, IPlayerActionsOutput
{
	public TutorialPlayerActionsPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}
}
