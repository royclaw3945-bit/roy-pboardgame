using System;
using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Game.PlayersSummary;

public class PlayerViewModel
{
	public Action<string, float, float> ActionOnHover;

	public Action ActionOnHoverExit;

	public bool IsCurrent { get; set; }

	public int Money { get; set; }

	public int VP { get; set; }

	public int IncomeLevel { get; set; }

	public int IncomeMarkerPosition { get; set; }

	public int IncomeThreshold { get; set; }

	public int MoneySpent { get; set; }

	public string Name { get; set; }

	public PlayerColor Color { get; set; }

	public PlayerAvatar Avatar { get; set; }
}
