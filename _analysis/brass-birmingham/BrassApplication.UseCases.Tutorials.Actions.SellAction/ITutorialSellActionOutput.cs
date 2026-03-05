using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.SellAction;

namespace BrassApplication.UseCases.Tutorials.Actions.SellAction;

public interface ITutorialSellActionOutput : ISellActionOutput, IBaseActionOutput
{
	void NextTutorialStage();

	void ShowTutorialNextSellChoicePanel(List<BuildingSlot> buildingSlotsWithIndustriesToSell);
}
