using System.Collections.Generic;

namespace HtmlTools.Diffing
{
    public class FilterResult
    {
        public FilterResult()
        {
            MatchedKeywords = new List<string>();
        }
        
        public bool FailedIgnoreWord { get; set; }
        public bool FailedKeyWords { get; set; }
        
        public List<string> MatchedKeywords { get; private set; }
    }
}
