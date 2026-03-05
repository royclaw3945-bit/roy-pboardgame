using System;
using System.Threading.Tasks;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Actions.LoanAction;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class LoanActionView : BaseView, ILoanActionView, IBaseView
{
	private LoanActionController _controller;

	private CanvasGroup _panelCanvasGroup;

	private Button _buttonTakeLoan;

	private IncomeTrackView _incomeTrackView;

	private AudioSource _audioSource;

	public AudioClip StartLoanClip;

	public AudioClip TakeLoanClip;

	[SerializeField]
	private GameObject _currentIncomePlayerToken;

	[SerializeField]
	private GameObject _targetIncomePlayerToken;

	[SerializeField]
	private GameObject _arrowToTargetPrefab;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is LoanActionController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public async void ShowIncomePanel(LoanActionViewModel loanActionViewModel, bool isDelayed)
	{
		if (isDelayed)
		{
			await Task.Delay(750);
		}
		_audioSource.PlayOneShot(StartLoanClip);
		_incomeTrackView.ShowLoanView(loanActionViewModel, _currentIncomePlayerToken, _targetIncomePlayerToken, _arrowToTargetPrefab, _controller);
		_panelCanvasGroup.DOFade(1f, UIProperties.AnimationDuration).OnComplete(delegate
		{
			_panelCanvasGroup.interactable = true;
			_panelCanvasGroup.blocksRaycasts = true;
		});
	}

	public void OnTakeLoanClicked()
	{
		_controller.OnTakeLoanClicked();
	}

	public IAnimation CreateLoanActionAnimation(LoanActionViewModel loanActionViewModel, float delay)
	{
		DOTweenAnimationSequence dOTweenAnimationSequence = new DOTweenAnimationSequence();
		dOTweenAnimationSequence.AppendCallback(delegate
		{
			_audioSource.PlayOneShot(TakeLoanClip);
		});
		IAnimation animation = _incomeTrackView.CreateLoanActionAnimation(_controller, loanActionViewModel, _buttonTakeLoan, delay);
		dOTweenAnimationSequence.Append(animation);
		return dOTweenAnimationSequence;
	}

	public void SetLoanButtonVisibility(bool isActive)
	{
		_buttonTakeLoan.gameObject.SetActive(isActive);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_panelCanvasGroup = base.transform.Find("SafeArea/Popup/Panel").GetComponent<CanvasGroup>();
		_incomeTrackView = base.transform.Find("SafeArea/Popup/Panel/IncomeTrackContainer/IncomeTrackPrefab").GetComponent<IncomeTrackView>();
		_buttonTakeLoan = base.transform.Find("SafeArea/Popup/Panel/TakeLoanButton").GetComponent<Button>();
		FadeOutAndDisableImmediately(_panelCanvasGroup);
		_audioSource = GetComponent<AudioSource>();
	}
}
