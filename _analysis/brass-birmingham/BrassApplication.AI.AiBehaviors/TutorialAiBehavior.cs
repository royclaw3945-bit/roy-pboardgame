using System;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;
using BoardGameRules.Entities.TurnPhases;
using BrassApplication.Game;

namespace BrassApplication.AI.AiBehaviors;

public class TutorialAiBehavior : HardAiBehavior
{
	public TutorialAiBehavior(string playerId)
		: base(playerId)
	{
	}

	public override Action GetNextAIMove(BrassApplicationGame brassApplicationGame)
	{
		BoardGameRulesGame game = brassApplicationGame.Game;
		if (game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			switch (game.GameProgressInformation.CurrentRoundNumber)
			{
			case 5:
			{
				BaseCard card7 = game.GameProgressInformation.CurrentPlayer.Hand.First();
				return Loan(game.GameProgressInformation, game.TurnFlow, card7);
			}
			case 6:
			{
				if (game.TurnFlow.GetCurrentActionNumber() == 1)
				{
					BaseCard card5 = game.GameProgressInformation.CurrentPlayer.Hand.First((BaseCard c) => c is LocationCard locationCard && locationCard.Region == RegionName.Derby);
					BuildingSlot buildingSlot3 = game.Board.GetRegionByName(RegionName.Derby).BuildingSlots.First((BuildingSlot bs) => bs.BuildingTypes.Contains(BuildingType.Manufacturer));
					return Build(game.GameProgressInformation, game.TurnFlow, card5, buildingSlot3, BuildingType.Manufacturer);
				}
				BaseCard card6 = game.GameProgressInformation.CurrentPlayer.Hand.First();
				Link link3 = game.Board.GetRegionByName(RegionName.Derby).Links.First((Link l) => l is Canal && (l.Region1.Name == RegionName.BurtonOnTrent || l.Region2.Name == RegionName.BurtonOnTrent));
				return Network(game.GameProgressInformation, game.TurnFlow, card6, link3);
			}
			case 7:
			{
				if (game.TurnFlow.GetCurrentActionNumber() == 1)
				{
					BaseCard card3 = game.GameProgressInformation.CurrentPlayer.Hand.First((BaseCard c) => (c is IndustryCard industryCard && industryCard.BuildingTypes.Contains(BuildingType.Brewery)) || (c is LocationCard locationCard && locationCard.Region == RegionName.BurtonOnTrent));
					BuildingSlot buildingSlot2 = game.Board.GetRegionByName(RegionName.BurtonOnTrent).BuildingSlots.First((BuildingSlot bs) => bs.BuildingTypes.Contains(BuildingType.Brewery));
					return Build(game.GameProgressInformation, game.TurnFlow, card3, buildingSlot2, BuildingType.Brewery);
				}
				BaseCard card4 = game.GameProgressInformation.CurrentPlayer.Hand.First();
				Link link2 = game.Board.GetRegionByName(RegionName.Derby).Links.First((Link l) => l is Canal && (l.Region1.Name == RegionName.Nottingham || l.Region2.Name == RegionName.Nottingham));
				return Network(game.GameProgressInformation, game.TurnFlow, card4, link2);
			}
			case 8:
			{
				if (game.TurnFlow.GetCurrentActionNumber() == 1)
				{
					BaseCard card = game.GameProgressInformation.CurrentPlayer.Hand.First((BaseCard c) => c is LocationCard locationCard && locationCard.Region == RegionName.Cannock);
					BuildingSlot buildingSlot = game.Board.GetRegionByName(RegionName.Cannock).BuildingSlots.First((BuildingSlot bs) => bs.BuildingTypes.Count == 1);
					return Build(game.GameProgressInformation, game.TurnFlow, card, buildingSlot, BuildingType.CoalMine);
				}
				BaseCard card2 = game.GameProgressInformation.CurrentPlayer.Hand.First();
				Link link = game.Board.GetRegionByName(RegionName.Cannock).Links.First((Link l) => l is Canal && (l.Region1.Name == RegionName.Central || l.Region2.Name == RegionName.Central));
				return Network(game.GameProgressInformation, game.TurnFlow, card2, link);
			}
			default:
				return BaseAiBehavior.PassFirstCard(brassApplicationGame.Game.GameProgressInformation, brassApplicationGame.Game.TurnFlow);
			}
		}
		return base.GetNextAIMove(brassApplicationGame);
	}

	private static Action Loan(GameProgressInformation gameProgressInformation, TurnFlow turnFlow, BaseCard card)
	{
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		return delegate
		{
			LoanAction loanAction = turnFlow.StartLoanAction();
			loanAction.DiscardCard(card, currentPlayer);
			loanAction.Loan(currentPlayer.Color);
		};
	}

	private static Action Build(GameProgressInformation gameProgressInformation, TurnFlow turnFlow, BaseCard card, BuildingSlot buildingSlot, BuildingType buildingType)
	{
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		return delegate
		{
			BuildAction buildAction = turnFlow.StartBuildAction();
			buildAction.DiscardCard(card, currentPlayer);
			buildAction.Build(buildingSlot, buildingType);
		};
	}

	private static Action Network(GameProgressInformation gameProgressInformation, TurnFlow turnFlow, BaseCard card, Link link)
	{
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		return delegate
		{
			NetworkAction networkAction = turnFlow.StartNetworkAction();
			networkAction.DiscardCard(card, currentPlayer);
			networkAction.PlaceLink(link);
		};
	}
}
