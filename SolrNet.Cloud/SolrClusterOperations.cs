using System;
using System.Collections.Generic;
using SolrNet.ClusterStatus;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrNet.Cloud {
    internal class SolrClusterOperations<T> : ISolrOperations<T> {
        public SolrClusterOperations(ISolrClusterBalancer clusterBalancer, SolrClusterExceptionHandlers exceptionHandlers, int maxAttempts, ISolrOperationsProvider operationsProvider, ISolrClusterReplicas usableReplicas)
        {
            this.clusterBalancer = clusterBalancer;
            this.exceptionHandlers = exceptionHandlers;
            this.maxAttempts = maxAttempts;
            this.operationsProvider = operationsProvider;
            this.usableReplicas = usableReplicas;
        }

        private readonly ISolrClusterBalancer clusterBalancer;

        private readonly SolrClusterExceptionHandlers exceptionHandlers;

        private readonly int maxAttempts;

        private readonly ISolrOperationsProvider operationsProvider;

        private readonly ISolrClusterReplicas usableReplicas;

        private TResult Balance<TResult>(Func<ISolrOperations<T>, TResult> operation, bool write = false) {
            var attempt = 0;
            while (attempt++ < maxAttempts) {
                var replica = clusterBalancer.Balance(usableReplicas, write);
                if (replica == null)
                    throw new ApplicationException("No appropriate replica was selected to perform the operation.");
                var operations = operationsProvider.GetOperations<T>(replica.Url);
                if (operations == null)
                    throw new ApplicationException("Operation provider returned null.");
                try {
                    return operation(operations);
                } catch (Exception exception) {
                    // todo: possibly deactivate zombie node when status is not 401, 500
                    exceptionHandlers.Handle(exception);
                }
            }
            throw new ApplicationException("Atempts limit was depleted.");
        }

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return Balance(operations => operations.Query(query, options));
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options) {
            return Balance(operations => operations.MoreLikeThis(query, options));
        }

        public ResponseHeader Ping() {
            return Balance(operations => operations.Ping());
        }

        public SolrSchema GetSchema(string schemaFileName) {
            return Balance(operations => operations.GetSchema(schemaFileName));
        }

        public SolrClusterStatus GetClusterStatus() {
            return Balance(operations => operations.GetClusterStatus());
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options) {
            return Balance(operations => operations.GetDIHStatus(options));
        }

        public SolrQueryResults<T> Query(string q) {
            return Balance(operations => operations.Query(q));
        }

        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders) {
            return Balance(operations => operations.Query(q, orders));
        }

        public SolrQueryResults<T> Query(string q, QueryOptions options) {
            return Balance(operations => operations.Query(q, options));
        }

        public SolrQueryResults<T> Query(ISolrQuery q) {
            return Balance(operations => operations.Query(q));
        }

        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders) {
            return Balance(operations => operations.Query(query, orders));
        }

        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets) {
            return Balance(operations => operations.FacetFieldQuery(facets));
        }

        public ResponseHeader Commit() {
            return Balance(operations => operations.Commit(), true);
        }

        public ResponseHeader Rollback() {
            return Balance(operations => operations.Commit(), true);
        }

        public ResponseHeader Optimize() {
            return Balance(operations => operations.Commit(), true);
        }

        public ResponseHeader Add(T doc) {
            return Balance(operations => operations.Add(doc), true);
        }

        public ResponseHeader Add(T doc, AddParameters parameters) {
            return Balance(operations => operations.Add(doc, parameters), true);
        }

        public ResponseHeader AddWithBoost(T doc, double boost) {
            return Balance(operations => operations.AddWithBoost(doc, boost), true);
        }

        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters) {
            return Balance(operations => operations.AddWithBoost(doc, boost, parameters), true);
        }

        public ExtractResponse Extract(ExtractParameters parameters) {
            return Balance(operations => operations.Extract(parameters), true);
        }

        public ResponseHeader Add(IEnumerable<T> docs) {
            return Balance(operations => operations.Add(docs), true);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs) {
            return Balance(operations => operations.AddRange(docs), true);
        }

        public ResponseHeader Add(IEnumerable<T> docs, AddParameters parameters) {
            return Balance(operations => operations.Add(docs, parameters), true);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters) {
            return Balance(operations => operations.AddRange(docs, parameters), true);
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            return Balance(operations => operations.AddWithBoost(docs), true);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            return Balance(operations => operations.AddRangeWithBoost(docs), true);
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            return Balance(operations => operations.AddWithBoost(docs, parameters), true);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            return Balance(operations => operations.AddRangeWithBoost(docs, parameters), true);
        }

        public ResponseHeader Delete(T doc) {
            return Balance(operations => operations.Delete(doc), true);
        }

        public ResponseHeader Delete(T doc, DeleteParameters parameters) {
            return Balance(operations => operations.Delete(doc, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<T> docs) {
            return Balance(operations => operations.Delete(docs), true);
        }

        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters) {
            return Balance(operations => operations.Delete(docs, parameters), true);
        }

        public ResponseHeader Delete(ISolrQuery q) {
            return Balance(operations => operations.Delete(q), true);
        }

        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters) {
            return Balance(operations => operations.Delete(q, parameters), true);
        }

        public ResponseHeader Delete(string id) {
            return Balance(operations => operations.Delete(id), true);
        }

        public ResponseHeader Delete(string id, DeleteParameters parameters) {
            return Balance(operations => operations.Delete(id, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids) {
            return Balance(operations => operations.Delete(ids), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters) {
            return Balance(operations => operations.Delete(ids, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q) {
            return Balance(operations => operations.Delete(ids, q), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters) {
            return Balance(operations => operations.Delete(ids, q, parameters), true);
        }

        public ResponseHeader BuildSpellCheckDictionary() {
            return Balance(operations => operations.BuildSpellCheckDictionary(), true);
        }

        public IEnumerable<ValidationResult> EnumerateValidationResults() {
            return Balance(operations => operations.EnumerateValidationResults());
        }
    }
}