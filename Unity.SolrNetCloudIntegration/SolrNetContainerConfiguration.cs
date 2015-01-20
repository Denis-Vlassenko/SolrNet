using System;
using Microsoft.Practices.Unity;
using SolrNet;
using SolrNet.Cloud;

namespace Unity.SolrNetCloudIntegration
{
    public class SolrNetContainerConfiguration
    {
        public IUnityContainer ConfigureContainer(ISolrCloudStateProvider cloudStateProvider, IUnityContainer container) {
            if (cloudStateProvider == null)
                throw new ArgumentNullException("cloudStateProvider");
            if (container == null)
                throw new ArgumentNullException("container");
            if (container.IsRegistered<ISolrCloudStateProvider>(cloudStateProvider.Key))
                return container;
            cloudStateProvider.Init();
            foreach (var collection in cloudStateProvider.GetCloudState().Collections.Keys) {
                if (!container.IsRegistered<ISolrCloudStateProvider>())
                    RegisterFirstCollection(cloudStateProvider, container);
                RegisterCollection(cloudStateProvider, collection, container);
            }
            container.RegisterInstance(cloudStateProvider.Key, cloudStateProvider);
            if (!container.IsRegistered<ISolrOperationsProvider>())
                container.RegisterInstance<ISolrOperationsProvider>(new OperationsProvider());
            //container.RegisterType<IReadOnlyMappingManager, MemoizingMappingManager>(new InjectionConstructor(new ResolvedParameter(typeof(AttributesMappingManager))));
            //container.RegisterType(typeof(ISolrDocumentActivator<>), typeof(SolrDocumentActivator<>));
            //container.RegisterType(typeof(ISolrQueryExecuter<>), typeof(SolrQueryExecuter<>));
            //container.RegisterType<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
            //container.RegisterType<IMappingValidator, MappingValidator>();
            //RegisterParsers(container);
            //RegisterValidationRules(container);
            //RegisterSerializers(container);
            return container;
        }

        private static void RegisterCollection(ISolrCloudStateProvider cloudStateProvider, string collection, IUnityContainer container) {
            var injection = new InjectionConstructor(
                    new ResolvedParameter<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    new ResolvedParameter<ISolrOperationsProvider>(),
                    collection);
            container.RegisterType(typeof(ISolrBasicOperations<>), typeof(SolrCloudBasicOperations<>), collection, injection);
            container.RegisterType(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrCloudBasicOperations<>), collection, injection);
            container.RegisterType(typeof(ISolrOperations<>), typeof(SolrCloudOperations<>), collection, injection);
            container.RegisterType(typeof(ISolrReadOnlyOperations<>), typeof(SolrCloudOperations<>), collection, injection);
        }

        private static void RegisterFirstCollection(ISolrCloudStateProvider cloudStateProvider, IUnityContainer container) {
            var injection = new InjectionConstructor(
                    new ResolvedParameter<ISolrCloudStateProvider>(cloudStateProvider.Key),
                    new ResolvedParameter<ISolrOperationsProvider>());
            container.RegisterType(typeof(ISolrBasicOperations<>), typeof(SolrCloudBasicOperations<>), injection);
            container.RegisterType(typeof(ISolrBasicReadOnlyOperations<>), typeof(SolrCloudBasicOperations<>), injection);
            container.RegisterType(typeof(ISolrOperations<>), typeof(SolrCloudOperations<>), injection);
            container.RegisterType(typeof(ISolrReadOnlyOperations<>), typeof(SolrCloudOperations<>), injection);
        }

        //private static void RegisterCore(IUnityContainer container, Type documentType, string url) {
        //    var name = string.Concat(url, "/", documentType.FullName);
        //    if (container.IsRegistered<ISolrConnection>(name))
        //        return;
        //    container.RegisterType<ISolrConnection, SolrConnection>(name, new InjectionConstructor(url));
        //    RegisterSolrQueryExecuter(container, documentType, name);
        //    RegisterBasicOperations(container, documentType, name);
        //    RegisterSolrOperations(container, documentType, name);
        //}

        //private static void RegisterParsers(IUnityContainer container)
        //{
        //    container.RegisterType(typeof(ISolrDocumentResponseParser<>), typeof(SolrDocumentResponseParser<>));
        //    container.RegisterType<ISolrDocumentResponseParser<Dictionary<string, object>>, SolrDictionaryDocumentResponseParser>();
        //    container.RegisterType(typeof(ISolrAbstractResponseParser<>), typeof(DefaultResponseParser<>));
        //    container.RegisterType(typeof(ISolrAbstractResponseParser<>), typeof(DefaultResponseParser<>), "UnityFix");
        //    container.RegisterType<ISolrHeaderResponseParser, HeaderResponseParser<string>>();
        //    container.RegisterType<ISolrExtractResponseParser, ExtractResponseParser>();
        //    container.RegisterType(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>), typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
        //    container.RegisterType<ISolrFieldParser, DefaultFieldParser>();
        //    container.RegisterType<ISolrSchemaParser, SolrSchemaParser>();
        //    container.RegisterType<ISolrDIHStatusParser, SolrDIHStatusParser>();
        //    container.RegisterType<ISolrStatusResponseParser, SolrStatusResponseParser>();
        //    container.RegisterType<ISolrCoreAdmin, SolrCoreAdmin>();
        //}

