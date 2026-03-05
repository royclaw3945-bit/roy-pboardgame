using System;

namespace InterfaceAdapters.Common;

public interface IAnimationSequence : IAnimation
{
	IAnimationSequence Append(IAnimation animation);

	new IAnimationSequence AppendCallback(Action action);

	IAnimationSequence AppendInterval(float intervalInSeconds);

	IAnimationSequence Insert(float delayInSeconds, IAnimation animation);
}
