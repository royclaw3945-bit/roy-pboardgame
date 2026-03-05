using BrassApplication.Utils;

namespace Utils.NativePlatformAPI;

public class NativePlatformSettings
{
	private static readonly string DID_ASK_FOR_PUSH = "DID_ASK_FOR_PUSH";

	private static readonly string DECLINED_OR_RATED_APP = "DECLINED_OR_RATED_APP";

	private readonly ISettingStorage _settingStorage;

	public bool DidUserSeenQuestionAboutPush
	{
		get
		{
			return _settingStorage.GetInt(DID_ASK_FOR_PUSH) == 1;
		}
		set
		{
			_settingStorage.SetInt(DID_ASK_FOR_PUSH, value ? 1 : 0);
			_settingStorage.Save();
		}
	}

	public bool DidRateOrDeclineToRateTheApp
	{
		get
		{
			return _settingStorage.GetInt(DECLINED_OR_RATED_APP) == 1;
		}
		set
		{
			_settingStorage.SetInt(DECLINED_OR_RATED_APP, value ? 1 : 0);
			_settingStorage.Save();
		}
	}

	public NativePlatformSettings(ISettingStorage settingStorage)
	{
		_settingStorage = settingStorage;
	}
}
