using System;
using System.Collections.Generic;

namespace SolrNet.Cloud
{
    internal class SolrClusterExceptionHandlers {
        public SolrClusterExceptionHandlers(object sender) {
            this.sender = sender;
            handlers = new List<EventHandler<SolrClusterExceptionEventArgs>>();
        }

        private readonly List<EventHandler<SolrClusterExceptionEventArgs>> handlers;

        private readonly object sender;

        public void Add(EventHandler<SolrClusterExceptionEventArgs> handler) {
            lock (handlers)
                if (handlers.IndexOf(handler) < 0)
                    handlers.Add(handler);
        }

        public void Handle(Exception exception) {
            var args = new SolrClusterExceptionEventArgs(exception);
            lock (handlers)
                foreach (var handler in handlers)
                    try {
                        handler(sender, args);
                    } catch {}
        }

        public void Remove(EventHandler<SolrClusterExceptionEventArgs> handler) {
            lock (handlers)
                handlers.Remove(handler);
        }
    }
}
