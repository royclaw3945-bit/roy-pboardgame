using BrassApplication.Game;
using BrassApplication.UseCases.Menu.Tutorial;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.LoadScene;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Menu.Tutorial;

public class TutorialPresenter : IPresenter, ITutorialOutput
{
	private readonly IRouter _router;

	public TutorialPresenter(IRouter router)
	{
		_router = router;
	}

	public void GameCreated(BrassApplicationGame game)
	{
		_router.ShowView<ILoadSceneView>().OnComplete(delegate
		{
			_router.GetView<ILoadSceneView>().LoadScene("GameScene");
		});
	}

	public void HideView()
	{
		_router.HideView<ITutorialView>().OnComplete(delegate
		{
			_router.ShowView<IMainMenuView>();
		});
	}
}
