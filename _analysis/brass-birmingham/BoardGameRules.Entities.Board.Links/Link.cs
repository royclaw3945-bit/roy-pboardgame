using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Board.Links;

public abstract class Link
{
	public readonly string LinkId;

	public Player Owner { get; private set; }

	public Region Region1 { get; }

	public Region Region2 { get; }

	public Region Region3 { get; }

	public bool HasOwner => Owner != null;

	protected Link(Region r1, Region r2)
	{
		Region1 = r1;
		Region2 = r2;
		Region1.Links.Add(this);
		Region2.Links.Add(this);
		LinkId = string.Concat(GetType().Name + Region1.Name, Region2.Name);
	}

	protected Link(Region r1, Region r2, Region r3)
		: this(r1, r2)
	{
		Region3 = r3;
		Region3.Links.Add(this);
	}

	public void SetOwner(Player player)
	{
		if (HasOwner)
		{
			throw new InvalidOperationException("Owner already set up");
		}
		Owner = player;
	}

	public void RemoveOwner()
	{
		Owner = null;
	}

	public List<Region> GetOtherEnd(Region r)
	{
		List<Region> list = new List<Region>();
		if (r != Region1)
		{
			list.Add(Region1);
		}
		if (r != Region2)
		{
			list.Add(Region2);
		}
		if (Region3 != null && r != Region3)
		{
			list.Add(Region3);
		}
		return list;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		Link link = (Link)obj;
		if (Region1 == link.Region1)
		{
			return Region2 == link.Region2;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)(Region1.Name ^ Region2.Name);
	}

	public override string ToString()
	{
		string text = "[ " + Region1.ToString() + ", " + Region2.ToString();
		if (Region3 != null)
		{
			text = text + ", " + Region3.ToString();
		}
		return text + "]";
	}
}
