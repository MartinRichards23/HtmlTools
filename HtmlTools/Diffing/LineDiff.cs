using HtmlTools.Converter;

namespace HtmlTools.Diffing
{
    public class LineDiff
    {
        public LineDiff(DiffType operation, HtmlLine line)
        {
            Operation = operation;
            Line = line;
        }

        #region Properties

        public DiffType Operation { get; set; }
        public HtmlLine Line { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}: '{1}'", Operation, Line);
        }
    }
}
