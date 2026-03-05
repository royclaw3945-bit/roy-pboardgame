using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BrassApplication.Game;

namespace BrassApplication.Persistence.Repository;

public interface IAsyncRepository
{
	IAdvancedRepository Advanced { get; }

	Task<ReadOnlyCollection<GameMetadata>> GetGamesMetadataAsync(IGamesFilter gamesFilter = null);

	Task<BrassApplicationGame> CreateAsync(GameMetadata gameMetadata);

	Task<BrassApplicationGame> LoadAsync(string gameId, int maxRevisionNumber = int.MaxValue);

	Task SaveAsync(BrassApplicationGame brassApplicationGame);

	Task SaveAsync(PlayerMetadata playerMetadata);

	Task DeleteAsync(string gameId);

	Task JoinGameAsync(string gameId, string password, PlayerMetadata playerMetadata);

	Task LeaveGameAsync(string gameId, string playerId, bool isThisAnAbandonRequest);
}
