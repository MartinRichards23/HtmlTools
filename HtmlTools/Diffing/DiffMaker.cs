using HtmlAgilityPack;
using HtmlTools.Converter;
using HtmlTools.Filtering;
using System.Collections.Generic;

namespace HtmlTools.Diffing
{
    public class DiffMaker
    {
        public DiffMaker()
        {

        }

        public DiffResult Make()
        {
           HtmlFilter htmlFilter = new HtmlFilter();
            HtmlConverter convert = new HtmlConverter();
            LineDiffer diffEngine = new LineDiffer();

            string html1 = "";
            string html2 = "";

            HtmlDocument doc1 = new HtmlDocument();
            doc1.Load(html1);

            htmlFilter.CleanHtml(doc1.DocumentNode);
            IList<HtmlLine> lines1 = convert.GetLines(doc1.DocumentNode, ConvertOptions.Default);

            HtmlDocument doc2 = new HtmlDocument();
            doc2.Load(html2);

            htmlFilter.CleanHtml(doc2.DocumentNode);
            IList<HtmlLine> lines2 = convert.GetLines(doc2.DocumentNode, ConvertOptions.Default);

            DiffResult result = diffEngine.GetDiff(lines1, lines2, true);

            return result;
        }
    }
}
