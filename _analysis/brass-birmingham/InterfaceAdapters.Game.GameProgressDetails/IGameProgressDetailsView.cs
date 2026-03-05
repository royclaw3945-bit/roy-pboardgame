using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.GameProgressDetails;

public interface IGameProgressDetailsView : IBaseView
{
	void UpdateGameProgressDetailsView(GameProgressDetailsViewModel gameProgressDetailsViewModel);
}
