using BoardGameRules.Entities.Cards;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.HandOfCards;

public interface IHandOfCardsInput
{
	void UpdateCards(PlayerMetadata replayWatchingPlayerMetadata);

	void UpdateCardsForPlayer(PlayerMetadata playerMetadata);

	void OnCardSelected(BaseCard card);

	string GetRegionFriendlyName(string regionName);
}
