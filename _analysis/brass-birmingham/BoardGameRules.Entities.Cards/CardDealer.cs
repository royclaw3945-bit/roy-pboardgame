using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Cards;

public class CardDealer
{
	protected readonly List<BaseCard> _deck;

	protected Func<int, List<BaseCard>, List<BaseCard>> takeFromDeck = delegate(int n, List<BaseCard> d)
	{
		if (d.Count >= n)
		{
			List<BaseCard> result = d.Take(n).ToList();
			d.RemoveRange(0, n);
			return result;
		}
		return new List<BaseCard>();
	};

	public CardDealer(List<BaseCard> deck)
	{
		_deck = deck;
	}

	public void TakeOneCardFromDeckAtGameStartAndDiscardIt(List<Player> players)
	{
		foreach (Player player in players)
		{
			player.DiscardPile.AddRange(takeFromDeck(1, _deck));
		}
	}

	public virtual void DealCardsForStartOfEra(List<Player> players)
	{
		foreach (Player player in players)
		{
			player.Hand.Clear();
			player.Hand.AddRange(takeFromDeck(8, _deck));
		}
	}

	public virtual void DealCardsForEndOfTurn(Player player)
	{
		player?.Hand.AddRange(takeFromDeck(8 - player.Hand.Count, _deck));
	}
}
