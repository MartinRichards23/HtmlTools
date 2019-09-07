using HtmlTools.Converter;

namespace HtmlTools.Diffing
{
    public class LineDiff
    {
        public LineDiff(OperationType operation, HtmlLine line)
        {
            Operation = operation;
            Line = line;
        }

        public OperationType Operation { get; set; }
        public HtmlLine Line { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: '{1}'", Operation, Line);
        }
    }
}
