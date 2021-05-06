using System;
using SAPHub.StateDb.Model;

namespace SAPHub.ApiModule.ApiModel
{
    public static class OperationsMappingExtensions
    {
        public static OperationModel ToStateModel(this Operation operation)
        {
            return new OperationModel {Id = operation.Id, Status = operation.Status};
        }

        public static Operation ToApiModel(this OperationModel operation)
        {
            if (operation == null)
                return null;

            return new Operation{ Id = operation.Id, Status = operation.Status };
        }


    }
}