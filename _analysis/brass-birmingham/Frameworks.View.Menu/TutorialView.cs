using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Tutorial;

namespace Frameworks.View.Menu;

public class TutorialView : BaseView, ITutorialView, IBaseView
{
	private TutorialController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is TutorialController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void StartTutorial()
	{
		_controller.StartTutorial();
	}
}
