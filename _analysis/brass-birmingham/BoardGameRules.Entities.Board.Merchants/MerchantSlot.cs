using System;
using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Board.Merchants;

public class MerchantSlot : IBeerSource
{
	private bool _hasBeer;

	public string UniqueId { get; }

	public NumberOfPlayers MinNumberOfPlayers { get; }

	public MerchantTile MerchantTile { get; set; }

	public bool HasBeer => _hasBeer;

	public int AvailableBeer
	{
		get
		{
			if (!_hasBeer)
			{
				return 0;
			}
			return 1;
		}
	}

	public MerchantSlot(string id, NumberOfPlayers minNumberOfPlayers)
	{
		UniqueId = id;
		MinNumberOfPlayers = minNumberOfPlayers;
	}

	public void SetMerchant(MerchantTile merchantTile)
	{
		MerchantTile = merchantTile;
		ResetBeer();
	}

	public MerchantTile GetMerchant()
	{
		return MerchantTile;
	}

	public void ResetBeer()
	{
		_hasBeer = MerchantTile != null && MerchantTile.BuildingTypes.Count > 0;
	}

	public bool GetBeer()
	{
		if (_hasBeer)
		{
			_hasBeer = false;
			return true;
		}
		return false;
	}

	public int BeerPrice(int amount = 1)
	{
		return 0;
	}

	public void ConsumeBeer(int amount = 1)
	{
		if (!GetBeer())
		{
			throw new InvalidOperationException("Not enough Beer to consume.");
		}
	}
}
