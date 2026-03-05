using BrassApplication.UseCases.Common.LoadScene;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Common.LoadScene;

public class LoadScenePresenter : IPresenter, ILoadSceneOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public LoadScenePresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void DisplayTip(int tipLocalizationId, bool wasLastTipNew)
	{
		string translation = _localization.GetTranslation("LoadingTips/TIP_" + tipLocalizationId);
		_router.GetView<ILoadSceneView>().DisplayTip(translation, tipLocalizationId, wasLastTipNew);
	}

	public bool GetWasLastTipNewGlobal()
	{
		return _router.GetView<ILoadSceneView>().GetWasLastTipNewGlobal();
	}

	public int GetLastTipIdGlobal()
	{
		return _router.GetView<ILoadSceneView>().GetLastTipIdGlobal();
	}

	public void HideView()
	{
		_router.HideView<ILoadSceneView>();
	}
}
