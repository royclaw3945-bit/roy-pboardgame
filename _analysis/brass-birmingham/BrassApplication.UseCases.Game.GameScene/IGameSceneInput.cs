namespace BrassApplication.UseCases.Game.GameScene;

public interface IGameSceneInput
{
	void StartGameScene();

	void OpenMenu();

	void ShowBackToMenuTooltip(float x, float y);

	void HideTooltip();
}
