using System.Collections;
using UnityEngine;

namespace Unity.Cloud.UserReporting.Plugin;

public class UnityUserReportingUpdater : IEnumerator
{
	private int step;

	private WaitForEndOfFrame waitForEndOfFrame;

	public object Current { get; private set; }

	public UnityUserReportingUpdater()
	{
		waitForEndOfFrame = new WaitForEndOfFrame();
	}

	public bool MoveNext()
	{
		if (step == 0)
		{
			UnityUserReporting.CurrentClient.Update();
			Current = null;
			step = 1;
			return true;
		}
		if (step == 1)
		{
			Current = waitForEndOfFrame;
			step = 2;
			return true;
		}
		if (step == 2)
		{
			UnityUserReporting.CurrentClient.UpdateOnEndOfFrame();
			Current = null;
			step = 3;
			return false;
		}
		return false;
	}

	public void Reset()
	{
		step = 0;
	}
}
