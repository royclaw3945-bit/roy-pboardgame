namespace BoardGameRules.Entities.Common;

public interface IBeerSource
{
	string UniqueId { get; }

	bool HasBeer { get; }

	int AvailableBeer { get; }

	int BeerPrice(int amount = 1);

	void ConsumeBeer(int amount = 1);
}
