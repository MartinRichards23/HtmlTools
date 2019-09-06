using HtmlTools;
using HtmlTools.Converter;
using System;
using System.Collections.Generic;
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

                string originalHtml;

                using (WebClient client = new WebClient())
                {
                    originalHtml = await client.DownloadStringTaskAsync(uri);
                }

                HtmlConverter converter = new HtmlConverter();

                var lines = converter.GetLines(originalHtml, new ConvertOptions());

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
                            string img = string.Format("<img src=\"{0}\" alt=\"{1}\" width=\"150\" />", src, alt);
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
    }
}
