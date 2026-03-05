namespace BoardGameRules.Entities.Common;

public interface ICoalSource
{
	string UniqueId { get; }

	bool HasCoal { get; }

	int AvailableCoal { get; }

	int CoalPrice(int amount);

	void ConsumeCoal(int amount);
}
