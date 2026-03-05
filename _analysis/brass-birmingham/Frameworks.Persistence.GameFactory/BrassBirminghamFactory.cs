using BoardGameRules.Entities.EventSourcing.Snapshot;
using BoardGameRules.Entities.Game;
using BrassApplication.Game;

namespace Frameworks.Persistence.GameFactory;

public class BrassBirminghamFactory : IGameFactory
{
	public BrassApplicationGame CreateGame(string gameId, GameMetadata gameMetadata, ISnapshot snapshot)
	{
		if (snapshot == null)
		{
			return new BrassApplicationGame(new BoardGameRulesGame(gameMetadata.GameStartConfiguration), gameMetadata);
		}
		return new BrassApplicationGame(snapshot.Payload as BrassApplicationGame);
	}
}
