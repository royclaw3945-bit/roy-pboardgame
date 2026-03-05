using I2.Loc;
using InterfaceAdapters.Utils;

namespace Frameworks.Utils.Localization;

public class I2Localization : ILocalization
{
	public string GetTranslation(string term)
	{
		return LocalizationManager.GetTranslation(term);
	}
}
