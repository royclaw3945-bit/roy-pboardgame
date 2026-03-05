using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Extensions;
using BrassApplication.AI;
using BrassApplication.AI.AiBehaviors;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Persistence.Repository;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.Tutorial;

public class TutorialUseCase : IUseCase, ITutorialInput
{
	private readonly ITutorialOutput _presenter;

	private readonly IRepositoryFactory _repositoryFactory;

	private readonly IGameHistory _gameHistory;

	public TutorialUseCase(ITutorialOutput presenter, IRepositoryFactory repositoryFactory, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_repositoryFactory = repositoryFactory;
		_gameHistory = gameHistory;
	}

	public void StartTutorial()
	{
		_gameHistory.Repository = _repositoryFactory.CreateTutorialGameRepository();
		foreach (GameMetadata gamesMetadatum in _gameHistory.Repository.GetGamesMetadata())
		{
			if (gamesMetadatum.GameType == GameType.Tutorial)
			{
				_gameHistory.Repository.Delete(gamesMetadatum.GameId);
			}
		}
		GameMetadata gameMetadata = new GameMetadata(new GameStartConfiguration(NumberOfPlayers.Two), GameType.Local);
		IList<PlayerAvatar> list = Enum.GetValues(typeof(PlayerAvatar)).Cast<PlayerAvatar>().ToList()
			.GenerateRandomElements(gameMetadata.GameStartConfiguration.NumberOfPlayersAsInt);
		gameMetadata.GameStartConfiguration.Players[0] = new PlayerStartConfiguration(PlayerColor.Yellow, 0);
		gameMetadata.GameStartConfiguration.Players[1] = new PlayerStartConfiguration(PlayerColor.Red, 1);
		List<PlayerColor> list2 = new List<PlayerColor>
		{
			PlayerColor.Yellow,
			PlayerColor.Red
		};
		for (int i = 0; i < 2; i++)
		{
			PlayerStartConfiguration playerStartConfiguration = new PlayerStartConfiguration(list2[i], i);
			gameMetadata.PlayerMetadata.Add(new PlayerMetadata
			{
				PlayerStartConfiguration = playerStartConfiguration,
				NickName = $"Player {playerStartConfiguration.Color}",
				PlayerNetworkId = "Player",
				AiType = ((i != 0) ? AIType.Tutorial : AIType.Human),
				PlayerAvatar = list[i]
			});
		}
		BrassApplicationGame brassApplicationGame = _gameHistory.Repository.Create(gameMetadata);
		brassApplicationGame.Metadata.PlayerMetadata.ForEach(delegate(PlayerMetadata pm)
		{
			pm.AIBehaviour = AiBehaviorFactory.CreateAiBehavior(pm);
		});
		brassApplicationGame.Metadata.GameType = GameType.Tutorial;
		brassApplicationGame.Metadata.GameStartConfiguration.IsTutorialGame = true;
		brassApplicationGame.Metadata.GameStartConfiguration.Seed = 604579263;
		brassApplicationGame.Game.StartGame();
		brassApplicationGame.Metadata.PlayerMetadata.ForEach(delegate(PlayerMetadata pm)
		{
			pm.LastSeenEventRevision = brassApplicationGame.Game.Revision;
		});
		_gameHistory.Repository.Save(brassApplicationGame);
		_gameHistory.SetGame(brassApplicationGame);
		brassApplicationGame.Game.ApplyEvent(new MoneySpent(PlayerColor.Yellow, -30));
		_presenter.GameCreated(brassApplicationGame);
	}
}
