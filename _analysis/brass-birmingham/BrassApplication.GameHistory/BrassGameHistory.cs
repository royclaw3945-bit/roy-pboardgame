using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;

namespace BrassApplication.GameHistory;

public class BrassGameHistory : IGameHistory
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(BrassGameHistory));

	private bool _movedBackInHistory;

	private ReadOnlyCollection<IEvent> _events;

	public IRepository Repository { get; set; }

	public BrassApplicationGame CurrentGame { get; private set; }

	private BrassApplicationGame LatestGame { get; set; }

	public BrassGameHistory()
	{
		Repository = null;
		CurrentGame = null;
	}

	public void SetGame(BrassApplicationGame game)
	{
		CurrentGame = game;
		_events = null;
		_movedBackInHistory = false;
		LatestGame = null;
	}

	public void MoveGameToRevision(int targetRevision)
	{
		Logger.Debug($"MoveGameToRevision, revision: {targetRevision}");
		if (Repository == null)
		{
			throw new InvalidOperationException("Can't move game history with Repository == null. Set repository first.");
		}
		if (CurrentGame == null)
		{
			throw new InvalidOperationException("Can't move game history with CurrentGame == null. SetGame first.");
		}
		if (!_movedBackInHistory)
		{
			LatestGame = CurrentGame;
			List<IEvent> list = new List<IEvent>(Repository.Advanced.GameStorage.LoadEvents(CurrentGame.Metadata.GameId, 0, CurrentGame.Game.Revision));
			ReadOnlyCollection<IEvent> uncommittedEvents = CurrentGame.Game.GetUncommittedEvents();
			list.AddRange(uncommittedEvents);
			_events = list.AsReadOnly();
			_movedBackInHistory = true;
		}
		int revision = CurrentGame.Game.Revision;
		BrassApplicationGame loadedGame = CurrentGame;
		if (revision > targetRevision)
		{
			loadedGame = Repository.Load(CurrentGame.Metadata.GameId, targetRevision);
		}
		if (loadedGame.Game.Revision < targetRevision)
		{
			_events.Where((IEvent e) => e.Metadata.Revision > loadedGame.Game.Revision && e.Metadata.Revision <= targetRevision).ToList().ForEach(delegate(IEvent e2)
			{
				loadedGame.Game.ApplyEvent(e2);
			});
		}
		if (loadedGame.Game.Revision != targetRevision)
		{
			throw new InvalidOperationException("It is not possible to load game from that revision");
		}
		CurrentGame = loadedGame;
	}

	public void MoveToLatestGame()
	{
		SetGame(LatestGame);
	}

	public IReadOnlyCollection<IEvent> GetUnseenEvents(PlayerColor playerColor)
	{
		int lastSeenEventRevision = CurrentGame.Metadata.GetPlayerMetadataByColor(playerColor).LastSeenEventRevision;
		return LoadNotSeenEvents(lastSeenEventRevision);
	}

	public bool IsUndoActionAvailable()
	{
		if (CurrentGame == null)
		{
			return false;
		}
		if (_movedBackInHistory)
		{
			return false;
		}
		if (CurrentGame.Game.GetUncommittedEvents().Count == 0)
		{
			return false;
		}
		if (CurrentGame.Game.GetUncommittedEvents().Last().GetType() == typeof(ActionCountDecreased))
		{
			return true;
		}
		return false;
	}

	public void UndoAction()
	{
		if (!IsUndoActionAvailable())
		{
			throw new InvalidOperationException("Can't undo action when UndoAction is not available");
		}
		UndoAndCancelImpl();
	}

	public bool IsCancelActionAvailable()
	{
		if (CurrentGame == null)
		{
			return false;
		}
		if (_movedBackInHistory)
		{
			return false;
		}
		if (CurrentGame.Game.GetUncommittedEvents().Count == 0)
		{
			return false;
		}
		if (CurrentGame.Game.GetUncommittedEvents().Last().GetType() == typeof(ActionCountDecreased))
		{
			return false;
		}
		return true;
	}

	public void CancelAction()
	{
		if (!IsCancelActionAvailable())
		{
			throw new InvalidOperationException("Can't cancel action when UndoAction is not available");
		}
		UndoAndCancelImpl();
	}

	private void UndoAndCancelImpl()
	{
		int beginningOfActionRevision = GetBeginningOfActionRevision();
		Logger.Debug($"UndoAndCancelImpl, beginningOfActionRevision: {beginningOfActionRevision}");
		MoveGameToRevision(beginningOfActionRevision);
		CurrentGame.Game.ClearUncommittedEventsAfterRevision(beginningOfActionRevision);
		CurrentGame.UpdatePlayerLastSeenRevisionAfterUndoOrCancel();
		SetGame(CurrentGame);
	}

	private int GetBeginningOfActionRevision()
	{
		List<IEvent> list = CurrentGame.Game.GetUncommittedEvents().ToList();
		if (list.Count == 0)
		{
			return CurrentGame.Game.Revision;
		}
		bool num = list.Last().GetType() == typeof(ActionCountDecreased);
		int num2 = 0;
		int num3 = 0;
		if (num)
		{
			num2 = list.Count - 2;
			num3 = list.Count - 1;
		}
		else
		{
			num2 = list.Count - 1;
			num3 = list.Count;
		}
		int num4 = list.FindLastIndex(num2, num3, (IEvent e) => e.GetType() == typeof(ActionCountDecreased));
		if (num4 == -1)
		{
			return CurrentGame.Game.Revision - list.Count;
		}
		return list.ElementAt(num4).Metadata.Revision;
	}

	private IReadOnlyCollection<IEvent> LoadNotSeenEvents(int lastSeenRevision)
	{
		List<IEvent> list = new List<IEvent>(Repository.Advanced.GameStorage.LoadEvents(CurrentGame.Metadata.GameId, lastSeenRevision, int.MaxValue));
		ReadOnlyCollection<IEvent> uncommittedEvents = CurrentGame.Game.GetUncommittedEvents();
		list.AddRange(uncommittedEvents);
		return list.AsReadOnly();
	}
}
