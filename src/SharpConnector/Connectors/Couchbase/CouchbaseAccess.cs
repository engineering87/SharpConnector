// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Couchbase;
using Couchbase.KeyValue;
using SharpConnector.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConnector.Connectors.Couchbase
{
    public class CouchbaseAccess : IAsyncDisposable
    {
        private readonly Lazy<Task<ICluster>> _cluster;
        private readonly string _bucketName;
        private readonly SemaphoreSlim _bucketLock = new(1, 1);
        private IBucket _bucket;

        public CouchbaseAccess(CouchbaseConfig config)
        {
            _cluster = new Lazy<Task<ICluster>>(() => ConnectClusterAsync(config));
            _bucketName = config.BucketName;
        }

        public string BucketName => _bucketName;

        private async Task<ICluster> ConnectClusterAsync(CouchbaseConfig config)
        {
            var clusterOptions = new ClusterOptions
            {
                UserName = config.Username,
                Password = config.Password
            };
            return await Cluster.ConnectAsync(config.ConnectionString, clusterOptions);
        }

        public async Task<ICluster> GetClusterAsync()
        {
            return await _cluster.Value;
        }

        public async Task<IBucket> GetBucketAsync()
        {
            if (_bucket != null)
                return _bucket;

            await _bucketLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_bucket == null)
                {
                    var cluster = await GetClusterAsync();
                    _bucket = await cluster.BucketAsync(_bucketName);
                }
                return _bucket;
            }
            finally
            {
                _bucketLock.Release();
            }
        }

        public async Task<ICouchbaseCollection> GetCollectionAsync(string scopeName, string collectionName)
        {
            var bucket = await GetBucketAsync();
            var scope = await bucket.ScopeAsync(scopeName);
            return await scope.CollectionAsync(collectionName);
        }

        public async Task<ICouchbaseCollection> GetDefaultCollectionAsync()
            => await (await GetBucketAsync()).DefaultCollectionAsync();

        public async ValueTask DisposeAsync()
        {
            if (_bucket != null)
            {
                await _bucket.DisposeAsync();
                _bucket = null;
            }

            if (_cluster.IsValueCreated)
            {
                var cluster = await _cluster.Value;
                await cluster.DisposeAsync();
            }

            _bucketLock.Dispose();
        }
    }
}
