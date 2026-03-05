namespace BrassApplication.UseCases.Common.LoadScene;

public interface ILoadSceneOutput
{
	void DisplayTip(int tipLocalizationId, bool wasLastTip);

	void HideView();

	bool GetWasLastTipNewGlobal();

	int GetLastTipIdGlobal();
}
