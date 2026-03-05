using BrassApplication.UseCases.Common;
using InterfaceAdapters.Common;

namespace Frameworks.Routing;

public struct PUCContainer
{
	public IController Controller { get; set; }

	public IUseCase UseCase { get; set; }

	public IPresenter Presenter { get; set; }
}
