using InterfaceAdapters.Common;

namespace InterfaceAdapters.Tutorials.Tutorial;

public interface ITutorialCardOverviewView : IBaseView
{
	void HighlightLocations();

	void HighlightBoth();
}
