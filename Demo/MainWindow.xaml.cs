using HtmlAgilityPack;
using HtmlTools;
using HtmlTools.Converter;
using HtmlTools.Diffing;
using HtmlTools.Filtering;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Utilities.HideScriptErrors(webBrowserDiff, true);
        }

        private async void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = txtUrl.Text;
                Uri uri = new Uri(url);

                // download the html
                string originalHtml = await GetHtml(uri);

                // create the document
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(originalHtml);

                // filter out parts of little interest
                HtmlFilter filter = new HtmlFilter();
                filter.CleanHtml(doc.DocumentNode);

                // convert html into lines of text (and images)
                HtmlConverter converter = new HtmlConverter();
                var lines = converter.GetLines(doc.DocumentNode, new ConvertOptions());

                StringBuilder htmlContent = new StringBuilder();

                // create html from lines
                foreach (var textLine in lines)
                {
                    // is an image line so add image here
                    if (textLine is HtmlImageLine imageLine)
                    {
                        string src = NetTools.MakeAbsoluteUrl(uri.Host, imageLine.Src);
                        string alt = StringTools.HtmlEncode(imageLine.Text);

                        if (!string.IsNullOrWhiteSpace(src))
                        {
                            string img = $"<img src=\"{src}\" alt=\"{alt}\" width=\"150\" />";
                            htmlContent.AppendLine(img);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(textLine.Text))
                    {
                        string text = textLine.Text.Trim();
                        
                        text = StringTools.HtmlEncode(text);

                        text = text.Replace("\r\n", "<br/>");
                        text = HtmlTools.HtmlTools.WrapContent(text, textLine.Node);

                        htmlContent.Append(text);
                    }

                    htmlContent.Append("<br style=\"clear:both; \" />");
                }
                
                webBrowser.NavigateToString(htmlContent.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // First load the html into a document objects we can manipulate
                string html1 = await File.ReadAllTextAsync("Resources/bbc1.html");
                HtmlDocument doc1 = new HtmlDocument();
                doc1.LoadHtml(html1);

                string html2 = await File.ReadAllTextAsync("Resources/bbc2.html");
                HtmlDocument doc2 = new HtmlDocument();
                doc2.LoadHtml(html2);

                // Filter the html to remove parts that are not very interesting
                HtmlFilter filter = new HtmlFilter();
                filter.CleanHtml(doc1.DocumentNode);
                filter.CleanHtml(doc2.DocumentNode);

                // convert the html into lines
                HtmlConverter converter = new HtmlConverter();
                var lines1 = converter.GetLines(doc1.DocumentNode, ConvertOptions.Default);
                var lines2 = converter.GetLines(doc2.DocumentNode, ConvertOptions.Default);
                
                // find the difference between these 2 sets of lines
                LineDiffer differ = new LineDiffer();
                var diffResult = differ.GetDiff(lines1, lines2, true);

                // load the original html into a document again
                HtmlDocument originalDoc = new HtmlDocument();
                originalDoc.LoadHtml(html1);

                // highlight the changed parts of the document
                Highlighter.ApplyHighlights(originalDoc, "#FFFF99", diffResult.Added.Select(i => i.Line.XPath));

                string htmlHighlighted = originalDoc.Save();

                webBrowserDiff.NavigateToString(htmlHighlighted);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task<string> GetHtml(Uri uri)
        {
            string html;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.KeepAlive = true;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";

            using (HttpWebResponse response = (HttpWebResponse) (await request.GetResponseAsync()))
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                html = await readStream.ReadToEndAsync();
            }

            html = HtmlTools.HtmlTools.AddBaseTag(html, uri);

            return html;
        }
    }
}
