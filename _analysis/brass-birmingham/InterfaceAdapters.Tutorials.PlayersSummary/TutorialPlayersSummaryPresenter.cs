using BrassApplication.UseCases.Game.PlayersSummary;
using BrassApplication.UseCases.Tutorials.PlayersSummary;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Tutorials.PlayersSummary;

public class TutorialPlayersSummaryPresenter : PlayersSummaryPresenter, ITutorialPlayersSummaryOutput, IPlayersSummaryOutput
{
	public TutorialPlayersSummaryPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}
}
