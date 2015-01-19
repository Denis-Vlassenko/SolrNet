namespace SolrNet.Cloud {
    public class SolrCloudRouter {
        public SolrCloudRouter(string name) {
            Name = name;
        }

        public string Name { get; set; }

        public static string Explicit = "explicit";
        public static string CompositId = "compositeId";

    }
}
