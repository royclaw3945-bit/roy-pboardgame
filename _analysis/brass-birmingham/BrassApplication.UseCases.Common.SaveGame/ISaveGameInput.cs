using BrassApplication.Game;

namespace BrassApplication.UseCases.Common.SaveGame;

public interface ISaveGameInput
{
	void SaveGame(BrassApplicationGame game, ISaveGameDelegate saveGameDelegate);
}
