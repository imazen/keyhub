using System;
using System.Collections.Generic;
using System.Threading;

namespace KeyHub.Web.Composition.WindsorLifestyles.Contextual
{
	public class ContainerContextStore : IContainerContextStore
	{
		private readonly Dictionary<Thread, Stack<ContainerContext>> contexts = new Dictionary<Thread, Stack<ContainerContext>>();

		public void RegisterCurrent(ContainerContext context)
		{
			lock (contexts)
			{
				var stack = GetContextHistory(Thread.CurrentThread);
				stack.Push(context);
			}
		}

		public void UnregisterCurrent(ContainerContext context)
		{
			lock (contexts)
			{
				var stack = GetContextHistory(Thread.CurrentThread);
				var lastContext = stack.Pop();
				if (lastContext != context)
				{
					throw new InvalidOperationException(
						"Something goes wrong. The current context is not the one initialized at last !?");
				}
			}
		}

		public ContainerContext GetCurrent()
		{
			lock (contexts)
			{
				var stack = GetContextHistory(Thread.CurrentThread);
				return stack.Count == 0 ? null : stack.Peek();
			}
		}

		private Stack<ContainerContext> GetContextHistory(Thread thread)
		{
			Stack<ContainerContext> contextHistory;
			if (contexts.TryGetValue(thread, out contextHistory) == false)
			{
				contextHistory = new Stack<ContainerContext>();
				contexts.Add(thread, contextHistory);
			}
			return contextHistory;
		}
	}
}