namespace BrassApplication.AI.RuleBasedSystem;

public static class Priority
{
	public const int SellIndustryBecauseOfDebt = 10000;

	public const int BuildIndustryForExtraSale = 2700;

	public const int TakeLoanBeforeSell = 2620;

	public const int SellIndustryToMerchant_High = 2610;

	public const int BuildIndustryForMerchantBeer_High = 2600;

	public const int TakeLoanBeforeIron = 2510;

	public const int BuildIronForHighDemand_High = 2500;

	public const int TakeLoanBeforeCoal = 2410;

	public const int BuildCoalForHighDemand_High = 2400;

	public const int TakeLoan_High = 2300;

	public const int BuildBreweryForRail_High = 2210;

	public const int ConnectCoalToManufactureForMerchantBeer = 2200;

	public const int SellIndustryToMerchant_Low = 2130;

	public const int BuildBreweryToSellIndustry = 2120;

	public const int ConnectToBuildBrewery = 2110;

	public const int ConnectIndustryForMerchantBeer = 2100;

	public const int ConnectCoalForHighDemand_High = 2010;

	public const int BuildIndustryForMerchantBeer_Low = 2000;

	public const int ConnectIronForHighDemand_High = 1900;

	public const int ScoutRule_High = 1810;

	public const int DevelopManufactureForMerchantBeer = 1800;

	public const int ConnectIndustryToMerchant = 1700;

	public const int ConnectCoalForHighDemand_Medium = 1640;

	public const int BuildIndustryForSale_High = 1630;

	public const int BuildBreweryForRail_Low = 1620;

	public const int TakeLoan_Medium = 1610;

	public const int BuildCoalNearSingleConnectionMerchantBeer = 1600;

	public const int DevelopCanalEraBrewery = 1430;

	public const int PlaceLinkDouble = 1420;

	public const int BuildCoalForHighDemand_Low = 1410;

	public const int ConnectCoalForHighDemand_Low = 1400;

	public const int DevelopCanalEraIndustries_LateCanalGame = 1310;

	public const int BuildIronForHighDemand_Low = 1300;

	public const int DevelopCanalEraIndustries_MidGame = 1260;

	public const int BuildBreweryForStock = 1250;

	public const int BuildCoalForStock = 1240;

	public const int BuildBreweryForCanalEra = 1230;

	public const int ConnectIronForHighDemand_Low = 1220;

	public const int BuildCoalNearManufactureForMerchantBeer = 1210;

	public const int DevelopCanalEraIndustries_EarlyCanalGame = 1200;

	public const int ForceDevelopForIronOverbuild = 800;

	public const int BuildIndustryForSale_Low = 600;

	public const int ScoutRule_Med = 510;

	public const int PlaceLink = 500;

	public const int TakeLoan_Low = 450;

	public const int BuildIndustry = 400;

	public const int ScoutRule_Low = 250;

	public const int PassRule = -100;

	public const int ConnectUnflippedCoalMineToPotentialBuildLocations = -1610;
}
