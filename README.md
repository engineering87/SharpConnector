<div align="center">

<img src="https://github.com/engineering87/SharpConnector/blob/main/sharpconnector_logo.jpg" width="260" alt="SharpConnector logo">

# SharpConnector

**One API, nine NoSQL databases.**
A unified, DI-friendly .NET client for key-based CRUD across Redis, MongoDB, LiteDB, Memcached, RavenDB, Couchbase, DynamoDB, ArangoDB and Cassandra.

[![NuGet](https://img.shields.io/nuget/v/SharpConnector?logo=nuget)](https://www.nuget.org/packages/SharpConnector)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SharpConnector)](https://www.nuget.org/packages/SharpConnector)
[![Build](https://github.com/engineering87/SharpConnector/actions/workflows/dotnet.yml/badge.svg)](https://github.com/engineering87/SharpConnector/actions/workflows/dotnet.yml)
![.NET](https://img.shields.io/badge/.NET-10.0%20%7C%209.0-512BD4?logo=dotnet)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Issues](https://img.shields.io/github/issues/engineering87/SharpConnector)](https://github.com/engineering87/SharpConnector/issues)
[![Stars](https://img.shields.io/github/stars/engineering87/SharpConnector?style=social)](https://github.com/engineering87/SharpConnector)

</div>

---

## Table of Contents

- [Why SharpConnector](#why-sharpconnector)
- [Supported databases](#supported-databases)
- [Requirements](#requirements)
- [Installation](#installation)
- [Quick start](#quick-start)
- [Configuration](#configuration)
- [API overview](#api-overview)
- [Operation semantics](#operation-semantics)
- [Upgrading from 4.x to 5.0](#upgrading-from-4x-to-50)
- [Building from source](#building-from-source)
- [Extending](#extending)
- [Contributing](#contributing)
- [Third-party licenses](#third-party-licenses)
- [License](#license)
- [Contact](#contact)

## Why SharpConnector

Every NoSQL engine has its own driver, data model and quirks. SharpConnector hides those differences behind a single generic interface, `ISharpConnectorClient<T>`, so that you can:

- switch database by changing **one line of configuration**;
- use the **same CRUD code** (sync and async, with `CancellationToken`) everywhere;
- get **consistent semantics** for upserts, updates, deletes and expiration on every backend;
- plug it into ASP.NET Core with a **single DI call**.

## Supported databases

| Database   | `Instance`  | Model        | Driver                  |
|------------|-------------|--------------|-------------------------|
| Redis      | `Redis`     | Key-value    | StackExchange.Redis     |
| MongoDB    | `MongoDb`   | Document     | MongoDB.Driver          |
| LiteDB     | `LiteDb`    | Embedded doc | LiteDB                  |
| Memcached  | `Memcached` | Key-value    | EnyimMemcachedCore      |
| RavenDB    | `RavenDb`   | Document     | RavenDB.Client          |
| Couchbase  | `Couchbase` | Document     | CouchbaseNetClient      |
| DynamoDB   | `DynamoDb`  | Key-value    | AWSSDK.DynamoDBv2       |
| ArangoDB   | `ArangoDb`  | Multi-model  | ArangoDBNetStandard     |
| Cassandra  | `Cassandra` | Wide-column  | CassandraCSharpDriver   |

## Requirements

- **.NET 10** or **.NET 9**: the package ships `net10.0` and `net9.0` assemblies, and NuGet automatically picks the best match for your project.

## Installation

```bash
dotnet add package SharpConnector
```

## Quick start

**1. Add a `ConnectorConfig` section to `appsettings.json`**

```json
{
  "ConnectorConfig": {
	"Instance": "Redis",
	"DatabaseNumber": 0,
	"ConnectionString": "localhost:6379"
  }
}
```

**2a. Use it with dependency injection (recommended)**

```csharp
builder.Services.AddSharpConnectorServices<string>();

app.MapGet("/items/{key}", async (string key, ISharpConnectorClient<string> client, CancellationToken ct) =>
	await client.GetAsync(key, ct) is { } value ? Results.Ok(value) : Results.NotFound());
```

**2b. Or create the client directly**

```csharp
var client = new SharpConnectorClient<string>();            // reads appsettings.json
// new SharpConnectorClient<string>(configuration);          // from an IConfiguration
// new SharpConnectorClient<string>("connector.json");       // from a custom JSON file

client.Insert("greeting", "hello");
client.Insert("session", "abc", TimeSpan.FromMinutes(20));  // with expiration
var value = await client.GetAsync("greeting");
```

### Registration lifetimes

| Method                                    | Lifetime  | Notes                                             |
|-------------------------------------------|-----------|---------------------------------------------------|
| `AddSharpConnectorServices<T>()`          | Singleton | Recommended: one connection shared by the app     |
| `AddSharpConnectorServicesSingleton<T>()` | Singleton | Same as above, explicit name                      |
| `AddSharpConnectorServicesScoped<T>()`    | Scoped    | Opens a new database connection for every scope   |

> Starting from 5.0 these methods only register SharpConnector. If you use MVC controllers, call `builder.Services.AddControllers()` yourself.

## Configuration

All settings live under the `ConnectorConfig` section. Key names are case-insensitive.

| Key                       | Used by                                | Description                                             |
|---------------------------|----------------------------------------|---------------------------------------------------------|
| `Instance`                | all                                    | Connector type (see table above)                        |
| `ConnectionString`        | all except DynamoDB                    | Server address / connection string / file path          |
| `DatabaseNumber`          | Redis                                  | Logical database index                                  |
| `KeyPrefix`               | Redis                                  | *Optional.* Prefix applied to every key, e.g. `app:`    |
| `DatabaseName`            | MongoDB, RavenDB, Cassandra (keyspace) | Database name                                           |
| `CollectionName`          | MongoDB, LiteDB, ArangoDB              | Collection name                                         |
| `TableName`               | DynamoDB, Cassandra                    | Table name                                              |
| `BucketName`              | Couchbase                              | Bucket name                                             |
| `Username` / `Password`   | Couchbase, ArangoDB, Cassandra         | Credentials (username is trimmed, password is verbatim) |
| `CertificatePath`         | RavenDB                                | *Optional.* Path to a PFX client certificate            |
| `CertificatePassword`     | RavenDB                                | *Optional.* Password of the PFX certificate             |
| `AccessKey` / `SecretKey` | DynamoDB                               | AWS credentials                                         |
| `Region`                  | DynamoDB                               | AWS region (required if `ServiceUrl` is not set)        |
| `ServiceUrl`              | DynamoDB                               | Custom endpoint, e.g. DynamoDB Local                    |
| `UseHttp`                 | DynamoDB                               | Use HTTP instead of HTTPS                               |

<details>
<summary><b>Redis</b></summary>

```json
{
  "ConnectorConfig": {
	"Instance": "Redis",
	"DatabaseNumber": 0,
	"KeyPrefix": "myapp:",
	"ConnectionString": "redisServer:6380,password=password,ssl=True,abortConnect=False"
  }
}
```
</details>

<details>
<summary><b>MongoDB</b></summary>

```json
{
  "ConnectorConfig": {
	"Instance": "MongoDb",
	"DatabaseName": "test",
	"CollectionName": "test",
	"ConnectionString": "mongodb://localhost:27017"
  }
}
```
</details>

<details>
<summary><b>LiteDB</b></summary>

```json
{
  "ConnectorConfig": {
	"Instance": "LiteDb",
	"CollectionName": "test",
	"ConnectionString": "LiteDbTest.db"
  }
}
```
</details>

<details>
<summary><b>Memcached</b></summary>

`ConnectionString` accepts a comma-separated list of `host:port` servers.

```json
{
  "ConnectorConfig": {
	"Instance": "Memcached",
	"ConnectionString": "127.0.0.1:11211"
  }
}
```
</details>

<details>
<summary><b>RavenDB</b></summary>

`ConnectionString` must be an absolute URL and `DatabaseName` is required. The certificate settings are optional and needed only for secured servers.

```json
{
  "ConnectorConfig": {
	"Instance": "RavenDb",
	"DatabaseName": "test",
	"ConnectionString": "https://a.mycluster.ravendb.cloud",
	"CertificatePath": "certs/client.pfx",
	"CertificatePassword": "pfx-password"
  }
}
```
</details>

<details>
<summary><b>Couchbase</b></summary>

```json
{
  "ConnectorConfig": {
	"Instance": "Couchbase",
	"ConnectionString": "couchbase://localhost",
	"Username": "Administrator",
	"Password": "password",
	"BucketName": "example_bucket"
  }
}
```
</details>

<details>
<summary><b>DynamoDB</b></summary>

`TableName` is required, together with either `Region` or `ServiceUrl`. The table must use a string hash key named `Key`.

```json
{
  "ConnectorConfig": {
	"Instance": "DynamoDb",
	"AccessKey": "your-access-key-here",
	"SecretKey": "your-secret-key-here",
	"Region": "us-west-2",
	"TableName": "MyTableName"
  }
}
```
</details>

<details>
<summary><b>ArangoDB</b></summary>

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
</details>

<details>
<summary><b>Cassandra</b></summary>

`ConnectionString` accepts a comma-separated list of contact points (`host`, `host:port`, `[IPv6]:port`); all ports must match. `DatabaseName` is the keyspace.

```json
{
  "ConnectorConfig": {
	"Instance": "Cassandra",
	"ConnectionString": "127.0.0.1:9042",
	"DatabaseName": "sharpconnector",
	"TableName": "items",
	"Username": "cassandra",
	"Password": "cassandra"
  }
}
```
</details>

## API overview

The entry point is `ISharpConnectorClient<T>`. Every operation has a sync and an async version; async methods accept a `CancellationToken`.

| Operation | Methods |
|-----------|---------|
| Read      | `Get(key)`, `GetAll()`, `Exists(key)`, `Query(Func<T, bool> filter)` |
| Insert    | `Insert(key, value)`, `Insert(key, value, TimeSpan expiration)` |
| Bulk      | `InsertMany(Dictionary<string, T>)`, `InsertMany(Dictionary<string, T>, TimeSpan expiration)` |
| Update    | `Update(key, value)` |
| Delete    | `Delete(key)` |

`InsertManyAsync(IEnumerable<T> values, CancellationToken ct)` is available in async form only: it generates the keys and returns them as `IReadOnlyCollection<string>`.

## Operation semantics

The behavior is the same on every backend:

- **Insert / InsertMany** are upserts: an existing key is overwritten.
- **Update** only changes existing keys and returns `false` when the key does not exist.
- **Delete** returns `true` only when an item was actually removed.
- **InsertMany** throws `ArgumentException` when a key is null/empty or a value is null.
- **Expiration**: expired items are never returned by `Get`, `GetAll`, `Exists` or `Query`, even on backends without native TTL.
- **Configuration** is validated when the client is created: invalid or incomplete settings throw `ArgumentException`.
- **Sync vs async**: prefer the async methods. The DynamoDB, ArangoDB and Couchbase drivers are async-only, so their sync methods run the async call on the thread pool.

## Upgrading from 4.x to 5.0

5.0 contains breaking changes:

1. **MVC is no longer registered.** `AddSharpConnectorServices*` no longer calls `AddControllers()`; add it to your host if you need controllers.
2. **Stricter configuration.** RavenDB (absolute URL + `DatabaseName`) and DynamoDB (`TableName` + `Region` or `ServiceUrl`) now throw `ArgumentException` when settings are missing or invalid.
3. **Consistent semantics.** `Update` returns `false` for missing keys and `Delete` returns `false` when nothing was removed, on every backend.

New in 5.0: Redis `KeyPrefix` and RavenDB client certificate authentication.

## Building from source

Requires the **.NET 10 SDK**, which builds both the `net10.0` and `net9.0` targets.

```bash
git clone https://github.com/engineering87/SharpConnector.git
cd SharpConnector/src/SharpConnector
dotnet build SharpConnector.sln -c Release
dotnet test SharpConnector.sln -c Release
```

## Extending

To add a new connector:

1. Add a value to `ConnectorTypeEnums` and a configuration class implementing `IConnectorConfig`.
2. Implement an access layer (connection management) and a wrapper (CRUD on the native driver).
3. Implement `IOperations<T>` on top of the wrapper.
4. Add unit tests.

## Contributing

Contributions are welcome! Fork the repository, create a branch and open a pull request.

- Add unit tests for new features or connectors.
- Use [Conventional Commits](https://www.conventionalcommits.org/): commit messages are checked by commitlint in CI.
- Keep third-party dependencies compatible with MIT.
- Found a bug or have an idea? [Open an issue](https://github.com/engineering87/SharpConnector/issues).

## Third-party licenses

SharpConnector relies on the following libraries, each under its own license:

| Library               | License |
|-----------------------|---------|
| StackExchange.Redis   | [MIT](https://github.com/StackExchange/StackExchange.Redis/blob/main/LICENSE) |
| MongoDB.Driver        | [Apache 2.0](https://github.com/mongodb/mongo-csharp-driver/blob/main/LICENSE.md) |
| LiteDB                | [MIT](https://github.com/mbdavid/LiteDB/blob/master/LICENSE) |
| EnyimMemcachedCore    | [Apache 2.0](https://github.com/cnblogs/EnyimMemcachedCore/blob/main/LICENSE) |
| RavenDB.Client        | [See license](https://github.com/ravendb/ravendb/blob/v6.2/LICENSE.txt) |
| CouchbaseNetClient    | [Apache 2.0](https://github.com/couchbase/couchbase-net-client/blob/master/LICENSE) |
| AWSSDK.DynamoDBv2     | [Apache 2.0](https://github.com/aws/aws-sdk-net/blob/main/License.txt) |
| ArangoDBNetStandard   | [Apache 2.0](https://github.com/ArangoDB-Community/arangodb-net-standard/blob/master/LICENSE) |
| CassandraCSharpDriver | [Apache 2.0](https://github.com/datastax/csharp-driver/blob/master/LICENSE) |

## License

SharpConnector is released under the [MIT License](LICENSE).

## Contact

For any details, contact francesco.delre[at]protonmail.com.
