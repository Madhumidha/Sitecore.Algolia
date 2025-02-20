using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Score.ContentSearch.Algolia.Dto;
using Algolia.Search.Models.Search;
using Algolia.Search.Models.Common;

namespace Score.ContentSearch.Algolia.Abstract
{
    public interface IAlgoliaRepository
    {
        Task<BatchIndexingResponse> SaveObjectsAsync(IEnumerable<JObject> objects);
        Task<BatchIndexingResponse> AddObjectAsync(JObject content, string objectId = null);
        Task<int> DeleteAllObjByTag(string tag);
        Task WaitTaskAsync(long taskID);
        Task<SearchResponse<JObject>> SearchAsync(global::Algolia.Search.Models.Search.Query q);
        Task<DeleteResponse> ClearIndexAsync();
        AlgoliaIndexInfo GetIndexInfo();
    }
}