﻿using System;
using System.Collections.Generic;

namespace SAPHub.ApiModule.ApiModel
{
    public class OperationResult<T>
    {
        public IEnumerable<T> Result { get; set; }
        public OperationStatus OperationStatus { get; set; }
        public Guid OperationId { get; set; }
        public string Message { get; set; }
    }
}