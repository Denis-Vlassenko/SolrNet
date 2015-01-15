using System;

namespace SolrNet.Cloud {
    public class SolrCloudExceptionEventArgs : EventArgs {
        public SolrCloudExceptionEventArgs(Exception exception) {
            Exception = exception;
        }

        public Exception Exception { get; private set; }
    }
}
