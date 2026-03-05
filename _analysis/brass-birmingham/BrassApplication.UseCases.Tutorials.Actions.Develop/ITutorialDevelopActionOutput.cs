using System.Collections.Generic;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.DevelopAction;

namespace BrassApplication.UseCases.Tutorials.Actions.Develop;

public interface ITutorialDevelopActionOutput : IDevelopActionOutput, IBaseActionOutput
{
	void HighlightAllPosiible(BrassApplicationGame game);

	void HighlightCottonMillOnly(BrassApplicationGame game);

	void HighlightNone();

	void HighlightIronWorks(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> slotsToHighlight);

	void ShowPlayerInfoToSelectIronWorks();
}
