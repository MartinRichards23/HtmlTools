using System.Collections.Generic;

namespace HtmlTools.Converter
{
    /// <summary>
    /// Base class for html lines
    /// </summary>
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

    /// <summary>
    /// A html line of text
    /// </summary>
    public class HtmlTextLine : HtmlLine
    {
        public HtmlTextLine(int lineNumber, string node, string xPath) : base(lineNumber, node, xPath)
        { }
    }

    /// <summary>
    /// An html image
    /// </summary>
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

    /// <summary>
    /// A collection of html lines
    /// </summary>
    public class HtmlLineCollection
    {
        public HtmlLineCollection()
        {
            Lines = new List<HtmlLine>();
        }

        public List<HtmlLine> Lines { get; private set; }
        public HtmlTextLine CurrentLine { get; private set; }

        /// <summary>
        /// Add a new image line
        /// </summary>
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

        /// <summary>
        /// Add a new text line
        /// </summary>
        public void AddTextLine(string node, string xPath)
        {
            CheckLastLine();
            
            HtmlTextLine textLine = new HtmlTextLine(Lines.Count, node, xPath);
            Lines.Add(textLine);

            CurrentLine = textLine;
        }

        /// <summary>
        /// Adds text to the current line
        /// </summary>
        public void AddText(string text)
        {
            if (CurrentLine != null)                
                CurrentLine.Text += text;
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
    }
}
