using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.SellAction;
using BrassApplication.UseCases.Tutorials.Actions.SellAction;
using BrassApplication.UseCases.Tutorials.Tutorial;
using InterfaceAdapters.Game.Actions.SellAction;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Tutorials.Actions.SellAction;

public class TutorialSellActionPresenter : SellActionPresenter, ITutorialSellActionOutput, ISellActionOutput, IBaseActionOutput
{
	public TutorialSellActionPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public void NextTutorialStage()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialGameSceneUseCase>().MoveToTheNextStage();
	}

	public void ShowTutorialNextSellChoicePanel(List<BuildingSlot> buildingSlotsWithIndustriesToSell)
	{
		List<string> list = new List<string>();
		foreach (BuildingSlot item in buildingSlotsWithIndustriesToSell)
		{
			list.Add(item.GetCurrentImageName());
		}
		_router.ShowView<ITutorialSellActionView>();
		_router.GetView<ITutorialSellActionView>().SetSellActionView(list);
	}

	public override void HideNextSellChoicePanel()
	{
		_router.HideView<ITutorialSellActionView>();
	}
}
