using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Algolia.Search.Models.Search;
using Algolia.Search.Clients;
using Newtonsoft.Json.Linq;
using Score.ContentSearch.Algolia.Abstract;
using Score.ContentSearch.Algolia.Dto;
using Sitecore.ContentSearch.Linq.Indexing;
using Algolia.Search.Models.Common;
using Sitecore.ContentSearch.Linq.Extensions;

namespace Score.ContentSearch.Algolia
{
    public class AlgoliaRepository : IAlgoliaRepository
    {
        
        private readonly SearchIndex _index;
        private readonly string _indexName;
        private readonly SearchClient _client;
        private int ApiChunkSize = 1000;

        public AlgoliaRepository(IAlgoliaConfig algoliaConfig)
        {
            _client = new SearchClient(algoliaConfig.ApplicationId, algoliaConfig.FullApiKey);
            _indexName = algoliaConfig.IndexName;
            _index = _client.InitIndex(_indexName);
        }

        public Task<BatchIndexingResponse> SaveObjectsAsync(IEnumerable<JObject> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
             return _index.SaveObjectsAsync<JObject>(objects);
        }

        public Task<BatchIndexingResponse> AddObjectAsync(JObject content, string objectId = null)
        {
            var result = _index.SaveObjectAsync<JObject>(content);
            return result;
        }

        public async Task<int> DeleteAllObjByTag(string tag)
        {
            IEnumerable<IEnumerable<string>> tagFilter = new List<List<string>>
            {
                new List<string> { tag }
            };
            var query = new Query();
            query.TagFilters = tagFilter;
            //query.Filters = tag;
            query.HitsPerPage = ApiChunkSize;
            query.AttributesToRetrieve = new List<string> { "objectID" };

            int processed = 0;
            IEnumerable<string> hits = await GetElements(query);
            while (hits.Any())
            {
                //BatchIndexingResponse deletionResponse = _index.DeleteObjects(hits);
                //var taskId = deletionResponse.Responses[0].TaskID;
                hits = await GetElements(query);
            }

            return processed;
        }

        private async Task<ICollection<string>> GetElements(Query query)
        {
            var data = await _index.SearchAsync<JObject>(query);
            var hits = data.Hits;

            var objectIds = hits.Select(hit => (string)hit["objectID"]).ToList();
            return objectIds;
        }

        public Task WaitTaskAsync(long taskID)
        {
            return _index.WaitTaskAsync(taskID);
        }

        public Task<SearchResponse<JObject>> SearchAsync(Query q)
        {
            return _index.SearchAsync<JObject>(q);
        }

        public AlgoliaIndexInfo GetIndexInfo()
        {
            ListIndicesResponse response = _client.ListIndices();
            return AlgoliaIndexInfo.LoadFromJson(response, _indexName);
        }
        
        public Task<DeleteResponse> ClearIndexAsync()
        {
            return _index.ClearObjectsAsync();
        }
    }
}
