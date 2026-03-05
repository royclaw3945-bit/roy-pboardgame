using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Actions.BuildAction;

namespace Frameworks.View.Game;

public class BuildActionView : BaseView, IBuildActionView, IBaseView
{
	private BuildActionController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is BuildActionController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}
}
