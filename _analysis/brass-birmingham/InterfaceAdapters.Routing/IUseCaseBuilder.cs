using System;
using BrassApplication.UseCases.Common;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Routing;

public interface IUseCaseBuilder
{
	void BuildUseCases();

	void RestoreUseCases();

	void RestoreUseCaseOfType(Type useCaseViewInterfaceType);

	IController GetController(Type typeOfView);

	TUseCase GetUseCase<TUseCase>() where TUseCase : IUseCase;
}
