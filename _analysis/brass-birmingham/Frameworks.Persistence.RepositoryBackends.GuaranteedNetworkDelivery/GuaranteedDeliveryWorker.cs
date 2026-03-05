using System;
using System.Threading;
using System.Threading.Tasks;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public class GuaranteedDeliveryWorker : IDisposable
{
	private static IGuaranteedDeliveryJobQueue _jobQueue;

	private static bool _isRunning;

	private static Task _workerTask;

	private static IGameStorage _networkGameStorage;

	static GuaranteedDeliveryWorker()
	{
		_workerTask = new Task(WorkerLoop);
	}

	public GuaranteedDeliveryWorker(IGuaranteedDeliveryJobQueue jobQueue, IGameStorage networkGameStorage)
	{
		_jobQueue = jobQueue;
		_networkGameStorage = networkGameStorage;
	}

	~GuaranteedDeliveryWorker()
	{
		Dispose();
	}

	public void Dispose()
	{
		_isRunning = false;
	}

	public void StartWorker()
	{
		if (!_isRunning)
		{
			_isRunning = true;
			if (_workerTask != null && _workerTask.Status == TaskStatus.RanToCompletion)
			{
				_workerTask = new Task(WorkerLoop);
			}
			if (_workerTask != null && _workerTask.Status != TaskStatus.Running)
			{
				_workerTask.Start();
			}
		}
	}

	public void StopWorker()
	{
		Dispose();
	}

	private static void WorkerLoop()
	{
		while (_isRunning)
		{
			while (_jobQueue.Count() > 0)
			{
				IJob job = _jobQueue.Peek();
				try
				{
					job.ProcessUsing(_networkGameStorage);
					_jobQueue.Dequeue();
				}
				catch (Exception)
				{
				}
			}
			Thread.Sleep(500);
		}
	}
}
