using System;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Game.Board;

public class LinkViewModel
{
	public Action<string, float, float> OnHover = delegate
	{
	};

	public Action OnHoverExit = delegate
	{
	};

	public Link Link { get; set; }

	public bool IsRiverVisible { get; set; }

	public bool IsRailVisible { get; set; }

	public bool IsTokenVisible { get; set; }

	public bool IsInteractive { get; set; }

	public bool IsOutlineVisible { get; set; }

	public GameEra CurrentEra { get; set; }

	public PlayerColor TokenColor { get; set; }
}
