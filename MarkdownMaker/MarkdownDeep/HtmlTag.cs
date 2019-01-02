using MarkdownMaker.MarkdownDeep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownMaker.MarkdownDeep
{
    /// <summary>
    /// Class HtmlTag.
    /// </summary>
    public class HtmlTag
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTag"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public HtmlTag(string name)
		{
			m_name = name;
		}

        // Get the tag name eg: "div"
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name
		{
			get { return m_name; }
		}

        // Get a dictionary of attribute values (no decoding done)
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public Dictionary<string, string> attributes
		{
			get { return m_attributes; }
		}

        // Is this tag closed eg; <br />
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="HtmlTag"/> is closed.
        /// </summary>
        /// <value><c>true</c> if closed; otherwise, <c>false</c>.</value>
        public bool closed
		{
			get { return m_closed; }
			set { m_closed = value; }
		}

        // Is this a closing tag eg: </div>
        /// <summary>
        /// Gets a value indicating whether this <see cref="HtmlTag"/> is closing.
        /// </summary>
        /// <value><c>true</c> if closing; otherwise, <c>false</c>.</value>
        public bool closing
		{
			get { return m_closing; }
		}

        /// <summary>
        /// The m name
        /// </summary>
        string m_name;
        /// <summary>
        /// The m attributes
        /// </summary>
        Dictionary<string, string> m_attributes = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        /// <summary>
        /// The m closed
        /// </summary>
        bool m_closed;
        /// <summary>
        /// The m closing
        /// </summary>
        bool m_closing;
        /// <summary>
        /// The m flags
        /// </summary>
        HtmlTagFlags m_flags = 0;

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        public HtmlTagFlags Flags
		{
			get
			{
				if (m_flags == 0)
				{
					if (!m_tag_flags.TryGetValue(name.ToLower(), out m_flags))
					{
						m_flags |= HtmlTagFlags.Inline;
					}
				}

				return m_flags;
			}
		}

        /// <summary>
        /// The m allowed tags
        /// </summary>
        static string[] m_allowed_tags = new string [] {
			"b","blockquote","code","dd","dt","dl","del","em","h1","h2","h3","h4","h5","h6","i","kbd","li","ol","ul",
			"p", "pre", "s", "sub", "sup", "strong", "strike", "img", "a"
		};

        /// <summary>
        /// The m allowed attributes
        /// </summary>
        static Dictionary<string, string[]> m_allowed_attributes = new Dictionary<string, string[]>() {
			{ "a", new string[] { "href", "title", "class" } },
			{ "img", new string[] { "src", "width", "height", "alt", "title", "class" } },
		};

        /// <summary>
        /// The m tag flags
        /// </summary>
        static Dictionary<string, HtmlTagFlags> m_tag_flags = new Dictionary<string, HtmlTagFlags>() {
			{ "p", HtmlTagFlags.Block | HtmlTagFlags.ContentAsSpan }, 
			{ "div", HtmlTagFlags.Block }, 
			{ "h1", HtmlTagFlags.Block | HtmlTagFlags.ContentAsSpan }, 
			{ "h2", HtmlTagFlags.Block | HtmlTagFlags.ContentAsSpan}, 
			{ "h3", HtmlTagFlags.Block | HtmlTagFlags.ContentAsSpan}, 
			{ "h4", HtmlTagFlags.Block | HtmlTagFlags.ContentAsSpan}, 
			{ "h5", HtmlTagFlags.Block | HtmlTagFlags.ContentAsSpan}, 
			{ "h6", HtmlTagFlags.Block | HtmlTagFlags.ContentAsSpan}, 
			{ "blockquote", HtmlTagFlags.Block }, 
			{ "pre", HtmlTagFlags.Block }, 
			{ "table", HtmlTagFlags.Block }, 
			{ "dl", HtmlTagFlags.Block }, 
			{ "ol", HtmlTagFlags.Block }, 
			{ "ul", HtmlTagFlags.Block }, 
			{ "form", HtmlTagFlags.Block }, 
			{ "fieldset", HtmlTagFlags.Block }, 
			{ "iframe", HtmlTagFlags.Block }, 
			{ "script", HtmlTagFlags.Block | HtmlTagFlags.Inline }, 
			{ "noscript", HtmlTagFlags.Block | HtmlTagFlags.Inline }, 
			{ "math", HtmlTagFlags.Block | HtmlTagFlags.Inline }, 
			{ "ins", HtmlTagFlags.Block | HtmlTagFlags.Inline }, 
			{ "del", HtmlTagFlags.Block | HtmlTagFlags.Inline }, 
			{ "img", HtmlTagFlags.Block | HtmlTagFlags.Inline }, 
			{ "li", HtmlTagFlags.ContentAsSpan}, 
			{ "dd", HtmlTagFlags.ContentAsSpan}, 
			{ "dt", HtmlTagFlags.ContentAsSpan}, 
			{ "td", HtmlTagFlags.ContentAsSpan}, 
			{ "th", HtmlTagFlags.ContentAsSpan}, 
			{ "legend", HtmlTagFlags.ContentAsSpan}, 
			{ "address", HtmlTagFlags.ContentAsSpan}, 
			{ "hr", HtmlTagFlags.Block | HtmlTagFlags.NoClosing}, 
			{ "!", HtmlTagFlags.Block | HtmlTagFlags.NoClosing}, 
			{ "head", HtmlTagFlags.Block }, 
		};

        // Check if this tag is safe
        /// <summary>
        /// Determines whether this instance is safe.
        /// </summary>
        /// <returns><c>true</c> if this instance is safe; otherwise, <c>false</c>.</returns>
        public bool IsSafe()
		{
			string name_lower=m_name.ToLowerInvariant();

			// Check if tag is in whitelist
			if (!Utils.IsInList(name_lower, m_allowed_tags))
				return false;

			// Find allowed attributes
			string[] allowed_attributes;
			if (!m_allowed_attributes.TryGetValue(name_lower, out allowed_attributes))
			{
				// No allowed attributes, check we don't have any
				return m_attributes.Count == 0;
			}

			// Check all are allowed
			foreach (var i in m_attributes)
			{
				if (!Utils.IsInList(i.Key.ToLowerInvariant(), allowed_attributes))
					return false;
			}

			// Check href attribute is ok
			string href;
			if (m_attributes.TryGetValue("href", out href))
			{
				if (!Utils.IsSafeUrl(href))
					return false;
			}

			string src;
			if (m_attributes.TryGetValue("src", out src))
			{
				if (!Utils.IsSafeUrl(src))
					return false;
			}

			// Passed all white list checks, allow it
			return true;
		}

        // Render opening tag (eg: <tag attr="value">
        /// <summary>
        /// Renders the opening.
        /// </summary>
        /// <param name="dest">The dest.</param>
        public void RenderOpening(StringBuilder dest)
		{
			dest.Append("<");
			dest.Append(name);
			foreach (var i in m_attributes)
			{
				dest.Append(" ");
				dest.Append(i.Key);
				dest.Append("=\"");
				dest.Append(i.Value);
				dest.Append("\"");
			}

			if (m_closed)
				dest.Append(" />");
			else
				dest.Append(">");
		}

        // Render closing tag (eg: </tag>)
        /// <summary>
        /// Renders the closing.
        /// </summary>
        /// <param name="dest">The dest.</param>
        public void RenderClosing(StringBuilder dest)
		{
			dest.Append("</");
			dest.Append(name);
			dest.Append(">");
		}

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="pos">The position.</param>
        /// <returns>HtmlTag.</returns>
        public static HtmlTag Parse(string str, ref int pos)
		{
			StringScanner sp = new StringScanner(str, pos);
			var ret = Parse(sp);

			if (ret!=null)
			{
				pos = sp.position;
				return ret;
			}

			return null;
		}

        /// <summary>
        /// Parses the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>HtmlTag.</returns>
        public static HtmlTag Parse(StringScanner p)
		{
			// Save position
			int savepos = p.position;

			// Parse it
			var ret = ParseHelper(p);
			if (ret!=null)
				return ret;

			// Rewind if failed
			p.position = savepos;
			return null;
		}

        /// <summary>
        /// Parses the helper.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>HtmlTag.</returns>
        private static HtmlTag ParseHelper(StringScanner p)
		{
			// Does it look like a tag?
			if (p.current != '<')
				return null;

			// Skip '<'
			p.SkipForward(1);

			// Is it a comment?
			if (p.SkipString("!--"))
			{
				p.Mark();

				if (p.Find("-->"))
				{
					var t = new HtmlTag("!");
					t.m_attributes.Add("content", p.Extract());
					t.m_closed = true;
					p.SkipForward(3);
					return t;
				}
			}

			// Is it a closing tag eg: </div>
			bool bClosing = p.SkipChar('/');

			// Get the tag name
			string tagName=null;
			if (!p.SkipIdentifier(ref tagName))
				return null;

			// Probably a tag, create the HtmlTag object now
			HtmlTag tag = new HtmlTag(tagName);
			tag.m_closing = bClosing;


			// If it's a closing tag, no attributes
			if (bClosing)
			{
				if (p.current != '>')
					return null;

				p.SkipForward(1);
				return tag;
			}

			while (!p.eof)
			{
				// Skip whitespace
				p.SkipWhitespace();

				// Check for closed tag eg: <hr />
				if (p.SkipString("/>"))
				{
					tag.m_closed=true;
					return tag;
				}

				// End of tag?
				if (p.SkipChar('>'))
				{
					return tag;
				}

				// attribute name
				string attributeName = null;
				if (!p.SkipIdentifier(ref attributeName))
					return null;

				// Skip whitespace
				p.SkipWhitespace();

				// Skip equal sign
				if (p.SkipChar('='))
				{
					// Skip whitespace
					p.SkipWhitespace();

					// Optional quotes
					if (p.SkipChar('\"'))
					{
						// Scan the value
						p.Mark();
						if (!p.Find('\"'))
							return null;

						// Store the value
						tag.m_attributes.Add(attributeName, p.Extract());

						// Skip closing quote
						p.SkipForward(1);
					}
					else
					{
						// Scan the value
						p.Mark();
						while (!p.eof && !char.IsWhiteSpace(p.current) && p.current != '>' && p.current != '/')
							p.SkipForward(1);

						if (!p.eof)
						{
							// Store the value
							tag.m_attributes.Add(attributeName, p.Extract());
						}
					}
				}
				else
				{
					tag.m_attributes.Add(attributeName, "");
				}
			}

			return null;
		}
	}
}