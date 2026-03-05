namespace BoardGameRules.Utils;

public static class UniqueIdGenerator
{
	public static string GenerateIndustryUniqueId(IRandomer randomer)
	{
		string result = "";
		if (randomer != null)
		{
			result = "industry" + randomer.Next();
		}
		return result;
	}

	public static string GenerateCardUniqueId(IRandomer randomer)
	{
		string result = "";
		if (randomer != null)
		{
			result = "card" + randomer.Next();
		}
		return result;
	}

	public static string GenerateMerchantSlotUniqueId(IRandomer randomer)
	{
		string result = "";
		if (randomer != null)
		{
			result = "merchantSlot" + randomer.Next();
		}
		return result;
	}

	public static string GenerateBuildingSlotUniqueId(IRandomer randomer)
	{
		string result = "";
		if (randomer != null)
		{
			result = "buildingSlot" + randomer.Next();
		}
		return result;
	}
}
