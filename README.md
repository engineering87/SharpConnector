# SharpConnector

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/SharpConnector?style=plastic)](https://www.nuget.org/packages/SharpConnector)
![NuGet Downloads](https://img.shields.io/nuget/dt/SharpConnector)
[![issues - dotnet-design-patterns](https://img.shields.io/github/issues/engineering87/SharpConnector)](https://github.com/engineering87/SharpConnector/issues)
[![Build](https://github.com/engineering87/SharpConnector/actions/workflows/dotnet.yml/badge.svg)](https://github.com/engineering87/SharpConnector/actions/workflows/dotnet.yml)
[![stars - dotnet-design-patterns](https://img.shields.io/github/stars/engineering87/SharpConnector?style=social)](https://github.com/engineering87/SharpConnector)

<img src="https://github.com/engineering87/SharpConnector/blob/main/sharpconnector_logo.jpg" width="300">

SharpConnector is a .NET library designed to streamline integration with NoSQL databases. It provides a unified interface that simplifies database operations, eliminating the need to develop custom logic for each specific database connector. Since each NoSQL database has its own unique characteristics, such as being document-oriented or key-value-based, SharpConnector abstracts these differences, providing a consistent and simplified access layer to accelerate development.

✅ Now targeting .NET 9.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
  - [How it works](#how-it-works)
  - [How to use it](#how-to-use-it)
- [API Overview](#api-overview)
- [Contributing](#contributing)
- [Extending](#extending)
- [License](#license)
- [External References](#external-references)
- [Contact](#contact)

## Features

- A single, generic client API for CRUD across multiple NoSQL engines
- Supported stores:
	- Key–value: Redis, EnyimMemcached, DynamoDb
	- Document: MongoDB, LiteDB, RavenDB, Couchbase
	- Multi-model: ArangoDB
- Simple configuration via `appsettings.json` and DI-friendly
- Sync and async operations (with CancellationToken support)
- Easy to extend: implement `IOperations<T>` for new connectors

## Installation

You can install the library via the NuGet package manager with the following command:

```bash
dotnet add package SharpConnector
```

### How it works
SharpConnector offers a unified interface for performing CRUD operations on various types of NoSQL databases. While NoSQL databases often differ in their internal structures (e.g., key-value stores, document databases), this library abstracts these distinctions, enabling streamlined key-value-based CRUD operations.
Through SharpConnector, you can use a consistent interface to perform Insert, Get, Delete, and Update operations across multiple NoSQL systems, currently supporting:

* **Redis (key-value)**
* **MongoDB (key-value or document-oriented)**
* **LiteDB (embedded document database)**
* **EnyimMemcached (key-value)**
* **RavenDB (document-oriented)**
* **Couchbase (document-oriented)**
* **DynamoDb (key-value or document-oriented)**
* **ArangoDB (multi-model)**

SharpConnector thus simplifies the development process, providing flexibility and compatibility across diverse NoSQL paradigms without the need to handle specific database implementations.

### How to use it
To get started with SharpConnector, configure the connector *instance* type. 
Then, add the specif `ConnectorConfig` node within your *appsettings.json* file:

- Redis
	```json
	{
	  "ConnectorConfig": {
		"Instance": "Redis",
		"DatabaseNumber": 0,
		"ConnectionString": "redisServer:6380,password=password,ssl=True,abortConnect=False"
	  }
	}
	```
- MongoDb
	```json
	{
	  "ConnectorConfig": {
		"Instance": "MongoDb",
		"DatabaseName": "test",
		"CollectionName": "test",
		"ConnectionString": "mongodb_connectionstring_here"
	  }
	}
	```
	
- LiteDB
	```json
	{
	  "ConnectorConfig": {
		"Instance": "LiteDb",
		"CollectionName": "test",
		"ConnectionString": "LiteDbTest.db"
	  }
	}
	```

- Memcached
	```json
	{
	  "ConnectorConfig": {
		"Instance": "Memcached",
		"ConnectionString": "127.0.0.1:11211"
	  }
	}
	```
	
- RavenDB
	```json
	{
	  "ConnectorConfig": {
		"Instance": "RavenDb",
		"DatabaseName": "test",
		"ConnectionString": "http://live-test.ravendb.net"
	  }
	}
	```

- Couchbase
	```json
	{
	  "ConnectorConfig": {
		"Instance": "Couchbase",
		"ConnectionString": "couchbase://localhost",
		"Username": "Administrator",
		"Password": "password",
		"BucketName": "example_bucket",
	  }
	}
	```

- DynamoDb
	```json
	{
	  "ConnectorConfig": {
		"Instance": "DynamoDb",
		"AccessKey": "your-access-key-here",
		"SecretKey": "your-secret-key-here",
		"Region": "us-west-2",
		"ServiceUrl": "https://dynamodb.us-west-2.amazonaws.com",
		"UseHttp": false,
		"TableName": "MyTableName"
	  }
	}
	```

- ArangoDB
	```json
	{
		"ConnectorConfig": {
			"Instance": "ArangoDb",
			"ConnectionString": "http://localhost:8529",
			"Username": "username",
			"Password": "password",
			"CollectionName": "test"
		}
	}
	```

Once configured, create a new SharpConnector client, specifying the payload type (e.g., string):

```csharp
SharpConnectorClient<string> client = new SharpConnectorClient<string>()
```

Alternatively, you can integrate SharpConnector client using dependency injection. Here how to register the SharpConnector service with a simple string payload type:

```csharp
// Register the SharpConnector services with string payload type.
builder.Services.AddSharpConnectorServices<string>();
```
This setup provides flexibility in working with different payload types and makes SharpConnector easy to use within dependency injection configurations.

## API Overview
The main entry point is the generic `ISharpConnectorClient<T>`:
- `T Get(string key)`
- `Task<T> GetAsync(string key, CancellationToken ct = default)`
- `IEnumerable<T> GetAll()`
- `Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)`
- `bool Insert(string key, T value)`
- `bool Insert(string key, T value, TimeSpan expiration)`
- `Task<bool> InsertAsync(string key, T value, CancellationToken ct = default)`
- `Task<bool> InsertAsync(string key, T value, TimeSpan expiration, CancellationToken ct = default)`
- `bool InsertMany(Dictionary<string,T> values)`
- `bool InsertMany(Dictionary<string,T> values, TimeSpan expiration)`
- `Task<bool> InsertManyAsync(IEnumerable<T> values, CancellationToken ct = default)`
- `bool Update(string key, T value)`
- `Task<bool> UpdateAsync(string key, T value, CancellationToken ct = default)`
- `bool Delete(string key)`
- `Task<bool> DeleteAsync(string key, CancellationToken ct = default)`
- `bool Exists(string key)`
- `Task<bool> ExistsAsync(string key, CancellationToken ct = default)`
- `IEnumerable<T> Query(Func<T,bool> filter)`
- `Task<IEnumerable<T>> QueryAsync(Func<T,bool> filter, CancellationToken ct = default)`

## Contributing
Thanks for considering a contribution! Please fork, branch, and open a PR.
- Add unit tests for new features or connectors
- Keep third-party dependencies compatible with MIT and document their licenses
- Useful links:
	- [Setting up Git](https://docs.github.com/en/get-started/getting-started-with-git/set-up-git)
	- [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
	- [Open an issue](https://github.com/engineering87/SharpConnector/issues) if you encounter a bug or have a suggestion for improvements/features

## Extending
To add a new connector:
- Implement `IOperations<T>` for your backend.
- Provide a wrapper/access layer (connection management, collection/table handles).
- Add configuration binding under ConnectorConfig.
- Include unit tests.

## License
SharpConnector source code is available under MIT License, see license in the source.

### External References
The SharpConnector library relies on several third-party libraries to deliver advanced functionality. 
Each of these libraries operates under a specific license, which governs its usage. To ensure transparency and compliance, the libraries and their licenses are listed in this repository:

* **StackExchange.Redis**, a General purpose redis client, see **license** [here](https://github.com/StackExchange/StackExchange.Redis/blob/main/LICENSE)
* **MongoDB.Driver**, the Official C# .NET Driver for MongoDB, see **license** [here](https://github.com/mongodb/mongo-csharp-driver/blob/main/LICENSE.md)
* **LiteDB**, a .NET NoSQL Document Store in a single data file, see **license** [here](https://github.com/mbdavid/LiteDB/blob/master/LICENSE)
* **EnyimMemcached**, a C# Memcached client, see **license** [here](https://github.com/enyim/EnyimMemcached/blob/develop/LICENSE)
* **RavenDB**, ACID Document Database, see **license** [here](https://github.com/ravendb/ravendb/blob/v6.2/LICENSE.txt)
* **Couchbase**, the official Couchbase SDK for .NET Core and Full Frameworks, see **license** [here](https://github.com/couchbase/couchbase-net-client/blob/master/LICENSE)
* **DynamoDb**, the official AWS SDK for .NET, see **license** [here](https://github.com/aws/aws-sdk-net/blob/main/License.txt)
* **ArangoDB**, a consistent, comprehensive, minimal driver for ArangoDB, see **license** [here](https://github.com/ArangoDB-Community/arangodb-net-standard/blob/master/LICENSE)

Each library is included to enhance the functionality of SharpConnector while adhering to its licensing terms.

## Contact
Please contact at francesco.delre[at]protonmail.com for any details.
