using BoardGameRules.Entities.Cards;

namespace BrassApplication.UseCases.Game.Actions;

public interface ISelectCardDelegate
{
	void SelectCard(BaseCard card);
}
