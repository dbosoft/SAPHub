using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SAPHub.ApiModule.ApiModel
{
    /// <summary>
    /// Model of a API result. It will either contain the result data
    /// or error information.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OperationResult<T>
    {
        /// <summary>
        /// Result of operation. Can be null
        /// </summary>
        [AllowNull] public IEnumerable<T> Result { get; set; }
        public OperationStatus OperationStatus { get; set; }
        public Guid OperationId { get; set; }

        /// <summary>
        /// Message set for operation. Can be null.
        /// </summary>
        [AllowNull] public string Message { get; set; }
    }
}