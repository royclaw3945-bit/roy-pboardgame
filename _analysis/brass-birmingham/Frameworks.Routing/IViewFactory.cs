using InterfaceAdapters.Common;

namespace Frameworks.Routing;

public interface IViewFactory
{
	T CreateView<T>() where T : class, IBaseView;

	void DestroyView(IBaseView view);
}
