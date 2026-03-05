using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Menu.OnlineGame.ChoosePortraitAndJoin;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Menu.ChoosePortraitAndJoin;

public class ChoosePortraitAndJoinPresenter : IChoosePortraitAndJoinOutput, IPresenter
{
	private readonly IRouter _router;

	public ChoosePortraitAndJoinPresenter(IRouter router)
	{
		_router = router;
	}

	public void HideView()
	{
		_router.HideView<IChoosePortraitAndJoinView>();
	}

	public void SetPortrait(PlayerAvatar currentAvatar)
	{
		_router.GetView<IChoosePortraitAndJoinView>().UpdateAvatar(currentAvatar);
	}

	public void SetView(PlayerAvatar currentAvatar, PlayerColor color)
	{
		_router.GetView<IChoosePortraitAndJoinView>().UpdateView(color, currentAvatar);
	}
}
