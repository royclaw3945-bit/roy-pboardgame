using InterfaceAdapters.Routing;

namespace Frameworks.Routing;

public interface IInputSource
{
	bool GetKeyDown(KeyEnum key);

	bool GetKey(KeyEnum key);

	bool GetKeyUp(KeyEnum key);
}
