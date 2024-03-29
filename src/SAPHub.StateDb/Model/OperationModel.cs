﻿using System;

namespace SAPHub.StateDb.Model;

public class OperationModel
{
    public Guid Id { get; set; }

    public OperationStatus Status { get; set; }

    public string StatusMessage { get; set; }

    public string ResultData { get; set; }
    public string ResultType { get; set; }


    public byte[] Timestamp { get; set; }
}