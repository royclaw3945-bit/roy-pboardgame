using System;

namespace InterfaceAdapters.Common;

public interface IAnimation
{
	IAnimation Play();

	IAnimation Kill();

	IAnimation OnComplete(Action action);

	IAnimation OnKill(Action action);

	IAnimation AppendCallback(Action action);

	bool IsPlaying();
}
