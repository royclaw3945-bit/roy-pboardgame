using System.Threading.Tasks;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;

namespace BrassApplication.UseCases.Common.SaveGame;

public class SaveGameUseCase : IUseCase, ISaveGameInput
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(SaveGameUseCase));

	private readonly ISaveGameOutput _presenter;

	private readonly IRepository _repository;

	public SaveGameUseCase(ISaveGameOutput presenter, IRepository repository)
	{
		_presenter = presenter;
		_repository = repository;
	}

	public void SaveGame(BrassApplicationGame game, ISaveGameDelegate saveGameDelegate)
	{
		Logger.Debug("SaveGame requested.");
		if (game.Metadata.GameType.Equals(GameType.Network))
		{
			_presenter.SaveGameStarted();
		}
		_repository.Async.SaveAsync(game).ContinueWith(delegate(Task t)
		{
			bool isSaveSuccessful = t.Status.Equals(TaskStatus.RanToCompletion);
			_presenter.SaveGameCompleted(isSaveSuccessful, saveGameDelegate);
		});
	}
}
