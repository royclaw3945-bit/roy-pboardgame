using System.Collections.Generic;

namespace BrassApplication.ApplicationSettings;

public interface ILanguageSettings
{
	string GetSelectedLanguage();

	void SetSelectedLanguage(string language);

	IList<string> GetAvailableLanguages();
}
