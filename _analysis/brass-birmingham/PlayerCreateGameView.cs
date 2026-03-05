using System;
using BoardGameRules.Entities.Players;
using BrassApplication.AI;
using BrassApplication.Game;
using InterfaceAdapters.Common.CreateGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCreateGameView : MonoBehaviour
{
	private CanvasGroup _playerContainer;

	private PlayerMetadata _playerMetadata;

	private TMP_InputField _nicknameText;

	private TextMeshProUGUI _playerAITypeText;

	private TextMeshProUGUI _playerAIDifficultyText;

	private Image _playerPortrait;

	private Image _playerColorCircle;

	private Button _addPlayerButton;

	private Button _removePlayerButton;

	private Action<PlayerMetadata> _onNextDifficultyClickedAction = delegate
	{
	};

	private Action<PlayerMetadata> _onPreviousDifficultyClickedAction = delegate
	{
	};

	private Action<PlayerMetadata> _onEditPortraitAndColorAction = delegate
	{
	};

	private Action<PlayerColor, string> _onPlayerNameChangedAction = delegate
	{
	};

	private Action _onAddPlayerClickedAction = delegate
	{
	};

	private Action _onRemovePlayerClickedAction = delegate
	{
	};

	public void Awake()
	{
		PrepareReferences();
	}

	public void SetPlayer(PlayerCreateGameViewModel viewModel)
	{
		if (viewModel.Editable)
		{
			_removePlayerButton.gameObject.SetActive(viewModel.CurrentPlayerCount > 2 && viewModel.PlayerNumber == viewModel.CurrentPlayerCount);
			_addPlayerButton.gameObject.SetActive(viewModel.CurrentPlayerCount < 4 && viewModel.PlayerNumber == viewModel.CurrentPlayerCount + 1);
			_onAddPlayerClickedAction = viewModel.OnAddPlayerClickedAction;
			_onRemovePlayerClickedAction = viewModel.OnRemovePlayerClickedAction;
			_nicknameText.interactable = viewModel.AiType == AIType.Human;
		}
		else
		{
			_removePlayerButton.gameObject.SetActive(value: false);
			_addPlayerButton.gameObject.SetActive(value: false);
			_nicknameText.interactable = false;
		}
		if (viewModel.PlayerNumber > viewModel.CurrentPlayerCount)
		{
			_playerContainer.gameObject.SetActive(value: false);
			return;
		}
		_playerContainer.gameObject.SetActive(value: true);
		_playerMetadata = viewModel.PlayerMetadata;
		_nicknameText.text = viewModel.Nickname;
		_playerAIDifficultyText.text = viewModel.AiTypeText;
		if (viewModel.AiType != AIType.Human)
		{
			_playerAITypeText.text = "AI";
			_playerAIDifficultyText.gameObject.SetActive(value: true);
			switch (viewModel.AiType)
			{
			case AIType.Easy:
				_playerAIDifficultyText.color = new Color(0.72156864f, 0.9254902f, 0.5882353f);
				break;
			case AIType.Medium:
				_playerAIDifficultyText.color = new Color(1f, 52f / 85f, 0.18431373f);
				break;
			case AIType.Hard:
				_playerAIDifficultyText.color = new Color(1f, 19f / 85f, 16f / 51f);
				break;
			}
		}
		else
		{
			_playerAITypeText.text = viewModel.AiTypeText;
			_playerAIDifficultyText.gameObject.SetActive(value: false);
		}
		_playerPortrait.sprite = Resources.Load<Sprite>("Player/BbFramePanelPortrait" + viewModel.PlayerMetadata.PlayerAvatar);
		_playerColorCircle.sprite = Resources.Load<Sprite>("Player/BbFramePanelPlayer" + viewModel.PlayerColor);
		_onNextDifficultyClickedAction = viewModel.OnNextDifficultyClickedAction;
		_onPreviousDifficultyClickedAction = viewModel.OnPreviousDifficultyClickedAction;
		_onEditPortraitAndColorAction = viewModel.OnEditPortraitAndColorAction;
		_onPlayerNameChangedAction = viewModel.OnPlayerNameChangedAction;
	}

	private void PrepareReferences()
	{
		_nicknameText = base.transform.Find("PlayerContainer/PlayerName").GetComponent<TMP_InputField>();
		_playerAITypeText = base.transform.Find("PlayerContainer/PlayerAIType").GetComponent<TextMeshProUGUI>();
		_playerAIDifficultyText = base.transform.Find("PlayerContainer/PlayerAIDifficulty").GetComponent<TextMeshProUGUI>();
		_playerPortrait = base.transform.Find("PlayerContainer/PlayerPortrait").GetComponent<Image>();
		_playerColorCircle = base.transform.Find("PlayerContainer/PlayerColor").GetComponent<Image>();
		_addPlayerButton = base.transform.Find("ButtonPlus").GetComponent<Button>();
		_removePlayerButton = base.transform.Find("ButtonMinus").GetComponent<Button>();
		_playerContainer = base.transform.Find("PlayerContainer").GetComponent<CanvasGroup>();
	}

	public void OnNextDifficultyClick()
	{
		_onNextDifficultyClickedAction(_playerMetadata);
	}

	public void OnPreviousDifficultyClick()
	{
		_onPreviousDifficultyClickedAction(_playerMetadata);
	}

	public void OnEditPortraitAndColorClick()
	{
		_onEditPortraitAndColorAction(_playerMetadata);
	}

	public void OnAddPlayerClick()
	{
		_onAddPlayerClickedAction();
	}

	public void OnRemovePlayerClick()
	{
		_onRemovePlayerClickedAction();
	}

	public void OnPlayerNameChanged()
	{
		_onPlayerNameChangedAction(_playerMetadata.PlayerStartConfiguration.Color, _nicknameText.text);
	}
}
