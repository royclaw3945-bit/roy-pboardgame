using BrassApplication.UseCases.Game.GameScene;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.GameScene;

public class GameSceneController : IController
{
	private readonly IGameSceneInput _useCase;

	public GameSceneController(IGameSceneInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void StartGameScene()
	{
		_useCase.StartGameScene();
	}

	public void OnMouseEnter(float x, float y)
	{
		_useCase.ShowBackToMenuTooltip(x, y);
	}

	public void OnMouseExit()
	{
		_useCase.HideTooltip();
	}

	public void OpenMenu()
	{
		_useCase.OpenMenu();
	}
}
