using System;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Replay;

public static class DoubleDispatchForChangeEventToText
{
	public static Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> DoubleDispatchChangeEventToText(this ChangeEventToTextHelper changeEventToTextHelper, object @event)
	{
		return DoubleDispatch.Dispatch<Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>>(changeEventToTextHelper, "ChangeEventToText", typeof(Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string>), @event);
	}
}
