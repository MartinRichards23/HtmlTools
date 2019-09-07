using HtmlTools.Converter;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlTools.Diffing
{
    /// <summary>
    /// Find the diff between 2 sets of lines
    /// </summary>
    public class LineDiffer
    {
        public LineDiffer()
        {

        }

        public DiffResult GetDiff(IList<HtmlLine> newLines, IList<HtmlLine> oldLines, bool ignoreSmallChanges)
        {
            IList<LineDiff> changes = GetDiffLines(newLines, oldLines);

            IList<LineDiff> addedDiffs = new List<LineDiff>();
            IList<LineDiff> removedDiffs = new List<LineDiff>();
            ISet<string> uniqueInserts = new HashSet<string>();
            ISet<string> uniqueDeletes = new HashSet<string>();
            
            foreach (LineDiff change in changes)
            {
                HtmlLine textLine = change.Line;

                string origText = textLine.Text?.Trim();

                if (string.IsNullOrWhiteSpace(origText))
                    continue;

                if (change.Operation == OperationType.Insert)
                {
                    // ensure no duplication
                    if (!uniqueInserts.Add(origText))
                        continue;

                    if (ignoreSmallChanges)
                    {
                        // ignore short change
                        if (origText.Length < 10)
                            continue;
                    }
                    
                    addedDiffs.Add(change);
                }
                else if (change.Operation == OperationType.Delete)
                {
                    // ensure no duplication
                    if (!uniqueDeletes.Add(origText))
                        continue;

                    if (ignoreSmallChanges)
                    {
                        // ignore short change
                        if (origText.Length < 10)
                            continue;
                    }

                    removedDiffs.Add(change);
                }
            }
            
            return new DiffResult(addedDiffs, removedDiffs);
        }

        private IList<LineDiff> GetDiffLines(IList<HtmlLine> newLines, IList<HtmlLine> oldLines)
        {
            IList<LineDiff> diffs = new List<LineDiff>();

            // make hashsets for quick lookup
            HashSet<string> newLinesSet = new HashSet<string>();
            foreach (var item in newLines)
            {
                newLinesSet.Add(item.NormalisedValue);
            }

            HashSet<string> oldLinesSet = new HashSet<string>();
            foreach (var item in oldLines)
            {
                oldLinesSet.Add(item.NormalisedValue);
            }

            // find all added lines
            foreach (HtmlLine line in newLines)
            {
                if (oldLinesSet.Contains(line.NormalisedValue))
                {
                    // old lines also has this line, so not new
                    continue;
                }

                LineDiff diff = new LineDiff(OperationType.Insert, line);
                diffs.Add(diff);
            }

            // find all deleted lines
            foreach (HtmlLine line in oldLines)
            {
                if (newLinesSet.Contains(line.NormalisedValue))
                {
                    // new lines also has this line, so not removed
                    continue;
                }

                //if (lines1.Any(l => l.NormalisedValue == line.NormalisedValue))
                //    continue;

                LineDiff diff = new LineDiff(OperationType.Delete, line);
                diffs.Add(diff);
            }

            return diffs;
        }
    }
}
