namespace MarkdownMaker
{
    public interface ITableCell
    {
        int RequiredWidth { get; }
        string BuildCodeFormattedString(TableCellRenderSpecification spec);
    }
}