using System;
using BoardGameRules.Entities.Players;
using DG.Tweening;
using Frameworks.Utils;
using InterfaceAdapters.Game.PlayersSummary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
	public PlayerColor PlayerColor;

	public bool IsSet;

	private TextMeshProUGUI _playerNameText;

	private TextMeshProUGUI _vpText;

	private TextMeshProUGUI _currentMoneyText;

	private TextMeshProUGUI _incomeText;

	private TextMeshProUGUI _incomeProgressText;

	private TextMeshProUGUI _moneySpentText;

	private TextMeshProUGUI _victoryPointsForMoneyText;

	private TextMeshProUGUI _victoryPointsForIncomeText;

	private Image _avatarImage;

	private Image _avatarColor;

	private Action<string, float, float> _actionOnHover;

	private Action _actionOnHoverExit;

	private AudioSource _audioSource;

	public virtual void Awake()
	{
		PrepareReferences();
	}

	public virtual void SetPlayer(PlayerViewModel playerViewModel)
	{
		PlayerColor = playerViewModel.Color;
		IsSet = true;
		_playerNameText.text = playerViewModel.Name;
		_vpText.text = playerViewModel.VP.ToString();
		_currentMoneyText.text = playerViewModel.Money.ToString();
		_incomeText.text = ((playerViewModel.IncomeLevel > 0) ? "+" : "") + playerViewModel.IncomeLevel;
		_incomeProgressText.text = $"{playerViewModel.IncomeMarkerPosition}/{playerViewModel.IncomeThreshold}";
		_moneySpentText.text = playerViewModel.MoneySpent.ToString();
		_avatarColor.sprite = Resources.Load<Sprite>("Player/BbFramePanelPlayer" + playerViewModel.Color);
		_avatarImage.sprite = Resources.Load<Sprite>("Player/BbFramePanelPortrait" + playerViewModel.Avatar);
		_actionOnHover = playerViewModel.ActionOnHover;
		_actionOnHoverExit = playerViewModel.ActionOnHoverExit;
	}

	public Sequence CreateChangeVPAnimation(string newAmount, AudioClip clipToPlay)
	{
		if (newAmount != _vpText.text)
		{
			return CreateChangeValueAnimation(newAmount, _vpText, clipToPlay);
		}
		return DOTween.Sequence();
	}

	public Sequence CreateChangeMoneyAnimation(string newAmount, AudioClip clipToPlay)
	{
		return CreateChangeValueAnimation(newAmount, _currentMoneyText, clipToPlay);
	}

	public Sequence CreateChangeMoneySpentAnimation(string newAmount, AudioClip clipToPlay)
	{
		return CreateChangeValueAnimation(newAmount, _moneySpentText, clipToPlay);
	}

	public Sequence CreateChangeIncomeAnimation(string newAmount, AudioClip clipToPlay)
	{
		return CreateChangeValueAnimation(newAmount, _incomeText, clipToPlay);
	}

	public Sequence CreateChangeIncomeMarkerAnimation(string newAmount, AudioClip clipToPlay)
	{
		return CreateChangeValueAnimation(newAmount, _incomeProgressText, clipToPlay);
	}

	private Sequence CreateChangeValueAnimation(string newAmount, TextMeshProUGUI valueToChange, AudioClip clipToPlay)
	{
		Sequence sequence = DOTween.Sequence();
		Transform parent = valueToChange.transform.parent;
		RectTransform parentRT = parent.GetComponent<RectTransform>();
		Vector2 startingPos = parentRT.anchoredPosition;
		Vector3 startingScale = parent.localScale;
		sequence.AppendCallback(delegate
		{
			if (clipToPlay != null)
			{
				_audioSource.PlayOneShot(clipToPlay);
			}
			valueToChange.text = "99";
			parent.localScale = new Vector3(startingScale.x, startingScale.y * 0.5f, startingScale.z);
			parent.Find("Blur").gameObject.SetActive(value: true);
			parentRT.anchoredPosition += new Vector2(0f, 5f);
		});
		sequence.Append(parentRT.DOBlendableMoveBy(new Vector3(0f, -10f, 0f), 2f * UIProperties.FrameDuration));
		sequence.Append(parentRT.DOBlendableMoveBy(new Vector3(0f, 10f, 0f), 2f * UIProperties.FrameDuration));
		sequence.Append(parentRT.DOBlendableMoveBy(new Vector3(0f, -10f, 0f), 2f * UIProperties.FrameDuration));
		sequence.Append(parentRT.DOBlendableMoveBy(new Vector3(0f, 10f, 0f), 2f * UIProperties.FrameDuration));
		sequence.Append(parentRT.DOBlendableMoveBy(new Vector3(0f, -10f, 0f), 2f * UIProperties.FrameDuration));
		sequence.Append(parentRT.DOBlendableMoveBy(new Vector3(0f, 10f, 0f), 2f * UIProperties.FrameDuration));
		sequence.Append(parentRT.DOBlendableMoveBy(new Vector3(0f, -10f, 0f), 2f * UIProperties.FrameDuration));
		sequence.Append(parentRT.DOBlendableMoveBy(new Vector3(0f, 5f, 0f), 2f * UIProperties.FrameDuration));
		sequence.AppendCallback(delegate
		{
			valueToChange.text = newAmount;
			parent.localScale = new Vector3(startingScale.x, startingScale.y, startingScale.z);
			parent.Find("Blur").gameObject.SetActive(value: false);
			parentRT.anchoredPosition = startingPos;
		});
		sequence.OnKill(delegate
		{
			valueToChange.text = newAmount;
			parent.localScale = new Vector3(startingScale.x, startingScale.y, startingScale.z);
			parent.Find("Blur").gameObject.SetActive(value: false);
			parentRT.anchoredPosition = startingPos;
		});
		return sequence;
	}

	public Sequence CreateVictoryPointsForMoneyAnimation(int amount)
	{
		Sequence sequence = DOTween.Sequence();
		_victoryPointsForMoneyText.text = _victoryPointsForMoneyText.text.Replace("1", amount.ToString());
		sequence.Append(_victoryPointsForMoneyText.DOFade(1f, 0.5f));
		sequence.AppendInterval(0.5f);
		sequence.Append(_victoryPointsForMoneyText.DOScale(2f, 1f)).Join(_victoryPointsForMoneyText.DOFade(0f, 1f));
		sequence.OnKill(delegate
		{
			_victoryPointsForMoneyText.text = _victoryPointsForMoneyText.text.Replace(amount.ToString(), "1");
			_victoryPointsForMoneyText.transform.localScale = Vector3.one;
			_victoryPointsForMoneyText.color = new Color(_victoryPointsForMoneyText.color.r, _victoryPointsForMoneyText.color.g, _victoryPointsForMoneyText.color.b, 0f);
		});
		return sequence;
	}

	public Sequence CreateVictoryPointsForIncomeAnimation(int amount)
	{
		Sequence sequence = DOTween.Sequence();
		_victoryPointsForIncomeText.text = _victoryPointsForIncomeText.text.Replace("1", amount.ToString());
		sequence.Append(_victoryPointsForIncomeText.DOFade(1f, 0.5f));
		sequence.AppendInterval(0.5f);
		sequence.Append(_victoryPointsForIncomeText.DOScale(2f, 1f)).Join(_victoryPointsForIncomeText.DOFade(0f, 1f));
		sequence.OnKill(delegate
		{
			_victoryPointsForIncomeText.text = _victoryPointsForIncomeText.text.Replace(amount.ToString(), "1");
			_victoryPointsForIncomeText.transform.localScale = Vector3.one;
			_victoryPointsForIncomeText.color = new Color(_victoryPointsForIncomeText.color.r, _victoryPointsForIncomeText.color.g, _victoryPointsForIncomeText.color.b, 0f);
		});
		return sequence;
	}

	public void OnMouseEnterVP()
	{
		_actionOnHover?.Invoke("VP", base.transform.position.x + (float)(Screen.width / 15), base.transform.position.y);
	}

	public void OnMouseEnterCurrentMoney()
	{
		_actionOnHover?.Invoke("CurrentMoney", base.transform.position.x + (float)(Screen.width / 15), base.transform.position.y);
	}

	public void OnMouseEnterIncome()
	{
		_actionOnHover?.Invoke("Income", base.transform.position.x + (float)(Screen.width / 15), base.transform.position.y);
	}

	public void OnMouseEnterIncomeProgress()
	{
		_actionOnHover?.Invoke("IncomeProgress", base.transform.position.x + (float)(Screen.width / 15), base.transform.position.y);
	}

	public void OnMouseEnterMoneySpent()
	{
		_actionOnHover?.Invoke("MoneySpent", base.transform.position.x + (float)(Screen.width / 15), base.transform.position.y);
	}

	public void OnMouseExit()
	{
		_actionOnHoverExit?.Invoke();
	}

	protected virtual void PrepareReferences()
	{
		_playerNameText = base.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
		_vpText = base.transform.Find("VP/VPText").GetComponent<TextMeshProUGUI>();
		_currentMoneyText = base.transform.Find("CurrentMoney/CurrentMoneyText").GetComponent<TextMeshProUGUI>();
		_victoryPointsForMoneyText = base.transform.Find("CurrentMoney/VictoryPointsForMoney").GetComponent<TextMeshProUGUI>();
		_incomeText = base.transform.Find("Income/IncomeText").GetComponent<TextMeshProUGUI>();
		_victoryPointsForIncomeText = base.transform.Find("Income/VictoryPointsForIncome").GetComponent<TextMeshProUGUI>();
		_incomeProgressText = base.transform.Find("IncomeProgress/IncomeProgressText").GetComponent<TextMeshProUGUI>();
		_moneySpentText = base.transform.Find("MoneySpent/IconBlank/PropBlank/MoneySpentText").GetComponent<TextMeshProUGUI>();
		_avatarImage = base.transform.Find("PlayerPortrait").GetComponent<Image>();
		_avatarColor = base.transform.Find("PlayerColor").GetComponent<Image>();
		_audioSource = GetComponent<AudioSource>();
	}
}
