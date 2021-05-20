using System;

namespace SAPHub.ApiModule.ApiModel
{
    /// <summary>
    /// REST API Model of a operation requested on API
    /// </summary>
    public class Operation
    {
        public Guid Id { get; set; }
        public OperationStatus Status { get; set; }
    }
}