using System.Collections.Generic;
using BrassApplication.ApplicationSettings;
using I2.Loc;
using UnityEngine;

namespace Frameworks.Settings;

public class LanguageSettings : ILanguageSettings
{
	private const string SELECTED_LANGUAGE_PREFIX = "SelectedLanguage";

	public string GetSelectedLanguage()
	{
		if (!PlayerPrefs.HasKey("SelectedLanguage"))
		{
			return "English";
		}
		return PlayerPrefs.GetString("SelectedLanguage");
	}

	public IList<string> GetAvailableLanguages()
	{
		return LocalizationManager.GetAllLanguages();
	}

	public void SetSelectedLanguage(string language)
	{
		if (LocalizationManager.GetAllLanguages().Contains(language))
		{
			LocalizationManager.CurrentLanguage = language;
			PlayerPrefs.SetString("SelectedLanguage", language);
		}
	}
}
