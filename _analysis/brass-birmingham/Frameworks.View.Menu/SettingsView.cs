using System;
using Frameworks.Audio;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Settings;
using InterfaceAdapters.Routing;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu;

public class SettingsView : BaseView, ISettingsView, IBaseView
{
	private SettingsController _controller;

	protected AudioMixerController _audioMixerController;

	private Image _nightButtonBackground;

	private Image _nightButtonIcon;

	private Image _dayButtonBackground;

	private Image _dayButtonIcon;

	protected Slider _musicVolumeSlider;

	protected Slider _soundVolumeSlider;

	private Toggle LanguageEnglishToggle;

	private Toggle LanguageGermanToggle;

	private Toggle LanguagePolishToggle;

	private Toggle LanguageFrenchToggle;

	private Toggle LanguageSpanishToggle;

	private Toggle activeToggle;

	[SerializeField]
	private Sprite buttonBackgroundNormal;

	[SerializeField]
	private Sprite buttonBackgroundPressed;

	protected bool _showFullSettings = true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is SettingsController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public virtual void RefreshSettings(SettingsViewModel viewModel)
	{
		_musicVolumeSlider.value = viewModel.MusicVolume;
		_soundVolumeSlider.value = viewModel.SoundVolume;
		if (_showFullSettings)
		{
			OnChangeMapClicked(viewModel.IsNightMap);
			switch (viewModel.LanguageSelected)
			{
			case "Polish":
				activeToggle = LanguagePolishToggle;
				break;
			case "English":
				activeToggle = LanguageEnglishToggle;
				break;
			case "German":
				activeToggle = LanguageGermanToggle;
				break;
			case "French":
				activeToggle = LanguageFrenchToggle;
				break;
			case "Spanish":
				activeToggle = LanguageSpanishToggle;
				break;
			}
			activeToggle.SetIsOnWithoutNotify(value: true);
		}
	}

	public void OnBackClicked()
	{
		_controller.OnBackClicked();
	}

	public void OnMusicSliderValueChanged()
	{
		_controller.OnMusicChanged(_musicVolumeSlider.value);
		_audioMixerController.RestoreSavedVolumeSettings();
	}

	public void OnSoundSliderValueChanged()
	{
		_controller.OnSoundChanged(_soundVolumeSlider.value);
		_audioMixerController.RestoreSavedVolumeSettings();
	}

	public void OnChangeMapClicked(bool isNightMap)
	{
		_controller.OnMapChanged(isNightMap);
		if (isNightMap)
		{
			_dayButtonBackground.sprite = buttonBackgroundNormal;
			_nightButtonBackground.sprite = buttonBackgroundPressed;
			_nightButtonIcon.color = new Color(1f, 1f, 1f, 0.6f);
			_dayButtonIcon.color = new Color(1f, 1f, 1f, 1f);
			_nightButtonIcon.rectTransform.anchoredPosition = new Vector3(0f, -3f, 0f);
			_dayButtonIcon.rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
		}
		else
		{
			_dayButtonBackground.sprite = buttonBackgroundPressed;
			_nightButtonBackground.sprite = buttonBackgroundNormal;
			_dayButtonIcon.color = new Color(1f, 1f, 1f, 0.6f);
			_nightButtonIcon.color = new Color(1f, 1f, 1f, 1f);
			_dayButtonIcon.rectTransform.anchoredPosition = new Vector3(0f, -3f, 0f);
			_nightButtonIcon.rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
		}
	}

	public void OnLanguageClicked(string language)
	{
		_controller.OnLanguageChanged(language);
		switch (language)
		{
		case "Polish":
			if (activeToggle != LanguagePolishToggle)
			{
				activeToggle.SetIsOnWithoutNotify(value: false);
				activeToggle = LanguagePolishToggle;
			}
			else
			{
				activeToggle.SetIsOnWithoutNotify(value: true);
			}
			break;
		case "English":
			if (activeToggle != LanguageEnglishToggle)
			{
				activeToggle.SetIsOnWithoutNotify(value: false);
				activeToggle = LanguageEnglishToggle;
			}
			else
			{
				activeToggle.SetIsOnWithoutNotify(value: true);
			}
			break;
		case "German":
			if (activeToggle != LanguageGermanToggle)
			{
				activeToggle.SetIsOnWithoutNotify(value: false);
				activeToggle = LanguageGermanToggle;
			}
			else
			{
				activeToggle.SetIsOnWithoutNotify(value: true);
			}
			break;
		case "French":
			if (activeToggle != LanguageFrenchToggle)
			{
				activeToggle.SetIsOnWithoutNotify(value: false);
				activeToggle = LanguageFrenchToggle;
			}
			else
			{
				activeToggle.SetIsOnWithoutNotify(value: true);
			}
			break;
		case "Spanish":
			if (activeToggle != LanguageSpanishToggle)
			{
				activeToggle.SetIsOnWithoutNotify(value: false);
				activeToggle = LanguageSpanishToggle;
			}
			else
			{
				activeToggle.SetIsOnWithoutNotify(value: true);
			}
			break;
		}
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackClicked();
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_soundVolumeSlider = base.transform.Find("SafeArea/Popup/SoundSlider").GetComponent<Slider>();
		_musicVolumeSlider = base.transform.Find("SafeArea/Popup/MusicSlider").GetComponent<Slider>();
		if (_showFullSettings)
		{
			LanguageEnglishToggle = base.transform.Find("SafeArea/Popup/ButtonLanguageEnglish").GetComponent<Toggle>();
			LanguageGermanToggle = base.transform.Find("SafeArea/Popup/ButtonLanguageGerman").GetComponent<Toggle>();
			LanguagePolishToggle = base.transform.Find("SafeArea/Popup/ButtonLanguagePolish").GetComponent<Toggle>();
			LanguageFrenchToggle = base.transform.Find("SafeArea/Popup/ButtonLanguageFrench").GetComponent<Toggle>();
			LanguageSpanishToggle = base.transform.Find("SafeArea/Popup/ButtonLanguageSpanish").GetComponent<Toggle>();
			_dayButtonBackground = base.transform.Find("SafeArea/Popup/DayButton/Background").GetComponent<Image>();
			_nightButtonBackground = base.transform.Find("SafeArea/Popup/NightButton/Background").GetComponent<Image>();
			_dayButtonIcon = base.transform.Find("SafeArea/Popup/DayButton/DayIcon").GetComponent<Image>();
			_nightButtonIcon = base.transform.Find("SafeArea/Popup/NightButton/NightIcon").GetComponent<Image>();
		}
		_audioMixerController = GameObject.Find("Utils/AudioMixerController").GetComponent<AudioMixerController>();
	}
}
