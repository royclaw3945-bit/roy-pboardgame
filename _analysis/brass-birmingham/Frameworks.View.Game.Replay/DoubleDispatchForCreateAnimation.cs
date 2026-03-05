using InterfaceAdapters.Common;
using InterfaceAdapters.Utils;

namespace Frameworks.View.Game.Replay;

public static class DoubleDispatchForCreateAnimation
{
	public static IAnimation DoubleDispatchCreateAnimation(this ReplayView replayView, object @event)
	{
		return DoubleDispatch.Dispatch<IAnimation>(replayView, "CreateAnimation", typeof(IAnimation), @event);
	}
}
