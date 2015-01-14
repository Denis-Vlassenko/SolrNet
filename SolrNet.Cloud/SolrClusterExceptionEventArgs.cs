using System;

namespace SolrNet.Cloud {
    public class SolrClusterExceptionEventArgs : EventArgs {
        public SolrClusterExceptionEventArgs(Exception exception) {
            Exception = exception;
        }

        public Exception Exception { get; private set; }
    }
}
