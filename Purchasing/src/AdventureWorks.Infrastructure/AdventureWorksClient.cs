using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using CluedIn.Core;
using Microsoft.Extensions.Logging;
using CluedIn.Core.Providers;
using CluedIn.Crawling.AdventureWorksPurchasing.Core;
using CluedIn.Crawling.AdventureWorksPurchasing.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace CluedIn.Crawling.AdventureWorksPurchasing.Infrastructure
{
    // TODO - This class should act as a client to retrieve the data to be crawled.
    // It should provide the appropriate methods to get the data
    // according to the type of data source (e.g. for AD, GetUsers, GetRoles, etc.)
    // It can receive a IRestClient as a dependency to talk to a RestAPI endpoint.
    // This class should not contain crawling logic (i.e. in which order things are retrieved)
    public class AdventureWorksPurchasingClient
    {
        private const string BaseUri = "http://sample.com";

        private readonly ILogger<AdventureWorksPurchasingClient> _log;
        private readonly AdventureWorksPurchasingCrawlJobData _jobData;
        private readonly int _retryIntervalMs = 10000;

        public AdventureWorksPurchasingClient(ILogger<AdventureWorksPurchasingClient> log, AdventureWorksPurchasingCrawlJobData AdventureWorksPurchasingCrawlJobData) // TODO: pass on any extra dependencies
        {
            if (AdventureWorksPurchasingCrawlJobData == null)
            {
                throw new ArgumentNullException(nameof(AdventureWorksPurchasingCrawlJobData));
            }

            _jobData = AdventureWorksPurchasingCrawlJobData;
           

            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IEnumerable<T> GetObject<T>(int numSelect) where T : BaseSqlEntity
        {
            var tableName = ((DisplayNameAttribute)typeof(T).GetCustomAttribute(typeof(DisplayNameAttribute))).DisplayName;


            using (var connection = new SqlConnection(_jobData.ConnectionString))
                using (var command = connection.CreateCommand())
                {
                    SqlDataReader reader = null;
                    try
                    {
                        connection.Open();
                        reader = ActionExtensions.ExecuteWithRetry(() =>
                        {
                            command.CommandText = $@"SELECT TOP {numSelect} * FROM {tableName}";

                            command.CommandTimeout = 180;

                            return command.ExecuteReader();
                        },
                        retryIntervalMilliseconds: _retryIntervalMs,
                        retryCount: 3,
                        isTransient: ex => ex is SqlException || ex.IsTransient());
                    }
                    catch (Exception exception)
                    {
                        _log.LogError(exception.Message, exception);
                        yield break;
                    }


                    while (reader.Read())
                    {
                        T sqlEntity = default;

                        try
                        {
                            sqlEntity = (T)Activator.CreateInstance(typeof(T), reader);
                        }
                        catch (Exception exception)
                        {
                            _log.LogError(exception.Message, exception);
                            continue;
                        }

                        yield return sqlEntity;
                    }
                }
        }


        public AccountInformation GetAccountInformation()
        {
            //TODO - return some unique information about the remote data source
            // that uniquely identifies the account
            return new AccountInformation("", ""); 
        }
    }
}