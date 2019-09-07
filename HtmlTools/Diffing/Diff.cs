using System;

namespace HtmlTools.Diffing
{
    public enum OperationType
    {
        Delete, Insert, Equal
    }

    /// <summary>
    /// Class representing one diff operation.
    /// </summary>
    public class Diff
    {
        public Diff(OperationType operation, string text)
        {
            Operation = operation;
            Text = text;
            OriginalText = text;
        }

        public OperationType Operation { get; set; }
        public string Text { get; set; }
        public string OriginalText { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}: '{1}'", Operation, Text);
        }

        /**
         * Is this Diff equivalent to another Diff?
         * @param d Another Diff to compare against.
         * @return true or false.
         */
        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Diff return false.
            Diff p = obj as Diff;
            if (p == null)
                return false;

            // Return true if the fields match.
            return p.Operation == Operation && p.Text == Text;
        }

        public bool Equals(Diff obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // Return true if the fields match.
            return obj.Operation == Operation && obj.Text == Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode() ^ Operation.GetHashCode();
        }
    }
}
