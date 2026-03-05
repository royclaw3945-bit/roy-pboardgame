using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.IndustriesSummary;

public interface IIndustriesSummaryOutput
{
	void UpdateIndustriesSummary(BrassApplicationGame game, PlayerColor playerColor, IDictionary<BuildingType, IList<CantUseBuildActionReason>> cantBuildReasons);

	void ShowIndustryDetails();

	void ShowTooltip(string element, float x, float y, int industryLevel = 0, int resources = 0);

	void HideTooltip();
}
