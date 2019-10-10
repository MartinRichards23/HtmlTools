using System;

namespace HtmlTools.Diffing
{
    /// <summary>
    /// Class representing one diff operation.
    /// </summary>
    public class Diff
    {
        public Diff(DiffType operation, string text)
        {
            Operation = operation;
            Text = text;
            OriginalText = text;
        }

        #region Properties

        public DiffType Operation { get; set; }
        public string Text { get; set; }
        public string OriginalText { get; private set; }

        #endregion

        #region Public methods

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
                return false;

            // If parameter cannot be cast to Diff return false.
            if (!(obj is Diff p))
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

        public override string ToString()
        {
            return string.Format("{0}: '{1}'", Operation, Text);
        }

        #endregion
    }

    public enum DiffType
    {
        Delete, Insert, Equal
    }
}
