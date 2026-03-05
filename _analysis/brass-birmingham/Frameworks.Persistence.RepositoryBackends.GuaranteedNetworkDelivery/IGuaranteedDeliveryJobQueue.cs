namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public interface IGuaranteedDeliveryJobQueue
{
	int Count();

	void Enqueue(IJob job);

	IJob Dequeue();

	IJob Peek();

	void Clear();
}
