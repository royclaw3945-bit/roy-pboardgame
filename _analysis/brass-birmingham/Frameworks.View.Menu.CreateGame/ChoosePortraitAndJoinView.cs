using System;
using BoardGameRules.Entities.Players;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.ChoosePortraitAndJoin;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.CreateGame;

public class ChoosePortraitAndJoinView : BaseView, IChoosePortraitAndJoinView, IBaseView
{
	private ChoosePortraitAndJoinController _controller;

	private Image _colorFrame;

	private Image _portrait;

	private TextMeshProUGUI _characterName;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is ChoosePortraitAndJoinController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateView(PlayerColor playerColor, PlayerAvatar playerAvatar)
	{
		UpdateColor(playerColor);
		UpdateAvatar(playerAvatar);
	}

	public void UpdateColor(PlayerColor playerColor)
	{
		_colorFrame.sprite = Resources.Load<Sprite>("Player/BigPortraits/BbFramePortrait" + playerColor);
	}

	public void UpdateAvatar(PlayerAvatar playerAvatar)
	{
		_portrait.sprite = Resources.Load<Sprite>("Player/BigPortraits/BbPortrait_" + playerAvatar);
		_characterName.text = Enum.GetName(typeof(PlayerAvatarFullName), (int)playerAvatar).Replace("_", " ");
	}

	public void OnBackButtonClicked()
	{
		_controller.BackClicked();
	}

	public void OnNextPortraitClicked()
	{
		_controller.OnNextPortraitClicked();
	}

	public void OnPreviousPortraitClicked()
	{
		_controller.OnPreviousPortraitClicked();
	}

	public void OnJoinClicked()
	{
		_controller.OnJoinClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackButtonClicked();
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_colorFrame = base.transform.Find("SafeArea/Popup/PortraitContainer/PlayerColor").GetComponent<Image>();
		_portrait = base.transform.Find("SafeArea/Popup/PortraitContainer/Portrait").GetComponent<Image>();
		_characterName = base.transform.Find("SafeArea/Popup/PortraitContainer/Banner/CharacterNameText").GetComponent<TextMeshProUGUI>();
	}
}
