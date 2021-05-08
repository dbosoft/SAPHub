# SAPHub
A sample implementation for Husuto and YaNco

This is a sample application to demonstrate how you can combine [Hosuto](https://github.com/dbosoft/Hosuto) and [YaNco](https://github.com/dbosoft/YaNco) to build a reliable and scaleable service with SAP backend integration.


# Requirements

Beside of cloning this repo please consider following requirements:

* **Visual Studio 2019**  
  It is strongly recommended to use at least Visual Studio 2019 16.8.4 to build/run the applications.   
  If you would like to try the docker or azure integration also the docker/azure tools have to be installed.

* **SAP RFC SDK**  
  Please note that to build and run this project you have to download the SAP Netweaver RFC SDK from the SAP Support Portal.   
  Please see the requirements section for YaNco how to obtain the SDK: https://github.com/dbosoft/YaNco#platforms--prerequisites 

  You have to download both the Windows_X64 and Linux_x64 Netweaver RFC SDK binaries.
  Copy them to repos nwrfcsdk directory.

* **Azure account**  
  To run only locally no azure account is required. However to scale out or to use the Azure Service Bus you require a azure account ([free accounts available](https://azure.microsoft.com/en-us/free) for dev purposes),

# Usage

This sample uses [Hosuto](https://github.com/dbosoft/Hosuto) for building microservices that can be either combined in one process or run standalone. 
So the application can be either be fully distributed (and scaled) or run as a single process. Hosuto calls the microservice implementation **Module**, and the microservice runtime process **App**:

## Apps

The SAPHub comes with 3 pre-build applications:

### SAPHub.Server
  
This is the "all-in-one" server, that both runs the API Module and SAP Connector Module. It requires no additional message exchange system and runs as a console app (only Windows).   

**Quickstart:**  
To run this application from Visual Studio first configure the SAP connection settings in your user secrets ([see below](#sap-connection)) on project **SAPHub.Server**.  
Then start **SAPHub.Server** project and navigate to http://localhost:62089/api to access the Api.Endpoint.

### SAPHub.ApiEndpoint
  
The API Endpoint runs only the API Module in a aspnetcore 5.0 environment. You can host it in a container or on a hyperscaler like AWS/Azure. 
To communicate with the SAP Connector Module it requires a message exchange system to be set up. To scale horizontally you have to set up a Cosmos DB (see below).

**Quickstart:**  
You can start this app together with SAPHub.Connector and RabbitMq as message exchange using docker compose. See [Quickstart for SAPHub.SAPConnector](#saphubsapconnector). 
For other setups please check [configuration](#configuration) section below. 

### SAPHub.SAPConnector
  
The SAPConnector Service runs only the SAPConnector Module. 
You can run only in a network that has direct access to the SAP system.
To communicate with the API Module it requires a message exchange to be set up ([see below](#message-exchanges)).

**Quickstart:**  
You can start this app together with SAPHub.ApiEndpoint and RabbitMq as message exchange using docker compose.  
1. Configure the SAP connection settings in your user secrets ([see below](#sap-connection)) on project **SAPHub.SAPConnector**.  
2. Open a command prompt in project directory.
3. Run command `docker-compose up`.  

This will automatically start docker containers running rabbitmq, SAPHub.ApiEndpoint and SAPHub.SAPConnector.

For other setups please check [configuration](#configuration) section below.  


## Modules

### API Module

The sample provides a REST API to read company records from the SAP backend asynchronously. 

![swagger screenshot](https://raw.githubusercontent.com/dbosoft/SAPHub/main/.github/swagger.png)

**Requests / Responses**  
A request of `/company` will not directly return the company data, but responds with a `Operation` record. The record contains the operation id. 
This operation id can be used to query the status of operation on `/operation/<operationId>`. 

*Query response:*
``` json
{
"id": "49d017f5-4a1b-4d32-aee2-5986daa7f211",
"status":0
}
```

Once it is finished the data can be requested with `/Company/result/<operationId>`

**Message Flow**  
1. For each request the API module first creates a record for the operation in it's state store. 
2. Then it sends the operation to the SAP Connector queue for processing. 
3. When a SAP Connector reports status change events, it updates the state of operation in it's state store. 

**Scaling**  
To scale the API Module horizontally you have to enable the cosmos db storage.  
Reason: when a SAP Connector sends a status event update only one API Endpoint will process the changes and update it's state store. Therefore all API Module instances have to use the same state store. 

### SAP Connector Module

The SAP connector module will be used to establish the connection to the SAP system. For development we used our internal ERP EHP 8 IDES system, but it should work with almost any ERP or S/4 system.

**Message Flow**  
1. For each operation recieved the SAP Connector will establish a connection to the SAP system to retrieve the data (2 requests max. in parallel). 
2. Then it sends a operation update event with result or error state. 

**Scaling**  
The SAP Connector can be scaled freely.  
Keep in mind that each SAP Connector will use up to 3 connections (2 processors, 1 for metadata) at same time to backend, so do not overload the backend SAP system with to many connectors. 

## Configuration

The apps are configured by .NET configuration settings.
Following methods for setting the config are supported:
- [User Secrets](https://blog.elmah.io/asp-net-core-not-that-secret-user-secrets-explained/)  
  This is the prefered method, as it will automatically be used in Visual Studio and containers.

- [appsetings.json](https://github.com/dbosoft/SAPHub/blob/main/src/SAPHub.Server/appsettings.json)

- Environment Variables  
  You can also use environment variables prefixed with *SAPHUB_* to set configuration settings. For example the bus type: 
  `SAPHUB_BUS__TYPE=rabbitmq`

The following settings are required/supported in the apps:

* SAPHub.Server:
  - SAP Connection (required)
  - Message Exchange (optional)
  - CosmosDb (optional)

* SAPHub.ApiEndpoint:
  - Message Exchange (required)
  - CosmosDb (optional, required to scale)

* SAPHub.SAPConnector:
  - Message Exchange (required)
  - SAP Connection (required)


### SAP Connection
For the SAP Connector Module you have to configure the SAP connection.

**Configuration:**

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


### Message Exchanges
The API Module and the SAP Connector module communicate asynchronously via a message exchange. The following messages exchanges are supported:

- **In-Memory**  
  Supports only the communication between modules hosted in same process.
  
  The in-memory exchange is automatically used in the SAPHub.Server app and only makes sense if you run one single instance of SAPHub.Server.

  **Configuration**: 

  ``` json
  { "bus" : { "type" : "inmemory" } }
  ``` 


- **Azure Service Bus**  
  Uses a Azure Service Bus as message exchange. 
  
  **Configuration**: 

  ``` json
  { "bus" : { "type" : "azureservicebus", "connectionstring": "" } }
  ``` 

- **RabbitMq**  
  Uses RabbitMq as message exchange. 
  
  **Configuration**: 

  ``` json
  { "bus" : { "type" : "rabbitmq", "connectionstring": "" } }
  ``` 

### Azure Cosmos DB
To scale out the API Module (by starting multiple SAPHub.APIEndpoint or SAPHub.Server apps) you have to enable the Azure Cosmos DB as shared DB storage for the API endpoints. 

Please note that you can also use the CosmosDB emulator to run this locally. 

  **Configuration**: 

  ``` json
  { "cosmosdb" : { "databaseName" : "<your_db_name>", "connectionstring": "" } }
  ``` 

# Contribute
Even if this is only a sample implementation we will continue to maintain it as reference architecture. You are welcome to contribute enhancements or to report issues. 


# License
The code of this sample is licensed under the MIT License - see the LICENSE file for details. Feel free to use it in any application.

# Trademark notice
SAP, Netweaver are trademarks of SAP SE
