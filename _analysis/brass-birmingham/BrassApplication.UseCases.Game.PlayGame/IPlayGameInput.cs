using BrassApplication.UseCases.Common.SaveGame;

namespace BrassApplication.UseCases.Game.PlayGame;

public interface IPlayGameInput : ISaveGameDelegate
{
	void PlayGame();
}
