using System.Collections.ObjectModel;
using BrassApplication.Game;

namespace BrassApplication.Persistence.Repository;

public interface IRepository
{
	IAdvancedRepository Advanced { get; }

	IAsyncRepository Async { get; }

	ReadOnlyCollection<GameMetadata> GetGamesMetadata(IGamesFilter gamesFilter = null);

	BrassApplicationGame Create(GameMetadata gameMetadata);

	BrassApplicationGame Load(string gameId, int maxRevisionNumber = int.MaxValue);

	void Save(BrassApplicationGame brassApplicationGame);

	void Save(PlayerMetadata playerMetadata);

	void Delete(string gameId);

	void JoinGame(string gameId, string password, PlayerMetadata playerMetadata);

	void LeaveGame(string gameId, string playerId, bool isThisAnAbandonRequest);
}
