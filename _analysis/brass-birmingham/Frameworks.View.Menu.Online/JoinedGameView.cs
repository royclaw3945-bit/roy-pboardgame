using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using I2.Loc;
using InterfaceAdapters.Menu.OnlineGameMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.Online;

public class JoinedGameView : NetworkGameView
{
	private TextMeshProUGUI _abandonedText;

	private TextMeshProUGUI _abandonedTimeoutText;

	private TextMeshProUGUI _roundText;

	private TextMeshProUGUI _eraText;

	private TextMeshProUGUI _lastMoveInfoText;

	private Transform _yourTurnText;

	private Transform _waitingText;

	private Button _openButton;

	private Button _exitButton;

	private Action<string> _launchGameAction = delegate
	{
	};

	private Action<string> _leaveGameAction = delegate
	{
	};

	public override void SetGame(NetworkGameViewModel networkGameViewModel)
	{
		base.SetGame(networkGameViewModel);
		_launchGameAction = networkGameViewModel.LaunchGameAction;
		_leaveGameAction = networkGameViewModel.LeaveGameAction;
		int num = networkGameViewModel.TimeLeft / 60;
		string text = "m";
		if (num >= 60)
		{
			num /= 60;
			text = "h";
		}
		if (num >= 24)
		{
			num /= 24;
			text = "d";
		}
		switch (networkGameViewModel.GameStatus)
		{
		case NetworkGameProgressInformation.GameStatus.AbortedTimeout:
			_openButton.gameObject.SetActive(value: false);
			_exitButton.gameObject.SetActive(value: false);
			_waitingText.gameObject.SetActive(value: false);
			_abandonedTimeoutText.gameObject.SetActive(value: true);
			_abandonedText.gameObject.SetActive(value: false);
			_yourTurnText.gameObject.SetActive(value: false);
			_lastMoveInfoText.gameObject.SetActive(value: true);
			_lastMoveInfoText.text = networkGameViewModel.TimeLeftText + " " + num + text;
			_roundText.gameObject.SetActive(value: true);
			_roundText.text = networkGameViewModel.Round;
			_eraText.gameObject.SetActive(value: true);
			_eraText.text = networkGameViewModel.Era;
			if (networkGameViewModel.CurrentPlayerColor.HasValue)
			{
				SetActivePlayer(networkGameViewModel.PlayersStartConfigurations, networkGameViewModel.CurrentPlayerColor.Value);
				_abandonedTimeoutText.GetComponent<Localize>().enabled = false;
				_abandonedTimeoutText.text = _abandonedTimeoutText.text.Replace("<nickname>", networkGameViewModel.JoinedPlayersMetadata.First((PlayerMetadata player) => player.PlayerStartConfiguration.Color == networkGameViewModel.CurrentPlayerColor.Value).NickName);
			}
			break;
		case NetworkGameProgressInformation.GameStatus.AbortedPlayerLeft:
			_openButton.gameObject.SetActive(value: false);
			_exitButton.gameObject.SetActive(value: false);
			_waitingText.gameObject.SetActive(value: false);
			_abandonedText.gameObject.SetActive(value: true);
			_abandonedTimeoutText.gameObject.SetActive(value: false);
			_yourTurnText.gameObject.SetActive(value: false);
			_lastMoveInfoText.gameObject.SetActive(value: true);
			_lastMoveInfoText.text = networkGameViewModel.TimeLeftText + " " + num + text;
			_roundText.gameObject.SetActive(value: true);
			_roundText.text = networkGameViewModel.Round;
			_eraText.gameObject.SetActive(value: true);
			_eraText.text = networkGameViewModel.Era;
			if (networkGameViewModel.CurrentPlayerColor.HasValue)
			{
				SetActivePlayer(networkGameViewModel.PlayersStartConfigurations, networkGameViewModel.CurrentPlayerColor.Value);
			}
			break;
		case NetworkGameProgressInformation.GameStatus.InProgress:
			_openButton.gameObject.SetActive(value: true);
			_exitButton.gameObject.SetActive(value: false);
			_waitingText.gameObject.SetActive(value: false);
			_abandonedText.gameObject.SetActive(value: false);
			_abandonedTimeoutText.gameObject.SetActive(value: false);
			_yourTurnText.gameObject.SetActive(networkGameViewModel.IsLoggedPlayerTurn);
			_lastMoveInfoText.gameObject.SetActive(value: true);
			_lastMoveInfoText.text = networkGameViewModel.TimeLeftText + " " + num + text;
			_roundText.gameObject.SetActive(value: true);
			_roundText.text = networkGameViewModel.Round;
			_eraText.gameObject.SetActive(value: true);
			_eraText.text = networkGameViewModel.Era;
			if (networkGameViewModel.CurrentPlayerColor.HasValue)
			{
				SetActivePlayer(networkGameViewModel.PlayersStartConfigurations, networkGameViewModel.CurrentPlayerColor.Value);
			}
			break;
		case NetworkGameProgressInformation.GameStatus.Open:
			_openButton.gameObject.SetActive(value: false);
			_exitButton.gameObject.SetActive(value: true);
			_waitingText.gameObject.SetActive(value: true);
			_abandonedText.gameObject.SetActive(value: false);
			_abandonedTimeoutText.gameObject.SetActive(value: false);
			_yourTurnText.gameObject.SetActive(value: false);
			_lastMoveInfoText.gameObject.SetActive(value: false);
			_roundText.gameObject.SetActive(value: false);
			_eraText.gameObject.SetActive(value: false);
			break;
		case NetworkGameProgressInformation.GameStatus.Finished:
			break;
		}
	}

	public void SetActivePlayer(IList<PlayerStartConfiguration> playersStartConfigurations, PlayerColor currentPlayerColor)
	{
		for (int i = 0; i < _maxPlayersCount; i++)
		{
			if (playersStartConfigurations[i].Color == currentPlayerColor)
			{
				_players[i].GetChild(0).gameObject.SetActive(value: true);
			}
			else
			{
				_players[i].GetChild(0).gameObject.SetActive(value: false);
			}
		}
	}

	public void LaunchGame()
	{
		_launchGameAction(_gameId);
	}

	public void LeaveGame()
	{
		_leaveGameAction(_gameId);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_yourTurnText = base.transform.Find("YourTurnText");
		_waitingText = base.transform.Find("WaitingText");
		_abandonedText = base.transform.Find("AbandonedText").GetComponent<TextMeshProUGUI>();
		_abandonedTimeoutText = base.transform.Find("AbandonedTimeoutText").GetComponent<TextMeshProUGUI>();
		_roundText = base.transform.Find("TurnText").GetComponent<TextMeshProUGUI>();
		_eraText = base.transform.Find("EraText").GetComponent<TextMeshProUGUI>();
		_lastMoveInfoText = base.transform.Find("LastMoveInfoText").GetComponent<TextMeshProUGUI>();
		_openButton = base.transform.Find("OpenButton").GetComponent<Button>();
		_exitButton = base.transform.Find("ExitButton").GetComponent<Button>();
	}
}
