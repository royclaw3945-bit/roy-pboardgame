using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;
using UnityEngine.XR;

public class UserReportingXRExtensions : MonoBehaviour
{
	private void Start()
	{
		if (XRDevice.isPresent)
		{
			UnityUserReporting.CurrentClient.AddDeviceMetadata("XRDeviceModel", XRDevice.model);
		}
	}

	private void Update()
	{
		if (XRDevice.isPresent)
		{
			if (XRStats.TryGetDroppedFrameCount(out var droppedFrameCount))
			{
				UnityUserReporting.CurrentClient.SampleMetric("XR.DroppedFrameCount", droppedFrameCount);
			}
			if (XRStats.TryGetFramePresentCount(out var framePresentCount))
			{
				UnityUserReporting.CurrentClient.SampleMetric("XR.FramePresentCount", framePresentCount);
			}
			if (XRStats.TryGetGPUTimeLastFrame(out var gpuTimeLastFrame))
			{
				UnityUserReporting.CurrentClient.SampleMetric("XR.GPUTimeLastFrame", gpuTimeLastFrame);
			}
		}
	}
}
