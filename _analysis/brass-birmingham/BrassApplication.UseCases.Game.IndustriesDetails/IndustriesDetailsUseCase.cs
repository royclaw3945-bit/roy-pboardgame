using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Players;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Game.Actions;

namespace BrassApplication.UseCases.Game.IndustriesDetails;

public class IndustriesDetailsUseCase : IUseCase, IIndustriesDetailsInput
{
	private readonly IIndustriesDetailsOutput _presenter;

	private readonly IGameHistory _gameHistory;

	private ISelectIndustryOfTypeDelegate _selectIndustryOfActionDelegate;

	public IndustriesDetailsUseCase(IIndustriesDetailsOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void LoadIndustries()
	{
		_presenter.UpdateIndustriesDetails(_gameHistory.CurrentGame);
	}

	public void LoadIndustries(PlayerColor color)
	{
		_presenter.UpdateIndustriesDetails(_gameHistory.CurrentGame, color);
	}

	public void CloseIndustries()
	{
		_presenter.HideIndustriesDetails();
	}

	public void SetIndustryActionDelegate(ISelectIndustryOfTypeDelegate selectIndustryOfActionDelegate)
	{
		_selectIndustryOfActionDelegate = selectIndustryOfActionDelegate;
	}

	public void OnIndustryClicked(BuildingType buildingType)
	{
		_selectIndustryOfActionDelegate?.SelectIndustryOfType(buildingType);
	}

	public void SetPlayersSummaries()
	{
		_presenter.SetPlayerSummary(_gameHistory.CurrentGame);
	}

	public void ShowCantDevelopTooltip(float x, float y)
	{
		_presenter.ShowCantDevelopTooltip(x, y);
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}
}
