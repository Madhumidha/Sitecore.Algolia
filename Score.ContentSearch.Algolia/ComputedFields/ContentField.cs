using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using DocumentFormat.OpenXml.Bibliography;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Search.Crawlers.FieldCrawlers;
using Sitecore.Text;

namespace Score.ContentSearch.Algolia.ComputedFields
{
    public class ContentField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        private readonly string[] _textFieldTypes = { "single-line text", "multi-line text", "rich text" };

        private readonly string[] _referenceFieldTypes = { "nmdp multiroot treelist", "treelist", "treelist ex", "multilist" };      

        public object ComputeFieldValue(IIndexable indexable)
        {
            try
            {
                List<string> result = new List<string>();
                var indexableItem = (SitecoreIndexableItem)indexable;
                if (indexableItem == null)
                    return null;
                var database = indexableItem.Item.Database;
                if (database == null)
                    return null;

                if (TemplateCheck(database, indexableItem.Item))
                {

                    // Get all renderings
                    var renderings = indexableItem.Item.Visualization.GetRenderings(DeviceItem.ResolveDevice(database), false);
                    foreach (var reference in renderings)
                    {
                        // Get the source item
                        if (reference.RenderingItem == null)
                            continue;

                        var datasource = reference.Settings.DataSource;

                        if (string.IsNullOrEmpty(datasource))
                            continue;

                        var sourceId = ID.Parse(datasource);

                        var source = database.GetItem(sourceId, indexableItem.Item.Language);

                        if (source == null)
                            continue;

                        // Go through all fields
                        foreach (Field field in source.Fields)
                        {
                            if (field != null && !field.Name.StartsWith("__"))
                            //if (field != null && !field.Name.StartsWith("__") && GetFieldConfigList(database).Contains(field.Key))
                            {
                                if (!string.IsNullOrEmpty(field.TypeKey))
                                {
                                    switch (field.TypeKey)
                                    {
                                        case "single-line text":
                                        case "multi-line text":
                                        case "rich text":
                                            FieldCrawlerBase fieldCrawler = FieldCrawlerFactory.GetFieldCrawler(field);
                                            var fieldvalue = fieldCrawler != null ? fieldCrawler.GetValue() : string.Empty;
                                            result.Add(fieldvalue);
                                            break;
                                        case "nmdp multiroot treelist":
                                        case "treelist":
                                        case "treelist ex":
                                        case "multilist":
                                            GetReferenceFieldData(field, database, result);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                //var value = GetFieldValue(field);

                                //if (!string.IsNullOrEmpty(value))
                                //    result.Add(value);
                            }
                        }
                    }

                    if (result.Any())
                    {
                        var concatinatedResults = string.Join(" ", result);
                        return concatinatedResults;
                    }               

                }

            }
            catch (Exception ex)
            {
                Log.Error("Error occurred at Algolia.ComputedFields.ContentField", ex.Message);
            }

            return null;
        }

        private void GetReferenceFieldData(Field field, Database db, List<string> result)
        {
            string referenceField = field.Value;
            if (!string.IsNullOrEmpty(referenceField))
            {
                string[] items = referenceField.Split('|');
                if (items != null && items.Any())
                {
                    foreach (string item in items)
                    {
                        Item listItem = db.GetItem(item);
                        foreach (Field field2 in listItem.Fields)
                        {
                            if (field2 != null && !field2.Name.StartsWith("__"))
                            {
                                if (IsTextField(field2))
                                {
                                    FieldCrawlerBase fieldCrawler = FieldCrawlerFactory.GetFieldCrawler(field2);
                                    var fieldValue = fieldCrawler != null ? fieldCrawler.GetValue() : string.Empty;
                                    result.Add(fieldValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool TemplateCheck(Database database, Item item)
        {
            Item configItem = database.GetItem("{C3A10091-B242-4981-B7AD-A30C777F6051}");
            if (configItem != null)
            {
                MultilistField multilistField = configItem.Fields["AllowedTemplates"];

                var allowedTemplateItems = multilistField.GetItems();

                if (allowedTemplateItems != null && allowedTemplateItems.Any())
                {
                    return allowedTemplateItems.Any(x => x.ID == item.Template.ID);
                }
            }

            return false;
        }

        //private string GetFieldValue(Field field)
        //{

        //    if (IsTextField(field))
        //    {
        //        FieldCrawlerBase fieldCrawler = FieldCrawlerFactory.GetFieldCrawler(field);
        //        return fieldCrawler != null ? fieldCrawler.GetValue() : string.Empty;
        //    }
        //    else if (IsReferenceField(field))
        //    {

        //        var listString = field.Value;
        //    }
        //}


        protected virtual bool IsTextField(Field field)
        {
            Assert.ArgumentNotNull(field, "field");
            if (_textFieldTypes.Contains(field.TypeKey))
            {
                return true;
            }
            //TemplateField templateField = field.GetTemplateField();
            //return templateField == null || !templateField.ExcludeFromTextSearch;

            return false;
        }

        protected virtual bool IsReferenceField(Field field)
        {
            Assert.ArgumentNotNull(field, "field");
            if (_referenceFieldTypes.Contains(field.TypeKey))
            {
                return true;
            }
            //TemplateField templateField = field.GetTemplateField();
            //return templateField == null || !templateField.ExcludeFromTextSearch;

            return false;
        }

        private List<string> GetFieldConfigList(Database db)
        {
            Item configItem = db.GetItem("{C3A10091-B242-4981-B7AD-A30C777F6051}");
            if (configItem != null)
            {
                string fieldConfigString = configItem.Fields["FieldNameConfig"].Value;
                if (!string.IsNullOrEmpty(fieldConfigString))
                {
                    List<string> splitList = fieldConfigString.Split(',').ToList();
                    if (splitList != null && splitList.Any())
                    {
                        return splitList.Select(x => x.TrimStart().TrimEnd()).ToList();
                    }
                }
            }
            return new List<string>();
        }
    }
}
