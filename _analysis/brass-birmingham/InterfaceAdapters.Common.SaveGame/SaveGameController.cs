using BrassApplication.UseCases.Common.SaveGame;

namespace InterfaceAdapters.Common.SaveGame;

public class SaveGameController : IController
{
	private readonly ISaveGameInput useCase;

	public SaveGameController(ISaveGameInput useCase)
	{
		this.useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}
}
