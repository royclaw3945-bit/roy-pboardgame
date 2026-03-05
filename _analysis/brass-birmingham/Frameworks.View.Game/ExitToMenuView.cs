using System;
using Frameworks.View.Menu;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ExitToMenu;
using InterfaceAdapters.Menu.Settings;
using UnityEngine;

namespace Frameworks.View.Game;

public class ExitToMenuView : SettingsView, IExitToMenuView, ISettingsView, IBaseView
{
	private ExitToMenuController _controller;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is ExitToMenuController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
			base.Controller = _controller;
		}
	}

	public override void RefreshSettings(SettingsViewModel viewModel)
	{
		_musicVolumeSlider.value = viewModel.MusicVolume;
		_soundVolumeSlider.value = viewModel.SoundVolume;
	}

	public void OnBackToMenuClicked()
	{
		_controller.OnBackToMenuClicked();
	}

	public void OnResignClicked()
	{
	}

	public void HidePopup()
	{
		Popup.GetComponent<CanvasGroup>().alpha = 0f;
	}

	public void ShowPopup()
	{
		Popup.GetComponent<CanvasGroup>().alpha = 1f;
	}

	public string GetTranslation(string key)
	{
		return _controller.GetTranslation(key);
	}

	public (string metadata, string events) GetSave()
	{
		return _controller.GetSave();
	}

	protected override void PrepareReferences()
	{
		_showFullSettings = false;
		base.PrepareReferences();
	}
}
