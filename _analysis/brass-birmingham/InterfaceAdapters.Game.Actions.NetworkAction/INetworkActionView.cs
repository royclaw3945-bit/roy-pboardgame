using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.NetworkAction;

public interface INetworkActionView : IBaseView
{
	void EnableBuildingAnotherLink(bool enableBuilding);

	void SetCantBuildMessage(string message);
}
