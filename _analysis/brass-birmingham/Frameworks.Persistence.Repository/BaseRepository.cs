using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.EventSourcing.Snapshot;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;
using BrassApplication.Persistence.RepositoryBackends;
using Frameworks.Persistence.GameFactory;

namespace Frameworks.Persistence.Repository;

public class BaseRepository : IRepository
{
	private class AdvancedRepository : IAdvancedRepository
	{
		private BaseRepository Repository { get; set; }

		public IGameStorage GameStorage => Repository.GameStorage;

		public ISnapshotStorage SnapshotStorage => Repository.SnapshotStorage;

		public AdvancedRepository(BaseRepository repository)
		{
			Repository = repository;
		}
	}

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(BaseRepository));

	protected IGameStorage GameStorage { get; private set; }

	protected ISnapshotStorage SnapshotStorage { get; private set; }

	protected IGameFactory GameFactory { get; private set; }

	public IAdvancedRepository Advanced => new AdvancedRepository(this);

	public IAsyncRepository Async => new AsyncRepository(this);

	public BaseRepository(IGameStorage gameStorage, ISnapshotStorage snapshotStorage, IGameFactory gameFactory)
	{
		if (gameStorage == null)
		{
			throw new ArgumentException("GameStorage is null");
		}
		if (snapshotStorage == null)
		{
			throw new ArgumentException("SnapshotStorage is null");
		}
		if (gameFactory == null)
		{
			throw new ArgumentException("GameFactory is null");
		}
		GameStorage = gameStorage;
		SnapshotStorage = snapshotStorage;
		GameFactory = gameFactory;
	}

	public BrassApplicationGame Create(GameMetadata gameMetadata)
	{
		if (gameMetadata == null)
		{
			throw new ArgumentException("GameStartConfiguration is null");
		}
		BrassApplicationGame brassApplicationGame = GameFactory.CreateGame(null, gameMetadata, null);
		string gameId = GameStorage.CreateGame(brassApplicationGame.Metadata);
		brassApplicationGame.Metadata.GameId = gameId;
		Logger.Debug("Repository.Create(): gameId = " + brassApplicationGame.Metadata.GameId);
		return brassApplicationGame;
	}

	public BrassApplicationGame Load(string gameId, int maxRevisionNumber = int.MaxValue)
	{
		Logger.Debug("Repository.Load(): gameId = " + gameId + ", maxRevisionNumber = " + maxRevisionNumber);
		Stopwatch stopwatch = Stopwatch.StartNew();
		int minRevision = 0;
		ISnapshot snapshot = SnapshotStorage.LoadSnapshot(gameId, maxRevisionNumber);
		if (snapshot != null)
		{
			minRevision = snapshot.Metadata.Revision;
		}
		GameMetadata gameMetadata = GameStorage.LoadGameMetadata(gameId);
		BrassApplicationGame brassApplicationGame = GameFactory.CreateGame(gameId, gameMetadata, snapshot);
		IList<IEvent> list = GameStorage.LoadEvents(gameId, minRevision, maxRevisionNumber);
		foreach (IEvent item in list ?? new List<IEvent>())
		{
			brassApplicationGame.Game.ApplyEvent(item);
		}
		brassApplicationGame.Game.ClearUncommittedEvents();
		stopwatch.Stop();
		Logger.Debug($"Repository.Load(): took {stopwatch.ElapsedMilliseconds}ms to load");
		return brassApplicationGame;
	}

	public void Save(BrassApplicationGame game)
	{
		if (game == null)
		{
			throw new ArgumentException("GameState can't be null");
		}
		Logger.Debug("Repository.Save(): Save game started for gameId = " + game.Metadata.GameId + ", final save revision = " + game.Game.Revision);
		ReadOnlyCollection<IEvent> uncommittedEvents = game.Game.GetUncommittedEvents();
		GameStorage.SaveGame(game.Metadata.GameId, uncommittedEvents, game.Metadata);
		game.Game.ClearUncommittedEvents();
		foreach (IEvent item in uncommittedEvents ?? new List<IEvent>().AsReadOnly())
		{
			Logger.Debug("Repository.Save(): gameId = " + game.Metadata.GameId + ", save event = " + item);
		}
	}

	public void Save(PlayerMetadata playerMetadata)
	{
		if (playerMetadata == null)
		{
			throw new ArgumentException("PlayerMetadata can't be null");
		}
		Logger.Debug("Repository.Save(): Save player metadata. PlayerId = " + playerMetadata.NickName + ", lastSeenRevision = " + playerMetadata.LastSeenEventRevision);
		GameStorage.SavePlayerMetadata(playerMetadata.GameId, playerMetadata);
	}

	public void Delete(string gameId)
	{
		Logger.Debug("Repository.Delete(): gameId = " + gameId);
		try
		{
			GameStorage.DeleteEvents(gameId);
			GameStorage.DeleteGameMetadata(gameId);
			SnapshotStorage.DeleteSnapshots(gameId);
		}
		catch (ArgumentException ex)
		{
			Logger.Info("There were no event and/or snapshots for given gameId = " + gameId + ". Ignoring.");
			Logger.Verbose("Exception was e = " + ex);
		}
	}

	public ReadOnlyCollection<GameMetadata> GetGamesMetadata(IGamesFilter gamesFilter = null)
	{
		ReadOnlyCollection<GameMetadata> gamesMetadata = GameStorage.GetGamesMetadata(gamesFilter);
		foreach (GameMetadata item in gamesMetadata)
		{
			Logger.Debug("Repository.GetGamesMetadata(): gameId = " + item.GameId);
		}
		return gamesMetadata;
	}

	public void JoinGame(string gameId, string password, PlayerMetadata playerMetadata)
	{
		GameStorage.JoinGame(gameId, password, playerMetadata);
	}

	public void LeaveGame(string gameId, string playerId, bool isThisAnAbandonRequest)
	{
		GameStorage.LeaveGame(gameId, playerId, isThisAnAbandonRequest);
	}
}
