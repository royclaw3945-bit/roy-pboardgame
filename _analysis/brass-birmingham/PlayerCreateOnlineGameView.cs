using System;
using BoardGameRules.Entities.Players;
using InterfaceAdapters.Menu.Online.CreateOnlineGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCreateOnlineGameView : MonoBehaviour
{
	private CanvasGroup _playerContainer;

	private PlayerColor _playerColor;

	private PlayerAvatar _playerAvatar;

	private TMP_InputField _nicknameText;

	private TextMeshProUGUI _playerAITypeText;

	private Image _playerPortrait;

	private Image _playerColorCircle;

	private Button _addPlayerButton;

	private Button _removePlayerButton;

	private Button _editPortaitAndColorButton;

	private Button _joinGameButton;

	private Action<PlayerColor, PlayerAvatar> _onEditPortraitAndColorAction = delegate
	{
	};

	private Action _onAddPlayerClickedAction = delegate
	{
	};

	private Action _onRemovePlayerClickedAction = delegate
	{
	};

	private int _slotNumber;

	public void Awake()
	{
		PrepareReferences();
	}

	public void SetPlayer(PlayerOnlineGameViewModel viewModel)
	{
		_removePlayerButton.gameObject.SetActive(viewModel.CurrentPlayerCount > 2 && viewModel.PlayerNumber == viewModel.CurrentPlayerCount);
		_addPlayerButton.gameObject.SetActive(viewModel.CurrentPlayerCount < 4 && viewModel.PlayerNumber == viewModel.CurrentPlayerCount + 1);
		_onAddPlayerClickedAction = viewModel.OnAddPlayerClickedAction;
		_onRemovePlayerClickedAction = viewModel.OnRemovePlayerClickedAction;
		_nicknameText.interactable = false;
		if (viewModel.PlayerNumber == 1)
		{
			_playerAvatar = viewModel.PlayerAvatar;
			_editPortaitAndColorButton.gameObject.SetActive(value: true);
			_playerPortrait.gameObject.SetActive(value: true);
			_playerPortrait.sprite = Resources.Load<Sprite>("Player/BbFramePanelPortrait" + _playerAvatar);
		}
		else
		{
			_editPortaitAndColorButton.gameObject.SetActive(value: false);
			_playerPortrait.gameObject.SetActive(value: false);
		}
		if (viewModel.PlayerNumber > viewModel.CurrentPlayerCount)
		{
			_playerContainer.gameObject.SetActive(value: false);
			return;
		}
		_playerContainer.gameObject.SetActive(value: true);
		_nicknameText.text = viewModel.Nickname;
		_playerAITypeText.text = viewModel.AiTypeText;
		_playerColor = viewModel.PlayerColor;
		_playerColorCircle.sprite = Resources.Load<Sprite>("Player/BbFramePanelPlayer" + _playerColor);
		_onEditPortraitAndColorAction = viewModel.OnEditPortraitAndColorAction;
	}

	public void SetJoinAction(Action<int> onJoinGameClicked)
	{
		_joinGameButton.onClick.AddListener(delegate
		{
			onJoinGameClicked(_slotNumber);
		});
	}

	public void SetPlayerForJoin(PlayerOnlineGameViewModel viewModel)
	{
		_removePlayerButton.gameObject.SetActive(value: false);
		_addPlayerButton.gameObject.SetActive(value: false);
		_onAddPlayerClickedAction = viewModel.OnAddPlayerClickedAction;
		_onRemovePlayerClickedAction = viewModel.OnRemovePlayerClickedAction;
		_editPortaitAndColorButton.gameObject.SetActive(value: false);
		_nicknameText.interactable = false;
		_slotNumber = viewModel.PlayerNumber;
		if (viewModel.HasJoined)
		{
			_playerAvatar = viewModel.PlayerAvatar;
			_playerPortrait.gameObject.SetActive(value: true);
			_playerPortrait.sprite = Resources.Load<Sprite>("Player/BbFramePanelPortrait" + _playerAvatar);
			_joinGameButton.gameObject.SetActive(value: false);
			_nicknameText.text = viewModel.Nickname;
		}
		else
		{
			_playerPortrait.gameObject.SetActive(value: false);
			_joinGameButton.gameObject.SetActive(value: true);
			_nicknameText.gameObject.SetActive(value: false);
		}
		if (viewModel.PlayerNumber > viewModel.CurrentPlayerCount)
		{
			_playerContainer.gameObject.SetActive(value: false);
			return;
		}
		_playerContainer.gameObject.SetActive(value: true);
		_playerAITypeText.text = viewModel.AiTypeText;
		_playerColor = viewModel.PlayerColor;
		_playerColorCircle.sprite = Resources.Load<Sprite>("Player/BbFramePanelPlayer" + _playerColor);
		_onEditPortraitAndColorAction = viewModel.OnEditPortraitAndColorAction;
	}

	public void OnEditPortraitAndColorClick()
	{
		_onEditPortraitAndColorAction(_playerColor, _playerAvatar);
	}

	public void OnAddPlayerClick()
	{
		_onAddPlayerClickedAction();
	}

	public void OnRemovePlayerClick()
	{
		_onRemovePlayerClickedAction();
	}

	private void PrepareReferences()
	{
		_nicknameText = base.transform.Find("PlayerContainer/PlayerNameAndType/PlayerName").GetComponent<TMP_InputField>();
		_playerAITypeText = base.transform.Find("PlayerContainer/PlayerNameAndType/PlayerAIType").GetComponent<TextMeshProUGUI>();
		_playerPortrait = base.transform.Find("PlayerContainer/PlayerPortrait").GetComponent<Image>();
		_playerColorCircle = base.transform.Find("PlayerContainer/PlayerColor").GetComponent<Image>();
		_addPlayerButton = base.transform.Find("ButtonPlus").GetComponent<Button>();
		_removePlayerButton = base.transform.Find("ButtonMinus").GetComponent<Button>();
		_editPortaitAndColorButton = base.transform.Find("PlayerContainer/IconEdit").GetComponent<Button>();
		_joinGameButton = base.transform.Find("PlayerContainer/PlayerNameAndType/Button Join").GetComponent<Button>();
		_playerContainer = base.transform.Find("PlayerContainer").GetComponent<CanvasGroup>();
	}
}
