using System.Numerics;

namespace InterfaceAdapters.Common;

public interface ITooltipView : IBaseView
{
	void ShowTooltip(string text, Vector3 position, Vector2 pivot);

	void HideTooltip();
}
