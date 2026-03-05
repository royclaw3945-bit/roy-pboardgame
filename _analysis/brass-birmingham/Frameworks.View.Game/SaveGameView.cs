using System;
using Frameworks.View.Common;
using I2.Loc;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.SaveGame;

namespace Frameworks.View.Game;

public class SaveGameView : InfoPopupView, ISaveGameView, IBaseView
{
	private SaveGameController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is SaveGameController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void ShowSaveInProgress()
	{
		_confirmButton.gameObject.SetActive(value: false);
		Setup(LocalizationManager.GetTranslation("UI/SAVE_NETWORK_GAME_IN_PROGRESS"), LocalizationManager.GetTranslation("UI/OK"), delegate
		{
		});
	}

	public void ShowSaveFailed(Action onConfirm)
	{
		_confirmButton.gameObject.SetActive(value: true);
		Setup(LocalizationManager.GetTranslation("UI/SAVE_NETWORK_GAME_FAILED"), LocalizationManager.GetTranslation("UI/OK"), onConfirm);
	}
}
