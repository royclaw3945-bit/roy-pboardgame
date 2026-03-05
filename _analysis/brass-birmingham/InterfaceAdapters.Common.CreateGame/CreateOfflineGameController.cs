using BrassApplication.UseCases.Menu.OfflineGame.CreateOfflineGame;

namespace InterfaceAdapters.Common.CreateGame;

public class CreateOfflineGameController : IController
{
	private readonly ICreateOfflineGameInput _createOfflineGameUseCase;

	public CreateOfflineGameController(ICreateOfflineGameInput createOfflineGameUseCase)
	{
		_createOfflineGameUseCase = createOfflineGameUseCase;
	}

	public void OnViewCreated()
	{
		_createOfflineGameUseCase.CreateNewGameConfiguration();
		_createOfflineGameUseCase.UpdateResumeOfflineGameView();
	}

	public void OnViewDestroyed()
	{
	}

	public void BackClicked()
	{
		_createOfflineGameUseCase.BackClicked();
	}

	public void StartGameClicked()
	{
		_createOfflineGameUseCase.StartGameClicked();
	}

	public void OnIntroductoryBoxClicked(bool isOn)
	{
		_createOfflineGameUseCase.SetIntroductoryGame(isOn);
	}

	public void ContinueClicked()
	{
		_createOfflineGameUseCase.ContinueClicked();
	}

	public void ShowIntroductoryGameTooltip(float x, float y)
	{
		_createOfflineGameUseCase.ShowIntroductoryGameTooltip(x, y);
	}

	public void HideTooltip()
	{
		_createOfflineGameUseCase.HideTooltip();
	}
}
