using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board.Events;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.GameLog;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.GameLogView;

public class GameLogPresenter : IPresenter, IGameLogOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	private readonly ChangeEventToTextHelper _changeEventToTextHelper;

	private int maxRounds = -1;

	public GameLogPresenter(IRouter router, ILocalization localization, IGameHistory gameHistory)
	{
		_router = router;
		_localization = localization;
		_changeEventToTextHelper = new ChangeEventToTextHelper(gameHistory, localization);
		BrassApplicationGame currentGame = gameHistory.CurrentGame;
		maxRounds = currentGame.Game.GameFlow.GetNumberOfRounds(currentGame.Metadata.GameStartConfiguration.NumberOfPlayers);
	}

	public void UpdateGameLogView(IList<IEvent> eventsList, GameProgressInformation gameProgressInformation, GameMetadata metadata)
	{
		ActionLogViewModel actionLogViewModel = GetActionLogViewModel(eventsList, gameProgressInformation, metadata);
		_router.GetView<IGameLogView>().UpdateGameLogView(actionLogViewModel);
	}

	public void UpdateReplaySummaryView(IList<IEvent> eventsList, GameProgressInformation gameProgressInformation, GameMetadata metadata, Action onDismiss)
	{
		ActionLogViewModel actionLogViewModel = GetActionLogViewModel(eventsList, gameProgressInformation, metadata);
		if (actionLogViewModel.Rounds.Count > 0)
		{
			_router.ShowView<IReplaySummaryView>();
			_router.GetView<IReplaySummaryView>().UpdateReplaySummaryView(actionLogViewModel, delegate
			{
				onDismiss?.Invoke();
				_router.HideView<IReplaySummaryView>();
			});
		}
		else
		{
			onDismiss?.Invoke();
		}
	}

	private ActionLogViewModel GetActionLogViewModel(IList<IEvent> eventsList, GameProgressInformation gameProgressInformation, GameMetadata metadata)
	{
		ActionLogViewModel actionLogViewModel = new ActionLogViewModel
		{
			Rounds = new List<ActionLogRoundViewModel>(),
			ActionTranslation = _localization.GetTranslation("UI/ACTION")
		};
		Dictionary<PlayerColor, PlayerAvatar> dictionary = new Dictionary<PlayerColor, PlayerAvatar>();
		foreach (PlayerMetadata playerMetadatum in metadata.PlayerMetadata)
		{
			dictionary.Add(playerMetadatum.PlayerStartConfiguration.Color, playerMetadatum.PlayerAvatar);
		}
		string translation = _localization.GetTranslation("UI/ROUND");
		ActionLogRoundViewModel actionLogRoundViewModel = new ActionLogRoundViewModel();
		int num = 0;
		int num2 = 1;
		bool flag = false;
		foreach (IEvent events in eventsList)
		{
			if (events is RoundStarted roundStarted)
			{
				string translation2 = _localization.GetTranslation("UI/" + ((roundStarted.GameEra == GameEra.CanalEra) ? "CANAL_ERA" : "RAIL_ERA"));
				actionLogRoundViewModel = new ActionLogRoundViewModel
				{
					Actions = new Dictionary<PlayerColor, List<Tuple<ActionLogRoundViewModel.TextEventType, string>>>(),
					Avatars = dictionary,
					Title = GetRoundText(translation2, translation, roundStarted.RoundNumber),
					Era = translation2,
					Turn = $"{translation} <#FFF>{roundStarted.RoundNumber}/{maxRounds}"
				};
				num = roundStarted.RoundNumber;
			}
			else if (events is RoundEnded)
			{
				if (actionLogRoundViewModel.Actions != null && actionLogRoundViewModel.Actions.Count > 0)
				{
					actionLogViewModel.Rounds.Add(actionLogRoundViewModel);
				}
				actionLogRoundViewModel = null;
			}
			else if (events is ActionCountDecreased)
			{
				num2++;
			}
			else if (events is MerchantBonusReceived { BonusType: BonusType.Develop })
			{
				flag = true;
			}
			Tuple<PlayerColor, ActionLogRoundViewModel.TextEventType, string> tuple = _changeEventToTextHelper.ChangeEventToText(events);
			if (string.IsNullOrEmpty(tuple.Item3))
			{
				continue;
			}
			if (flag && tuple.Item2 == ActionLogRoundViewModel.TextEventType.INDUSTRY_DEVELOPED)
			{
				flag = false;
				continue;
			}
			if (actionLogRoundViewModel.Actions == null)
			{
				num = ((!eventsList.Any((IEvent e) => e is RoundStarted)) ? gameProgressInformation.CurrentRoundNumber : (gameProgressInformation.CurrentRoundNumber - 1));
				string translation3 = _localization.GetTranslation("UI/" + ((gameProgressInformation.CurrentGameEra == GameEra.CanalEra) ? "CANAL_ERA" : "RAIL_ERA"));
				actionLogRoundViewModel = new ActionLogRoundViewModel
				{
					Actions = new Dictionary<PlayerColor, List<Tuple<ActionLogRoundViewModel.TextEventType, string>>>(),
					Avatars = dictionary,
					Title = GetRoundText(translation3, translation, num),
					Era = translation3,
					Turn = $"{translation} <#FFF>{num}/{maxRounds}"
				};
			}
			if (actionLogRoundViewModel.Actions.Count > 0 && actionLogRoundViewModel.Actions.Last().Key == tuple.Item1)
			{
				if (num2 == 1)
				{
					if (actionLogRoundViewModel.Actions[tuple.Item1][0].Item1 == ActionLogRoundViewModel.TextEventType.INDUSTRY_DEVELOPED)
					{
						if (tuple.Item2 == ActionLogRoundViewModel.TextEventType.INDUSTRY_DEVELOPED)
						{
							actionLogRoundViewModel.Actions[tuple.Item1][0] = _changeEventToTextHelper.FuseEventTexts(actionLogRoundViewModel.Actions[tuple.Item1][0].Item2, tuple.Item3, ActionLogRoundViewModel.TextEventType.FUSED_DEVELOP);
							continue;
						}
						throw new InvalidOperationException("Invalid action sequence");
					}
					if (actionLogRoundViewModel.Actions[tuple.Item1][0].Item1 == ActionLogRoundViewModel.TextEventType.LINK_PLACED)
					{
						if (tuple.Item2 == ActionLogRoundViewModel.TextEventType.LINK_PLACED)
						{
							actionLogRoundViewModel.Actions[tuple.Item1][0] = _changeEventToTextHelper.FuseEventTexts(actionLogRoundViewModel.Actions[tuple.Item1][0].Item2, tuple.Item3, ActionLogRoundViewModel.TextEventType.FUSED_LINK);
							continue;
						}
						throw new InvalidOperationException("Invalid action sequence");
					}
					if (actionLogRoundViewModel.Actions[tuple.Item1][0].Item1 != ActionLogRoundViewModel.TextEventType.INDUSTRY_FLIPPED && actionLogRoundViewModel.Actions[tuple.Item1][0].Item1 != ActionLogRoundViewModel.TextEventType.FUSED_INDUSTRY_FLIPPED)
					{
						throw new InvalidOperationException("Invalid action sequence");
					}
					if (tuple.Item2 != ActionLogRoundViewModel.TextEventType.INDUSTRY_FLIPPED)
					{
						throw new InvalidOperationException("Invalid action sequence");
					}
					actionLogRoundViewModel.Actions[tuple.Item1][0] = _changeEventToTextHelper.FuseEventTexts(actionLogRoundViewModel.Actions[tuple.Item1][0].Item2, tuple.Item3, ActionLogRoundViewModel.TextEventType.FUSED_INDUSTRY_FLIPPED);
				}
				else if (actionLogRoundViewModel.Actions[tuple.Item1].Count == 1)
				{
					actionLogRoundViewModel.Actions[tuple.Item1].Add(new Tuple<ActionLogRoundViewModel.TextEventType, string>(tuple.Item2, tuple.Item3));
				}
				else if (actionLogRoundViewModel.Actions[tuple.Item1][1].Item1 == ActionLogRoundViewModel.TextEventType.INDUSTRY_DEVELOPED)
				{
					if (tuple.Item2 != ActionLogRoundViewModel.TextEventType.INDUSTRY_DEVELOPED)
					{
						throw new InvalidOperationException("Invalid action sequence");
					}
					actionLogRoundViewModel.Actions[tuple.Item1][1] = _changeEventToTextHelper.FuseEventTexts(actionLogRoundViewModel.Actions[tuple.Item1][1].Item2, tuple.Item3, ActionLogRoundViewModel.TextEventType.FUSED_DEVELOP);
				}
				else if (actionLogRoundViewModel.Actions[tuple.Item1][1].Item1 == ActionLogRoundViewModel.TextEventType.LINK_PLACED)
				{
					if (tuple.Item2 != ActionLogRoundViewModel.TextEventType.LINK_PLACED)
					{
						throw new InvalidOperationException("Invalid action sequence");
					}
					actionLogRoundViewModel.Actions[tuple.Item1][1] = _changeEventToTextHelper.FuseEventTexts(actionLogRoundViewModel.Actions[tuple.Item1][1].Item2, tuple.Item3, ActionLogRoundViewModel.TextEventType.FUSED_LINK);
				}
				else
				{
					if (actionLogRoundViewModel.Actions[tuple.Item1][1].Item1 != ActionLogRoundViewModel.TextEventType.INDUSTRY_FLIPPED && actionLogRoundViewModel.Actions[tuple.Item1][1].Item1 != ActionLogRoundViewModel.TextEventType.FUSED_INDUSTRY_FLIPPED)
					{
						throw new InvalidOperationException("Invalid action sequence");
					}
					if (tuple.Item2 != ActionLogRoundViewModel.TextEventType.INDUSTRY_FLIPPED)
					{
						throw new InvalidOperationException("Invalid action sequence");
					}
					actionLogRoundViewModel.Actions[tuple.Item1][1] = _changeEventToTextHelper.FuseEventTexts(actionLogRoundViewModel.Actions[tuple.Item1][1].Item2, tuple.Item3, ActionLogRoundViewModel.TextEventType.FUSED_INDUSTRY_FLIPPED);
				}
			}
			else if (!actionLogRoundViewModel.Actions.ContainsKey(tuple.Item1))
			{
				num2 = 1;
				actionLogRoundViewModel.Actions.Add(tuple.Item1, new List<Tuple<ActionLogRoundViewModel.TextEventType, string>>
				{
					new Tuple<ActionLogRoundViewModel.TextEventType, string>(tuple.Item2, tuple.Item3)
				});
			}
			else
			{
				tuple = _changeEventToTextHelper.ChangeEventToText(events);
			}
		}
		if (actionLogRoundViewModel != null && actionLogRoundViewModel.Actions.Count > 0)
		{
			actionLogViewModel.Rounds.Add(actionLogRoundViewModel);
		}
		return actionLogViewModel;
	}

	public void CloseGameLogView()
	{
		_router.HideView<IGameLogView>();
	}

	private string GetRoundText(string era, string round, int roundNumber)
	{
		return $"{era}, {round} {roundNumber}";
	}
}
