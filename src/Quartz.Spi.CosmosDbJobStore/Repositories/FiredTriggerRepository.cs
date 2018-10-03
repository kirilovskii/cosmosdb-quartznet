﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Quartz.Spi.CosmosDbJobStore.Entities;

namespace Quartz.Spi.CosmosDbJobStore.Repositories
{
    internal class FiredTriggerRepository : CosmosDbRepositoryBase<PersistentFiredTrigger>
    {
        public FiredTriggerRepository(IDocumentClient documentClient, string databaseId, string collectionId, string instanceName)
            : base(documentClient, databaseId, collectionId, PersistentFiredTrigger.EntityType, instanceName)
        {
        }


        public async Task<IList<PersistentFiredTrigger>> GetAllByJob(string jobName, string jobGroup)
        {
            return _documentClient
                .CreateDocumentQuery<PersistentFiredTrigger>(_collectionUri)
                .Where(x => x.Type == _type && x.InstanceName == _instanceName && x.JobGroup == jobGroup && x.JobName == jobName)
                .AsEnumerable()
                .ToList();
        }

        public async Task<IList<PersistentFiredTrigger>> GetAllRecoverableByInstanceId(string instanceId)
        {
            return _documentClient
                .CreateDocumentQuery<PersistentFiredTrigger>(_collectionUri)
                .Where(x => x.Type == _type && x.InstanceName == _instanceName && x.InstanceId == instanceId && x.RequestsRecovery)
                .AsEnumerable()
                .ToList();
        }
        
        public async Task<IList<PersistentFiredTrigger>> GetAllByInstanceId(string instanceId)
        {
            return _documentClient
                .CreateDocumentQuery<PersistentFiredTrigger>(_collectionUri)
                .Where(x => x.Type == _type && x.InstanceName == _instanceName && x.InstanceId == instanceId)
                .AsEnumerable()
                .ToList();
        }

        /// <summary>
        /// Deletes FiredTrigger + 
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public async Task<int> DeleteAllByInstanceId(string instanceId)
        {
            // We may introduce paging if performance boost is necessary
            
            var triggers = _documentClient
                .CreateDocumentQuery<PersistentFiredTrigger>(_collectionUri)
                .Where(x => x.Type == _type && x.InstanceName == _instanceName && x.InstanceId == instanceId)
                .Select(x => x.Id)
                .AsEnumerable()
                .ToList();

            foreach (var trigger in triggers)
            {
                await _documentClient.DeleteDocumentAsync(CreateDocumentUri(trigger));
            }
            
            return triggers.Count;
        }

        public async Task<IList<PersistentFiredTrigger>> GetAllByTrigger(string triggerKeyName, string triggerKeyGroup)
        {
            return _documentClient
                .CreateDocumentQuery<PersistentFiredTrigger>(_collectionUri)
                .Where(x => x.Type == _type && x.InstanceName == _instanceName && x.TriggerGroup == triggerKeyGroup && (x.TriggerName == null || x.TriggerName == triggerKeyName))
                .AsEnumerable()
                .ToList();
        }
    }
}