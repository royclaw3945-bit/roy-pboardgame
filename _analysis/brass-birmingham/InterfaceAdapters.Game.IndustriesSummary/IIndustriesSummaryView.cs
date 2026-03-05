using System.Collections.Generic;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.IndustriesSummary;

public interface IIndustriesSummaryView : IBaseView
{
	void UpdateIndustriesSummary(List<IndustryViewModel> industryViewModels, LinkCountViewModel linkCountViewModel);
}
