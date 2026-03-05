using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;

namespace Frameworks.Persistence.Repository;

public class AsyncRepository : IAsyncRepository
{
	protected IRepository Repository { get; set; }

	public IAdvancedRepository Advanced => Repository.Advanced;

	public AsyncRepository(IRepository repository)
	{
		if (repository == null)
		{
			throw new ArgumentException("Repository is null");
		}
		Repository = repository;
	}

	public Task<ReadOnlyCollection<GameMetadata>> GetGamesMetadataAsync(IGamesFilter gamesFilter = null)
	{
		return Task.Run(() => Repository.GetGamesMetadata(gamesFilter));
	}

	public Task<BrassApplicationGame> CreateAsync(GameMetadata gameMetadata)
	{
		return Task.Run(() => Repository.Create(gameMetadata));
	}

	public Task<BrassApplicationGame> LoadAsync(string gameId, int maxRevisionNumber = int.MaxValue)
	{
		return Task.Run(() => Repository.Load(gameId, maxRevisionNumber));
	}

	public Task SaveAsync(BrassApplicationGame game)
	{
		return Task.Run(delegate
		{
			Repository.Save(game);
		});
	}

	public Task SaveAsync(PlayerMetadata playerMetadata)
	{
		return Task.Run(delegate
		{
			Repository.Save(playerMetadata);
		});
	}

	public Task DeleteAsync(string gameId)
	{
		return Task.Run(delegate
		{
			Repository.Delete(gameId);
		});
	}

	public Task JoinGameAsync(string gameId, string password, PlayerMetadata playerMetadata)
	{
		return Task.Run(delegate
		{
			Repository.JoinGame(gameId, password, playerMetadata);
		});
	}

	public Task LeaveGameAsync(string gameId, string playerId, bool isThisAnAbandonRequest)
	{
		return Task.Run(delegate
		{
			Repository.LeaveGame(gameId, playerId, isThisAnAbandonRequest);
		});
	}
}
