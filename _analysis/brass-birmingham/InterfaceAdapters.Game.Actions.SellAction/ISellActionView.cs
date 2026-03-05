using System.Collections.Generic;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.SellAction;

public interface ISellActionView : IBaseView
{
	void SetSellActionView(List<string> industries);
}
