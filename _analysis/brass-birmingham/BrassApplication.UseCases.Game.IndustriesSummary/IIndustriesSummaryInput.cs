namespace BrassApplication.UseCases.Game.IndustriesSummary;

public interface IIndustriesSummaryInput
{
	void UpdateIndustries();

	void ShowIndustryDetails();

	void ShowTooltip(string element, float x, float y);

	void HideTooltip();
}
