using BrassApplication.Persistence.Repository;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;
using Frameworks.Persistence.GameFactory;
using Frameworks.Persistence.Repository;
using Frameworks.Persistence.RepositoryBackends.InMemory;
using Frameworks.Persistence.Serialization;

namespace Utils.Wireup;

public class Wireup
{
	private readonly NanoContainer _container;

	private readonly Wireup _inner;

	protected virtual NanoContainer Container => _container ?? _inner.Container;

	protected Wireup(NanoContainer container)
	{
		_container = container;
	}

	protected Wireup(Wireup inner)
	{
		_inner = inner;
	}

	public static Wireup Init()
	{
		NanoContainer nanoContainer = new NanoContainer();
		nanoContainer.Register((IGameStorage)new InMemoryGameStorage());
		nanoContainer.Register((ISnapshotStorage)new InMemorySnapshotStorage());
		nanoContainer.Register((ISerialize)new JsonSerializer());
		nanoContainer.Register((IGameFactory)new NullGameFactory());
		nanoContainer.Register(BuildRepository);
		return new Wireup(nanoContainer);
	}

	public virtual Wireup With<T>(T instance) where T : class
	{
		Container.Register(instance);
		return this;
	}

	public virtual IRepository BuildRepository()
	{
		if (_inner != null)
		{
			return _inner.BuildRepository();
		}
		return Container.Resolve<IRepository>();
	}

	private static IRepository BuildRepository(NanoContainer context)
	{
		IGameStorage gameStorage = context.Resolve<IGameStorage>();
		ISnapshotStorage snapshotStorage = context.Resolve<ISnapshotStorage>();
		IGameFactory gameFactory = context.Resolve<IGameFactory>();
		return new BaseRepository(gameStorage, snapshotStorage, gameFactory);
	}
}
