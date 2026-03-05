using BrassApplication.GameHistory;
using BrassApplication.Network;
using BrassApplication.Utils;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace Utils;

public class Global
{
	private static volatile Global _instance;

	public static Global Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new Global();
			}
			return _instance;
		}
	}

	public IMainThreadDispatcher MainThreadDispatcher { get; set; }

	public IRouter Router { get; set; }

	public IAnalytics Analytics { get; set; }

	public ISettingStorage SettingStorage { get; set; }

	public IVersionReader VersionReader { get; set; }

	public INativePlatform NativePlatform { get; set; }

	public ILocalization Localization { get; set; }

	public IUseCaseBuilder UseCaseBuilder { get; set; }

	public IGameHistory BrassGameHistory { get; set; }

	public INetworkBackendClient NetworkBackendClient { get; set; }

	public bool SplashSeen { get; set; }

	public bool GoToCampaignMenu { get; set; }

	public bool PlayerDidWonPreviousGame { get; set; }

	public bool CameFromMenu { get; set; }

	public bool WasLastTipNew { get; set; }

	public int LastTipId { get; set; }

	private Global()
	{
	}
}
