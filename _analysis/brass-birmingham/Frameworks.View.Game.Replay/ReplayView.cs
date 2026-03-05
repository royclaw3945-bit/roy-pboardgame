using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Events;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Actions.LoanAction;
using InterfaceAdapters.Game.Actions.ScoutAction;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.EndOfEra;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.IndustriesDetails;
using InterfaceAdapters.Game.Market;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Frameworks.View.Game.Replay;

public class ReplayView : BaseView, IReplayView, IBaseView
{
	private ReplayController _controller;

	private new static readonly ILog Logger = LogFactory.BuildLogger(typeof(ReplayView));

	private const string AnimationFactoryTweenId = "AnimationFactory";

	public GameObject ReplayGameObjectPrefab;

	private readonly IRouter _router = Global.Instance.Router;

	private TextMeshProUGUI _nickname;

	private TextMeshProUGUI _action;

	private TextMeshProUGUI _fastForward;

	private GameObject _skipButton;

	private GameObject _fastForwardButton;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is ReplayController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public override IAnimation CreateShowViewAnimation()
	{
		_skipButton.SetActive(value: true);
		return base.CreateShowViewAnimation();
	}

	private new void Awake()
	{
		PrepareReferences();
	}

	public IAnimationSequence CreateAnimationSequence()
	{
		return new DOTweenAnimationSequence();
	}

	public void PauseAllAnimations()
	{
		List<Tween> list = DOTween.TweensById("AnimationFactory", playingOnly: true);
		if (list == null)
		{
			return;
		}
		foreach (Tween item in list)
		{
			if (item.IsActive())
			{
				item.timeScale = 0f;
			}
		}
	}

	public void ResumeAllAnimations()
	{
		List<Tween> list = DOTween.TweensById("AnimationFactory", playingOnly: true);
		if (list == null)
		{
			return;
		}
		foreach (Tween item in list)
		{
			if (item.IsActive())
			{
				item.timeScale = UIProperties.CurrentAnimationSpeed;
			}
		}
	}

	public void KillAllAnimations()
	{
		List<Tween> list = DOTween.TweensById("AnimationFactory", playingOnly: true);
		if (list == null)
		{
			return;
		}
		foreach (Tween item in list)
		{
			if (item.IsActive())
			{
				item.Kill();
			}
		}
	}

	public void SetCurrentPlayerName(string playerColor)
	{
		_nickname.text = playerColor;
	}

	public void SetCurrentPlayerAction(string action)
	{
		_action.text = action;
	}

	public void SkipToMyTurnClicked()
	{
		_controller.SkipToMyTurnClicked();
	}

	public void FastForwardReplay()
	{
		UIProperties.CurrentAnimationSpeed = UIProperties.FastForwardAnimationSpeed;
		_fastForward.gameObject.SetActive(value: true);
		ResumeAllAnimations();
	}

	public void RestoreDefaultAnimationSpeed()
	{
		UIProperties.CurrentAnimationSpeed = UIProperties.DefaultAnimationSpeed;
		_fastForward.gameObject.SetActive(value: false);
		ResumeAllAnimations();
	}

	public void HideSkipButton()
	{
		_skipButton.SetActive(value: false);
	}

	public void SetSkipButtonInteractable(bool interactable)
	{
		_skipButton.GetComponent<Button>().interactable = interactable;
	}

	public IAnimation CreateAnimation(IEvent @event)
	{
		try
		{
			IAnimation animation = this.DoubleDispatchCreateAnimation(@event);
			((DOTweenAnimationSequence)animation).Sequence.SetId("AnimationFactory");
			((DOTweenAnimationSequence)animation).Sequence.timeScale = UIProperties.CurrentAnimationSpeed;
			return animation;
		}
		catch (ArgumentException)
		{
			Logger.Debug(string.Concat("CreateAnimation not supported for event ", @event, ", returning empty animation"));
		}
		return CreateAnimationSequence();
	}

	public bool IsAnyTweenInProgress()
	{
		return DOTween.TotalPlayingTweens() != 0;
	}

