namespace InterfaceAdapters.Common.LoadScene;

public interface ILoadSceneView : IBaseView
{
	void LoadScene(string sceneName);

	void DisplayTip(string tipText, int tipLocalizationId, bool wasLastTipNew);

	int GetLastTipIdGlobal();

	bool GetWasLastTipNewGlobal();
}
