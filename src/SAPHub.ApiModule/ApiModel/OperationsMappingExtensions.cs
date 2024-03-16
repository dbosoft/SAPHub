using SAPHub.StateDb.Model;

namespace SAPHub.ApiModule.ApiModel;

public static class OperationsMappingExtensions
{
    /// <summary>
    /// Converts a <see cref="OperationModel"/> from state db to REST API model <see cref="Operation"/>
    /// </summary>
    /// <param name="operation">state db operation</param>
    /// <returns></returns>
    public static Operation ToApiModel(this OperationModel operation) =>
        operation == null 
            ? null 
            : new Operation{ Id = operation.Id, Status = operation.Status };
}