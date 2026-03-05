using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.Market;

public class MarketUseCase : IUseCase, IMarketInput
{
	private readonly IMarketOutput _presenter;

	private readonly IGameHistory _gameHistory;

	public MarketUseCase(IMarketOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}

	public void ShowTooltip(string element, float x, float y)
	{
		int num = -1;
		num = ((!(element == "Coal")) ? _gameHistory.CurrentGame.Game.Board.IronMarket.IronPrice() : _gameHistory.CurrentGame.Game.Board.CoalMarket.CoalPrice(1));
		_presenter.ShowTooltip(element, x, y, num);
	}

	public void UpdateMarket()
	{
		_presenter.UpdateMarket(_gameHistory.CurrentGame);
	}
}
