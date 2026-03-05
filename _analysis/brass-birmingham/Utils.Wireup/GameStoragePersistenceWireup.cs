namespace Utils.Wireup;

public abstract class GameStoragePersistenceWireup : Wireup
{
	public const string Name = "GameStorage";

	protected GameStoragePersistenceWireup(Wireup inner)
		: base(inner)
	{
	}
}
