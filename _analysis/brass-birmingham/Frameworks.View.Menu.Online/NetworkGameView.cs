using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Game;
using BrassApplication.Game;
using InterfaceAdapters.Menu.OnlineGameMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.Online;

public class NetworkGameView : MonoBehaviour
{
	private TextMeshProUGUI _name;

	private NetworkGameViewModel _networkGameViewModel;

	private Transform _iconLocked;

	protected int _maxPlayersCount;

	private int _currentPlayersCount;

	protected List<Transform> _players;

	protected string _gameId;

	public virtual void Awake()
	{
		PrepareReferences();
	}

	public virtual void SetGame(NetworkGameViewModel networkGameViewModel)
	{
		_networkGameViewModel = networkGameViewModel;
		_name.text = networkGameViewModel.Name;
		_maxPlayersCount = networkGameViewModel.MaxPlayerCount;
		_currentPlayersCount = networkGameViewModel.CurrentPlayerCount;
		_iconLocked.gameObject.SetActive(networkGameViewModel.IsPrivate);
		_gameId = networkGameViewModel.GameId;
		SetPlayers(networkGameViewModel.PlayersStartConfigurations, networkGameViewModel.JoinedPlayersMetadata);
	}

	private void SetPlayers(IList<PlayerStartConfiguration> playersStartConfigurations, IList<PlayerMetadata> joinedPlayersMetadata)
	{
		joinedPlayersMetadata = joinedPlayersMetadata.Where((PlayerMetadata m) => m != null).ToList();
		playersStartConfigurations = playersStartConfigurations.Where((PlayerStartConfiguration p) => !joinedPlayersMetadata.Any((PlayerMetadata m) => m.PlayerStartConfiguration.Color == p.Color)).ToList();
		for (int num = 0; num < _players.Count; num++)
		{
			if (num < _maxPlayersCount)
			{
				if (num < _currentPlayersCount)
				{
					_players[num].GetChild(0).gameObject.SetActive(value: false);
					_players[num].GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Player/BbFramePanelPortrait" + joinedPlayersMetadata[num].PlayerAvatar);
					Color color = _players[num].GetChild(1).GetComponent<Image>().color;
					_players[num].GetChild(1).GetComponent<Image>().color = new Vector4(color.r, color.g, color.b, 1f);
					_players[num].GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("Player/BbFramePanelPlayer" + joinedPlayersMetadata[num].PlayerStartConfiguration.Color);
				}
				else
				{
					_players[num].GetChild(0).gameObject.SetActive(value: false);
					_players[num].GetChild(1).GetComponent<Image>().color *= (Color)new Vector4(1f, 1f, 1f, 0f);
					_players[num].GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("Player/BbFramePanelPlayer" + playersStartConfigurations[num - joinedPlayersMetadata.Count].Color);
				}
			}
			else
			{
				_players[num].gameObject.SetActive(value: false);
			}
		}
	}

	protected virtual void PrepareReferences()
	{
		_name = base.transform.Find("Name").GetComponent<TextMeshProUGUI>();
		_iconLocked = base.transform.Find("IconLockedSmall");
		Transform transform = base.transform.Find("Players");
		_players = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			_players.Add(transform.GetChild(i));
		}
	}
}
