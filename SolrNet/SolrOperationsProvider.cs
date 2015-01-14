using System;
using System.Collections.Generic;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;

namespace SolrNet
{
    public class SolrOperationsProvider : ISolrOperationsProvider {
        private static readonly Type DictionaryType = typeof (Dictionary<string, object>);

        public ISolrBasicOperations<T> GetBasicOperations<T>(ISolrConnection connection)
        {
            var fieldParser = new DefaultFieldParser();
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            var visitor = new DefaultDocumentVisitor(mapper, fieldParser);
            var parser = typeof (T) == DictionaryType
                ? new SolrDictionaryDocumentResponseParser(fieldParser) as ISolrDocumentResponseParser<T>
                : new SolrDocumentResponseParser<T>(mapper, visitor, new SolrDocumentActivator<T>());
            var resultParser = new DefaultResponseParser<T>(parser) as ISolrAbstractResponseParser<T>;
            var fieldSerializer = new DefaultFieldSerializer();
            var querySerializer = new DefaultQuerySerializer(fieldSerializer);
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, fieldSerializer);
            // validate why only this?
            var mlthResultParser = new SolrMoreLikeThisHandlerQueryResultsParser<T>(new[] { resultParser });
            var executor = new SolrQueryExecuter<T>(resultParser, connection, querySerializer, facetQuerySerializer, mlthResultParser);
            var documentSerializer = typeof (T) == DictionaryType
                ? (ISolrDocumentSerializer<T>) new SolrDictionarySerializer(fieldSerializer)
                : new SolrDocumentSerializer<T>(mapper, fieldSerializer);
            var schemaParser = new SolrSchemaParser();
            var headerParser = new HeaderResponseParser<T>();
            var dihStatusParser = new SolrDIHStatusParser();
            var extractResponseParser = new ExtractResponseParser(headerParser);
            return new SolrBasicServer<T>(connection, executor, documentSerializer, schemaParser, headerParser, querySerializer, dihStatusParser, extractResponseParser);
        }

        public ISolrBasicOperations<T> GetBasicOperations<T>(string url)
        {
            return GetBasicOperations<T>(new SolrConnection(url));
        }

        public ISolrOperations<T> GetOperations<T>(string url)
        {
            return GetOperations<T>(new SolrConnection(url));
        }

        public ISolrOperations<T> GetOperations<T>(ISolrConnection connection)
        {
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            return new SolrServer<T>(
                GetBasicOperations<T>(connection),
                mapper,
                new MappingValidator(mapper, new IValidationRule[] {
                    new MappedPropertiesIsInSolrSchemaRule(),
                    new RequiredFieldsAreMappedRule(),
                    new UniqueKeyMatchesMappingRule(),
                    new MultivaluedMappedToCollectionRule()
                }));
        }
    }
}
