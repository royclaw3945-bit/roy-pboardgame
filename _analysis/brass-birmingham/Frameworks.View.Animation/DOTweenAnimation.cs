using System;
using DG.Tweening;
using InterfaceAdapters.Common;

namespace Frameworks.View.Animation;

public class DOTweenAnimation : IAnimation
{
	public Sequence Sequence { get; }

	public DOTweenAnimation(Sequence sequence)
	{
		Sequence = sequence;
		Sequence.Pause();
	}

	public DOTweenAnimation(Tweener tweener)
	{
		Sequence = DOTween.Sequence();
		Sequence.Append(tweener);
		Sequence.Pause();
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

	public IAnimation AppendCallback(Action action)
	{
		Sequence.AppendCallback(delegate
		{
			action();
		});
		return this;
	}

	public bool IsPlaying()
	{
		return Sequence.IsPlaying();
	}
}
