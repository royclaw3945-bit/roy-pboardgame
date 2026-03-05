using BoardGameRules.Entities.EventSourcing.Events;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Replay;

public interface IReplayView : IBaseView
{
	IAnimation CreateAnimation(IEvent @event);

	void PauseAllAnimations();

	void ResumeAllAnimations();

	void KillAllAnimations();

	void SetCurrentPlayerName(string playerColor);

	void SetCurrentPlayerAction(string action);

	void RestoreDefaultAnimationSpeed();

	void HideSkipButton();

	void SetSkipButtonInteractable(bool interactable);

	bool IsAnyTweenInProgress();

	void SetFastForwardActive(bool isActive);
}
