namespace BoardGameRules.Entities.Common;

public interface IIronSource
{
	string UniqueId { get; }

	bool HasIron { get; }

	int AvailableIron { get; }

	int IronPrice(int amount = 1);

	void ConsumeIron(int amount);
}
