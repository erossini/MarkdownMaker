using MarkdownMaker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.MarkdownMaker.Extensions;

namespace UnitTests.MarkdownMaker
{
    [TestClass]
    public class HorizontalRuleTests
    {
        [TestMethod]
        public void TestHorizontalRuleCanBeDrawn()
        {
            var rule = new HorizontalRule();

            rule.AssertOutputEquals(
                "--------------------------------------------------------------------------------\r\n"
                ,
                "<hr />\n");
        }

    }
}