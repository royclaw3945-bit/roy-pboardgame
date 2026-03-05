using BoardGameRules.Entities.EventSourcing.Events;

namespace InterfaceAdapters.Common;

public interface IAnimationFactory
{
	IAnimationSequence CreateAnimationSequence();

	IAnimation CreateAnimation(IEvent @event);

	void PauseAllAnimations();

	void ResumeAllAnimations();

	void KillAllAnimations();
}
