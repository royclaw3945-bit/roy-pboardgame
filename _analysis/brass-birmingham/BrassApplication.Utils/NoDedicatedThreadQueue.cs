using System;
using System.Collections.Generic;
using System.Threading;

namespace BrassApplication.Utils;

public class NoDedicatedThreadQueue
{
	private Queue<Action> _jobs = new Queue<Action>();

	private bool _delegateQueuedOrRunning;

	public void Enqueue(Action job)
	{
		lock (_jobs)
		{
			_jobs.Enqueue(job);
			if (!_delegateQueuedOrRunning)
			{
				_delegateQueuedOrRunning = true;
				ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems, null);
			}
		}
	}

	private void ProcessQueuedItems(object ignored)
	{
		while (true)
		{
			Action action;
			lock (_jobs)
			{
				if (_jobs.Count == 0)
				{
					_delegateQueuedOrRunning = false;
					break;
				}
				action = _jobs.Dequeue();
			}
			try
			{
				action();
			}
			catch
			{
				ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems, null);
				throw;
			}
		}
	}
}
