[![Github license](mit.svg)](https://github.com/engineering87/SharpConnector/blob/master/LICENSE)

<img src="https://github.com/engineering87/SharpConnector/blob/main/sharpconnector_logo.jpg" width="300">

SharpConnector is a general purpose multiple connector to NoSQL database. It simplifies the integration with the NoSql database by unifying the operations in a single interface without the need to develop specific logic for each connector. SharpConnector is a **.NET 6** library.
Each NoSQL database has its own peculiarities, some are document-oriented, others are of the key-value type,SharpConnector tries to unify the access interfaces in order to simplify the developments.

### How it works
SharpConnector provides access to **CRUD** operations to NoSql databases with *<Key, Value>*, abstracting the interface from the implementation. **Insert, Get, Delete, Update** operations are currently exposed to the following databases:

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

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
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
Please contact at francesco.delre.87[at]gmail.com for any details.
