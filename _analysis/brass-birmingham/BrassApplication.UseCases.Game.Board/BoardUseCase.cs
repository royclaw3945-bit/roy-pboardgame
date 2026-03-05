using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.ApplicationSettings;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Game.Actions;

namespace BrassApplication.UseCases.Game.Board;

public class BoardUseCase : IUseCase, IBoardInput, IActionInProgressCancelDelegate
{
	private readonly IBoardOutput _presenter;

	protected readonly IGameHistory _gameHistory;

	private ILinkSelectedDelegate _linkSelectedDelegate;

	private IGameSettings _gameSettings;

	private readonly ILog Logger = LogFactory.BuildLogger(typeof(BoardUseCase));

	private bool _allowBuildingFromBoard = true;

	public BoardUseCase(IBoardOutput presenter, IGameHistory gameHistory, IGameSettings gameSettings)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
		_gameSettings = gameSettings;
	}

	public void UpdateBoard()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Logger.Debug("Current game revision in board = " + currentGame.Game.Revision);
		_presenter.UpdateBoardView(currentGame.Game.Board, currentGame.Game.GameProgressInformation.CurrentGameEra, RegionSelected);
	}

	public void RegionSelected(RegionName regionName)
	{
		if (_allowBuildingFromBoard)
		{
			StartBuildActionFromBoard(regionName);
		}
		else
		{
			ShowRegionDetails(regionName);
		}
	}

	private void StartBuildActionFromBoard(RegionName regionName)
	{
		if (_gameHistory.CurrentGame.Game.TurnFlow.GetRemainingActionsCount() > 0)
		{
			_presenter.StartBuildActionFromBoard(regionName);
		}
	}

	protected void ShowRegionDetails(RegionName regionName)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Dictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>> cantBuildReasons = new Dictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>>();
		_ = currentGame.Game.Board.GetRegionByName(regionName).BuildingSlots;
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		_ = currentGame.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		_presenter.ShowRegionDetailsView(regionName, currentGame, cantBuildReasons);
	}

	public int GetPointsForLink(string linkId)
	{
		int num = 0;
		BoardGameRules.Entities.Board.Board board = _gameHistory.CurrentGame.Game.Board;
		foreach (Region region in board.Regions)
		{
			if (!region.Links.Contains(board.GetLinkById(linkId)))
			{
				continue;
			}
			if (region is BorderRegion)
			{
				num += 2;
				continue;
			}
			foreach (BuildingSlot buildingSlot in region.BuildingSlots)
			{
				if (buildingSlot.BuildedIndustry != null && buildingSlot.BuildedIndustry.IsFlipped)
				{
					num += buildingSlot.BuildedIndustry.VPPerLink;
				}
			}
		}
		return num;
	}

	public void CancelAction()
	{
		_presenter.HideRegionDetailsView();
	}

	public void SetLinkSelectedDelegate(ILinkSelectedDelegate linkSelectedDelegate)
	{
		_linkSelectedDelegate = linkSelectedDelegate;
	}

	public void ShowTooltipIndustry(string slotUniqueId, float x, float y, int width, int height)
	{
		BuildingSlot buildingSlotByUniqueId = _gameHistory.CurrentGame.Game.Board.GetBuildingSlotByUniqueId(slotUniqueId);
		BaseIndustry buildedIndustry = buildingSlotByUniqueId.BuildedIndustry;
		if (buildedIndustry == null)
		{
			Region regionContainingGivenSlot = _gameHistory.CurrentGame.Game.Board.GetRegionContainingGivenSlot(buildingSlotByUniqueId);
			_presenter.ShowTooltipBuildingSlot(buildingSlotByUniqueId.BuildingTypes, GetRegionFriendlyNameLocalization(regionContainingGivenSlot.Name), regionContainingGivenSlot.Color, x, y, width, height);
		}
		else
		{
			_presenter.ShowTooltipIndustry(buildedIndustry.BuildingType, buildedIndustry.IsFlipped, buildedIndustry.Level, buildedIndustry.VPForBuilding, buildedIndustry.VPPerLink, buildedIndustry.IncomeIncrease, _gameHistory.CurrentGame.Metadata.GetPlayerMetadataByColor(buildedIndustry.Color).NickName, buildedIndustry.Color, x, y, width, height, buildedIndustry.GetResource(), buildedIndustry.GetMaxResource(), buildedIndustry.GetBeerNeeded());
		}
	}

	public void ShowTooltipLink(string linkId, float x, float y, int width, int height)
	{
		Link linkById = _gameHistory.CurrentGame.Game.Board.GetLinkById(linkId);
		string owner = (linkById.HasOwner ? _gameHistory.CurrentGame.Metadata.GetPlayerMetadataByColor(linkById.Owner.Color).NickName : null);
		PlayerColor? playerColor = linkById.Owner?.Color;
		Dictionary<string, RegionColor> dictionary = new Dictionary<string, RegionColor>();
		dictionary.Add(GetRegionFriendlyNameLocalization(linkById.Region1.Name), linkById.Region1.Color);
		dictionary.Add(GetRegionFriendlyNameLocalization(linkById.Region2.Name), linkById.Region2.Color);
		if (linkById.Region3 != null)
		{
			dictionary.Add(GetRegionFriendlyNameLocalization(linkById.Region3.Name), linkById.Region3.Color);
		}
		_presenter.ShowTooltipLink(owner, playerColor, GetPointsForLink(linkId), dictionary, linkById.GetType().Name, x, y, width, height);
	}

	public void ShowTooltipBorderRegion(RegionName regionName, float x, float y, int width, int height)
	{
		BorderRegion borderRegion = _gameHistory.CurrentGame.Game.Board.GetBorderRegions((BorderRegion r) => r.Name == regionName)[0];
		IEnumerable<BuildingType> enumerable = new List<BuildingType>();
		foreach (MerchantSlot merchantSlot in borderRegion.MerchantSlots)
		{
			MerchantTile merchant = merchantSlot.GetMerchant();
			if (merchant != null)
			{
				enumerable = enumerable.Concat(merchant.BuildingTypes);
			}
		}
		enumerable.Distinct();
		_presenter.ShowTooltipBorderRegion(GetRegionFriendlyNameLocalization(regionName), enumerable.ToList(), borderRegion.MerchantBeerBonusType, x, y, width, height);
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}

	private string GetRegionFriendlyNameLocalization(RegionName regionName)
	{
		return _presenter.GetRegionFriendlyNameLocalization(regionName);
	}

	public void SetAllowBuidlingFromBoard(bool allowBuildingFromBoard)
	{
		_allowBuildingFromBoard = allowBuildingFromBoard;
	}

	public bool GetIsNightMap()
	{
		return _gameSettings.GetIsNightMap();
	}
}
