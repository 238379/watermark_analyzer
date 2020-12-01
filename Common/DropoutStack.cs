using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public class DropoutStack<T>
	{
		private readonly int size;
		private readonly LinkedList<T> stack = new LinkedList<T>();

		public DropoutStack(int size)
		{
			ThrowIfInvalidSize(size);
			this.size = size;
		}

		public DropoutStack(int size, params T[] elements)
		{
			ThrowIfInvalidSize(size);
			this.size = size;
			PushAll(elements);
		}

		public DropoutStack(int size, T firstElement)
		{
			ThrowIfInvalidSize(size);
			this.size = size;
			Push(firstElement);
		}

		public bool IsEmpty => stack.Count == 0;

		public bool HasElements => stack.Count > 0;


		/***
		 * Returns false if it had to drop a element.
		 ***/
		public bool Push(T element)
		{
			stack.AddFirst(element);
			if (stack.Count > size)
			{
				stack.RemoveLast();
				return false;
			}
			return true;
		}

		public T Pop()
		{
			if(stack.Count > 0)
			{
				var element = stack.First;
				stack.RemoveFirst();
				return element.Value;
			}
			return default;
		}

		public void Clear()
		{
			stack.Clear();
		}

		private void ThrowIfInvalidSize(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException($"Size has to be a positive number but it is {size}.");
			}
		}

		private void PushAll(T[] elements)
		{
			foreach (var element in elements)
			{
				Push(element);
			}
		}
	}
}
