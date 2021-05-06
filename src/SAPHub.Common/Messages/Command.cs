using System;

namespace SAPHub
{
    public class OperationStatusEvent
    {
        public Guid Id { get; set; }
        public Guid OperationId { get; set; }

        public OperationStatus Status { get; set; }

        public string ResultData { get; set; }
        public string ResultType { get; set; }

        public string StatusMessage { get; set; }
    }
}