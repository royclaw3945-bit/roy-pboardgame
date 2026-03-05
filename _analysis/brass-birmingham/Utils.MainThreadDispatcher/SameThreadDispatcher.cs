using System;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace Utils.MainThreadDispatcher;

public class SameThreadDispatcher : IMainThreadDispatcher
{
	public bool isMainThread => true;

	public void InvokeAsync(Action action)
	{
		action();
	}

	public void Invoke(Action action)
	{
		action();
	}
}
