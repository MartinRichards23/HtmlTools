using System.Collections.Generic;

namespace HtmlTools.Converter
{
    public abstract class HtmlLine
    {
        public HtmlLine(int lineNumber, string node, string xPath)
        {
            LineNumber = lineNumber;
            Node = node;
            XPath = xPath;
        }

        public int LineNumber { get; private set; }
        public string Node { get; private set; }
        public string XPath { get; private set; }

        public string Text { get; set; }
        public string NormalisedValue { get; set; }

        public virtual string GetFormattedText(ConvertOptions options)
        {
            return Text;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class HtmlTextLine : HtmlLine
    {
        public HtmlTextLine(int lineNumber, string node, string xPath) : base(lineNumber, node, xPath)
        { }
    }

    public class HtmlImageLine : HtmlLine
    {
        public HtmlImageLine(int lineNumber, string node, string xPath) : base(lineNumber, node, xPath)
        { }

        public string Src { get; set; }

        public override string GetFormattedText(ConvertOptions options)
        {
            if (string.IsNullOrEmpty(Text))
                return null;

            if (options.ImageBrackets)
                return "[Image: " + Text + "]";
            else
                return Text;
        }
    }

    public class HtmlLineCollection
    {
        public HtmlLineCollection()
        {
            Lines = new List<HtmlLine>();
        }

        public List<HtmlLine> Lines { get; private set; }
        public HtmlTextLine CurrentLine { get; private set; }

        public void AddImageLine(string node, string xPath, string image, string alt)
        {
            CheckLastLine();

            HtmlImageLine imageLine = new HtmlImageLine(Lines.Count, node, xPath)
            {
                Src = image,
                Text = alt,
                NormalisedValue = image.ToUpperInvariant(),
            };

            Lines.Add(imageLine);

            CurrentLine = null;
        }

        public void AddTextLine(string node, string xPath)
        {
            CheckLastLine();
            
            HtmlTextLine textLine = new HtmlTextLine(Lines.Count, node, xPath);
            Lines.Add(textLine);

            CurrentLine = textLine;
        }

        private void CheckLastLine()
        {
            if (CurrentLine != null)
            {
                if (string.IsNullOrEmpty(CurrentLine.Text))
                {
                    Lines.Remove(CurrentLine);
                }
            }
        }

        public void AddText(string text)
        {
            if (CurrentLine != null)
                CurrentLine.Text += text;
        }
    }
}
