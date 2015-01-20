using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Org.Apache.Zookeeper.Data;
using SolrNet.Cloud.ZooKeeperClient;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;
using SolrNet.Utils;
using Parent = SolrNet.Startup;

namespace SolrNet.Cloud
{
    public static class Startup {
        static Startup() {
            AppDomain.CurrentDomain.DomainUnload += Dispose;
            CloudStatesProviders = new Dictionary<string, ISolrCloudStateProvider>(StringComparer.OrdinalIgnoreCase);
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
            Parent.Container.Register<ISolrBasicOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));

            Parent.Container.Register<ISolrBasicReadOnlyOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));

            Parent.Container.Register<ISolrOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));

            Parent.Container.Register<ISolrReadOnlyOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>()));
        }

        public static void Init<T>(string zooKeeperConnection, string collectionName) {
            EnsureRegistration(zooKeeperConnection);
            Parent.Container.Register<ISolrBasicOperations<T>>(
                collectionName,
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));

            Parent.Container.Register<ISolrBasicReadOnlyOperations<T>>(
                collectionName,
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));

            Parent.Container.Register<ISolrOperations<T>>(
                collectionName,
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));

            Parent.Container.Register<ISolrReadOnlyOperations<T>>(
                collectionName,
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(zooKeeperConnection),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));
        }

        private class OperationsProvider : ISolrOperationsProvider {
            public ISolrBasicOperations<T> GetBasicOperations<T>(string url) {
                return SolrNet.GetBasicServer<T>(url);
            }

            public ISolrOperations<T> GetOperations<T>(string url) {
                return SolrNet.GetServer<T>(url);
            }
        }
    }
}