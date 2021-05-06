using System;
using System.Collections.Generic;

namespace SAPHub.ApiModule.ApiModel
{
    public class Operation
    {
        public Guid Id { get; set; }
        public OperationStatus Status { get; set; }
    }
}