using System.Collections.Generic;
using BrassApplication.AI.RuleBasedSystem.Rules;

namespace BrassApplication.AI.RuleBasedSystem;

public static class RuleSetFactory
{
	public static RuleSet CreateBasicRuleSet(AIType aiLevel)
	{
		List<IRule> obj = new List<IRule>
		{
			new DevelopManufactureForMerchantBeer(),
			new BuildBreweryForRail(),
			new BuildBreweryForStock(),
			new BuildBreweryForCanalEra(),
			new ConnectToBuildBrewery(),
			new DevelopCanalEraIndustries(),
			new DevelopCanalEraBrewery(),
			new BuildCoalForStock(),
			new BuildCoalForHighDemand(),
			new BuildIronForHighDemand(),
			new ConnectIronForHighDemand(),
			new ConnectCoalForHighDemand(),
			new ConnectIndustryToMerchant(),
			new ConnectIndustryForMerchantBeer(),
			new BuildIndustryForMerchantBeer(),
			new BuildCoalNearManufactureForMerchantBeer(),
			new ConnectCoalToManufactureForMerchantBeer(),
			new BuildCoalNearSingleConnectionMerchantBeer(),
			new ConnectUnflippedCoalMineToPotentialBuildLocations(),
			new BuildBreweryToSellIndustry(),
			new BuildIndustryForSale(),
			new BuildIndustryForExtraSale(),
			new PassRule(),
			new ForceDevelopForIronOverbuild(),
			new SellIndustryToMerchant(),
			new BuildIndustry(),
			new PlaceLink(),
			new PlaceLinkDouble(),
			new TakeLoan(),
			new TakeLoanBeforeSell(),
			new TakeLoanBeforeIron(),
			new TakeLoanBeforeCoal(),
			new ScoutRule()
		};
		RuleSet ruleSet = new RuleSet();
		foreach (IRule item in obj)
		{
			if (item.AILevel <= aiLevel)
			{
				ruleSet.Add(item);
			}
		}
		return ruleSet;
	}

	public static RuleSet CreateSellIndustryRuleSet()
	{
		return new RuleSet
		{
			new SellIndustryBecauseOfDebt()
		};
	}
}
