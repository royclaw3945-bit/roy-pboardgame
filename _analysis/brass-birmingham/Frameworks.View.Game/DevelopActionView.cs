using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Actions.DevelopAction;

namespace Frameworks.View.Game;

public class DevelopActionView : BaseView, IDevelopActionView, IBaseView
{
	private DevelopActionController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is DevelopActionController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void OnOkButtonClicked()
	{
		_controller.OnOkButtonClicked();
	}
}
