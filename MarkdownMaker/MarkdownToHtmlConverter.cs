namespace MarkdownMaker
{
    public class MarkdownToHtmlConverter
    {
        public string Transform(string markdown)
        {
            var markdownDeep = new MarkdownDeep.Markdown { ExtraMode = true };
            return markdownDeep.Transform(markdown);
        }
    }
}