using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CluedIn.Core;
using CluedIn.Core.Crawling;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;
using CluedIn.Core.Webhooks;
using System.Configuration;
using System.Linq;
using CluedIn.Core.Configuration;
using CluedIn.Crawling.AdventureWorksPurchasing.Core;
using CluedIn.Crawling.AdventureWorksPurchasing.Infrastructure.Factories;
using CluedIn.Providers.Models;
using Newtonsoft.Json;

namespace CluedIn.Provider.AdventureWorksPurchasing
{
    public class AdventureWorksPurchasingProvider : ProviderBase, IExtendedProviderMetadata
    {
        private readonly IAdventureWorksPurchasingClientFactory _AdventureWorksPurchasingClientFactory;

        public AdventureWorksPurchasingProvider([NotNull] ApplicationContext appContext, IAdventureWorksPurchasingClientFactory AdventureWorksPurchasingClientFactory)
            : base(appContext, AdventureWorksPurchasingConstants.CreateProviderMetadata())
        {
            _AdventureWorksPurchasingClientFactory = AdventureWorksPurchasingClientFactory;
        }

        public override async Task<CrawlJobData> GetCrawlJobData(
            ProviderUpdateContext context,
            IDictionary<string, object> configuration,
            Guid organizationId,
            Guid userId,
            Guid providerDefinitionId)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var AdventureWorksPurchasingCrawlJobData = new AdventureWorksPurchasingCrawlJobData();
            if (configuration.ContainsKey(AdventureWorksPurchasingConstants.KeyName.ConnectionString))
            { AdventureWorksPurchasingCrawlJobData.ConnectionString = configuration[AdventureWorksPurchasingConstants.KeyName.ConnectionString].ToString(); }

            return await Task.FromResult(AdventureWorksPurchasingCrawlJobData);
        }

        public override Task<bool> TestAuthentication(
            ProviderUpdateContext context,
            IDictionary<string, object> configuration,
            Guid organizationId,
            Guid userId,
            Guid providerDefinitionId)
        {
            throw new NotImplementedException();
        }

        public override Task<ExpectedStatistics> FetchUnSyncedEntityStatistics(ExecutionContext context, IDictionary<string, object> configuration, Guid organizationId, Guid userId, Guid providerDefinitionId)
        {
            throw new NotImplementedException();
        }

        public override async Task<IDictionary<string, object>> GetHelperConfiguration(
            ProviderUpdateContext context,
            [NotNull] CrawlJobData jobData,
            Guid organizationId,
            Guid userId,
            Guid providerDefinitionId)
        {
            if (jobData == null)
                throw new ArgumentNullException(nameof(jobData));

            var dictionary = new Dictionary<string, object>();

            if (jobData is AdventureWorksPurchasingCrawlJobData AdventureWorksPurchasingCrawlJobData)
            {
                //TODO add the transformations from specific CrawlJobData object to dictionary
                // add tests to GetHelperConfigurationBehaviour.cs
                dictionary.Add(AdventureWorksPurchasingConstants.KeyName.ConnectionString, AdventureWorksPurchasingCrawlJobData.ConnectionString);
            }

            return await Task.FromResult(dictionary);
        }

        public override Task<IDictionary<string, object>> GetHelperConfiguration(
            ProviderUpdateContext context,
            CrawlJobData jobData,
            Guid organizationId,
            Guid userId,
            Guid providerDefinitionId,
            string folderId)
        {
            throw new NotImplementedException();
        }

        public override async Task<AccountInformation> GetAccountInformation(ExecutionContext context, [NotNull] CrawlJobData jobData, Guid organizationId, Guid userId, Guid providerDefinitionId)
        {
            if (jobData == null)
                throw new ArgumentNullException(nameof(jobData));

            if (!(jobData is AdventureWorksPurchasingCrawlJobData AdventureWorksPurchasingCrawlJobData))
            {
                throw new Exception("Wrong CrawlJobData type");
            }

            var client = _AdventureWorksPurchasingClientFactory.CreateNew(AdventureWorksPurchasingCrawlJobData);
            return await Task.FromResult(client.GetAccountInformation());
        }

        public override string Schedule(DateTimeOffset relativeDateTime, bool webHooksEnabled)
        {
            return webHooksEnabled && ConfigurationManager.AppSettings.GetFlag("Feature.Webhooks.Enabled", false) ? $"{relativeDateTime.Minute} 0/23 * * *"
                : $"{relativeDateTime.Minute} 0/4 * * *";
        }

        public override Task<IEnumerable<WebHookSignature>> CreateWebHook(ExecutionContext context, [NotNull] CrawlJobData jobData, [NotNull] IWebhookDefinition webhookDefinition, [NotNull] IDictionary<string, object> config)
        {
            if (jobData == null)
                throw new ArgumentNullException(nameof(jobData));
            if (webhookDefinition == null)
                throw new ArgumentNullException(nameof(webhookDefinition));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            throw new NotImplementedException();
        }

        public override Task<IEnumerable<WebhookDefinition>> GetWebHooks(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteWebHook(ExecutionContext context, [NotNull] CrawlJobData jobData, [NotNull] IWebhookDefinition webhookDefinition)
        {
            if (jobData == null)
                throw new ArgumentNullException(nameof(jobData));
            if (webhookDefinition == null)
                throw new ArgumentNullException(nameof(webhookDefinition));

            throw new NotImplementedException();
        }

        public override IEnumerable<string> WebhookManagementEndpoints([NotNull] IEnumerable<string> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            if (!ids.Any())
            {
                throw new ArgumentException(nameof(ids));
            }

            throw new NotImplementedException();
        }

        public override async Task<CrawlLimit> GetRemainingApiAllowance(ExecutionContext context, [NotNull] CrawlJobData jobData, Guid organizationId, Guid userId, Guid providerDefinitionId)
        {
            if (jobData == null)
                throw new ArgumentNullException(nameof(jobData));


            //There is no limit set, so you can pull as often and as much as you want.
            return await Task.FromResult(new CrawlLimit(-1, TimeSpan.Zero));
        }

        // TODO Please see https://cluedin-io.github.io/CluedIn.Documentation/docs/1-Integration/build-integration.html
        public string Icon => AdventureWorksPurchasingConstants.IconResourceName;
        public string Domain { get; } = AdventureWorksPurchasingConstants.Uri;
        public string About { get; } = AdventureWorksPurchasingConstants.CrawlerDescription;
        public AuthMethods AuthMethods { get; } = AdventureWorksPurchasingConstants.AuthMethods;
        public IEnumerable<Control> Properties => null;
        public string ServiceType { get; } = JsonConvert.SerializeObject(AdventureWorksPurchasingConstants.ServiceType);
        public string Aliases { get; } = JsonConvert.SerializeObject(AdventureWorksPurchasingConstants.Aliases);
        public Guide Guide { get; set; } = new Guide
        {
            Instructions = AdventureWorksPurchasingConstants.Instructions,
            Value = new List<string> { AdventureWorksPurchasingConstants.CrawlerDescription },
            Details = AdventureWorksPurchasingConstants.Details

        };

        public string Details { get; set; } = AdventureWorksPurchasingConstants.Details;
        public string Category { get; set; } = AdventureWorksPurchasingConstants.Category;
        public new IntegrationType Type { get; set; } = AdventureWorksPurchasingConstants.Type;
    }
}
