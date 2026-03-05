using System;
using BoardGameRules.Utils.Logging;
using DG.Tweening;
using Frameworks.Analytics;
using Frameworks.Routing;
using Frameworks.Routing.Unity;
using Frameworks.Settings;
using Frameworks.Utils;
using Frameworks.Utils.Localization;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils.MainThreadDispatcher;
using UnityEngine;
using Utils;
using Utils.Logging;
using Utils.MainThreadDispatcher;
using Utils.NativePlatformAPI;

namespace Frameworks.Init;

public class InitUnity : MonoBehaviour, IFrameworksDelegate
{
	private FrameworksInit _frameworks;

	private IMainThreadDispatcher _unityMainThreadDispatcher;

	private UnityInputSource _unityInputSource;

	private UnityViewFactory _unityViewFactory;

	private UnityRouter _unityRouter;

	private VersionReader _versionReader;

	private void Awake()
	{
		_frameworks = new FrameworksInit(this);
		PrepareReferences();
		Init();
		_frameworks.Init();
	}

	private void OnDestroy()
	{
		_frameworks.Dispose();
	}

	private void PrepareReferences()
	{
		_unityMainThreadDispatcher = GameObject.Find("Utils/UnityMainThreadDispatcher").GetComponent<UnityMainThreadDispatcher>();
		_unityInputSource = GameObject.Find("Utils/UnityInputSource").GetComponent<UnityInputSource>();
		_unityViewFactory = GameObject.Find("Utils/UnityViewFactory").GetComponent<UnityViewFactory>();
		_unityRouter = GameObject.Find("Utils/UnityRouter").GetComponent<UnityRouter>();
		_versionReader = GameObject.Find("Utils/VersionReader").GetComponent<VersionReader>();
	}

	private void Init()
	{
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 1;
		DOTween.Init();
		Global.Instance.Analytics = new UnityAnalytics();
		Global.Instance.SettingStorage = new UnitySettingStorage();
		Global.Instance.VersionReader = _versionReader;
		Global.Instance.NativePlatform = NativePlatformFactory.CreateNativePlatform(Global.Instance.SettingStorage);
	}

	public void LoggerInit()
	{
		LogFactory.BuildLogger = (Type type) => new UnityLogger(type);
	}

	public void MainThreadDispatcherInit()
	{
		Global.Instance.MainThreadDispatcher = _unityMainThreadDispatcher;
	}

	public void RouterInit()
	{
		UnityRouter unityRouter = _unityRouter;
		IRouter router = (Global.Instance.Router = new Router(_unityViewFactory, _unityInputSource));
		unityRouter.Router = router;
	}

	public void LocalizationInit()
	{
		Global.Instance.Localization = new I2Localization();
	}
}
