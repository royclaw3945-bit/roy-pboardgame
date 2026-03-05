using BrassApplication.UseCases.Game.GameScene;

namespace BrassApplication.UseCases.Tutorials.Tutorial;

public interface ITutorialGameSceneInput : IGameSceneInput
{
	void MoveToTheNextStage();
}
