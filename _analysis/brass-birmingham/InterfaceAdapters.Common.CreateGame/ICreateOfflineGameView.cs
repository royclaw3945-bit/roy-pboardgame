namespace InterfaceAdapters.Common.CreateGame;

public interface ICreateOfflineGameView : IBaseView
{
	void ShowNewGameState(CreateGameViewModel viewModel);

	void ShowExistingGameContainer(CreateGameViewModel viewModel);

	void HideExistingGameContainer();
}
