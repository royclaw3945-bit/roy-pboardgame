using InterfaceAdapters.Routing;
using UnityEngine;

namespace Frameworks.Routing.Unity;

public class UnityRouter : MonoBehaviour
{
	public IRouter Router;

	private void Update()
	{
		Router?.ProcessInput();
	}
}
