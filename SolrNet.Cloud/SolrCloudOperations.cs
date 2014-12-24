using System;
using System.Collections.Generic;
using SolrNet.ClusterStatus;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrNet.Cloud
{
    public class SolrCloudOperations<T>
        : ISolrCloudOperations<T> {

        public SolrCloudOperations(ISolrCloud cloud, IOperationsResolver resolver) {
            this.cloud = cloud;
            this.resolver = resolver;
        }

        private readonly ISolrCloud cloud;

        private readonly IOperationsResolver resolver;

        private TResult Perform<TResult>(Func<ISolrOperations<T>,  TResult> operation, bool leader = false) {
            var nodes = leader ? cloud.Leaders : cloud.Replicas;
            foreach (var node in nodes)
            {
                try {
                    return operation(resolver.Resolve<T>(node.Id));
                } catch (Exception exception) {
                    // todo
                }
            }
            throw new ApplicationException("No appropriate node found to perform this operation.");
        }

        public SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options) {
            return Perform(operations => operations.Query(query, options));
        }

        public SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options) {
            return Perform(operations => operations.MoreLikeThis(query, options));
        }

        public ResponseHeader Ping() {
            return Perform(operations => operations.Ping());
        }

        public SolrSchema GetSchema(string schemaFileName) {
            return Perform(operations => operations.GetSchema(schemaFileName));
        }

        public SolrClusterStatus GetClusterStatus() {
            return Perform(operations => operations.GetClusterStatus());
        }

        public SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options) {
            return Perform(operations => operations.GetDIHStatus(options));
        }

        public SolrQueryResults<T> Query(string q) {
            return Perform(operations => operations.Query(q));
        }

        public SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders) {
            return Perform(operations => operations.Query(q, orders));
        }

        public SolrQueryResults<T> Query(string q, QueryOptions options) {
            return Perform(operations => operations.Query(q, options));
        }

        public SolrQueryResults<T> Query(ISolrQuery q) {
            return Perform(operations => operations.Query(q));
        }

        public SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders) {
            return Perform(operations => operations.Query(query, orders));
        }

        public ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets) {
            return Perform(operations => operations.FacetFieldQuery(facets));
        }

        public ResponseHeader Commit() {
            return Perform(operations => operations.Commit(), true);
        }

        public ResponseHeader Rollback() {
            return Perform(operations => operations.Commit(), true);
        }

        public ResponseHeader Optimize() {
            return Perform(operations => operations.Commit(), true);
        }

        public ResponseHeader Add(T doc) {
            return Perform(operations => operations.Add(doc), true);
        }

        public ResponseHeader Add(T doc, AddParameters parameters) {
            return Perform(operations => operations.Add(doc, parameters), true);
        }

        public ResponseHeader AddWithBoost(T doc, double boost) {
            return Perform(operations => operations.AddWithBoost(doc, boost), true);
        }

        public ResponseHeader AddWithBoost(T doc, double boost, AddParameters parameters) {
            return Perform(operations => operations.AddWithBoost(doc, boost, parameters), true);
        }

        public ExtractResponse Extract(ExtractParameters parameters) {
            return Perform(operations => operations.Extract(parameters), true);
        }

        public ResponseHeader Add(IEnumerable<T> docs) {
            return Perform(operations => operations.Add(docs), true);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs) {
            return Perform(operations => operations.AddRange(docs), true);
        }

        public ResponseHeader Add(IEnumerable<T> docs, AddParameters parameters) {
            return Perform(operations => operations.Add(docs, parameters), true);
        }

        public ResponseHeader AddRange(IEnumerable<T> docs, AddParameters parameters) {
            return Perform(operations => operations.AddRange(docs, parameters), true);
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            return Perform(operations => operations.AddWithBoost(docs), true);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs) {
            return Perform(operations => operations.AddRangeWithBoost(docs), true);
        }

        public ResponseHeader AddWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            return Perform(operations => operations.AddWithBoost(docs, parameters), true);
        }

        public ResponseHeader AddRangeWithBoost(IEnumerable<KeyValuePair<T, double?>> docs, AddParameters parameters) {
            return Perform(operations => operations.AddRangeWithBoost(docs, parameters), true);
        }

        public ResponseHeader Delete(T doc) {
            return Perform(operations => operations.Delete(doc), true);
        }

        public ResponseHeader Delete(T doc, DeleteParameters parameters) {
            return Perform(operations => operations.Delete(doc, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<T> docs) {
            return Perform(operations => operations.Delete(docs), true);
        }

        public ResponseHeader Delete(IEnumerable<T> docs, DeleteParameters parameters) {
            return Perform(operations => operations.Delete(docs, parameters), true);
        }

        public ResponseHeader Delete(ISolrQuery q) {
            return Perform(operations => operations.Delete(q), true);
        }

        public ResponseHeader Delete(ISolrQuery q, DeleteParameters parameters) {
            return Perform(operations => operations.Delete(q, parameters), true);
        }

        public ResponseHeader Delete(string id) {
            return Perform(operations => operations.Delete(id), true);
        }

        public ResponseHeader Delete(string id, DeleteParameters parameters) {
            return Perform(operations => operations.Delete(id, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids) {
            return Perform(operations => operations.Delete(ids), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, DeleteParameters parameters) {
            return Perform(operations => operations.Delete(ids, parameters), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q) {
            return Perform(operations => operations.Delete(ids, q), true);
        }

        public ResponseHeader Delete(IEnumerable<string> ids, ISolrQuery q, DeleteParameters parameters) {
            return Perform(operations => operations.Delete(ids, q, parameters), true);
        }

        public ResponseHeader BuildSpellCheckDictionary() {
            return Perform(operations => operations.BuildSpellCheckDictionary(), true);
        }

        public IEnumerable<ValidationResult> EnumerateValidationResults() {
            return Perform(operations => operations.EnumerateValidationResults());
        }
    }
}