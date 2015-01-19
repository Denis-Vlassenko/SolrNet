using System;
using System.Collections.Generic;
using SolrNet.Cloud.ZooKeeperClient;
using SolrNet.Utils;
using Parent = SolrNet.Startup;

namespace SolrNet.Cloud
{
    public static class Startup {
        static Startup() {
            AppDomain.CurrentDomain.DomainUnload += Dispose;
            CloudStatesProviders = new Dictionary<string, ISolrCloudStateProvider>(StringComparer.OrdinalIgnoreCase);
            KnownRegistrations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public static IContainer Container {
            get { return Parent.Container; }
        }

        private static void Dispose(object sender, EventArgs eventArgs) {
            lock (CloudStatesProviders) {
                foreach (var state in CloudStatesProviders.Values)
                    state.Dispose();
                CloudStatesProviders.Clear();
            }
        }

        private static readonly Dictionary<string, ISolrCloudStateProvider> CloudStatesProviders;

        private static readonly HashSet<string> KnownRegistrations;

        private static void EnsureRegistration(string zooKeeperConnection) {
            lock (CloudStatesProviders) {
                ISolrCloudStateProvider provider;
                if (CloudStatesProviders.Count == 0) {
                    var operationsProvider = new OperationsProvider();
                    Parent.Container.Register<ISolrOperationsProvider>(c => operationsProvider);
                }
                if (CloudStatesProviders.TryGetValue(zooKeeperConnection, out provider))
                    return;
                provider = new SolrCloudStateProvider(zooKeeperConnection);
                provider.Init();
                CloudStatesProviders.Add(zooKeeperConnection, provider);
                Parent.Container.Register(zooKeeperConnection, container => provider);
            }
        }

        public static void Init<T>(string zooKeeperConnection) {
            EnsureRegistration(zooKeeperConnection);
            Parent.Container.Register<ISolrCloudBasicOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
            Parent.Container.Register<ISolrCloudBasicReadOnlyOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
            Parent.Container.Register<ISolrCloudOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
            Parent.Container.Register<ISolrCloudReadOnlyOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
        }

        public static void Init<T>(string zooKeeperConnection, string collectionName) {
            EnsureRegistration(zooKeeperConnection);
            Parent.Container.Register<ISolrCloudBasicOperations<T>>(
                collectionName,
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
            Parent.Container.Register<ISolrCloudBasicReadOnlyOperations<T>>(
                collectionName,
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
            Parent.Container.Register<ISolrCloudOperations<T>>(
                collectionName,
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
            Parent.Container.Register<ISolrCloudReadOnlyOperations<T>>(
                collectionName,
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
        }

        private class OperationsProvider : ISolrOperationsProvider {
            private void EnsureRegistration<T>(string url) {
                lock (KnownRegistrations)
                    if (KnownRegistrations.Add(string.Concat(url, "/", typeof(T).FullName)))
                        Parent.Init<T>(url);
            }

            public ISolrBasicOperations<T> GetBasicOperations<T>(string url) {
                EnsureRegistration<T>(url);
                return Parent.Container.GetInstance<ISolrBasicOperations<T>>(url);
            }

            public ISolrOperations<T> GetOperations<T>(string url) {
                EnsureRegistration<T>(url);
                return Parent.Container.GetInstance<ISolrOperations<T>>(url);
            }
        }
    }
}