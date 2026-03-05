using BoardGameRules.Entities.EventSourcing.Snapshot;
using BrassApplication.Game;

namespace Frameworks.Persistence.GameFactory;

public class NullGameFactory : IGameFactory
{
	public BrassApplicationGame CreateGame(string gameId, GameMetadata gameMetadata, ISnapshot snapshot)
	{
		return null;
	}
}
