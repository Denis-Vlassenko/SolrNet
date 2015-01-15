using System;
using System.Collections.Generic;
using SolrNet.Utils;
using Parent = SolrNet.Startup;

namespace SolrNet.Cloud
{
    public static class Startup {
        static Startup() {
            AppDomain.CurrentDomain.DomainUnload += Dispose;
            CloudStates = new Dictionary<string, ISolrCloudState>(StringComparer.OrdinalIgnoreCase);
            KnownRegistrations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public static IContainer Container {
            get { return Parent.Container; }
        }

        private static void Dispose(object sender, EventArgs eventArgs) {
            lock (CloudStates) {
                foreach (var state in CloudStates.Values)
                    state.Dispose();
                CloudStates.Clear();
            }
        }

        private static readonly Dictionary<string, ISolrCloudState> CloudStates;

        private static readonly HashSet<string> KnownRegistrations;

        private static void EnsureRegistration(string zooKeeperConnection) {
            lock (CloudStates) {
                ISolrCloudState state;
                if (CloudStates.Count == 0) {
                    var operationsProvider = new OperationsProvider();
                    Parent.Container.Register<ISolrOperationsProvider>(c => operationsProvider);
                }
                if (CloudStates.TryGetValue(zooKeeperConnection, out state))
                    return;
                state = new SolrCloudState(zooKeeperConnection);
                state.Init();
                CloudStates.Add(zooKeeperConnection, state);
                Parent.Container.Register(zooKeeperConnection, c => state);
            }
        }

        public static void Init<T>(string zooKeeperConnection) {
            EnsureRegistration(zooKeeperConnection);
            Parent.Container.Register<ISolrCloudBasicOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudState>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
            Parent.Container.Register<ISolrCloudBasicReadOnlyOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudState>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
            Parent.Container.Register<ISolrCloudOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudState>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
            Parent.Container.Register<ISolrCloudReadOnlyOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudState>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
        }

        public static void Init<T>(string zooKeeperConnection, string collectionName) {
            EnsureRegistration(zooKeeperConnection);
            Parent.Container.Register<ISolrCloudBasicOperations<T>>(
                collectionName,
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudState>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
            Parent.Container.Register<ISolrCloudBasicReadOnlyOperations<T>>(
                collectionName,
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudState>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
            Parent.Container.Register<ISolrCloudOperations<T>>(
                collectionName,
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudState>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
            Parent.Container.Register<ISolrCloudReadOnlyOperations<T>>(
                collectionName,
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudState>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
        }

        private class OperationsProvider : ISolrOperationsProvider {
            private void EnsureRegistration<T>(string url) {
                lock (KnownRegistrations)
                    if (KnownRegistrations.Add(string.Concat(url, "/", typeof(T))))
                        Parent.Init<T>(url);
            }

            public ISolrBasicOperations<T> GetBasicOperations<T>(string url) {
                EnsureRegistration<T>(url);
                return Parent.Container.GetInstance<ISolrBasicOperations<T>>();
            }

            public ISolrOperations<T> GetOperations<T>(string url) {
                EnsureRegistration<T>(url);
                return Parent.Container.GetInstance<ISolrOperations<T>>();
            }
        }
    }
}