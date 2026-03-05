using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Game.Replay;

namespace InterfaceAdapters.Utils;

public class ChangeEventToTextHelper
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(ReplayUseCase));

	private readonly IGameHistory _gameHistory;

	private readonly ILocalization _localization;

	public ChangeEventToTextHelper(IGameHistory gameHistory, ILocalization localization)
	{
		_gameHistory = gameHistory;
		_localization = localization;
	}

	public virtual Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> ChangeEventToText(IEvent @event)
	{
		try
		{
			return this.DoubleDispatchChangeEventToText(@event);
		}
		catch (ArgumentException)
		{
			Logger.Verbose(string.Concat("ChangeEventToText not supported for event ", @event, ", returning empty text"));
		}
		return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(PlayerColor.Blue, ActionLogRoundViewModel.TextEventType.ERROR, "");
	}

	public Tuple<ActionLogRoundViewModel.TextEventType, string> FuseEventTexts(string text1, string text2, ActionLogRoundViewModel.TextEventType newType)
	{
		string text3 = "";
		switch (newType)
		{
		case ActionLogRoundViewModel.TextEventType.FUSED_DEVELOP:
			text3 = _localization.GetTranslation("UI/INDUSTRY_DEVELOPED_EVENT_TEXT");
			break;
		case ActionLogRoundViewModel.TextEventType.FUSED_LINK:
			text3 = _localization.GetTranslation("UI/LINK_PLACED_EVENT_TEXT");
			break;
		case ActionLogRoundViewModel.TextEventType.FUSED_INDUSTRY_FLIPPED:
			text3 = _localization.GetTranslation("UI/INDUSTRY_FLIPPED_EVENT_TEXT");
			break;
		}
		string item = "";
		switch (newType)
		{
		case ActionLogRoundViewModel.TextEventType.FUSED_DEVELOP:
		{
			string text5 = (from s in text3.Split(new string[1] { "{0}" }, StringSplitOptions.None)
				where s.Length > 0
				select s).First();
			List<string> list2 = new List<string>();
			list2.Add((from s in text1.Split(new string[1] { text5 }, StringSplitOptions.None)
				where s.Length > 0
				select s).First());
			list2.Add((from s in text2.Split(new string[1] { text5 }, StringSplitOptions.None)
				where s.Length > 0
				select s).First());
			item = string.Format(_localization.GetTranslation("UI/INDUSTRY_DEVELOPED_EVENT_TEXT"), list2[0] + " " + _localization.GetTranslation("UI/AND") + " " + list2[1]);
			break;
		}
		case ActionLogRoundViewModel.TextEventType.FUSED_LINK:
		{
			string text4 = (from s in text3.Split(new string[1] { "{0}" }, StringSplitOptions.None)
				where s.Length > 0
				select s).First();
			List<string> list = new List<string>();
			list.Add((from s in text1.Split(new string[1] { text4 }, StringSplitOptions.None)
				where s.Length > 0
				select s).First());
			list.Add((from s in text2.Split(new string[1] { text4 }, StringSplitOptions.None)
				where s.Length > 0
				select s).First());
			item = text4 + " " + list[0] + " " + _localization.GetTranslation("UI/AND") + " " + list[1];
			break;
		}
		case ActionLogRoundViewModel.TextEventType.FUSED_INDUSTRY_FLIPPED:
			item = text1 + " " + _localization.GetTranslation("UI/AND") + " " + text2;
			break;
		}
		return new Tuple<ActionLogRoundViewModel.TextEventType, string>(newType, item);
	}

	private Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> ChangeEventToText(ActionPassed @event)
	{
		return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(@event.Player, ActionLogRoundViewModel.TextEventType.PASS, _localization.GetTranslation("UI/ACTION_PASSED_EVENT_TEXT"));
	}

	private Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> ChangeEventToText(LinkPlaced @event)
	{
		Link linkById = _gameHistory.CurrentGame.Game.Board.GetLinkById(@event.LinkId);
		return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(@event.Player, ActionLogRoundViewModel.TextEventType.LINK_PLACED, string.Format(_localization.GetTranslation("UI/LINK_PLACED_EVENT_TEXT"), (linkById is Canal) ? _localization.GetTranslation("UI/CANAL") : _localization.GetTranslation("UI/RAIL"), _localization.GetTranslation("Regions/" + linkById.Region1.Name), _localization.GetTranslation("Regions/" + linkById.Region2.Name)));
	}

	private Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> ChangeEventToText(IndustryBuilt @event)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		string translation = _localization.GetTranslation("Regions/" + currentGame.Game.Board.GetRegionContainingGivenSlot(currentGame.Game.Board.GetBuildingSlotByUniqueId(@event.SlotUniqueId)).Name);
		return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(@event.Player, ActionLogRoundViewModel.TextEventType.INDUSTRY_BUILT, string.Format(_localization.GetTranslation("UI/INDUSTRY_BUILT_EVENT_TEXT"), _localization.GetTranslation("UI/" + @event.Type), translation));
	}

	private Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> ChangeEventToText(IndustryDeveloped @event)
	{
		return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(@event.Player, ActionLogRoundViewModel.TextEventType.INDUSTRY_DEVELOPED, string.Format(_localization.GetTranslation("UI/INDUSTRY_DEVELOPED_EVENT_TEXT"), _localization.GetTranslation("UI/" + @event.Type)));
	}

	private Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> ChangeEventToText(IndustryFlipped @event)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		BaseIndustry baseIndustry = currentGame.Game.Board.GetIndustryByUniqueId(@event.IndustryUniqueId);
		if (baseIndustry == null)
		{
			baseIndustry = currentGame.Game.Board.GetRemovedIndustryByUniqueId(@event.IndustryUniqueId);
		}
		if (baseIndustry != null)
		{
			if (baseIndustry.BuildingType == BuildingType.Brewery || baseIndustry.BuildingType == BuildingType.IronWorks || baseIndustry.BuildingType == BuildingType.CoalMine)
			{
				return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(baseIndustry.Color, ActionLogRoundViewModel.TextEventType.INDUSTRY_FLIPPED, "");
			}
			return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(baseIndustry.Color, ActionLogRoundViewModel.TextEventType.INDUSTRY_FLIPPED, string.Format(_localization.GetTranslation("UI/INDUSTRY_FLIPPED_EVENT_TEXT"), _localization.GetTranslation("UI/" + baseIndustry.BuildingType), _localization.GetTranslation("Regions/" + @event.RegionName)));
		}
		return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(PlayerColor.Yellow, ActionLogRoundViewModel.TextEventType.ERROR, "Error");
	}

	private Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> ChangeEventToText(MoneyLoaned @event)
	{
		return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(@event.Player, ActionLogRoundViewModel.TextEventType.MONEY_LOANED, string.Format(_localization.GetTranslation("UI/MONEY_LOANED_EVENT_TEXT")));
	}

	private Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> ChangeEventToText(WildCardsTaken @event)
	{
		return new Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>(@event.Player, ActionLogRoundViewModel.TextEventType.WILD_CARDS_TAKEN, string.Format(_localization.GetTranslation("UI/SCOUT_ACTION_EVENT_TEXT")));
	}
}
