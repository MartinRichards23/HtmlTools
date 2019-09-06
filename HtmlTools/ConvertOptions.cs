using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlTools
{
    public class ConvertOptions
    {
        public bool ImageBrackets { get; set; } = true;

        public static ConvertOptions Default
        {
            get { return new ConvertOptions(); }
        }
    }
}
