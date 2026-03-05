using BrassApplication.UseCases.Game.Actions.BaseAction;

namespace BrassApplication.UseCases.Game.Actions.PassAction;

public interface IPassActionOutput : IBaseActionOutput
{
	void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount);

	void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate);
}
