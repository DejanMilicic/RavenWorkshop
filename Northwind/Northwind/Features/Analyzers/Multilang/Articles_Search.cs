using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Analyzers.Multilang;

public class Articles_Search : AbstractIndexCreationTask<Article>
{
    public class Entry
    {
        public string Text_en { get; set; }

        public string Text_fr { get; set; }

        public Dictionary<string, object> Text { get; set; }
    }

    public Articles_Search()
    {
        Map = articles => from article in articles
            let lang = string.IsNullOrWhiteSpace(article.Language) ?
                             "en" : article.Language.ToLower()
            select new
            {
                _ = CreateField("Text_" + lang, article.Text)
            };

        AnalyzersStrings = new Dictionary<string, string>
        {
            {"Text_en", "StandardAnalyzer"},
            {"Text_fr", "FrenchAnalyzer"}
        };
    }
}
