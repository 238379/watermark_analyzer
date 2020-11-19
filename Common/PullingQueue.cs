using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
	public class PullingQueue<T>
	{
		private readonly Func<T> pullingFunction;
		private readonly Action<Exception> exceptionAction;
		private readonly int capacity;
		private readonly int timeout;
		private readonly ConcurrentQueue<T> queue;

		public PullingQueue(Func<T> pullingFunction, Action<Exception> exceptionAction, int capacity, int timeout)
		{
			this.pullingFunction = pullingFunction;
			this.exceptionAction = exceptionAction;
			this.capacity = capacity;
			this.timeout = timeout;

			queue = new ConcurrentQueue<T>();

			PullToFull();
		}

		public T Pull()
		{
			DateTime t0 = DateTime.Now;
			T result = default;
			bool timeoutReached = false;
			while(!timeoutReached && !TryDequeue(out result))
			{
				Thread.Sleep(1);
				if(DateTime.Now.Subtract(t0).TotalMilliseconds >= timeout)
				{
					timeoutReached = true;
				}
			}
			if(timeoutReached)
			{
				throw new TimeoutException($"Timeout of {timeout}ms reached.");
			}
			return result;
		}

		/*public async Task<T> PullAsync()
		{
			// TODO
		}*/

		private void PullToFull()
		{
			for(int i = 0; i < capacity; i++)
			{
				EnqueueOne();
			}
		}

		private bool TryDequeue(out T result)
		{
			if(queue.TryDequeue(out result))
			{
				EnqueueOne();
				return true;
			}
			return false;
		}

		private void EnqueueOne()
		{
			GetOne().ContinueWith(t => queue.Enqueue(t.Result));
		}

		private Task<T> GetOne()
		{
			var pullingTask = Task.Run(() => pullingFunction());
			pullingTask.ContinueWith(async t =>
			{
				if (!t.IsCompletedSuccessfully)
				{
					exceptionAction(t.Exception);
					return await GetOne();
				}
				else
				{
					return await t;
				}
			});
			return pullingTask;
		}
	}
}
