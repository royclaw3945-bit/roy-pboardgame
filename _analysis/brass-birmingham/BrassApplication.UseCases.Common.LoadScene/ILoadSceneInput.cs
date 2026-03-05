namespace BrassApplication.UseCases.Common.LoadScene;

public interface ILoadSceneInput
{
	void GenerateGameTip();

	void SetIsBoardUpdated(bool isBoardUpdated, bool isTutorial = false);
}
