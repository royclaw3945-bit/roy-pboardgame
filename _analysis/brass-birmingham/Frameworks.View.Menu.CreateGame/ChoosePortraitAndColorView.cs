using System;
using BoardGameRules.Entities.Players;
using DG.Tweening;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.CreateGame;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.CreateGame;

public class ChoosePortraitAndColorView : BaseView, IChoosePortraitAndColorView, IBaseView
{
	private ChoosePortraitAndColorController _controller;

	private PlayerColor _playerColor;

	private PlayerAvatar _playerAvatar;

	private Image _colorFrame;

	private Image _portrait;

	private TextMeshProUGUI _characterName;

	private bool _isOnlineGame;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is ChoosePortraitAndColorController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateView(PlayerColor playerColor, PlayerAvatar playerAvatar, bool isOnlineGame = false)
	{
		UpdateColor(playerColor);
		UpdateAvatar(playerAvatar);
		_isOnlineGame = isOnlineGame;
	}

	public void UpdateColor(PlayerColor playerColor)
	{
		_playerColor = playerColor;
		_colorFrame.sprite = Resources.Load<Sprite>("Player/BigPortraits/BbFramePortrait" + playerColor);
	}

	public void UpdateAvatar(PlayerAvatar playerAvatar)
	{
		_playerAvatar = playerAvatar;
		_portrait.sprite = Resources.Load<Sprite>("Player/BigPortraits/BbPortrait_" + playerAvatar);
		_characterName.text = Enum.GetName(typeof(PlayerAvatarFullName), (int)playerAvatar).Replace("_", " ");
	}

	public void OnBackButtonClicked()
	{
		_controller.BackClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackButtonClicked();
		}
	}

	public void OnNextColorClicked()
	{
		_controller.OnNextColorClicked(_playerColor, _isOnlineGame);
	}

	public void OnPreviousColorClicked()
	{
		_controller.OnPreviousColorClicked(_playerColor, _isOnlineGame);
	}

	public void OnNextPortraitClicked()
	{
		_controller.OnNextPortraitClicked(_playerAvatar, _isOnlineGame);
	}

	public void OnPreviousPortraitClicked()
	{
		_controller.OnPreviousPortraitClicked(_playerAvatar, _isOnlineGame);
	}

	public override IAnimation CreateHideViewAnimation()
	{
		_isViewVisible = false;
		RectTransform component = Popup.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = Popup.GetComponent<CanvasGroup>();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			canvasGroup.blocksRaycasts = false;
		});
		sequence.Append(component.DOAnchorPos(Vector2.zero, 0.3f));
		sequence.Insert(0f, canvasGroup.DOFade(0f, 0.3f));
		sequence.AppendCallback(delegate
		{
			canvasGroup.interactable = false;
		});
		sequence.AppendCallback(delegate
		{
			HideViewCompleted();
		});
		if (Background != null)
		{
			sequence.Insert(0f, Background.DOFade(0f, 0.3f));
		}
		return new Frameworks.View.Animation.DOTweenAnimation(sequence);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_colorFrame = base.transform.Find("SafeArea/Popup/PortraitContainer/PlayerColor").GetComponent<Image>();
		_portrait = base.transform.Find("SafeArea/Popup/PortraitContainer/Portrait").GetComponent<Image>();
		_characterName = base.transform.Find("SafeArea/Popup/PortraitContainer/Banner/CharacterNameText").GetComponent<TextMeshProUGUI>();
	}
}
