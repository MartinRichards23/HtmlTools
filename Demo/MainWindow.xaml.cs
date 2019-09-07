using HtmlAgilityPack;
using HtmlTools;
using HtmlTools.Converter;
using HtmlTools.Filtering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = txtUrl.Text;
                Uri uri = new Uri(url);
                string originalHtml = await GetHtml(uri);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(originalHtml);

                HtmlFilter filter = new HtmlFilter();
                filter.CleanHtml(doc.DocumentNode);

                HtmlConverter converter = new HtmlConverter();
                var lines = converter.GetLines(doc.DocumentNode, new ConvertOptions());

                StringBuilder textContent = new StringBuilder();

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
                            textContent.AppendLine(img);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(textLine.Text))
                    {
                        string text = textLine.Text.Trim();
                        
                        text = StringTools.HtmlEncode(text);

                        //text = Highlighter.ApplyHighlights(text, WebMonitorSettings.KeywordColour, config.KeywordRegexes);
                        text = text.Replace("\r\n", "<br/>");
                        text = HtmlTools.HtmlTools.WrapContent(text, textLine.Node);

                        textContent.Append(text);
                    }

                    textContent.Append("<br style=\"clear:both; \" />");
                }
                
                webBrowser.NavigateToString(textContent.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task<string> GetHtml(Uri uri)
        {
            string html;

            using (WebClient client = new WebClient())
            {
                html = await client.DownloadStringTaskAsync(uri);
            }

            File.WriteAllText("html.html", html);

            return html;
        }
    }
}
