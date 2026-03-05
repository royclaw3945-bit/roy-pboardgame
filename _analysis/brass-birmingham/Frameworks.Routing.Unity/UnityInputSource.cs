using InterfaceAdapters.Routing;
using UnityEngine;

namespace Frameworks.Routing.Unity;

public class UnityInputSource : MonoBehaviour, IInputSource
{
	public bool GetKeyDown(KeyEnum key)
	{
		return Input.GetKeyDown((KeyCode)key);
	}

	public bool GetKey(KeyEnum key)
	{
		return Input.GetKey((KeyCode)key);
	}

	public bool GetKeyUp(KeyEnum key)
	{
		return Input.GetKeyUp((KeyCode)key);
	}
}
