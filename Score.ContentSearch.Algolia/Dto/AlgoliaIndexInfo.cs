using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algolia.Search.Models.Common;
using Newtonsoft.Json.Linq;

namespace Score.ContentSearch.Algolia.Dto
{
    /// <summary>
    /// Dto for https://www.algolia.com/doc/rest#list-indexes
    /// </summary>
    public class AlgoliaIndexInfo
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long Entries { get; set; }
        public bool PendingTask { get; set; }
        public int LastBuildTimeS { get; set; }
        public long DataSize { get; set; }


        public static AlgoliaIndexInfo LoadFromJson(ListIndicesResponse data, string indexName)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var indexInfo = (from info in data.Items
                where string.Equals((string)info.Name, indexName, StringComparison.InvariantCultureIgnoreCase)
                select info).FirstOrDefault();

            if (indexInfo == null)
                return new AlgoliaIndexInfo();

            DateTime createdAt = (DateTime)indexInfo.CreatedAt;
            DateTime updatedAt = (DateTime)indexInfo.UpdatedAt;
            return new AlgoliaIndexInfo
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                Entries = (long)indexInfo.Entries,
                PendingTask = (bool)indexInfo.PendingTask,
                LastBuildTimeS = (int)indexInfo.LastBuildTimes,
                DataSize = (long)indexInfo.DataSize,
            };
        }
    }
}
