using InterfaceAdapters.Game.EndOfEra;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class EndOfEraPlayerView : MonoBehaviour
{
	private Image _portraitFrame;

	private Image _portraitCharacter;

	private TextMeshProUGUI _playerNameText;

	private TextMeshProUGUI _sumText;

	private TextMeshProUGUI _linksText;

	private TextMeshProUGUI _industriesText;

	private TextMeshProUGUI _bonusesText;

	private void Awake()
	{
		PrepareReferences();
	}

	public void UpdatePlayer(EndOfEraPlayerViewModel viewModel)
	{
		PrepareReferences();
		_portraitFrame.sprite = Resources.Load<Sprite>($"Player/BbFramePanelPlayer{viewModel.PlayerColor}");
		_portraitCharacter.sprite = Resources.Load<Sprite>($"Player/BbFramePanelPortrait{viewModel.PlayerAvatar}");
		_playerNameText.text = viewModel.PlayerName;
		_linksText.text = viewModel.LinksVP.ToString();
		_industriesText.text = viewModel.IndustriesVP.ToString();
		_sumText.text = viewModel.SumVP.ToString();
		_bonusesText.text = viewModel.BonusesVP.ToString();
	}

	protected void PrepareReferences()
	{
		_playerNameText = base.transform.Find("VerticalLayout/Portrait/PlayerName").GetComponent<TextMeshProUGUI>();
		_sumText = base.transform.Find("VerticalLayout/Sum/SumText").GetComponent<TextMeshProUGUI>();
		_linksText = base.transform.Find("VerticalLayout/Links/LinksText").GetComponent<TextMeshProUGUI>();
		_industriesText = base.transform.Find("VerticalLayout/Industries/IndustriesText").GetComponent<TextMeshProUGUI>();
		_bonusesText = base.transform.Find("VerticalLayout/Bonuses/BonusesText").GetComponent<TextMeshProUGUI>();
		_portraitFrame = base.transform.Find("VerticalLayout/Portrait/PortraitFrame").GetComponent<Image>();
		_portraitCharacter = base.transform.Find("VerticalLayout/Portrait/PlayerPortrait").GetComponent<Image>();
	}
}
