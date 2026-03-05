using System;
using System.Collections.Generic;
using System.Threading;
using InterfaceAdapters.Utils.MainThreadDispatcher;
using UnityEngine;

namespace Utils.MainThreadDispatcher;

[AddComponentMenu("UnityToolbag/UnityMainThreadDispatcher")]
public class UnityMainThreadDispatcher : MonoBehaviour, IMainThreadDispatcher
{
	private static UnityMainThreadDispatcher _instance;

	private static bool _instanceExists;

	private static Thread _mainThread;

	private static readonly object _lockObject = new object();

	private static readonly Queue<Action> _actions = new Queue<Action>();

	public bool isMainThread => Thread.CurrentThread == _mainThread;

	public void InvokeAsync(Action action)
	{
		if (!_instanceExists)
		{
			Debug.LogError("No UnityMainThreadDispatcher exists in the scene. Actions will not be invoked!");
			return;
		}
		if (isMainThread)
		{
			action();
			return;
		}
		lock (_lockObject)
		{
			_actions.Enqueue(action);
		}
	}

	public void Invoke(Action action)
	{
		if (!_instanceExists)
		{
			Debug.LogError("No UnityMainThreadDispatcher exists in the scene. Actions will not be invoked!");
			return;
		}
		bool hasRun = false;
		InvokeAsync(delegate
		{
			action();
			hasRun = true;
		});
		while (!hasRun)
		{
			Thread.Sleep(5);
		}
	}

	private void Awake()
	{
		if ((bool)_instance)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		_instance = this;
		_instanceExists = true;
		_mainThread = Thread.CurrentThread;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
			_instanceExists = false;
		}
	}

	private void Update()
	{
		lock (_lockObject)
		{
			while (_actions.Count > 0)
			{
				_actions.Dequeue()();
			}
		}
	}
}
