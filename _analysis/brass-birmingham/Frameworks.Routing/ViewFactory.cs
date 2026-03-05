using System;
using System.Linq;
using System.Reflection;
using InterfaceAdapters.Common;

namespace Frameworks.Routing;

public class ViewFactory : IViewFactory
{
	public T CreateView<T>() where T : class, IBaseView
	{
		Type type = typeof(T);
		return (T)Activator.CreateInstance((from p in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly s) => s.GetTypes())
			where type.IsAssignableFrom(p)
			select p).First(), new object[0]);
	}

	public void DestroyView(IBaseView view)
	{
	}
}
