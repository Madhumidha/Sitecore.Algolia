using Sitecore.Abstractions;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.Eventing;

namespace Score.ContentSearch.Algolia
{
    public class AlgoliaSearchIndex : AlgoliaBaseIndex
    {
        //public AlgoliaSearchIndex(string name, string applicationId, string fullApiKey, string indexName, IEventQueue eventQueue) :
        //    base(name, new AlgoliaRepository(new AlgoliaConfig { ApplicationId = applicationId, FullApiKey = fullApiKey, IndexName = indexName }), eventQueue)
        //{

        //}



        public AlgoliaSearchIndex(string name, string applicationId, string fullApiKey, string indexName, IIndexPropertyStore propertyStore) :
           base(name, new AlgoliaRepository(new AlgoliaConfig { ApplicationId = applicationId, FullApiKey = fullApiKey, IndexName = indexName }), propertyStore)
        {

        }
    }
}
