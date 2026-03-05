using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Screenshots;

public class ScreenshotManager
{
	private class ScreenshotOperation
	{
		public Action<int, byte[]> Callback { get; set; }

		public byte[] Data { get; set; }

		public int FrameNumber { get; set; }

		public bool IsAwaiting { get; set; }

		public bool IsComplete { get; set; }

		public bool IsInUse { get; set; }

		public int MaximumHeight { get; set; }

		public int MaximumWidth { get; set; }

		public object Source { get; set; }

		public void Use()
		{
			Callback = null;
			Data = null;
			FrameNumber = 0;
			IsAwaiting = true;
			IsComplete = false;
			IsInUse = true;
			MaximumHeight = 0;
			MaximumWidth = 0;
			Source = null;
		}
	}

	private Action<byte[], object> screenshotCallbackDelegate;

	private List<ScreenshotOperation> screenshotOperations;

	private ScreenshotRecorder screenshotRecorder;

	public ScreenshotManager()
	{
		screenshotRecorder = new ScreenshotRecorder();
		screenshotCallbackDelegate = ScreenshotCallback;
		screenshotOperations = new List<ScreenshotOperation>();
	}

	private ScreenshotOperation GetScreenshotOperation()
	{
		foreach (ScreenshotOperation screenshotOperation2 in screenshotOperations)
		{
			if (!screenshotOperation2.IsInUse)
			{
				screenshotOperation2.Use();
				return screenshotOperation2;
			}
		}
		ScreenshotOperation screenshotOperation = new ScreenshotOperation();
		screenshotOperation.Use();
		screenshotOperations.Add(screenshotOperation);
		return screenshotOperation;
	}

	public void OnEndOfFrame()
	{
		foreach (ScreenshotOperation screenshotOperation in screenshotOperations)
		{
			if (!screenshotOperation.IsInUse)
			{
				continue;
			}
			if (screenshotOperation.IsAwaiting)
			{
				screenshotOperation.IsAwaiting = false;
				if (screenshotOperation.Source == null)
				{
					screenshotRecorder.Screenshot(screenshotOperation.MaximumWidth, screenshotOperation.MaximumHeight, ScreenshotType.Png, screenshotCallbackDelegate, screenshotOperation);
				}
				else if (screenshotOperation.Source is Camera)
				{
					screenshotRecorder.Screenshot(screenshotOperation.Source as Camera, screenshotOperation.MaximumWidth, screenshotOperation.MaximumHeight, ScreenshotType.Png, screenshotCallbackDelegate, screenshotOperation);
				}
				else if (screenshotOperation.Source is RenderTexture)
				{
					screenshotRecorder.Screenshot(screenshotOperation.Source as RenderTexture, screenshotOperation.MaximumWidth, screenshotOperation.MaximumHeight, ScreenshotType.Png, screenshotCallbackDelegate, screenshotOperation);
				}
				else if (screenshotOperation.Source is Texture2D)
				{
					screenshotRecorder.Screenshot(screenshotOperation.Source as Texture2D, screenshotOperation.MaximumWidth, screenshotOperation.MaximumHeight, ScreenshotType.Png, screenshotCallbackDelegate, screenshotOperation);
				}
				else
				{
					ScreenshotCallback(null, screenshotOperation);
				}
			}
			else
			{
				if (!screenshotOperation.IsComplete)
				{
					continue;
				}
				screenshotOperation.IsInUse = false;
				try
				{
					if (screenshotOperation != null && screenshotOperation.Callback != null)
					{
						screenshotOperation.Callback(screenshotOperation.FrameNumber, screenshotOperation.Data);
					}
				}
				catch
				{
				}
			}
		}
	}

	private void ScreenshotCallback(byte[] data, object state)
	{
		if (state is ScreenshotOperation screenshotOperation)
		{
			screenshotOperation.Data = data;
			screenshotOperation.IsComplete = true;
		}
	}

	public void TakeScreenshot(object source, int frameNumber, int maximumWidth, int maximumHeight, Action<int, byte[]> callback)
	{
		ScreenshotOperation screenshotOperation = GetScreenshotOperation();
		screenshotOperation.FrameNumber = frameNumber;
		screenshotOperation.MaximumWidth = maximumWidth;
		screenshotOperation.MaximumHeight = maximumHeight;
		screenshotOperation.Source = source;
		screenshotOperation.Callback = callback;
	}
}
