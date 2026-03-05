using InterfaceAdapters.Common;

namespace InterfaceAdapters.Routing;

public interface IRouter : IViewManager, IInputRegistration
{
	IUseCaseBuilder UseCaseBuilder { get; set; }

	IAnimation ShowView<T>() where T : class, IBaseView;

	IAnimation HideView<T>() where T : class, IBaseView;

	void MoveOnTop<T>() where T : class, IBaseView;

	bool IsViewVisible<T>() where T : class, IBaseView;

	bool IsViewCached<T>() where T : class, IBaseView;

	T GetView<T>() where T : class, IBaseView;

	void ProcessInput();
}
