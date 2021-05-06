# SAPHub
A sample implementation for Husuto and YaNco

This is a sample application to demonstrate how you can combine [Hosuto](https://github.com/dbosoft/Hosuto) and [YaNco](https://github.com/dbosoft/YaNco) to build a reliable and scaleable service with SAP backend integration.


# Requirements

**Visual Studio 2019**

It is strongly recommended to use a current Visual Studio 2019. If you would like to try the docker or azure integration also the docker/azure tools have to be installed.

**SAP RFC SDK**

Please note that to build and run this project you have to download the SAP Netweaver RFC SDK from the SAP Support Portal. 
Please see the requirements section for YaNco how to obtain the SDK: https://github.com/dbosoft/YaNco#platforms--prerequisites 



# Usage

This sample uses [Hosuto](https://github.com/dbosoft/Hosuto) for building microservices that can be either combined in one process or run standalone. 
So the application can be either be fully distributed (and scaled) or run as a single process. Hosuto calls the microservice implementation **Module**, and the microservice runtime process **App**:

## Apps

The SAPHub comes with 3 pre-build applications:
- **SAPHub.Server**
  
  This is the "all-in-one" server, that both runs the API Module and SAP Connector Module. It requires no additional Message Bus system and runs only as a console app on Windows. 

- **SAPHub.ApiEndpoint**
  
  The API Endpoint runs only the API Module in a aspnetcore 5.0 environment. You can host it in a container or on a hyperscaler like AWS/Azure. 
  To communicate with the SAP Connector Module it requires a Message Bus system to be set up.

- **SAPHub.SAPConnectorService**
  
  The SAPConnector Service runs only the SAPConnector Module. 
  You can run it on a on-premise Windows system that has direct network access to the SAP system.
  To communicate with the API Module it requires a Message Bus system to be set up.



## Modules

### API Module
The sample provides a REST API to read company records from the SAP backend asynchronously. 

![swagger screenshot](https://raw.githubusercontent.com/dbosoft/SAPHub/main/.github/swagger.png)

A request of /Company will not directly return the company data, but responds with a Operation record. The record contains the operation id. 
This operation id can be used to query the status of operation. 

``` json
{
  "id": "49d017f5-4a1b-4d32-aee2-5986daa7f211",
  "status":0
}
```

Once it is finished the data can be requested with `/Company/result/<operationId>`

**Please note:**
For simplification the API Module / API Endpoint is currently not fully horizontal scaleable. 
Each instance of the API Module keeps it's own copy of operation states in memory. For a production implementation the state should be shared/stored in a database. 

### SAP Connector Module

The SAP connector module will be used to establish the connection to the SAP system. For development we used our internal ERP EHP 8 IDES system, but it should work with almost any ERP or S/4 system.

To setup the SAP Connector Module you have to provide the connection settings. If you cloned the project you should use [User Secrets](https://blog.elmah.io/asp-net-core-not-that-secret-user-secrets-explained/). But you can also use the [appsetings.json](https://github.com/dbosoft/SAPHub/blob/main/src/SAPHub.Server/appsettings.json) to set the connection settings. 

Example:

``` json
{
  "saprfc": {
    "ashost": "<your SAP system hostname>",
    "sysnr": "<SAP system No>",
    "client": "<SAP Client No>",
    "user": "<UserName>",
    "passwd": "<Password>",
    "lang": "EN"
  }
}
```

## Message Bus
The API Module and the SAP Connector module communicate asynchronously via a message bus. The following messages buses are currently supported:

### Azure Queues
   TBD
   
### Azure Service Bus
   TBD
   
### RabbitMq
   TBD
   
