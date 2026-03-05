using BrassApplication.UseCases.Game.IndustriesSummary;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.IndustriesSummary;

public class IndustriesSummaryController : IController
{
	private readonly IIndustriesSummaryInput _useCase;

	public IndustriesSummaryController(IIndustriesSummaryInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
		_useCase.UpdateIndustries();
	}

	public void OnViewDestroyed()
	{
	}

	public void OnIndustryClicked()
	{
		_useCase.ShowIndustryDetails();
	}

	internal void OnMouseEnter(string element, float x, float y)
	{
		_useCase.ShowTooltip(element, x, y);
	}

	internal void OnMouseExit()
	{
		_useCase.HideTooltip();
	}
}
