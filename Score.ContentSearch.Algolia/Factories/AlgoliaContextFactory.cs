using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SolrProvider.Abstractions;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.SolrProvider;
using System.Collections.Generic;
using SolrNet;
using Sitecore.ContentSearch.Abstractions.Factories;
using static System.Windows.Forms.Design.AxImporter;

namespace Score.ContentSearch.Algolia.Factories
{

     public class AlgoliaContextFactory : AbstractContextFactory<AlgoliaBaseIndex> 
    //public class AlgoliaContextFactory : IProviderContextFactory
    ////public class AlgoliaContextFactory : IProviderContextFactory
    {

        protected override IProviderDeleteContext GetDeleteContext(AlgoliaBaseIndex searchIndex)
        {
            return searchIndex.CreateDeleteContext();
        }

        protected override IProviderSearchContext GetSearchContext(AlgoliaBaseIndex searchIndex, SearchSecurityOptions options)
        {
            return searchIndex.CreateSearchContext(options);
        }

        protected override IProviderUpdateContext GetUpdateContext(AlgoliaBaseIndex searchIndex)
        {
            return searchIndex.CreateUpdateContext();
        }
    }

    //public class AlgoliaContextFactory : ISolrSearchProviderContextFactory
    //{

    //    public IProviderDeleteContext CreateDeleteContext(ISearchIndex searchIndex)
    //    {
    //        return searchIndex.CreateDeleteContext();
    //    }

    //    public IProviderSearchContext CreateSearchContext(ISearchIndex searchIndex, SearchSecurityOptions options)
    //    {
    //        return searchIndex.CreateSearchContext(options);
    //    }       

    //    public IProviderUpdateContext CreateUpdateContext(ISearchIndex searchIndex)
    //    {
    //        return searchIndex.CreateUpdateContext();
    //    }

    //    public IProviderDeleteContextEx CreateDeleteContext(SolrSearchIndex searchIndex, ISolrOperations<Dictionary<string, object>> solr)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public IProviderUpdateContext CreateUpdateContext(SolrSearchIndex searchIndex, ISolrOperations<Dictionary<string, object>> solr)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //}

}
