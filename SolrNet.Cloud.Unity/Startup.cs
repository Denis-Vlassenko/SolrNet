using System;
using System.Collections.Generic;
using SolrNet.Utils;
using Parent = SolrNet.Startup;

namespace SolrNet.Cloud
{
    public static class Startup {
        static Startup() {
            Collections = new HashSet<string>();
            Providers = new Dictionary<string, ISolrCloudStateProvider>(StringComparer.OrdinalIgnoreCase);
        }

        public static IContainer Container {
            get { return Parent.Container; }
        }

        private static readonly HashSet<string> Collections;

        private static readonly IDictionary<string, ISolrCloudStateProvider> Providers;

        private static void EnsureRegistration(ISolrCloudStateProvider cloudStateProvider) {
            if (Providers.Count == 0)
                Parent.Container.Register<ISolrOperationsProvider>(c => new OperationsProvider());
            if (Providers.ContainsKey(cloudStateProvider.Key))
                return;
            cloudStateProvider.Init();
            Providers.Add(cloudStateProvider.Key, cloudStateProvider);
            Parent.Container.Register(cloudStateProvider.Key, container => cloudStateProvider);
        }

        public static void Init<T>(ISolrCloudStateProvider cloudStateProvider) {
            if (cloudStateProvider == null)
                throw new ArgumentNullException("cloudStateProvider");
            EnsureRegistration(cloudStateProvider);

            if (!Collections.Add(string.Empty))
                return;

            Parent.Container.Register<ISolrBasicOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>()));

            Parent.Container.Register<ISolrBasicReadOnlyOperations<T>>(
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>()));

            Parent.Container.Register<ISolrOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>()));

            Parent.Container.Register<ISolrReadOnlyOperations<T>>(
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>()));
        }

        public static void Init<T>(ISolrCloudStateProvider cloudStateProvider, string collectionName) {
            if (cloudStateProvider == null)
                throw new ArgumentNullException("cloudStateProvider");
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException("collectionName");
            EnsureRegistration(cloudStateProvider);

            if (!Collections.Add(collectionName))
                return;

            Parent.Container.Register<ISolrBasicOperations<T>>(
                collectionName,
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));

            Parent.Container.Register<ISolrBasicReadOnlyOperations<T>>(
                collectionName,
                container => new SolrCloudBasicOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));

            Parent.Container.Register<ISolrOperations<T>>(
                collectionName,
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    container.GetInstance<ISolrOperationsProvider>(),
                    collectionName));

            Parent.Container.Register<ISolrReadOnlyOperations<T>>(
                collectionName,
                container => new SolrCloudOperations<T>(
                    container.GetInstance<ISolrCloudStateProvider>(cloudStateProvider.Key),
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