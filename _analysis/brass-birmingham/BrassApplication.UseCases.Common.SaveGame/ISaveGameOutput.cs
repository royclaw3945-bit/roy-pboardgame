namespace BrassApplication.UseCases.Common.SaveGame;

public interface ISaveGameOutput
{
	void SaveGameCompleted(bool isSaveSuccessful, ISaveGameDelegate saveGameDelegate);

	void SaveGameStarted();
}
