using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;

namespace BrassApplication.GameHistory;

public interface IGameHistory
{
	IRepository Repository { get; set; }

	BrassApplicationGame CurrentGame { get; }

	void SetGame(BrassApplicationGame game);

	void MoveGameToRevision(int targetRevision);

	void MoveToLatestGame();

	IReadOnlyCollection<IEvent> GetUnseenEvents(PlayerColor playerColor);

	bool IsUndoActionAvailable();

	void UndoAction();

	bool IsCancelActionAvailable();

	void CancelAction();
}
