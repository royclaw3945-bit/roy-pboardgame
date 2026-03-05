using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Players;

namespace BrassApplication.UseCases.Game.Replay;

public class ActionLogRoundViewModel
{
	public enum TextEventType
	{
		ERROR,
		PASS,
		LINK_PLACED,
		INDUSTRY_BUILT,
		INDUSTRY_DEVELOPED,
		INDUSTRY_FLIPPED,
		MONEY_LOANED,
		WILD_CARDS_TAKEN,
		FUSED_DEVELOP,
		FUSED_LINK,
		FUSED_INDUSTRY_FLIPPED
	}

	public string Title { get; set; }

	public string Era { get; set; }

	public string Turn { get; set; }

	public IDictionary<PlayerColor, List<Tuple<TextEventType, string>>> Actions { get; set; }

	public IDictionary<PlayerColor, PlayerAvatar> Avatars { get; set; }
}
