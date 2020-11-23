[![Github license](mit.svg)](https://github.com/engineering87/SharpConnector/blob/master/LICENSE)

# SharpConnector
SharpConnector is a general purpose multiple connector to NoSQL database. It simplifies the integration with the NoSql database by unifying the operations in a single interface without the need to develop specific logic for each connector.

### How it works
SharpConnector provides access to **CRUD** operations to NoSql databases with *<Key, Value>*, abstracting the interface from the implementation. **Insert, Get, Delete, Update** operations are currently exposed to the following databases:

* **Redis**
* **MongoDb**

other connectors and operations are under development.

### How to use it
To use SharpConnector simply configure the *connectionstring* to the desired connector, here is the example for Redis:

```json
{
  "ConnectorConfig": {
    "DatabaseNumber": 0,
    "ConnectionString": "redisServer:6380,password=password,ssl=True,abortConnect=False"
  }
}
```

and instantiate a new client specifying the type of data (for example string) and the type of connector:

```csharp
SharpConnectorClient<string> client = new SharpConnectorClient<string>(ConnectorTypes.Redis)
```

SharpConnector works with any object that is serializable.

### Contributing
Thank you for considering to help out with the source code!
If you'd like to contribute, please fork, fix, commit and send a pull request for the maintainers to review and merge into the main code base.

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
 * [Open an issue](https://github.com/engineering87/SharpConnector/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
SharpConnector source code is available under MIT License, see license in the source.

### Contact
Please contact at francesco.delre.87[at]gmail.com for any details.
