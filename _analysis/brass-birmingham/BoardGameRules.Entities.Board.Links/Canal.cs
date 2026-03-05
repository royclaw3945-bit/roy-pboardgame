using BoardGameRules.Entities.Board.Regions;

namespace BoardGameRules.Entities.Board.Links;

public class Canal : Link
{
	public Canal(Region r1, Region r2)
		: base(r1, r2)
	{
	}

	public Canal(Region r1, Region r2, Region r3)
		: base(r1, r2, r3)
	{
	}
}
