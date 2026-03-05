using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;

namespace BrassApplication.Persistence.RepositoryBackends;

public interface IGameStorage
{
	ReadOnlyCollection<IEvent> LoadEvents(string gameId, int minRevision, int maxRevision);

	void SaveGame(string gameId, IList<IEvent> eventsToAppend, GameMetadata metadata);

	void DeleteEvents(string gameId);

	string CreateGame(GameMetadata gameMetadata);

	ReadOnlyCollection<GameMetadata> GetGamesMetadata(IGamesFilter gamesFilter = null);

	void SavePlayerMetadata(string gameId, PlayerMetadata playerMetadata);

	GameMetadata LoadGameMetadata(string gameId);

	void DeleteGameMetadata(string gameId);

	void JoinGame(string gameId, string password, PlayerMetadata playerMetadata);

	void LeaveGame(string gameId, string playerId, bool isThisAnAbandonRequest);

	void DeleteAll();
}
