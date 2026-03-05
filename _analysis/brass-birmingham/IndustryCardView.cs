using System.Linq;
using BoardGameRules.Entities.Cards;
using UnityEngine;

public class IndustryCardView : BaseCardView
{
	public override void SetCard(BaseCard card, bool shouldHideCards)
	{
		base.SetCard(card, shouldHideCards);
		IndustryCard industryCard = (IndustryCard)card;
		BackgroundImage.sprite = Resources.Load<Sprite>(string.Concat("Cards/Backgrounds/BbCardIndusty", industryCard.BuildingTypes.First(), industryCard.ImageTypeIndex));
	}
}
