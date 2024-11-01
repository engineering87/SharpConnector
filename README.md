# SharpConnector

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/SharpConnector?style=plastic)](https://www.nuget.org/packages/SharpConnector)
![NuGet Downloads](https://img.shields.io/nuget/dt/SharpConnector)
[![issues - dotnet-design-patterns](https://img.shields.io/github/issues/engineering87/SharpConnector)](https://github.com/engineering87/SharpConnector/issues)
[![Build](https://github.com/engineering87/SharpConnector/actions/workflows/dotnet.yml/badge.svg)](https://github.com/engineering87/SharpConnector/actions/workflows/dotnet.yml)
[![stars - dotnet-design-patterns](https://img.shields.io/github/stars/engineering87/SharpConnector?style=social)](https://github.com/engineering87/SharpConnector)

<img src="https://github.com/engineering87/SharpConnector/blob/main/sharpconnector_logo.jpg" width="300">

SharpConnector is a .NET library designed to streamline integration with NoSQL databases. It provides a unified interface that simplifies database operations, eliminating the need to develop custom logic for each specific database connector. Since each NoSQL database has its own unique characteristics—such as being document-oriented or key-value-based—SharpConnector abstracts these differences, providing a consistent and simplified access layer to accelerate development.

### How it works
SharpConnector offers a unified interface for performing CRUD operations on various types of NoSQL databases. While NoSQL databases often differ in their internal structures (e.g., key-value stores, document databases), this library abstracts these distinctions, enabling streamlined key-value-based CRUD operations.
Through SharpConnector, you can use a consistent interface to perform Insert, Get, Delete, and Update operations across multiple NoSQL systems, currently supporting:

* **Redis (key-value)**
* **MongoDB (document-oriented)**
* **LiteDB (embedded document database)**
* **EnyimMemcached (key-value)**
* **RavenDB (document-oriented)**

SharpConnector thus simplifies the development process, providing flexibility and compatibility across diverse NoSQL paradigms without the need to handle specific database implementations.

### How to use it
To get started with SharpConnector, configure your *connectionString* and specify the connector *instance* type. 
Then, add the ConnectorConfig node within your appsettings.json file. Here’s an example configuration for a Redis connector:

```json
{
  "ConnectorConfig": {
    "Instance": "Redis",
    "DatabaseNumber": 0,
    "ConnectionString": "redisServer:6380,password=password,ssl=True,abortConnect=False"
  }
}
```

Once configured, create a new SharpConnector client, specifying the payload type (e.g., string):

```csharp
SharpConnectorClient<string> client = new SharpConnectorClient<string>()
```

Alternatively, you can integrate SharpConnector client using dependency injection. Here’s how to register the SharpConnector service with a string payload type:

```csharp
// Register the SharpConnector services with string payload type.
builder.Services.AddSharpConnectorServices<string>();
```
This setup provides flexibility in working with different payload types and makes SharpConnector easy to use within dependency injection configurations.

### Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.
If you want to add new connectors, please follow these three rules: 

1) Each new connector must implement the **IOperations** interface.
2) For each new connector the relevant **UnitTest** class must be present.
3) Any third party libraries added in the code must be compatible with the MIT license, and the license must also be made explicit in the code.

 * [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
 * [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
 * [Open an issue](https://github.com/engineering87/SharpConnector/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
SharpConnector source code is available under MIT License, see license in the source.

### External references
SharpConnector uses the following externals references:
* **StackExchange.Redis** see license [here](https://github.com/StackExchange/StackExchange.Redis/blob/main/LICENSE)
* **MongoDB.Driver** see license [here](https://github.com/mongodb/mongo-csharp-driver/blob/master/License.txt)
* **LiteDB** see license [here](https://github.com/mbdavid/LiteDB/blob/master/LICENSE)
* **EnyimMemcached** see license [here](https://github.com/enyim/EnyimMemcached/blob/develop/LICENSE)
* **RavenDB** see license [here](https://github.com/ravendb/ravendb/blob/v5.2/LICENSE.txt)

### Contact
Please contact at francesco.delre[at]protonmail.com for any details.
