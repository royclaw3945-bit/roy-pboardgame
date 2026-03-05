using System;

namespace InterfaceAdapters.Utils.MainThreadDispatcher;

public interface IMainThreadDispatcher
{
	bool isMainThread { get; }

	void InvokeAsync(Action action);

	void Invoke(Action action);
}
