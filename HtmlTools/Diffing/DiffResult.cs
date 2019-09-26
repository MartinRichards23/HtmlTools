using System.Collections.Generic;

namespace HtmlTools.Diffing
{
    /// <summary>
    /// Result of a diffing operation
    /// </summary>
    public class DiffResult
    {
        public DiffResult(IList<LineDiff> added, IList<LineDiff> removed)
        {
            Added = added;
            Removed = removed;
        }

        #region Properties

        public IList<LineDiff> Added { get; }
        public IList<LineDiff> Removed { get; }

        #endregion
    }
}
