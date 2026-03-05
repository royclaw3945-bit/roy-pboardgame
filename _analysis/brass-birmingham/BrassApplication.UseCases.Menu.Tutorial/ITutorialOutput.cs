using BrassApplication.Game;

namespace BrassApplication.UseCases.Menu.Tutorial;

public interface ITutorialOutput
{
	void GameCreated(BrassApplicationGame game);

	void HideView();
}
