using BrassApplication.UseCases.Menu.Tutorial;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Tutorial;

public class TutorialController : IController
{
	private readonly ITutorialInput _tutorialUseCase;

	public TutorialController(ITutorialInput tutorialUseCase)
	{
		_tutorialUseCase = tutorialUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void StartTutorial()
	{
		_tutorialUseCase.StartTutorial();
	}
}
