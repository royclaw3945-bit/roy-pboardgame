using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Screenshots;

public class ScreenshotRecorder
{
	private class ScreenshotOperation
	{
		public WaitCallback EncodeCallbackDelegate;

		public Action<AsyncGPUReadbackRequest> ScreenshotCallbackDelegate;

		public Action<byte[], object> Callback { get; set; }

		public int Height { get; set; }

		public int Identifier { get; set; }

		public bool IsInUse { get; set; }

		public int MaximumHeight { get; set; }

		public int MaximumWidth { get; set; }

		public NativeArray<byte> NativeData { get; set; }

		public Texture Source { get; set; }

		public object State { get; set; }

		public ScreenshotType Type { get; set; }

		public int Width { get; set; }

		public ScreenshotOperation()
		{
			ScreenshotCallbackDelegate = ScreenshotCallback;
			EncodeCallbackDelegate = EncodeCallback;
		}

		private void EncodeCallback(object state)
		{
			byte[] dataRgba = NativeData.ToArray();
			dataRgba = Downsampler.Downsample(dataRgba, Width * 4, MaximumWidth, MaximumHeight, out var downsampledStride);
			if (Type == ScreenshotType.Png)
			{
				dataRgba = PngEncoder.Encode(dataRgba, downsampledStride);
			}
			if (Callback != null)
			{
				Callback(dataRgba, State);
			}
			NativeData.Dispose();
			IsInUse = false;
		}

		private void SavePngToDisk(byte[] byteData)
		{
			if (!Directory.Exists("Screenshots"))
			{
				Directory.CreateDirectory("Screenshots");
			}
			File.WriteAllBytes($"Screenshots/{Identifier % 60}.png", byteData);
		}

		private void ScreenshotCallback(AsyncGPUReadbackRequest request)
		{
			if (!request.hasError)
			{
				NativeLeakDetection.Mode = NativeLeakDetectionMode.Disabled;
				NativeArray<byte> data = request.GetData<byte>();
				NativeArray<byte> nativeData = new NativeArray<byte>(data, Allocator.Persistent);
				Width = request.width;
				Height = request.height;
				NativeData = nativeData;
				ThreadPool.QueueUserWorkItem(EncodeCallbackDelegate, null);
			}
			else if (Callback != null)
			{
				Callback(null, State);
			}
			if (Source != null)
			{
				UnityEngine.Object.Destroy(Source);
			}
		}
	}

	private static int nextIdentifier;

	private List<ScreenshotOperation> operationPool;

	public ScreenshotRecorder()
	{
		operationPool = new List<ScreenshotOperation>();
	}

	private ScreenshotOperation GetOperation()
	{
		foreach (ScreenshotOperation item in operationPool)
		{
			if (!item.IsInUse)
			{
				item.IsInUse = true;
				return item;
			}
		}
		ScreenshotOperation screenshotOperation = new ScreenshotOperation();
		screenshotOperation.IsInUse = true;
		operationPool.Add(screenshotOperation);
		return screenshotOperation;
	}

	public void Screenshot(int maximumWidth, int maximumHeight, ScreenshotType type, Action<byte[], object> callback, object state)
	{
		Texture2D source = ScreenCapture.CaptureScreenshotAsTexture();
		Screenshot(source, maximumWidth, maximumHeight, type, callback, state);
	}

	public void Screenshot(Camera source, int maximumWidth, int maximumHeight, ScreenshotType type, Action<byte[], object> callback, object state)
	{
		RenderTexture renderTexture = new RenderTexture(maximumWidth, maximumHeight, 24);
		RenderTexture targetTexture = source.targetTexture;
		source.targetTexture = renderTexture;
		source.Render();
		source.targetTexture = targetTexture;
		Screenshot(renderTexture, maximumWidth, maximumHeight, type, callback, state);
	}

	public void Screenshot(RenderTexture source, int maximumWidth, int maximumHeight, ScreenshotType type, Action<byte[], object> callback, object state)
	{
		ScreenshotInternal(source, maximumWidth, maximumHeight, type, callback, state);
	}

	public void Screenshot(Texture2D source, int maximumWidth, int maximumHeight, ScreenshotType type, Action<byte[], object> callback, object state)
	{
		ScreenshotInternal(source, maximumWidth, maximumHeight, type, callback, state);
	}

	private void ScreenshotInternal(Texture source, int maximumWidth, int maximumHeight, ScreenshotType type, Action<byte[], object> callback, object state)
	{
		ScreenshotOperation operation = GetOperation();
		operation.Identifier = nextIdentifier++;
		operation.Source = source;
		operation.MaximumWidth = maximumWidth;
		operation.MaximumHeight = maximumHeight;
		operation.Type = type;
		operation.Callback = callback;
		operation.State = state;
		AsyncGPUReadback.Request(source, 0, TextureFormat.RGBA32, operation.ScreenshotCallbackDelegate);
	}
}
