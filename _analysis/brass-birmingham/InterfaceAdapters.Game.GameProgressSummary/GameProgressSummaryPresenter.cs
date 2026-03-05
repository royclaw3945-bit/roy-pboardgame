using System.Numerics;
using BoardGameRules.Entities.GamePhases;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.GameProgressSummary;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.GameLogView;
using InterfaceAdapters.Game.GameProgressDetails;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.GameProgressSummary;

public class GameProgressSummaryPresenter : IPresenter, IGameProgressSummaryOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public GameProgressSummaryPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void UpdateGameProgressSummaryView(BrassApplicationGame game)
	{
		GameProgressSummaryViewModel gameProgressDetailsViewModel = new GameProgressSummaryViewModel
		{
			CurrentEra = game.Game.GameProgressInformation.CurrentGameEra,
			CurrentEraTranslation = _localization.GetTranslation((game.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra) ? "UI/CANAL_ERA" : "UI/RAIL_ERA"),
			RoundCount = game.Game.GameProgressInformation.CurrentRoundNumber,
			TurnCount = game.Game.GameProgressInformation.CurrentTurnNumber,
			MaxRound = game.Game.GameFlow.GetNumberOfRounds(game.Game.GameStartConfiguration.NumberOfPlayers),
			DeckSize = game.Game.Board.CardDeck.Capacity,
			DeckCount = game.Game.Board.CardDeck.Count,
			WildIndustryCardCount = game.Game.Board.WildIndustryCardDeck.Count,
			WildLocationCardCount = game.Game.Board.WildLocationCardDeck.Count
		};
		_router.GetView<IGameProgressSummaryView>().UpdateGameProgressSummaryView(gameProgressDetailsViewModel);
	}

	public void ShowGameProgressDetails()
	{
		_router.ShowView<IGameProgressDetailsView>();
	}

	public void ShowGameLog()
	{
		_router.ShowView<IGameLogView>();
	}

	public void ShowTooltip(string element, float x, float y)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		Vector3 zero = Vector3.Zero;
		string text = "ERROR";
		switch (element)
		{
		case "Deck":
			text = _localization.GetTranslation("Tooltips/HUD/GAMEPROGRESSSUMMARY/DECK");
			break;
		case "WildIndustry":
			text = _localization.GetTranslation("Tooltips/HUD/GAMEPROGRESSSUMMARY/WILDINDUSTRY");
			break;
		case "WildLocation":
			text = _localization.GetTranslation("Tooltips/HUD/GAMEPROGRESSSUMMARY/WILDLOCATION");
			break;
		case "EarningTrack":
			text = _localization.GetTranslation("Tooltips/HUD/GAMEPROGRESSSUMMARY/EARNINGTRACK");
			break;
		case "GameLog":
			text = _localization.GetTranslation("Tooltips/HUD/GAMEPROGRESSSUMMARY/GAMELOG");
			break;
		}
		zero = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(0.5f, 1f);
		_router.GetView<ITooltipView>().ShowTooltip(text, zero, pivot);
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}
}
