using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;

public class UserReportingConfigureOnly : MonoBehaviour
{
	private void Start()
	{
		if (UnityUserReporting.CurrentClient == null)
		{
			UnityUserReporting.Configure();
		}
	}
}