	public void SetFastForwardActive(bool isActive)
	{
		_fastForwardButton.SetActive(isActive);
	}

	private IAnimation CreateAnimationPlaceholder(IEvent @event)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(ReplayGameObjectPrefab, base.transform);
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			gameObject.GetComponent<TextMeshProUGUI>().text = $"Animating event: {@event}";
			Logger.Debug($"Animating event: {@event}");
		});
		animationSequence.AppendInterval(0.5f);
		animationSequence.AppendCallback(delegate
		{
			UnityEngine.Object.Destroy(gameObject);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(TurnStarted @event)
	{
		return _router.GetView<IPlayersSummaryView>().CreateShowCurrentPlayerAnimation(@event.PlayerColor);
	}

	private IAnimation CreateAnimation(RoundStarted @event)
	{
		return _router.GetView<IPlayersSummaryView>().CreateShuffleNewPlayersOrderAnimation(@event);
	}

	private IAnimation CreateAnimation(RoundEnded @event)
	{
		return _router.GetView<IPlayersSummaryView>().CreateEndOfRoundAnimation(@event);
	}

	private IAnimation CreateAnimation(CardDiscarded @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		if (_controller.GetReplayWatchingPlayerColor() != @event.Player)
		{
			return (IAnimationSequence)_router.GetView<IHandOfCardsView>().CreateOtherPlayerCardDiscardedAnimation(@event.CardToDiscard);
		}
		return (IAnimationSequence)_router.GetView<IHandOfCardsView>().CreateCardDiscardedAnimation(@event.CardToDiscard);
	}

	private IAnimation CreateAnimation(ActionPassed @event)
	{
		return CreateAnimationSequence();
	}

	private IAnimation CreateAnimation(MoneySpent @event)
	{
		string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).Money - @event.Amount).ToString();
		string newMoneySpent = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetMoneySpentByPlayerByColor(@event.Player) + @event.Amount).ToString();
		return _router.GetView<IPlayersSummaryView>().CreateChangeMoneySpentAnimation(newAmount, @event.Player, newMoneySpent);
	}

	private IAnimation CreateAnimation(MoneyLoaned @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		BrassApplicationGame currentGame = Global.Instance.BrassGameHistory.CurrentGame;
		Player playerByColor = currentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player);
		int num = IncomeTrack.DecreaseIncomeLevelBy(playerByColor.IncomeMarkerPosition, 3);
		LoanActionViewModel loanActionViewModel = new LoanActionViewModel
		{
			PlayerColor = playerByColor.Color,
			PlayerAvatar = Global.Instance.BrassGameHistory.CurrentGame.Metadata.GetPlayerMetadataByColor(playerByColor.Color).PlayerAvatar,
			CurrentIncomeLevel = playerByColor.IncomeLevel,
			CurrentIncomePosition = playerByColor.IncomeMarkerPosition,
			TargetIncomeLevel = IncomeTrack.GetCurrentIncomeLevel(num),
			TargetIncomePosition = num,
			IncomePositions = currentGame.Game.PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => p.IncomeMarkerPosition),
			IncomeLevels = currentGame.Game.PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => p.IncomeLevel),
			PlayerAvatars = currentGame.Metadata.PlayerMetadata.ToDictionary((PlayerMetadata p) => p.PlayerStartConfiguration.Color, (PlayerMetadata p) => p.PlayerAvatar)
		};
		IAnimation animation = null;
		if (!_router.GetView<ILoanActionView>().IsViewVisible())
		{
			_router.ShowView<ILoanActionView>();
			_router.MoveOnTop<IReplayView>();
			_router.GetView<ILoanActionView>().SetLoanButtonVisibility(isActive: false);
			_router.GetView<ILoanActionView>().ShowIncomePanel(loanActionViewModel, isDelayed: false);
			animation = _router.GetView<ILoanActionView>().CreateLoanActionAnimation(loanActionViewModel, 3f);
		}
		else
		{
			animation = _router.GetView<ILoanActionView>().CreateLoanActionAnimation(loanActionViewModel, 0f);
		}
		animationSequence.Append(animation);
		string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).Money + @event.Amount).ToString();
		IAnimation animation2 = _router.GetView<IPlayersSummaryView>().CreateChangeMoneyAnimation(newAmount, @event.Player);
		animationSequence.Append(animation2);
		animationSequence.OnKill(delegate
		{
			if (_router.GetView<ILoanActionView>().IsViewVisible())
			{
				_router.HideView<ILoanActionView>();
			}
			_router.GetView<ILoanActionView>().SetLoanButtonVisibility(isActive: true);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(IncomeMarkerPositionChanged @event)
	{
		int num = Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).IncomeMarkerPosition + @event.NumberOfPoints;
		string newIncomeMarker = $"{num}/{IncomeTrack.GetIncomeThreshold(num)}";
		int currentIncomeLevel = IncomeTrack.GetCurrentIncomeLevel(num);
		string newIncomeLevel = ((currentIncomeLevel > 0) ? "+" : "") + currentIncomeLevel;
		IAnimation animation = _router.GetView<IPlayersSummaryView>().CreateChangeIncomeAnimation(newIncomeLevel, newIncomeMarker, @event.Player);
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.Append(animation);
		BaseIndustry industryByUniqueId = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetIndustryByUniqueId(@event.SourceUniqueId);
		BuildingSlot buildingSlotContainingGivenIndustry = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetBuildingSlotContainingGivenIndustry(industryByUniqueId);
		IAnimation animation2 = _router.GetView<IBoardView>().CreateIncomeMarkerPositionChangedAnimation(@event.NumberOfPoints, buildingSlotContainingGivenIndustry.UniqueId);
		animationSequence.Insert(0f, animation2);
		return animationSequence;
	}

	private IAnimation CreateAnimation(IncomeLevelChanged @event)
	{
		Player playerByColor = Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player);
		int num = playerByColor.IncomeLevel - @event.NumberOfLevels;
		string newIncomeLevel = ((num > 0) ? "+" : "") + num;
		int num2 = IncomeTrack.DecreaseIncomeLevelBy(playerByColor.IncomeMarkerPosition, @event.NumberOfLevels);
		string newIncomeMarker = $"{num2}/{IncomeTrack.GetIncomeThreshold(num2)}";
		return _router.GetView<IPlayersSummaryView>().CreateChangeIncomeAnimation(newIncomeLevel, newIncomeMarker, @event.Player);
	}

	private IAnimation CreateAnimation(VictoryPointsChanged @event)
	{
		string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).VP + @event.Amount).ToString();
		return _router.GetView<IPlayersSummaryView>().CreateChangeVPAnimation(newAmount, @event.Player);
	}

	private IAnimation CreateAnimation(MerchantBonusReceived @event)
	{
		switch (@event.BonusType)
		{
		case BonusType.VP:
		{
			string newAmount2 = ((Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.PlayerColor).VP + @event.RegionName == RegionName.Shrewsbury) ? 4 : 3).ToString();
			return _router.GetView<IPlayersSummaryView>().CreateChangeVPAnimation(newAmount2, @event.PlayerColor);
		}
		case BonusType.Money:
		{
			string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.PlayerColor).Money + 5).ToString();
			return _router.GetView<IPlayersSummaryView>().CreateChangeMoneyAnimation(newAmount, @event.PlayerColor);
		}
		case BonusType.Income:
		{
			int num = Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.PlayerColor).IncomeMarkerPosition + 2;
			string newIncomeMarker = $"{num}/{IncomeTrack.GetIncomeThreshold(num)}";
			int currentIncomeLevel = IncomeTrack.GetCurrentIncomeLevel(num);
			string newIncomeLevel = ((currentIncomeLevel > 0) ? "+" : "") + currentIncomeLevel;
			IAnimation animation = _router.GetView<IPlayersSummaryView>().CreateChangeIncomeAnimation(newIncomeLevel, newIncomeMarker, @event.PlayerColor);
			IAnimationSequence animationSequence = CreateAnimationSequence();
			animationSequence.Append(animation);
			return animationSequence;
		}
		default:
			return CreateAnimationSequence();
		}
	}

	private IAnimation CreateAnimation(IronBought @event)
	{
		string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.PlayerColor).Money - @event.Price).ToString();
		string newMoneySpent = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetMoneySpentByPlayerByColor(@event.PlayerColor) + @event.Price).ToString();
		return _router.GetView<IPlayersSummaryView>().CreateChangeMoneySpentAnimation(newAmount, @event.PlayerColor, newMoneySpent);
	}

	private IAnimation CreateAnimation(CoalBought @event)
	{
		string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.PlayerColor).Money - @event.Price).ToString();
		string newMoneySpent = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetMoneySpentByPlayerByColor(@event.PlayerColor) + @event.Price).ToString();
		return _router.GetView<IPlayersSummaryView>().CreateChangeMoneySpentAnimation(newAmount, @event.PlayerColor, newMoneySpent);
	}

	private IAnimation CreateAnimation(IronSold @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		IAnimation animation = _router.GetView<IMarketView>().CreateAddResourcesToMarketAnimation(BuildingType.IronWorks, @event.Amount);
		animationSequence.Append(animation);
		animationSequence.Insert(animation: _router.GetView<IBoardView>().CreateResourceSoldAnimation(@event.SlotUniqueId, @event.Amount, "Iron", Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetBuildingSlotByUniqueId(@event.SlotUniqueId)), delayInSeconds: 23f * UIProperties.FrameDuration);
		string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.PlayerColor).Money + @event.MoneyReceived).ToString();
		animationSequence.Insert(animation: _router.GetView<IPlayersSummaryView>().CreateChangeMoneyAnimation(newAmount, @event.PlayerColor), delayInSeconds: 20f * UIProperties.FrameDuration);
		return animationSequence;
	}

	private IAnimation CreateAnimation(CoalSold @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		IAnimation animation = _router.GetView<IMarketView>().CreateAddResourcesToMarketAnimation(BuildingType.CoalMine, @event.Amount);
		animationSequence.Append(animation);
		animationSequence.Insert(animation: _router.GetView<IBoardView>().CreateResourceSoldAnimation(@event.SlotUniqueId, @event.Amount, "Coal", Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetBuildingSlotByUniqueId(@event.SlotUniqueId)), delayInSeconds: 23f * UIProperties.FrameDuration);
		string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.PlayerColor).Money + @event.MoneyReceived).ToString();
		animationSequence.Insert(animation: _router.GetView<IPlayersSummaryView>().CreateChangeMoneyAnimation(newAmount, @event.PlayerColor), delayInSeconds: 20f * UIProperties.FrameDuration);
		return animationSequence;
	}

	private IAnimation CreateAnimation(BeerSupplied @event)
	{
		BaseIndustry industryByUniqueId = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetIndustryByUniqueId(@event.BeerSourceId);
		if (industryByUniqueId != null)
		{
			BuildingSlot buildingSlotContainingGivenIndustry = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetBuildingSlotContainingGivenIndustry(industryByUniqueId);
			return _router.GetView<IBoardView>().CreateResourceSoldAnimation(buildingSlotContainingGivenIndustry.UniqueId, @event.Amount, "Beer", buildingSlotContainingGivenIndustry);
		}
		BorderRegion regionOfGivenMerchantId = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetRegionOfGivenMerchantId(@event.BeerSourceId);
		return _router.GetView<IBoardView>().CreateBeerSuppliedFromMerchantAnimation(@event.BeerSourceId, regionOfGivenMerchantId);
	}

	private IAnimation CreateAnimation(CoalSupplied @event)
	{
		BaseIndustry industryByUniqueId = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetIndustryByUniqueId(@event.SourceUniqueId);
		if (industryByUniqueId != null)
		{
			BuildingSlot buildingSlotContainingGivenIndustry = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetBuildingSlotContainingGivenIndustry(industryByUniqueId);
			return _router.GetView<IBoardView>().CreateResourceSoldAnimation(buildingSlotContainingGivenIndustry.UniqueId, @event.Amount, "Coal", buildingSlotContainingGivenIndustry);
		}
		return CreateAnimationSequence();
	}

	private IAnimation CreateAnimation(IronSupplied @event)
	{
		BaseIndustry industryByUniqueId = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetIndustryByUniqueId(@event.SourceId);
		if (industryByUniqueId != null)
		{
			BuildingSlot buildingSlotContainingGivenIndustry = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetBuildingSlotContainingGivenIndustry(industryByUniqueId);
			return _router.GetView<IBoardView>().CreateResourceSoldAnimation(buildingSlotContainingGivenIndustry.UniqueId, @event.Amount, "Iron", buildingSlotContainingGivenIndustry);
		}
		return CreateAnimationSequence();
	}

	private IAnimation CreateAnimation(LinkPlaced @event)
	{
		return _router.GetView<IBoardView>().CreateLinkPlacedAnimation(@event.LinkId, @event.Player);
	}

	private IAnimation CreateAnimation(IndustryBuilt @event)
	{
		BuildingSlot buildingSlotByUniqueId = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetBuildingSlotByUniqueId(@event.SlotUniqueId);
		BaseIndustry firstBuildingOfType = Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).Mat.GetFirstBuildingOfType(@event.Type);
		return _router.GetView<IBoardView>().CreateIndustryBuiltAnimation(buildingSlotByUniqueId, firstBuildingOfType, @event.Player);
	}

	private IAnimation CreateAnimation(IndustryDeveloped @event)
	{
		bool wasViewVisible = true;
		IAnimation animation;
		if (!_router.GetView<IIndustriesDetailsView>().IsViewVisible())
		{
			wasViewVisible = false;
			_router.ShowView<IIndustriesDetailsView>();
			_router.MoveOnTop<IReplayView>();
			animation = _router.GetView<IIndustriesDetailsView>().CreateIndustryDevelopedAnimation(@event.Type, 1.5f);
		}
		else
		{
			animation = _router.GetView<IIndustriesDetailsView>().CreateIndustryDevelopedAnimation(@event.Type);
		}
		animation.OnKill(delegate
		{
			if (!wasViewVisible)
			{
				_router.HideView<IIndustriesDetailsView>();
			}
		});
		return animation;
	}

	private IAnimation CreateAnimation(IndustryFlipped @event)
	{
		BaseIndustry industryByUniqueId = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetIndustryByUniqueId(@event.IndustryUniqueId);
		BuildingSlot buildingSlotContainingGivenIndustry = Global.Instance.BrassGameHistory.CurrentGame.Game.Board.GetBuildingSlotContainingGivenIndustry(industryByUniqueId);
		return _router.GetView<IBoardView>().CreateIndustryFlippedAnimation(buildingSlotContainingGivenIndustry);
	}

	private IAnimation CreateAnimation(WildCardsTaken @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		if (_controller.GetReplayWatchingPlayerColor() != @event.Player)
		{
			animationSequence.AppendCallback(delegate
			{
				_router.ShowView<IScoutActionView>();
				_router.MoveOnTop<IHandOfCardsView>();
				_router.MoveOnTop<IReplayView>();
				Tuple<object, int> cardObjectInfo = _router.GetView<IHandOfCardsView>().GetCardObjectInfo(@event.CardsToTrade[0]);
				((GameObject)cardObjectInfo.Item1).GetComponent<BaseCardView>().SetHiddenStatus(hidden: false);
				_router.GetView<IScoutActionView>().AnimateCardToSlot(0, cardObjectInfo, delegate
				{
				});
			});
			animationSequence.AppendInterval(3f);
			animationSequence.AppendCallback(delegate
			{
				Tuple<object, int> cardObjectInfo = _router.GetView<IHandOfCardsView>().GetCardObjectInfo(@event.CardsToTrade[1]);
				((GameObject)cardObjectInfo.Item1).GetComponent<BaseCardView>().SetHiddenStatus(hidden: false);
				_router.GetView<IScoutActionView>().AnimateCardToSlot(1, cardObjectInfo, delegate
				{
				});
			});
			animationSequence.AppendInterval(3f);
			animationSequence.AppendCallback(delegate
			{
				_router.HideView<IScoutActionView>();
			});
		}
		animationSequence.OnKill(delegate
		{
			_router.HideView<IScoutActionView>();
			_router.GetView<IHandOfCardsView>().EnableCardsLayoutGroup();
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(CanalEraEnded @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			IEndOfEraView view = _router.GetView<IEndOfEraView>();
			view.MorphSkipButtonToOkButton();
			view.SetEndOfEraPhaseText(6);
			EndOfEraViewModel endOfEraViewModel = new EndOfEraViewModel
			{
				PlayersStats = new List<EndOfEraPlayerViewModel>()
			};
			foreach (KeyValuePair<PlayerColor, int> endOfEraLinksVP in @event.EndOfEraLinksVPs)
			{
				endOfEraViewModel.PlayersStats.Add(new EndOfEraPlayerViewModel
				{
					PlayerColor = endOfEraLinksVP.Key,
					LinksVP = endOfEraLinksVP.Value,
					IndustriesVP = @event.EndOfEraIndustriesVPs[endOfEraLinksVP.Key],
					BonusesVP = @event.CurrentEraBonusesVPs[endOfEraLinksVP.Key],
					SumVP = endOfEraLinksVP.Value + @event.EndOfEraIndustriesVPs[endOfEraLinksVP.Key] + @event.CurrentEraBonusesVPs[endOfEraLinksVP.Key]
				});
			}
			view.UpdateEndOfEraView(endOfEraViewModel);
			view.ShowScoreWindow();
		});
		foreach (KeyValuePair<PlayerColor, int> endOfEraLinksVP2 in @event.EndOfEraLinksVPs)
		{
			animationSequence.Append(_router.GetView<IPlayersSummaryView>().CreateChangeVPAnimation((endOfEraLinksVP2.Value + @event.EndOfEraIndustriesVPs[endOfEraLinksVP2.Key]).ToString(), endOfEraLinksVP2.Key));
		}
		animationSequence.AppendCallback(delegate
		{
			_nickname.text = "";
			_action.text = "";
			_nickname.gameObject.SetActive(value: true);
			_action.gameObject.SetActive(value: true);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(CanalEraEndStarted @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			IEndOfEraView view = _router.GetView<IEndOfEraView>();
			_router.ShowView<IEndOfEraView>();
			view.HideScoreWindow();
			view.SetOnSkipButtonClicked(delegate
			{
				SkipToMyTurnClicked();
			});
			_nickname.text = "";
			_action.text = "";
			_nickname.gameObject.SetActive(value: false);
			_action.gameObject.SetActive(value: false);
			_router.GetView<IBoardView>().ZoomOutToWholeMap(GameEra.CanalEra);
		});
		animationSequence.Append(_router.GetView<IEndOfEraView>().ShowEndOfCanalEraTextAnimation(@event.IsIntroductoryGame));
		return animationSequence;
	}

	private IAnimation CreateAnimation(ScoreLinksPhaseStarted @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(0);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(LinksVictoryPointsChanged @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		if (@event.Amount > 0)
		{
			animationSequence.Append(_router.GetView<IBoardView>().CreateRemovePlayerLinksAnimation(@event.Player));
			string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).VP + @event.Amount).ToString();
			animationSequence.Append(_router.GetView<IPlayersSummaryView>().CreateChangeVPAnimation(newAmount, @event.Player));
		}
		return animationSequence;
	}

	private IAnimation CreateAnimation(ScoreFlippedIndustryTilesPhaseStarted @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(1);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(FlippedIndustriesVictoryPointsChanged @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		if (@event.Amount > 0)
		{
			animationSequence.Append(_router.GetView<IBoardView>().CreatePointPlayerFlippedIndustriesAnimation(@event.Player));
			string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).VP + @event.Amount).ToString();
			animationSequence.Append(_router.GetView<IPlayersSummaryView>().CreateChangeVPAnimation(newAmount, @event.Player));
		}
		animationSequence.OnKill(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(6);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(ObsoleteIndustriesRemoved @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(2);
		});
		animationSequence.Append(_router.GetView<IBoardView>().CreateRemovePlayerLvlOneIndustriesAnimation(@event.PlayerColor));
		animationSequence.OnKill(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(6);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(ScoreAdditionalMoneyPhaseStarted @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(3);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(IntroductoryGameAdditionalMoneyVictoryPointsChanged @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		if (@event.Amount > 0)
		{
			animationSequence.Append(_router.GetView<IPlayersSummaryView>().CreateVictoryPointsForMoneyAnimation(@event.Amount, @event.Player));
			string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).VP + @event.Amount).ToString();
			animationSequence.Append(_router.GetView<IPlayersSummaryView>().CreateChangeVPAnimation(newAmount, @event.Player));
		}
		return animationSequence;
	}

	private IAnimation CreateAnimation(ScoreAdditionalIncomePhaseStarted @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(4);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(IntroductoryGameAdditionalIncomeVictoryPointsChanged @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		if (@event.Amount > 0)
		{
			animationSequence.Append(_router.GetView<IPlayersSummaryView>().CreateVictoryPointsForIncomeAnimation(@event.Amount, @event.Player));
			string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).VP + @event.Amount).ToString();
			animationSequence.Append(_router.GetView<IPlayersSummaryView>().CreateChangeVPAnimation(newAmount, @event.Player));
		}
		return animationSequence;
	}

	private IAnimation CreateAnimation(ScoreAdditionalIndustryPhaseStarted @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(5);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(IntroductoryGameAdditionalIndustryVictoryPointsChanged @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		if (@event.Amount > 0)
		{
			animationSequence.Append(_router.GetView<IBoardView>().CreatePointPlayerFlippedLvlTwoPlusIndustriesAnimation(@event.Player));
			string newAmount = (Global.Instance.BrassGameHistory.CurrentGame.Game.PlayersTrack.GetPlayerByColor(@event.Player).VP + @event.Amount).ToString();
			animationSequence.Append(_router.GetView<IPlayersSummaryView>().CreateChangeVPAnimation(newAmount, @event.Player));
		}
		animationSequence.OnKill(delegate
		{
			_router.GetView<IEndOfEraView>().SetEndOfEraPhaseText(6);
		});
		return animationSequence;
	}

	private IAnimation CreateAnimation(RailEraEndStarted @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			IEndOfEraView view = _router.GetView<IEndOfEraView>();
			_router.ShowView<IEndOfEraView>();
			view.HideScoreWindow();
			view.SetOnSkipButtonClicked(delegate
			{
				SkipToMyTurnClicked();
			});
			_nickname.text = "";
			_action.text = "";
			_nickname.gameObject.SetActive(value: false);
			_action.gameObject.SetActive(value: false);
			_router.GetView<IBoardView>().ZoomOutToWholeMap(GameEra.RailEra);
		});
		animationSequence.Append(_router.GetView<IEndOfEraView>().ShowEndOfRailEraTextAnimation());
		return animationSequence;
	}

	private IAnimation CreateAnimation(RailEraEnded @event)
	{
		IAnimationSequence animationSequence = CreateAnimationSequence();
		animationSequence.AppendCallback(delegate
		{
			_router.HideView<IEndOfEraView>();
		});
		return animationSequence;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_nickname = base.transform.Find("SafeArea/Popup/ActionPanel/PlayerText").GetComponent<TextMeshProUGUI>();
		_action = base.transform.Find("SafeArea/Popup/ActionPanel/ActionText").GetComponent<TextMeshProUGUI>();
		_fastForward = base.transform.Find("SafeArea/Popup/ActionPanel/X2").GetComponent<TextMeshProUGUI>();
		_skipButton = base.transform.Find("SafeArea/Popup/ActionPanel/SkipAction").gameObject;
		_fastForwardButton = base.transform.Find("SafeArea/Popup/FastForward").gameObject;
		base.DestroyWhenClosed = false;
	}
}
