using System;
using BrassApplication.GameHistory;
using BrassApplication.Persistence.Serialization;
using Frameworks.Network.Rest;
using Frameworks.Persistence.Repository;
using Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;
using Frameworks.Persistence.Serialization;
using Frameworks.Routing;
using Utils;
using Utils.Wireup;

namespace Frameworks.Init;

public class FrameworksInit : IDisposable
{
	private readonly IFrameworksDelegate _delegate;

	public FrameworksInit(IFrameworksDelegate frameworksDelegate)
	{
		_delegate = frameworksDelegate;
	}

	public void Init()
	{
		_delegate.LoggerInit();
		_delegate.MainThreadDispatcherInit();
		_delegate.LocalizationInit();
		StorageInit();
		_delegate.RouterInit();
		UseCaseBuilderInit();
	}

	public void Dispose()
	{
	}

	private void StorageInit()
	{
		GuaranteedDeliveryJobQueue instance = new GuaranteedDeliveryJobQueue(RepositoryStorageLocation.GuaranteedDeliveryJobQueueStoragePath, new JsonSerializer());
		if (Global.Instance.NetworkBackendClient == null)
		{
			Global.Instance.NetworkBackendClient = Wireup.Init().With((IRestClientFactory)new RestClientFactory("https://brass-birmingham.api.cublogames.com/index.php", 15)).With((IRestRequestFactory)new RestRequestFactory())
				.With((ISerialize)new JsonSerializer())
				.With(Global.Instance.MainThreadDispatcher)
				.With((IGuaranteedDeliveryJobQueue)instance)
				.UsingNetworkBackendClientWireup()
				.BuildNetworkBackendClient();
		}
		if (Global.Instance.BrassGameHistory == null)
		{
			Global.Instance.BrassGameHistory = new BrassGameHistory();
		}
	}

	private void UseCaseBuilderInit()
	{
		Global.Instance.UseCaseBuilder = new UseCaseBuilder(Global.Instance.Router, Global.Instance.Localization, Global.Instance.NetworkBackendClient, new RepositoryFactory(), Global.Instance.BrassGameHistory);
		Global.Instance.UseCaseBuilder.BuildUseCases();
	}
}
