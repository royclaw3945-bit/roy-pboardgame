using BoardGameRules.Entities.EventSourcing.Snapshot;
using BrassApplication.Game;

namespace Frameworks.Persistence.GameFactory;

public interface IGameFactory
{
	BrassApplicationGame CreateGame(string gameId, GameMetadata gameMetadata, ISnapshot snapshot);
}
