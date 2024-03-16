using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SAPHub.UI.Shared;

[PublicAPI]
public class OperationResult<T>
{
    public IEnumerable<T> Result { get; set; }
    public OperationStatus OperationStatus { get; set; }
    public Guid OperationId { get; set; }
    public string Message { get; set; }
}

public enum OperationStatus
{
    Queued,
    Running,
    Failed,
    Completed
}