using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Settings;

public interface ISettingsView : IBaseView
{
	void RefreshSettings(SettingsViewModel viewModel);
}
