using System.Collections.Generic;
using InterfaceAdapters.Menu.Online;
using InterfaceAdapters.Menu.OnlineGameMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.Online;

public class GamesListView : MonoBehaviour, IGamesListView
{
	[SerializeField]
	private JoinableGameView _joinableGamePrefab;

	[SerializeField]
	private JoinedGameView _joinedGamePrefab;

	private Transform _content;

	private ScrollRect _scrollRect;

	private void Awake()
	{
		PrepareReferences();
	}

	public void UpdateOpenGamesList(List<NetworkGameViewModel> gamesList)
	{
		UpdateList(gamesList, _joinableGamePrefab);
	}

	public void UpdatePlayerGamesList(List<NetworkGameViewModel> gamesList)
	{
		UpdateList(gamesList, _joinedGamePrefab);
	}

	private void UpdateList(List<NetworkGameViewModel> gamesList, NetworkGameView gamePrefab)
	{
		RemoveAllGames();
		GenerateGames(gamesList, gamePrefab);
		_scrollRect.verticalNormalizedPosition = 0f;
	}

	private void GenerateGames(List<NetworkGameViewModel> gamesList, NetworkGameView gamePrefab)
	{
		_content.GetChild(0).gameObject.SetActive(value: false);
		foreach (NetworkGameViewModel games in gamesList)
		{
			NetworkGameView networkGameView = Object.Instantiate(gamePrefab, _content);
			networkGameView.Awake();
			networkGameView.SetGame(games);
		}
	}

	public void RemoveAllGames()
	{
		_content.GetChild(0).gameObject.SetActive(value: true);
		for (int num = _content.childCount - 1; num >= 1; num--)
		{
			Object.Destroy(_content.GetChild(num).gameObject);
		}
	}

	private void PrepareReferences()
	{
		_content = base.transform.Find("Scroll View/Viewport/GamesContainer");
		_scrollRect = base.transform.Find("Scroll View").GetComponent<ScrollRect>();
	}
}
