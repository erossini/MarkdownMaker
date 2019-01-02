using System;

namespace MarkdownMaker
{
    public class HorizontalRule : MarkdownElement
    {
        public override string ToMarkdown()
        {
            return new string('-', 80) + Environment.NewLine;
        }
    }
}