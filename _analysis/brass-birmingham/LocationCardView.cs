using System;
using BoardGameRules.Entities.Cards;
using TMPro;
using UnityEngine;

public class LocationCardView : BaseCardView
{
	private TextMeshProUGUI _nameText;

	private Func<string, string> _getFriendlyNameFunc;

	public override void SetCard(BaseCard card, bool shouldHideCards)
	{
		base.SetCard(card, shouldHideCards);
		LocationCard locationCard = (LocationCard)card;
		BackgroundImage.sprite = Resources.Load<Sprite>("Cards/Backgrounds/BbCardLocation" + locationCard.Region);
		BannerImage.sprite = Resources.Load<Sprite>("Cards/Banners/bb_banner_" + locationCard.Color);
		_nameText.text = _getFriendlyNameFunc(locationCard.Region.ToString());
	}

	public void SetGetFriendlyNameFunc(Func<string, string> getFriendlyNameFunc)
	{
		_getFriendlyNameFunc = getFriendlyNameFunc;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_nameText = HolderObject.Find("Banner/Name").GetComponent<TextMeshProUGUI>();
	}
}
