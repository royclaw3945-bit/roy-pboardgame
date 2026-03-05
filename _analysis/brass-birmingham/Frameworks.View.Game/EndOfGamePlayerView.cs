using BoardGameRules.Entities.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class EndOfGamePlayerView : MonoBehaviour
{
	private Image _portraitFrame;

	private Image _portraitCharacter;

	private TextMeshProUGUI _playerNameText;

	private TextMeshProUGUI _sumText;

	private TextMeshProUGUI _linksText;

	private TextMeshProUGUI _industriesText;

	private TextMeshProUGUI _bonusesText;

	private TextMeshProUGUI _canalText;

	private TextMeshProUGUI _moneyText;

	private TextMeshProUGUI _incomeText;

	private TextMeshProUGUI _higherIndustriesText;

	private void Awake()
	{
		PrepareReferences();
	}

	public void UpdatePlayer(PlayerColor color, PlayerAvatar playerAvatar, string playerName, int canalEraVPs, int vpsForLinkInLastEra, int vpsForIndustriesInLastEra, int vpsForBonusesInLastEra, int totalVP)
	{
		PrepareReferences();
		_canalText.transform.parent.gameObject.SetActive(value: true);
		_moneyText.transform.parent.gameObject.SetActive(value: false);
		_incomeText.transform.parent.gameObject.SetActive(value: false);
		_higherIndustriesText.transform.parent.gameObject.SetActive(value: false);
		_portraitFrame.sprite = Resources.Load<Sprite>($"Player/BbFramePanelPlayer{color}");
		_portraitCharacter.sprite = Resources.Load<Sprite>($"Player/BbFramePanelPortrait{playerAvatar}");
		_playerNameText.text = playerName;
		_canalText.text = canalEraVPs.ToString();
		_linksText.text = vpsForLinkInLastEra.ToString();
		_industriesText.text = vpsForIndustriesInLastEra.ToString();
		_bonusesText.text = vpsForBonusesInLastEra.ToString();
		_sumText.text = totalVP.ToString();
	}

	public void UpdatePlayerIntroductory(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int vpFromMoney, int vpFromIncome, int vpFromHigherIndustries)
	{
		PrepareReferences();
		_canalText.transform.parent.gameObject.SetActive(value: false);
		_moneyText.transform.parent.gameObject.SetActive(value: true);
		_incomeText.transform.parent.gameObject.SetActive(value: true);
		_higherIndustriesText.transform.parent.gameObject.SetActive(value: true);
		_portraitFrame.sprite = Resources.Load<Sprite>($"Player/BbFramePanelPlayer{color}");
		_portraitCharacter.sprite = Resources.Load<Sprite>($"Player/BbFramePanelPortrait{avatar}");
		_playerNameText.text = playerName;
		_linksText.text = vpFromLinksInLastEra.ToString();
		_industriesText.text = vpFromIndustriesInLastEra.ToString();
		_bonusesText.text = vpFromBonusesInLastEra.ToString();
		_sumText.text = totalVP.ToString();
		_moneyText.text = vpFromMoney.ToString();
		_incomeText.text = vpFromIncome.ToString();
		_higherIndustriesText.text = vpFromHigherIndustries.ToString();
	}

	protected void PrepareReferences()
	{
		_playerNameText = base.transform.Find("VerticalLayout/Portrait/PlayerName").GetComponent<TextMeshProUGUI>();
		_sumText = base.transform.Find("VerticalLayout/Sum/SumText").GetComponent<TextMeshProUGUI>();
		_linksText = base.transform.Find("VerticalLayout/Links/LinksText").GetComponent<TextMeshProUGUI>();
		_industriesText = base.transform.Find("VerticalLayout/Industries/IndustriesText").GetComponent<TextMeshProUGUI>();
		_bonusesText = base.transform.Find("VerticalLayout/Bonuses/BonusesText").GetComponent<TextMeshProUGUI>();
		_canalText = base.transform.Find("VerticalLayout/CanalEra/CanalEraText").GetComponent<TextMeshProUGUI>();
		_moneyText = base.transform.Find("VerticalLayout/Money/MoneyText").GetComponent<TextMeshProUGUI>();
		_incomeText = base.transform.Find("VerticalLayout/Income/IncomeText").GetComponent<TextMeshProUGUI>();
		_higherIndustriesText = base.transform.Find("VerticalLayout/HigherIndustries/HigherIndustriesText").GetComponent<TextMeshProUGUI>();
		_portraitFrame = base.transform.Find("VerticalLayout/Portrait/PortraitFrame").GetComponent<Image>();
		_portraitCharacter = base.transform.Find("VerticalLayout/Portrait/PlayerPortrait").GetComponent<Image>();
	}
}
