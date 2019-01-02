using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownMaker.MarkdownDeep
{
    /// <summary>
    /// Enum HtmlTagFlags
    /// </summary>
    [Flags]
    public enum HtmlTagFlags
    {
        /// <summary>
        /// Block tag
        /// </summary>
        Block = 0x0001,

        /// <summary>
        /// Inline tag
        /// </summary>
        Inline = 0x0002,

        /// <summary>
        /// No closing tag (eg: <hr> and <!-- -->)
        /// </summary>
        NoClosing = 0x0004,

        /// <summary>
        /// When markdown=1 treat content as span, not block
        /// </summary>
        ContentAsSpan = 0x0008,
    };
}