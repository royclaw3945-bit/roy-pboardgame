using System;
using DG.Tweening;
using InterfaceAdapters.Common;

namespace Frameworks.View.Animation;

public class DOTweenAnimationSequence : IAnimationSequence, IAnimation
{
	public Sequence Sequence { get; protected set; }

	public DOTweenAnimationSequence()
	{
		Sequence = DOTween.Sequence();
		Sequence.Pause();
	}

	public DOTweenAnimationSequence(Sequence doTweenSequence)
	{
		Sequence = doTweenSequence;
		Sequence.Pause();
	}

	public IAnimationSequence Append(IAnimation animation)
	{
		if (animation is DOTweenAnimationSequence dOTweenAnimationSequence)
		{
			dOTweenAnimationSequence.Sequence.Play();
			Sequence.Append(dOTweenAnimationSequence.Sequence);
		}
		return this;
	}

	public IAnimationSequence AppendCallback(Action action)
	{
		Sequence.AppendCallback(delegate
		{
			action();
		});
		return this;
	}

	public IAnimationSequence AppendInterval(float intervalInSeconds)
	{
		Sequence.AppendInterval(intervalInSeconds);
		return this;
	}

	public IAnimationSequence Insert(float delayInSeconds, IAnimation animation)
	{
		if (animation is DOTweenAnimationSequence dOTweenAnimationSequence)
		{
			dOTweenAnimationSequence.Sequence.Play();
			Sequence.Insert(delayInSeconds, dOTweenAnimationSequence.Sequence);
		}
		return this;
	}

	public IAnimation Play()
	{
		Sequence.Play();
		return this;
	}

	public IAnimation Kill()
	{
		Sequence.Kill();
		return this;
	}

	public IAnimation OnComplete(Action action)
	{
		Sequence.OnComplete(delegate
		{
			action();
		});
		return this;
	}

	public IAnimation OnKill(Action action)
	{
		Sequence.OnKill(delegate
		{
			action();
		});
		return this;
	}

	IAnimation IAnimation.AppendCallback(Action action)
	{
		return AppendCallback(action);
	}

	public bool IsPlaying()
	{
		return Sequence.IsPlaying();
	}
}
