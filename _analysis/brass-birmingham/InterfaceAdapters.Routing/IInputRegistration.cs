using InterfaceAdapters.Common;

namespace InterfaceAdapters.Routing;

public interface IInputRegistration
{
	void RegisterKey(KeyEnum keyEnum, IBaseView baseView);

	void UnregisterKey(KeyEnum keyEnum, IBaseView baseView);

	void UnregisterAllKeys(IBaseView baseView);
}
