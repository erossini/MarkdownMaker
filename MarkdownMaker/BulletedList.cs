using System.Collections.Generic;

namespace MarkdownMaker
{
    public class BulletedList : ListBase
    {
        public BulletedList(params string[] items) : base(items)
        {
        }

        public BulletedList(IEnumerable<string> items) : base(items)
        {
        }

        protected override string GetListItemFirstLinePrefix(int itemNumber)
        {
            return "   * ";
        }
    }
}