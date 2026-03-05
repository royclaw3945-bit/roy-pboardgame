using DG.Tweening;
using Frameworks.Utils;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Tutorials.HandOfCards;
using UnityEngine;

namespace Frameworks.View.Tutorial;

public class TutorialHandOfCardsView : HandOfCardsView, ITutorialHandOfCardsView, IHandOfCardsView, IBaseView
{
	public void SlipCardsOut()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.OnStart(delegate
		{
			for (int i = 0; i < _cardsContainerRectTransform.childCount; i++)
			{
				Transform child = _cardsContainerRectTransform.GetChild(i);
				LockCardInteractions(child.gameObject);
			}
		});
		for (int num = _cardsContainerRectTransform.childCount - 1; num >= 0; num--)
		{
			BaseCardView component = _cardsContainerRectTransform.GetChild(num).GetComponent<BaseCardView>();
			component.bonusY = 460f;
			sequence.Insert((float)(_cardsContainerRectTransform.childCount - num) * UIProperties.FrameDuration, component.HolderObject.GetComponent<RectTransform>().DOAnchorPosY(_baseYPosition + component.bonusY, 18f * UIProperties.FrameDuration).SetEase(Ease.InBack));
		}
		sequence.OnComplete(delegate
		{
			for (int i = 0; i < _cardsContainerRectTransform.childCount; i++)
			{
				Transform child = _cardsContainerRectTransform.GetChild(i);
				UnlockCardInteractions(child.gameObject);
			}
		});
	}
}
