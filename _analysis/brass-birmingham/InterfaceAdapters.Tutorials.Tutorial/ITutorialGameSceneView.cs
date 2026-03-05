using System;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.GameScene;

namespace InterfaceAdapters.Tutorials.Tutorial;

public interface ITutorialGameSceneView : IGameSceneView, IBaseView
{
	void SetOnClickAction(Action action);

	void MoveToTheNextStage(int stage);

	void HideStage(int stage);

	void MoveArrowFromStageToPosition(int stage, float? globalPositionX = null, float? globalPositionY = null, float offsetX = 0f, float offsetY = 0f);

	void HideArrowFromStage(int stage);
}
