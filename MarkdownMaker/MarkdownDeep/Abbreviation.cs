using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownMaker.MarkdownDeep
{
	class Abbreviation
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="Abbreviation"/> class.
        /// </summary>
        /// <param name="abbr">The abbr.</param>
        /// <param name="title">The title.</param>
        public Abbreviation(string abbr, string title)
		{
			Abbr = abbr;
			Title = title;
		}

        /// <summary>
        /// The abbr
        /// </summary>
        public string Abbr;

        /// <summary>
        /// The title
        /// </summary>
        public string Title;
	}
}
