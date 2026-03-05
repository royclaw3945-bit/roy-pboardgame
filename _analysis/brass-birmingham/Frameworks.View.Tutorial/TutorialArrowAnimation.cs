using DG.Tweening;
using UnityEngine;

namespace Frameworks.View.Tutorial;

public class TutorialArrowAnimation : MonoBehaviour
{
	private Vector3 startingPos = Vector3.zero;

	private RectTransform rect;

	private void Start()
	{
		startingPos = base.transform.localPosition;
		rect = base.transform.GetComponent<RectTransform>();
		AnimationMove();
	}

	private void AnimationMove()
	{
		rect.localPosition = startingPos - rect.up * 20f;
		Sequence sequence = DOTween.Sequence();
		sequence.Append(rect.DOLocalMove(startingPos + rect.up * 20f, 1f));
		sequence.Append(rect.DOLocalMove(startingPos - rect.up * 20f, 1f));
		sequence.SetEase(Ease.InOutSine);
		sequence.SetLoops(-1);
		sequence.Play();
	}
}
