using System.Collections.Generic;
using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Game.Actions.LoanAction;

public class LoanActionViewModel
{
	public PlayerColor PlayerColor { get; set; }

	public PlayerAvatar PlayerAvatar { get; set; }

	public int CurrentIncomePosition { get; set; }

	public int CurrentIncomeLevel { get; set; }

	public int TargetIncomePosition { get; set; }

	public int TargetIncomeLevel { get; set; }

	public Dictionary<PlayerColor, PlayerAvatar> PlayerAvatars { get; internal set; }

	public Dictionary<PlayerColor, int> IncomeLevels { get; internal set; }

	public Dictionary<PlayerColor, int> IncomePositions { get; internal set; }
}
