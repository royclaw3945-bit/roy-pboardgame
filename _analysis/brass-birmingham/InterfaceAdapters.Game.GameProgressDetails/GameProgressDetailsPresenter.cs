using System.Linq;
using System.Numerics;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.GameProgressDetails;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.GameProgressDetails;

public class GameProgressDetailsPresenter : IPresenter, IGameProgressDetailsOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public GameProgressDetailsPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void UpdateGameProgressDetailsView(BrassApplicationGame game)
	{
		GameProgressDetailsViewModel gameProgressDetailsViewModel = new GameProgressDetailsViewModel
		{
			DeckCount = game.Game.Board.CardDeck.Count,
			WildIndustryCardsCount = game.Game.Board.WildIndustryCardDeck.Count,
			WildLocationCardsCount = game.Game.Board.WildLocationCardDeck.Count,
			IncomePositions = game.Game.PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => p.IncomeMarkerPosition),
			IncomeLevels = game.Game.PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => p.IncomeLevel),
			PlayerAvatars = game.Metadata.PlayerMetadata.ToDictionary((PlayerMetadata p) => p.PlayerStartConfiguration.Color, (PlayerMetadata p) => p.PlayerAvatar),
			CurPlayerColor = game.Game.GameProgressInformation.CurrentPlayer.Color
		};
		_router.GetView<IGameProgressDetailsView>().UpdateGameProgressDetailsView(gameProgressDetailsViewModel);
	}

	public void HideGameProgressDetailsView()
	{
		_router.HideView<IGameProgressDetailsView>();
	}

	public void ShowTooltip(string element, float x, float y, string playerName, PlayerColor? playerColor)
	{
		if (_router.IsViewVisible<ITooltipView>())
		{
			_router.MoveOnTop<ITooltipView>();
		}
		else
		{
			_router.ShowView<ITooltipView>();
		}
		string text = "ERROR";
		Vector3 zero = Vector3.Zero;
		switch (element)
		{
		case "Token":
			text = playerName;
			break;
		case "Income":
			text = _localization.GetTranslation("Tooltips/EARNINGSTRACK/INCOME");
			break;
		case "IncomePoints":
			text = _localization.GetTranslation("Tooltips/EARNINGSTRACK/INCOMEPOINTS");
			break;
		}
		zero = new Vector3(x, y, 0f);
		Vector2 pivot = new Vector2(1f, 0.5f);
		_router.GetView<ITooltipView>().ShowTooltip(text, zero, pivot);
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}
}
