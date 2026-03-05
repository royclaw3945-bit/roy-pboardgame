using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Utils.Extensions;
using BrassApplication.ApplicationSettings;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;
using BrassApplication.UseCases.Menu.Settings;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.Game.ExitToMenu;

public class ExitToMenuUseCase : SettingsUseCase, IExitToMenuInput, ISettingsInput
{
	private IExitToMenuOutput _presenter;

	private readonly IGameHistory _gameHistory;

	private readonly ISerialize _serializer;

	public ExitToMenuUseCase(IExitToMenuOutput presenter, IGameHistory gameHistory, IAudioSettings audioSettings, ILanguageSettings languageSettings, IGameSettings gameSettings, IAnalytics analytics, ISerialize serializer)
		: base(presenter, audioSettings, languageSettings, gameSettings, analytics)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
		_serializer = serializer;
	}

	public void BackToMenu()
	{
		_presenter.ShowLoadingViewAndLoadMenuScene();
	}

	public (string metadata, string events) GetSaveForReport()
	{
		string gameId = _gameHistory.CurrentGame.Metadata.GameId;
		IGameStorage gameStorage = _gameHistory.Repository.Advanced.GameStorage;
		GameMetadata graph = gameStorage.GetGamesMetadata().First((GameMetadata gm) => gm.GameId == gameId);
		List<IEvent> graph2 = gameStorage.LoadEvents(gameId, 0, int.MaxValue).ToList();
		using MemoryStream memoryStream = new MemoryStream();
		using MemoryStream memoryStream2 = new MemoryStream();
		_serializer.Serialize(memoryStream, graph2);
		_serializer.Serialize(memoryStream2, graph);
		string item = memoryStream.ConvertToString();
		return (metadata: memoryStream2.ConvertToString(), events: item);
	}

	public string GetTranslation(string key)
	{
		return _presenter.GetTranslation(key);
	}
}
