using System;
using BoardGameRules.Entities.Board.BuildingSlots;

namespace InterfaceAdapters.Game.Board;

public class BuildingSlotViewModel
{
	public BuildingSlot BuildingSlot { get; set; }

	public string ImageName { get; set; }

	public string BackgroundImageName { get; set; }

	public int ResourceCount { get; set; }

	public bool Build { get; set; }

	public bool Highlighted { get; set; }

	public bool IsInteractive { get; set; }

	public bool HighlightResource { get; set; }

	public Action<BuildingSlot> ActionOnClick { get; set; } = delegate
	{
	};

	public Action ActionOnRegionClick { get; set; } = delegate
	{
	};

	public Action<string, float, float> ActionOnHover { get; set; } = delegate
	{
	};

	public Action ActionOnHoverExit { get; set; } = delegate
	{
	};
}
