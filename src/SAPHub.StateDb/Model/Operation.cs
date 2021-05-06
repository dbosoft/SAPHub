using System;
using System.ComponentModel.DataAnnotations;

namespace SAPHub.StateDb.Model
{
    public class OperationModel
    {
        public Guid Id { get; set; }

        [ConcurrencyCheck]
        public OperationStatus Status { get; set; }

        public string StatusMessage { get; set; }

        public string ResultData { get; set; }
        public string ResultType { get; set; }


        [Timestamp]
        public byte[] Timestamp { get; set; }
    }

}