        //private static void RegisterSerializers(IUnityContainer container)
        //{
        //    container.RegisterType(typeof(ISolrDocumentSerializer<>), typeof(SolrDocumentSerializer<>));
        //    container.RegisterType(typeof(ISolrDocumentSerializer<Dictionary<string, object>>), typeof(SolrDictionarySerializer));
        //    container.RegisterType<ISolrFieldSerializer, DefaultFieldSerializer>();
        //    container.RegisterType<ISolrQuerySerializer, DefaultQuerySerializer>();
        //    container.RegisterType<ISolrFacetQuerySerializer, DefaultFacetQuerySerializer>();
        //}

        //private static void RegisterSolrOperations(IUnityContainer container, Type documentType, string name)
        //{
        //    var server = typeof(SolrServer<>).MakeGenericType(documentType);
        //    var injectionConstructor = new InjectionConstructor(
        //        new ResolvedParameter(typeof(ISolrBasicOperations<>).MakeGenericType(documentType), name),
        //        new ResolvedParameter(typeof(IReadOnlyMappingManager)),
        //        new ResolvedParameter(typeof(IMappingValidator)));
        //    container.RegisterType(
        //        typeof(ISolrOperations<>).MakeGenericType(documentType), 
        //        server,
        //        name, 
        //        injectionConstructor);
        //    container.RegisterType(
        //        typeof(ISolrReadOnlyOperations<>).MakeGenericType(documentType), 
        //        server,
        //        name, 
        //        injectionConstructor);
        //}

        //private static void RegisterBasicOperations(IUnityContainer container, Type documentType, string name)
        //{
        //    var basicServer = typeof(SolrBasicServer<>).MakeGenericType(documentType);
        //    var injectionParameters = new InjectionConstructor(
        //       new ResolvedParameter(typeof(ISolrConnection), name),
        //       new ResolvedParameter(typeof(ISolrQueryExecuter<>).MakeGenericType(documentType), name),
        //       new ResolvedParameter(typeof(ISolrDocumentSerializer<>).MakeGenericType(documentType)),
        //       new ResolvedParameter(typeof(ISolrSchemaParser)),
        //       new ResolvedParameter(typeof(ISolrHeaderResponseParser)),
        //       new ResolvedParameter(typeof(ISolrQuerySerializer)),
        //       new ResolvedParameter(typeof(ISolrDIHStatusParser)),
        //       new ResolvedParameter(typeof(ISolrExtractResponseParser)));
        //    container.RegisterType(
        //        typeof(ISolrBasicOperations<>).MakeGenericType(documentType), 
        //        basicServer,
        //        name, 
        //        injectionParameters);
        //    container.RegisterType(
        //        typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(documentType), 
        //        basicServer,
        //        name, 
        //        injectionParameters);
        //}

        //private static void RegisterSolrQueryExecuter(IUnityContainer container, Type documentType, string name) {
        //    container.RegisterType(
        //        typeof (ISolrQueryExecuter<>).MakeGenericType(documentType),
        //        typeof (SolrQueryExecuter<>).MakeGenericType(documentType),
        //        name,
        //        new InjectionConstructor(
        //            new ResolvedParameter(typeof (ISolrAbstractResponseParser<>).MakeGenericType(documentType)),
        //            new ResolvedParameter(typeof (ISolrConnection), name),
        //            new ResolvedParameter(typeof (ISolrQuerySerializer)),
        //            new ResolvedParameter(typeof (ISolrFacetQuerySerializer)),
        //            new ResolvedParameter(typeof (ISolrMoreLikeThisHandlerQueryResultsParser<>).MakeGenericType(documentType))));
        //}

        //private static void RegisterValidationRules(IUnityContainer container)
        //{
        //    var validationRules = new[] {
        //        typeof (MappedPropertiesIsInSolrSchemaRule),
        //        typeof (RequiredFieldsAreMappedRule),
        //        typeof (UniqueKeyMatchesMappingRule),
        //        typeof (MultivaluedMappedToCollectionRule)
        //    };
        //    foreach (var validationRule in validationRules)
        //        container.RegisterType(typeof (IValidationRule), validationRule);
        //}

        private class OperationsProvider : ISolrOperationsProvider
        {
            //public OperationsProvider(IUnityContainer container) {
            //    this.container = container;
            //}

            // private readonly IUnityContainer container;

            public ISolrBasicOperations<T> GetBasicOperations<T>(string url)
            {
                // RegisterCore(container, typeof(T), url);
                // return container.Resolve<ISolrBasicOperations<T>>(url);
                return SolrNet.SolrNet.GetBasicServer<T>(url);
            }

            public ISolrOperations<T> GetOperations<T>(string url)
            {
                // RegisterCore(container, typeof(T), url);
                // return container.Resolve<ISolrOperations<T>>(url);
                return SolrNet.SolrNet.GetServer<T>(url);
            }
        }
    }
}