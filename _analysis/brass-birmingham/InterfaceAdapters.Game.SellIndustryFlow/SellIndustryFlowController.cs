using BrassApplication.UseCases.Game.SellIndustryFlow;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.SellIndustryFlow;

public class SellIndustryFlowController : IController
{
	private readonly ISellIndustryFlowInput _useCase;

	public SellIndustryFlowController(ISellIndustryFlowInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}
}
