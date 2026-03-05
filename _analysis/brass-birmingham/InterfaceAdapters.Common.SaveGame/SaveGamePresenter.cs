using BoardGameRules.Utils.Logging;
using BrassApplication.UseCases.Common.SaveGame;
using InterfaceAdapters.Common.LoadScene;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace InterfaceAdapters.Common.SaveGame;

public class SaveGamePresenter : IPresenter, ISaveGameOutput
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(SaveGamePresenter));

	private readonly IRouter _router;

	private readonly IMainThreadDispatcher _dispatcher;

	public SaveGamePresenter(IRouter router, IMainThreadDispatcher dispatcher)
	{
		_router = router;
		_dispatcher = dispatcher;
	}

	public void SaveGameCompleted(bool isSaveSuccessful, ISaveGameDelegate saveGameDelegate)
	{
		_dispatcher.InvokeAsync(delegate
		{
			Logger.Debug("SaveGame completed.");
			if (!isSaveSuccessful)
			{
				if (!_router.IsViewVisible<ISaveGameView>())
				{
					_router.ShowView<ISaveGameView>();
				}
				_router.GetView<ISaveGameView>().ShowSaveFailed(delegate
				{
					_router.ShowView<ILoadSceneView>().OnComplete(delegate
					{
						_router.GetView<ILoadSceneView>().LoadScene("MenuScene");
					});
				});
			}
			else
			{
				if (_router.IsViewVisible<ISaveGameView>())
				{
					_router.HideView<ISaveGameView>();
				}
				saveGameDelegate.SaveGameComplete(isSaveSuccessful);
			}
		});
	}

	public void SaveGameStarted()
	{
		_router.ShowView<ISaveGameView>();
		_router.GetView<ISaveGameView>().ShowSaveInProgress();
	}
}
