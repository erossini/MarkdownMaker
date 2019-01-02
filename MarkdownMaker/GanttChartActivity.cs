namespace MarkdownMaker
{
    public class GanttChartActivity
    {
        private string _name = "";

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public double StartValue { get; set; }
        public double EndValue { get; set; }
    }
}