namespace BrassApplication.Persistence.Repository;

public interface IRepositoryFactory
{
	IRepository CreateLocalGameRepository();

	IRepository CreateTutorialGameRepository();

	IRepository CreateTestGamesRepository();

	IRepository CreateOnlineGameRepository();

	IRepository CreateInMemoryGameRepository();
}
