using System.Globalization;

namespace SolrNet.Cloud {
    internal class SolrClusterShardRange : ISolrClusterShardRange {
        public SolrClusterShardRange(int start, int end) {
            this.end = end;
            this.start = start;
        }

        private readonly int end;

        private readonly int start;

        public int End {
            get { return end; }
        }

        public int Start {
            get { return start; }
        }

        public static SolrClusterShardRange Parse(string range) {
            if(string.IsNullOrEmpty(range))
                return new SolrClusterShardRange(int.MinValue, int.MaxValue);
            var parts = range.Split('-');
            return new SolrClusterShardRange(
                int.Parse(parts[0], NumberStyles.HexNumber),
                int.Parse(parts[1], NumberStyles.HexNumber));
        }

        public override string ToString() {
            return string.Concat(
                start.ToString("X"),
                "-",
                end.ToString("X"));
        }
    }
}