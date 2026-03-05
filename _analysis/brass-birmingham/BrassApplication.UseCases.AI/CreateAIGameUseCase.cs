using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Extensions;
using BrassApplication.AI;
using BrassApplication.ApplicationSettings;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Persistence.Repository;
using BrassApplication.UseCases.Menu.OfflineGame.CreateOfflineGame;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.AI;

public class CreateAIGameUseCase : CreateOfflineGameUseCase
{
	public CreateAIGameUseCase(ICreateOfflineGameOutput presenter, IRepositoryFactory repositoryFactory, IGameHistory gameHistory, IAnalytics analytics, IGameSettings gameSettings)
		: base(presenter, repositoryFactory, gameHistory, analytics, gameSettings)
	{
	}

	public override void CreateNewGameConfiguration()
	{
		CreateNewGameConfiguration(-1);
	}

	public void CreateNewGameConfiguration(int seed, AIType aIType = AIType.Easy)
	{
		_gameHistory.Repository = _repositoryFactory.CreateInMemoryGameRepository();
		GameStartConfiguration gameStartConfiguration = new GameStartConfiguration(NumberOfPlayers.Four);
		if (seed >= 0)
		{
			gameStartConfiguration.Seed = seed;
		}
		_newGameMetadata = new GameMetadata(gameStartConfiguration, GameType.Local);
		List<PlayerColor> list = Enum.GetValues(typeof(PlayerColor)).Cast<PlayerColor>().ToList();
		IList<PlayerAvatar> list2 = Enum.GetValues(typeof(PlayerAvatar)).Cast<PlayerAvatar>().ToList()
			.GenerateRandomElements(4);
		for (int i = 0; i < 4; i++)
		{
			PlayerStartConfiguration playerStartConfiguration = new PlayerStartConfiguration(list[i], i);
			gameStartConfiguration.Players[i] = playerStartConfiguration;
			_newGameMetadata.PlayerMetadata.Add(new PlayerMetadata
			{
				PlayerStartConfiguration = playerStartConfiguration,
				NickName = ((PlayerAvatarFullName[])Enum.GetValues(typeof(PlayerAvatarFullName)))[(int)list2[i]].ToString().Replace("_", " "),
				PlayerNetworkId = "Player",
				AiType = aIType,
				PlayerAvatar = list2[i]
			});
		}
		PresentGameConfiguration();
	}

	protected override bool CanStartGame()
	{
		return true;
	}
}
