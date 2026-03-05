using System;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.ScoutAction;

public interface IScoutActionView : IBaseView
{
	void AnimateCardToSlot(int slotIndex, Tuple<object, int> info, Action onCallback);
}
