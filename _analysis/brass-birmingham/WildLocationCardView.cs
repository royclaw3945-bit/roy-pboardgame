using BoardGameRules.Entities.Cards;
using TMPro;
using UnityEngine.UI;

public class WildLocationCardView : BaseCardView
{
	private TextMeshProUGUI _nameText1;

	private TextMeshProUGUI _nameText2;

	private TextMeshProUGUI _nameText3;

	private Image _nameBackgroundLeft;

	private Image _nameBackgroundRight;

	private Image _nameBackgroundBottom;

	public override void SetCard(BaseCard card, bool shouldHideCards)
	{
		base.SetCard(card, shouldHideCards);
		_ = (WildLocationCard)card;
	}
}
