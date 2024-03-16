using System;
using Rebus.DataBus;

namespace SAPHub.Messages;

public class OperationStatusEvent
{
    public Guid Id { get; set; }
    public Guid OperationId { get; set; }

    public OperationStatus Status { get; set; }

    public DataBusAttachment Attachment { get; set; }
    public string ResultType { get; set; }

    public string StatusMessage { get; set; }
}