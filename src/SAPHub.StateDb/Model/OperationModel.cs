using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SAPHub.StateDb.Model
{
    public class OperationModel
    {
        public Guid Id { get; set; }

        public OperationStatus Status { get; set; }

        public string StatusMessage { get; set; }

        public string ResultData { get; set; }
        public string ResultType { get; set; }


        [UsedImplicitly] 
        public byte[] Timestamp { get; set; }
    }

}