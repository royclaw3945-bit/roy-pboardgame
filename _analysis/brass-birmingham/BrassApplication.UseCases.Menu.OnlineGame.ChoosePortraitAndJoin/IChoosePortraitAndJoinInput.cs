using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.OnlineGame.ChoosePortraitAndJoin;

public interface IChoosePortraitAndJoinInput : IUseCase
{
	void BackClicked();

	void NextPortraitClicked();

	void PreviousPortraitClicked();

	void JoinGame();
}
