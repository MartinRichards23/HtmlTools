using System.Collections.Generic;

namespace HtmlTools.Diffing
{
    /// <summary>
    /// Holds a collection of consecutive line diffs
    /// </summary>
    public class LineDiffGroup
    {
        public List<LineDiff> Diffs { get; } = new List<LineDiff>();
    }
}
