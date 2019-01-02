using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownMaker.MarkdownDeep
{
    /// <summary>
    /// Class FootnoteReference.
    /// </summary>
    class FootnoteReference
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="FootnoteReference"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="id">The identifier.</param>
        public FootnoteReference(int index, string id)
		{
			this.index = index;
			this.id = id;
		}

		public int index;
		public string id;
	}
}
