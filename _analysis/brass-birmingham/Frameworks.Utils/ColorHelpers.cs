using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Players;
using UnityEngine;

namespace Frameworks.Utils;

public class ColorHelpers
{
	public static Color RegionColorToUnityColor(RegionColor regionColor)
	{
		return new Color(0.7647059f, 0.7647059f, 0.7647059f, 0.8509804f);
	}

	public static Color PlayerColorToUnityColor(PlayerColor playerColor)
	{
		return playerColor switch
		{
			PlayerColor.Blue => Color.blue, 
			PlayerColor.Red => Color.red, 
			PlayerColor.Violet => Color.magenta, 
			PlayerColor.Yellow => Color.yellow, 
			_ => Color.black, 
		};
	}
}
