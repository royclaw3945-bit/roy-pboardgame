using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Extensions;
using BoardGameRules.Utils.Logging;
using BrassApplication.AI;
using BrassApplication.AI.AiBehaviors;
using BrassApplication.ApplicationSettings;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Persistence.Repository;
using BrassApplication.UseCases.Common;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.Menu.OfflineGame.CreateOfflineGame;

public class CreateOfflineGameUseCase : IUseCase, ICreateOfflineGameInput
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(CreateOfflineGameUseCase));

	protected readonly ICreateOfflineGameOutput _presenter;

	protected readonly IRepositoryFactory _repositoryFactory;

	protected readonly IGameHistory _gameHistory;

	private IGameSettings _gameSettings;

	private readonly IAnalytics _analytics;

	protected GameMetadata _newGameMetadata;

	protected int _numberOfPlayers = 4;

	protected bool _anyGameToContinue;

	public CreateOfflineGameUseCase(ICreateOfflineGameOutput presenter, IRepositoryFactory repositoryFactory, IGameHistory gameHistory, IAnalytics analytics, IGameSettings gameSettings)
	{
		_presenter = presenter;
		_repositoryFactory = repositoryFactory;
		_gameHistory = gameHistory;
		_analytics = analytics;
		_gameSettings = gameSettings;
	}

	public void UpdateResumeOfflineGameView()
	{
		GameMetadata gameMetadata = null;
		try
		{
			gameMetadata = _gameHistory.Repository.GetGamesMetadata().FirstOrDefault();
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to load game list. Remove everything. Error: " + ex.Message);
			_gameHistory.Repository.Advanced.GameStorage.DeleteAll();
		}
		_anyGameToContinue = gameMetadata != null && !gameMetadata.GameProgressInformation.IsGameOver;
		if (_anyGameToContinue)
		{
			try
			{
				BrassApplicationGame game = _gameHistory.Repository.Load(gameMetadata.GameId);
				_presenter.ShowExistingGameContainer(game);
				return;
			}
			catch (Exception ex2)
			{
				Logger.Error("Failed to load game. Removing everything. Error: " + ex2.Message);
				_gameHistory.Repository.Advanced.GameStorage.DeleteAll();
				_presenter.HideExistingGameContainer();
				return;
			}
		}
		_presenter.HideExistingGameContainer();
	}

	public void ContinueClicked()
	{
		GameMetadata gameMetadata = _gameHistory.Repository.GetGamesMetadata().First();
		BrassApplicationGame brassApplicationGame = _gameHistory.Repository.Load(gameMetadata.GameId);
		_gameHistory.SetGame(brassApplicationGame);
		_presenter.GameLoaded(brassApplicationGame);
	}

	public virtual void CreateNewGameConfiguration()
	{
		_gameHistory.Repository = _repositoryFactory.CreateLocalGameRepository();
		_newGameMetadata = new GameMetadata(new GameStartConfiguration(NumberOfPlayers.Four), GameType.Local);
		if (_gameSettings.IsSavedDefaultGameSettings())
		{
			_numberOfPlayers = _gameSettings.GetNumberOfPlayers();
			for (int i = 0; i < 4; i++)
			{
				PlayerColor color = AvailableColorsList().First();
				PlayerAvatar playerAvatar = AvailableAvatarsList().First();
				AIType aiType = AIType.Easy;
				try
				{
					color = _gameSettings.GetPlayerIndexSavedColor(i);
					playerAvatar = _gameSettings.GetPlayerIndexSavedAvatar(i);
					aiType = _gameSettings.GetPlayerIndexSavedAIType(i);
				}
				catch (Exception)
				{
				}
				PlayerStartConfiguration playerStartConfiguration = new PlayerStartConfiguration(color, i);
				if (i >= _numberOfPlayers)
				{
					playerStartConfiguration = null;
				}
				_newGameMetadata.GameStartConfiguration.Players[i] = playerStartConfiguration;
				_newGameMetadata.PlayerMetadata.Add(new PlayerMetadata
				{
					PlayerStartConfiguration = playerStartConfiguration,
					NickName = playerAvatar.ToString().Replace("_", " "),
					PlayerNetworkId = "Player",
					AiType = aiType,
					PlayerAvatar = playerAvatar
				});
			}
		}
		else
		{
			List<PlayerColor> list = Enum.GetValues(typeof(PlayerColor)).Cast<PlayerColor>().ToList();
			for (int j = 0; j < 4; j++)
			{
				IList<PlayerAvatar> list2 = Enum.GetValues(typeof(PlayerAvatar)).Cast<PlayerAvatar>().ToList()
					.GenerateRandomElements(4);
				PlayerStartConfiguration playerStartConfiguration2 = new PlayerStartConfiguration(list[j], j);
				_newGameMetadata.GameStartConfiguration.Players[j] = playerStartConfiguration2;
				_newGameMetadata.PlayerMetadata.Add(new PlayerMetadata
				{
					PlayerStartConfiguration = playerStartConfiguration2,
					NickName = ((PlayerAvatarFullName[])Enum.GetValues(typeof(PlayerAvatarFullName)))[(int)list2[j]].ToString().Replace("_", " "),
					PlayerNetworkId = "Player",
					AiType = ((j != 0) ? AIType.Easy : AIType.Human),
					PlayerAvatar = list2[j]
				});
			}
		}
		PresentGameConfiguration();
	}

	protected void PresentGameConfiguration()
	{
		_presenter.PresentGameConfiguration(_newGameMetadata.PlayerMetadata.Take(_numberOfPlayers), NextAiDifficulty, PreviousAiDifficulty, EditPortraitAndColor, ChangePlayerName, IncreaseNumberOfPlayers, DecreaseNumberOfPlayers, CanStartGame());
	}

	public void SetIntroductoryGame(bool isIntroductoryGame)
	{
		_newGameMetadata.GameStartConfiguration.IsIntroductoryGame = isIntroductoryGame;
	}

	protected void NextAiType(PlayerMetadata playerMetadata)
	{
		if (playerMetadata.AiType == AIType.Human)
		{
			playerMetadata.AiType = AIType.Easy;
			playerMetadata.NickName = ((PlayerAvatarFullName[])Enum.GetValues(typeof(PlayerAvatarFullName)))[(int)playerMetadata.PlayerAvatar].ToString().Replace("_", " ");
		}
		else
		{
			playerMetadata.AiType = AIType.Human;
		}
		PresentGameConfiguration();
	}

	protected void PreviousAiType(PlayerMetadata playerMetadata)
	{
		NextAiType(playerMetadata);
	}

	protected void NextAiDifficulty(PlayerMetadata playerMetadata)
	{
		switch (playerMetadata.AiType)
		{
		case AIType.Easy:
			playerMetadata.AiType = AIType.Medium;
			break;
		case AIType.Medium:
			playerMetadata.AiType = AIType.Hard;
			break;
		case AIType.Hard:
			playerMetadata.AiType = AIType.Easy;
			break;
		}
		PresentGameConfiguration();
	}

	protected void PreviousAiDifficulty(PlayerMetadata playerMetadata)
	{
		switch (playerMetadata.AiType)
		{
		case AIType.Easy:
			playerMetadata.AiType = AIType.Hard;
			break;
		case AIType.Medium:
			playerMetadata.AiType = AIType.Easy;
			break;
		case AIType.Hard:
			playerMetadata.AiType = AIType.Medium;
			break;
		}
		PresentGameConfiguration();
	}

	protected void EditPortraitAndColor(PlayerMetadata playerMetadata)
	{
		_presenter.ShowChoosePortraitAndColorView(playerMetadata);
	}

	public void IncreaseNumberOfPlayers()
	{
		if (_numberOfPlayers < 4)
		{
			_numberOfPlayers++;
			PlayerAvatar playerAvatar = AvailableAvatarsList()[0];
			_newGameMetadata.PlayerMetadata[_numberOfPlayers - 1].PlayerAvatar = playerAvatar;
			_newGameMetadata.PlayerMetadata[_numberOfPlayers - 1].PlayerStartConfiguration = new PlayerStartConfiguration(AvailableColorsList()[0], _numberOfPlayers - 1);
			_newGameMetadata.PlayerMetadata[_numberOfPlayers - 1].NickName = Enum.GetName(typeof(PlayerAvatarFullName), (int)_newGameMetadata.PlayerMetadata[_numberOfPlayers - 1].PlayerAvatar).Replace("_", " ");
			_newGameMetadata.PlayerMetadata[_numberOfPlayers - 1].AiType = AIType.Easy;
			_newGameMetadata.GameStartConfiguration.Players[_numberOfPlayers - 1] = _newGameMetadata.PlayerMetadata[_numberOfPlayers - 1].PlayerStartConfiguration;
			PresentGameConfiguration();
			return;
		}
		throw new InvalidOperationException("Can't have more than 4 players");
	}

	public void DecreaseNumberOfPlayers()
	{
		if (_numberOfPlayers > 2)
		{
			_numberOfPlayers--;
			_newGameMetadata.PlayerMetadata[_numberOfPlayers].PlayerStartConfiguration = null;
			PresentGameConfiguration();
			return;
		}
		throw new InvalidOperationException("Can't have less than 2 players");
	}

	protected void ChangePlayerName(PlayerColor playerColor, string newName)
	{
		_newGameMetadata.GetPlayerMetadataByColor(playerColor).NickName = newName;
		PresentGameConfiguration();
	}

	private List<PlayerColor> AvailableColorsList()
	{
		List<PlayerColor> list = new List<PlayerColor>();
		PlayerColor[] array = (PlayerColor[])Enum.GetValues(typeof(PlayerColor));
		foreach (PlayerColor playerColor in array)
		{
			if (_newGameMetadata.GetPlayerMetadataByColor(playerColor) == null)
			{
				list.Add(playerColor);
			}
		}
		return list;
	}

	public void NextColorFor(PlayerColor startingColor, PlayerColor playerColor)
	{
		PlayerColor oldColor = playerColor;
		switch (playerColor)
		{
		case PlayerColor.Blue:
			playerColor = PlayerColor.Red;
			break;
		case PlayerColor.Red:
			playerColor = PlayerColor.Violet;
			break;
		case PlayerColor.Violet:
			playerColor = PlayerColor.Yellow;
			break;
		case PlayerColor.Yellow:
			playerColor = PlayerColor.Blue;
			break;
		}
		ChangeColor(startingColor, oldColor, playerColor);
	}

	public void PreviousColorFor(PlayerColor startingColor, PlayerColor playerColor)
	{
		PlayerColor oldColor = playerColor;
		switch (playerColor)
		{
		case PlayerColor.Blue:
			playerColor = PlayerColor.Yellow;
			break;
		case PlayerColor.Red:
			playerColor = PlayerColor.Blue;
			break;
		case PlayerColor.Violet:
			playerColor = PlayerColor.Red;
			break;
		case PlayerColor.Yellow:
			playerColor = PlayerColor.Violet;
			break;
		}
		ChangeColor(startingColor, oldColor, playerColor);
	}

	private void ChangeColor(PlayerColor startingColor, PlayerColor oldColor, PlayerColor newColor)
	{
		PlayerMetadata playerMetadataByColor = _newGameMetadata.GetPlayerMetadataByColor(oldColor);
		if (startingColor != oldColor)
		{
			PlayerMetadata playerMetadataByColor2 = _newGameMetadata.GetPlayerMetadataByColor(startingColor);
			if (playerMetadataByColor2 != null)
			{
				playerMetadataByColor2.PlayerStartConfiguration = new PlayerStartConfiguration(oldColor, playerMetadataByColor2.PlayerStartConfiguration.SittingOrder);
			}
		}
		PlayerMetadata playerMetadataByColor3 = _newGameMetadata.GetPlayerMetadataByColor(newColor);
		playerMetadataByColor.PlayerStartConfiguration = new PlayerStartConfiguration(newColor, playerMetadataByColor.PlayerStartConfiguration.SittingOrder);
		if (playerMetadataByColor3 != null)
		{
			playerMetadataByColor3.PlayerStartConfiguration = new PlayerStartConfiguration(startingColor, playerMetadataByColor3.PlayerStartConfiguration.SittingOrder);
		}
		_presenter.UpdatePortraitAndColorView(playerMetadataByColor);
		PresentGameConfiguration();
	}

	private List<PlayerAvatar> AvailableAvatarsList()
	{
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		PlayerAvatar[] array = (PlayerAvatar[])Enum.GetValues(typeof(PlayerAvatar));
		foreach (PlayerAvatar avatar in array)
		{
			if (!_newGameMetadata.PlayerMetadata.Any((PlayerMetadata pm) => pm.PlayerAvatar == avatar && pm.PlayerStartConfiguration != null))
			{
				list.Add(avatar);
			}
		}
		return list;
	}

	public void NextPortraitFor(PlayerAvatar startingAvatar, PlayerAvatar playerAvatar)
	{
		PlayerAvatar oldAvatar = playerAvatar;
		switch (playerAvatar)
		{
		case PlayerAvatar.Arkwright:
			playerAvatar = PlayerAvatar.Bessemer;
			break;
		case PlayerAvatar.Bessemer:
			playerAvatar = PlayerAvatar.Brunel;
			break;
		case PlayerAvatar.Brunel:
			playerAvatar = PlayerAvatar.Coade;
			break;
		case PlayerAvatar.Coade:
			playerAvatar = PlayerAvatar.Owen;
			break;
		case PlayerAvatar.Owen:
			playerAvatar = PlayerAvatar.Rothschild;
			break;
		case PlayerAvatar.Rothschild:
			playerAvatar = PlayerAvatar.Stephenson;
			break;
		case PlayerAvatar.Stephenson:
			playerAvatar = PlayerAvatar.Tinsley;
			break;
		case PlayerAvatar.Tinsley:
			playerAvatar = PlayerAvatar.Watt;
			break;
		case PlayerAvatar.Watt:
			playerAvatar = PlayerAvatar.Arkwright;
			break;
		}
		ChangeAvatar(startingAvatar, oldAvatar, playerAvatar);
	}

	public void PreviousPortraitFor(PlayerAvatar startingAvatar, PlayerAvatar playerAvatar)
	{
		PlayerAvatar oldAvatar = playerAvatar;
		switch (playerAvatar)
		{
		case PlayerAvatar.Arkwright:
			playerAvatar = PlayerAvatar.Watt;
			break;
		case PlayerAvatar.Bessemer:
			playerAvatar = PlayerAvatar.Arkwright;
			break;
		case PlayerAvatar.Brunel:
			playerAvatar = PlayerAvatar.Bessemer;
			break;
		case PlayerAvatar.Coade:
			playerAvatar = PlayerAvatar.Brunel;
			break;
		case PlayerAvatar.Owen:
			playerAvatar = PlayerAvatar.Coade;
			break;
		case PlayerAvatar.Rothschild:
			playerAvatar = PlayerAvatar.Owen;
			break;
		case PlayerAvatar.Stephenson:
			playerAvatar = PlayerAvatar.Rothschild;
			break;
		case PlayerAvatar.Tinsley:
			playerAvatar = PlayerAvatar.Stephenson;
			break;
		case PlayerAvatar.Watt:
			playerAvatar = PlayerAvatar.Tinsley;
			break;
		}
		ChangeAvatar(startingAvatar, oldAvatar, playerAvatar);
	}

	private void ChangeAvatar(PlayerAvatar startingAvatar, PlayerAvatar oldAvatar, PlayerAvatar newAvatar)
	{
		PlayerMetadata playerMetadataByAvatar = _newGameMetadata.GetPlayerMetadataByAvatar(oldAvatar);
		if (startingAvatar != oldAvatar)
		{
			PlayerMetadata playerMetadataByAvatar2 = _newGameMetadata.GetPlayerMetadataByAvatar(startingAvatar);
			if (playerMetadataByAvatar2 != null)
			{
				playerMetadataByAvatar2.PlayerAvatar = oldAvatar;
				if (playerMetadataByAvatar2.AiType != AIType.Human || playerMetadataByAvatar2.NickName == Enum.GetName(typeof(PlayerAvatarFullName), (int)startingAvatar).Replace("_", " "))
				{
					playerMetadataByAvatar2.NickName = Enum.GetName(typeof(PlayerAvatarFullName), (int)playerMetadataByAvatar2.PlayerAvatar).Replace("_", " ");
				}
			}
		}
		PlayerMetadata playerMetadataByAvatar3 = _newGameMetadata.GetPlayerMetadataByAvatar(newAvatar);
		playerMetadataByAvatar.PlayerAvatar = newAvatar;
		if (playerMetadataByAvatar.AiType != AIType.Human || playerMetadataByAvatar.NickName == Enum.GetName(typeof(PlayerAvatarFullName), (int)oldAvatar).Replace("_", " "))
		{
			playerMetadataByAvatar.NickName = Enum.GetName(typeof(PlayerAvatarFullName), (int)playerMetadataByAvatar.PlayerAvatar).Replace("_", " ");
		}
		if (playerMetadataByAvatar3 != null)
		{
			playerMetadataByAvatar3.PlayerAvatar = startingAvatar;
			if (playerMetadataByAvatar3.AiType != AIType.Human || playerMetadataByAvatar3.NickName == Enum.GetName(typeof(PlayerAvatarFullName), (int)newAvatar).Replace("_", " "))
			{
				playerMetadataByAvatar3.NickName = Enum.GetName(typeof(PlayerAvatarFullName), (int)playerMetadataByAvatar3.PlayerAvatar).Replace("_", " ");
			}
		}
		_presenter.UpdatePortraitAndColorView(playerMetadataByAvatar);
		PresentGameConfiguration();
	}

	protected virtual bool CanStartGame()
	{
		return _newGameMetadata.PlayerMetadata.Take(_numberOfPlayers).Any((PlayerMetadata p) => p.AiType == AIType.Human);
	}

	public void StartGameClicked()
	{
		if (_anyGameToContinue)
		{
			_presenter.AskOverwriteExistingGame(StartGame);
		}
		else
		{
			StartGame();
		}
	}

	public BrassApplicationGame StartGame()
	{
		return StartGame(-1);
	}

	public BrassApplicationGame StartGame(int seed)
	{
		foreach (GameMetadata gamesMetadatum in _gameHistory.Repository.GetGamesMetadata())
		{
			_gameHistory.Repository.Delete(gamesMetadatum.GameId);
		}
		if (!CanStartGame())
		{
			throw new InvalidOperationException("Can't start game with these AI configuration");
		}
		_gameSettings.SetDefaultGameSettings(_newGameMetadata, _numberOfPlayers);
		_newGameMetadata.PlayerMetadata = _newGameMetadata.PlayerMetadata.Take(_numberOfPlayers).ToList();
		_newGameMetadata.GameStartConfiguration.Players = _newGameMetadata.PlayerMetadata.Select((PlayerMetadata pm) => pm.PlayerStartConfiguration).ToList();
		if (seed >= 0)
		{
			_newGameMetadata.GameStartConfiguration.Seed = seed;
		}
		BrassApplicationGame brassApplicationGame = _gameHistory.Repository.Create(_newGameMetadata);
		List<PlayerMetadata> playerMetadata = brassApplicationGame.Metadata.PlayerMetadata;
		playerMetadata.ForEach(delegate(PlayerMetadata pm)
		{
			pm.AIBehaviour = AiBehaviorFactory.CreateAiBehavior(pm);
		});
		brassApplicationGame.Game.StartGame();
		playerMetadata.ForEach(delegate(PlayerMetadata pm)
		{
			pm.LastSeenEventRevision = 1;
		});
		_gameHistory.Repository.Save(brassApplicationGame);
		_gameHistory.SetGame(brassApplicationGame);
		_presenter.LaunchGame(brassApplicationGame);
		_analytics.GameStart(brassApplicationGame.Metadata);
		return brassApplicationGame;
	}

	public void ShowIntroductoryGameTooltip(float x, float y)
	{
		_presenter.ShowIntroductoryGameTooltip(x, y);
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}
}
