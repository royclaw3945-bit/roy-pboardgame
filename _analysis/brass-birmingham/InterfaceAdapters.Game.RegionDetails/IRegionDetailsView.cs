using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.RegionDetails;

public interface IRegionDetailsView : IBaseView
{
	void UpdateRegionDetailsView(RegionDetailsViewModel regionDetailsViewModel);
}
