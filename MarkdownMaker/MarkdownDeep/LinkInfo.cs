using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownMaker.MarkdownDeep
{
    /// <summary>
    /// Class LinkInfo.
    /// </summary>
    internal class LinkInfo
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkInfo"/> class.
        /// </summary>
        /// <param name="def">The definition.</param>
        /// <param name="link_text">The link text.</param>
        public LinkInfo(LinkDefinition def, string link_text)
		{
			this.def = def;
			this.link_text = link_text;
		}

        /// <summary>
        /// The definition
        /// </summary>
        public LinkDefinition def;

        /// <summary>
        /// The link text
        /// </summary>
        public string link_text;
	}
}