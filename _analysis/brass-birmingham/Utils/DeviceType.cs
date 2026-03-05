using UnityEngine;

namespace Utils;

public static class DeviceType
{
	public static string GetDeviceType()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			return "ios";
		case RuntimePlatform.Android:
			return "android";
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.LinuxPlayer:
			return "pc";
		default:
			return null;
		}
	}
}
