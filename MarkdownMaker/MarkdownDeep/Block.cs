using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownMaker.MarkdownDeep
{
	// Some block types are only used during block parsing, some
	// are only used during rendering and some are used during both
	internal enum BlockType
	{
        /// <summary>
        /// blank line (parse only)
        /// </summary>
        Blank,

        /// <summary>
        /// H1 headings (render and parse)
        /// </summary>
        h1,

        /// <summary>
        /// H2 headings (render and parse)
        /// </summary>
        h2,

        /// <summary>
        /// H3 headings (render and parse)
        /// </summary>
        h3,

        /// <summary>
        /// H4 headings (render and parse)
        /// </summary>
        h4,

        /// <summary>
        /// H5 headings (render and parse)
        /// </summary>
        h5,

        /// <summary>
        /// H6 headings (render and parse)
        /// </summary>
        h6,

        /// <summary>
        /// h1 setext heading lines (parse only)
        /// </summary>
        post_h1,

        /// <summary>
        /// h2 setext heading lines (parse only)
        /// </summary>
        post_h2,

        /// <summary>
        /// block quote (render and parse)
        /// </summary>
        quote,

        /// <summary>
        /// list item in an ordered list	(render and parse)
        /// </summary>
        ol_li,

        /// <summary>
        /// list item in an unordered list (render and parse)
        /// </summary>
        ul_li,

        /// <summary>
        /// paragraph (or plain line during parse)
        /// </summary>
        p,

        /// <summary>
        /// an indented line (parse only)
        /// </summary>
        indent,

        /// <summary>
        /// horizontal rule (render and parse)
        /// </summary>
        hr,

        /// <summary>
        /// The user break
        /// </summary>
        user_break,

        /// <summary>
        /// html content (render and parse)
        /// </summary>
        html,

        /// <summary>
        /// unsafe html that should be encoded
        /// </summary>
        unsafe_html,

        /// <summary>
        /// an undecorated span of text (used for simple list items where content is not wrapped in paragraph tags)
        /// </summary>
        span,

        /// <summary>
        /// a code block (render only)
        /// </summary>
        codeblock,

        /// <summary>
        /// a list item (render only)
        /// </summary>
        li,

        /// <summary>
        /// ordered list (render only)
        /// </summary>
        ol,

        /// <summary>
        /// unordered list (render only)
        /// </summary>
        ul,

        /// <summary>
        /// Data=(HtmlTag), children = content
        /// </summary>
        HtmlTag,

        /// <summary>
        /// Just a list of child blocks
        /// </summary>
        Composite,

        /// <summary>
        /// A table row specifier eg:  |---: | ---|	`data` = TableSpec reference
        /// </summary>
        table_spec,

        /// <summary>
        /// definition (render and parse)	`data` = bool true if blank line before
        /// </summary>
        dd,

        /// <summary>
        /// dt render only
        /// </summary>
        dt,

        /// <summary>
        /// dt render only
        /// </summary>
        dl,

        /// <summary>
        /// footnote definition  eg: [^id]   `data` holds the footnote id
        /// </summary>
        footnote,

        /// <summary>
        /// paragraph with footnote return link append.  Return link string is in `data`.
        /// </summary>
        p_footnote, 
	}

	class Block
	{
		internal Block()
		{

		}

		internal Block(BlockType type)
		{
			blockType = type;
		}

		public string Content
		{
			get
			{
				switch (blockType)
				{
					case BlockType.codeblock:
						StringBuilder s = new StringBuilder();
						foreach (var line in children)
						{
							s.Append(line.Content);
							s.Append('\n');
						}
						return s.ToString();
				}


				if (buf==null)
					return null;
				else
					return contentStart == -1 ? buf : buf.Substring(contentStart, contentLen);
			}
		}

		public int LineStart
		{
			get
			{
				return lineStart == 0 ? contentStart : lineStart;
			}
		}

		internal void RenderChildren(Markdown m, StringBuilder b)
		{
			foreach (var block in children)
			{
				block.Render(m, b);
			}
		}

		internal void RenderChildrenPlain(Markdown m, StringBuilder b)
		{
			foreach (var block in children)
			{
				block.RenderPlain(m, b);
			}
		}

		internal string ResolveHeaderID(Markdown m)
		{
			// Already resolved?
			if (this.data!=null && this.data is string)
				return (string)this.data;

			// Approach 1 - PHP Markdown Extra style header id
			int end=contentEnd;
			string id = Utils.StripHtmlID(buf, contentStart, ref end);
			if (id != null)
			{
				contentEnd = end;
			}
			else
			{
				// Approach 2 - pandoc style header id
				id = m.MakeUniqueHeaderID(buf, contentStart, contentLen);
			}

			this.data = id;
			return id;
		}

		internal void Render(Markdown m, StringBuilder b)
		{
			switch (blockType)
			{
				case BlockType.Blank:
					return;

				case BlockType.p:
					m.SpanFormatter.FormatParagraph(b, buf, contentStart, contentLen);
					break;

				case BlockType.span:
					m.SpanFormatter.Format(b, buf, contentStart, contentLen);
					b.Append("\n");
					break;

				case BlockType.h1:
				case BlockType.h2:
				case BlockType.h3:
				case BlockType.h4:
				case BlockType.h5:
				case BlockType.h6:
					if (m.ExtraMode && !m.SafeMode)
					{
						b.Append("<" + blockType.ToString());
						string id = ResolveHeaderID(m);
						if (!String.IsNullOrEmpty(id))
						{
							b.Append(" id=\"");
							b.Append(id);
							b.Append("\">");
						}
						else
						{
							b.Append(">");
						}
					}
					else
					{
						b.Append("<" + blockType.ToString() + ">");
					}
					m.SpanFormatter.Format(b, buf, contentStart, contentLen);
					b.Append("</" + blockType.ToString() + ">\n");
					break;

				case BlockType.hr:
					b.Append("<hr />\n");
					return;

				case BlockType.user_break:
					return;

				case BlockType.ol_li:
				case BlockType.ul_li:
					b.Append("<li>");
					m.SpanFormatter.Format(b, buf, contentStart, contentLen);
					b.Append("</li>\n");
					break;

				case BlockType.dd:
					b.Append("<dd>");
					if (children != null)
					{
						b.Append("\n");
						RenderChildren(m, b);
					}
					else
						m.SpanFormatter.Format(b, buf, contentStart, contentLen);
					b.Append("</dd>\n");
					break;

				case BlockType.dt:
				{
					if (children == null)
					{
						foreach (var l in Content.Split('\n'))
						{
							b.Append("<dt>");
							m.SpanFormatter.Format(b, l.Trim());
							b.Append("</dt>\n");
						}
					}
					else
					{
						b.Append("<dt>\n");
						RenderChildren(m, b);
						b.Append("</dt>\n");
					}
					break;
				}

				case BlockType.dl:
					b.Append("<dl>\n");
					RenderChildren(m, b);
					b.Append("</dl>\n");
					return;

				case BlockType.html:
					b.Append(buf, contentStart, contentLen);
					return;

				case BlockType.unsafe_html:
					m.HtmlEncode(b, buf, contentStart, contentLen);
					return;

				case BlockType.codeblock:
					if (m.FormatCodeBlock != null)
					{
						var sb = new StringBuilder();
						foreach (var line in children)
						{
							m.HtmlEncodeAndConvertTabsToSpaces(sb, line.buf, line.contentStart, line.contentLen);
							sb.Append("\n");
						}
						b.Append(m.FormatCodeBlock(m, sb.ToString()));
					}
					else
					{
						b.Append("<pre><code>");
						foreach (var line in children)
						{
							m.HtmlEncodeAndConvertTabsToSpaces(b, line.buf, line.contentStart, line.contentLen);
							b.Append("\n");
						}
						b.Append("</code></pre>\n\n");
					}
					return;

				case BlockType.quote:
					b.Append("<blockquote>\n");
					RenderChildren(m, b);
					b.Append("</blockquote>\n");
					return;

				case BlockType.li:
					b.Append("<li>\n");
					RenderChildren(m, b);
					b.Append("</li>\n");
					return;

				case BlockType.ol:
					b.Append("<ol>\n");
					RenderChildren(m, b);
					b.Append("</ol>\n");
					return;

				case BlockType.ul:
					b.Append("<ul>\n");
					RenderChildren(m, b);
					b.Append("</ul>\n");
					return;

				case BlockType.HtmlTag:
					var tag = (HtmlTag)data;

					// Prepare special tags
					var name=tag.name.ToLowerInvariant();
					if (name == "a")
					{
						m.OnPrepareLink(tag);
					}
					else if (name == "img")
					{
						m.OnPrepareImage(tag, m.RenderingTitledImage);
					}

					tag.RenderOpening(b);
					b.Append("\n");
					RenderChildren(m, b);
					tag.RenderClosing(b);
					b.Append("\n");
					return;

				case BlockType.Composite:
				case BlockType.footnote:
					RenderChildren(m, b);
					return;

				case BlockType.table_spec:
					((TableSpec)data).Render(m, b);
					break;

				case BlockType.p_footnote:
					b.Append("<p>");
					if (contentLen > 0)
					{
						m.SpanFormatter.Format(b, buf, contentStart, contentLen);
						b.Append("&nbsp;");
					}
					b.Append((string)data);
					b.Append("</p>\n");
					break;

				default:
					b.Append("<" + blockType.ToString() + ">");
					m.SpanFormatter.Format(b, buf, contentStart, contentLen);
					b.Append("</" + blockType.ToString() + ">\n");
					break;
			}
		}

		internal void RenderPlain(Markdown m, StringBuilder b)
		{
			switch (blockType)
			{
				case BlockType.Blank:
					return;

				case BlockType.p:
				case BlockType.span:
					m.SpanFormatter.FormatPlain(b, buf, contentStart, contentLen);
					b.Append(" ");
					break;

				case BlockType.h1:
				case BlockType.h2:
				case BlockType.h3:
				case BlockType.h4:
				case BlockType.h5:
				case BlockType.h6:
					m.SpanFormatter.FormatPlain(b, buf, contentStart, contentLen);
					b.Append(" - ");
					break;


				case BlockType.ol_li:
				case BlockType.ul_li:
					b.Append("* ");
					m.SpanFormatter.FormatPlain(b, buf, contentStart, contentLen);
					b.Append(" ");
					break;

				case BlockType.dd:
					if (children != null)
					{
						b.Append("\n");
						RenderChildrenPlain(m, b);
					}
					else
						m.SpanFormatter.FormatPlain(b, buf, contentStart, contentLen);
					break;

				case BlockType.dt:
					{
						if (children == null)
						{
							foreach (var l in Content.Split('\n'))
							{
								var str = l.Trim();
								m.SpanFormatter.FormatPlain(b, str, 0, str.Length);
							}
						}
						else
						{
							RenderChildrenPlain(m, b);
						}
						break;
					}

				case BlockType.dl:
					RenderChildrenPlain(m, b);
					return;

				case BlockType.codeblock:
					foreach (var line in children)
					{
						b.Append(line.buf, line.contentStart, line.contentLen);
						b.Append(" ");
					}
					return;

				case BlockType.quote:
				case BlockType.li:
				case BlockType.ol:
				case BlockType.ul:
				case BlockType.HtmlTag:
					RenderChildrenPlain(m, b);
					return;
			}
		}

		public void RevertToPlain()
		{
			blockType = BlockType.p;
			contentStart = lineStart;
			contentLen = lineLen;
		}

		public int contentEnd
		{
			get
			{
				return contentStart + contentLen;
			}
			set
			{
				contentLen = value - contentStart;
			}
		}

		// Count the leading spaces on a block
		// Used by list item evaluation to determine indent levels
		// irrespective of indent line type.
		public int leadingSpaces
		{
			get
			{
				int count = 0;
				for (int i = lineStart; i < lineStart + lineLen; i++)
				{
					if (buf[i] == ' ')
					{
						count++;
					}
					else
					{
						break;
					}
				}
				return count;
			}
		}

		public override string ToString()
		{
			string c = Content;
			return blockType.ToString() + " - " + (c==null ? "<null>" : c);
		}

		public Block CopyFrom(Block other)
		{
			blockType = other.blockType;
			buf = other.buf;
			contentStart = other.contentStart;
			contentLen = other.contentLen;
			lineStart = other.lineStart;
			lineLen = other.lineLen;
			return this;
		}

		internal BlockType blockType;
		internal string buf;
		internal int contentStart;
		internal int contentLen;
		internal int lineStart;
		internal int lineLen;
		internal object data;			// content depends on block type
		internal List<Block> children;
	}
}
