# SharpConnector

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/SharpConnector?style=plastic)](https://www.nuget.org/packages/SharpConnector)
[![issues - dotnet-design-patterns](https://img.shields.io/github/issues/engineering87/SharpConnector)](https://github.com/engineering87/SharpConnector/issues)
[![stars - dotnet-design-patterns](https://img.shields.io/github/stars/engineering87/SharpConnector?style=social)](https://github.com/engineering87/SharpConnector)

<img src="https://github.com/engineering87/SharpConnector/blob/main/sharpconnector_logo.jpg" width="300">

SharpConnector is a .NET general-purpose connector for NoSQL databases. It simplifies integration with NoSQL databases by unifying operations into a single interface, eliminating the need to develop specific logic for each connector. Each NoSQL database has its own peculiarities, some are document-oriented, while others are key-value stores. SharpConnector aims to unify access interfaces to streamline development.

### How it works
SharpConnector provides access to **CRUD** operations to NoSQL databases with **<Key, Value>**, abstracting the interface from the implementation. **Insert, Get, Delete, Update** operations are currently exposed to the following databases:

* **Redis**
* **MongoDB**
* **LiteDB**
* **EnyimMemcached**
* **RavenDB**

other connectors and operations are under development.

### How to use it
To use SharpConnector simply configure the *connectionString* to the desired connector and the *instance* type.
Add the *ConnectorConfig* node configuration within the *appsettings.json* file, here is the example for Redis connector:

```json
{
  "ConnectorConfig": {
    "Instance": "Redis",
    "DatabaseNumber": 0,
    "ConnectionString": "redisServer:6380,password=password,ssl=True,abortConnect=False"
  }
}
```

and instantiate a new client specifying the type of data (for example string):

```csharp
SharpConnectorClient<string> client = new SharpConnectorClient<string>()
```

SharpConnector works with any object that is serializable.

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
Please contact at francesco.delre.87[at]protonmail.com for any details.
