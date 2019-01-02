﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownMaker.MarkdownDeep
{
	internal enum ColumnAlignment
	{
		NA,
		Left,
		Right,
		Center,
	}
	internal class TableSpec
	{
		public TableSpec()
		{
		}

		public bool LeadingBar;
		public bool TrailingBar;

		public List<ColumnAlignment> Columns=new List<ColumnAlignment>();

		public List<string> Headers;
		public List<List<string>> Rows=new List<List<string>>();

		public List<string> ParseRow(StringScanner p)
		{
			p.SkipLinespace();

			if (p.eol)
				return null;		// Blank line ends the table

			bool bAnyBars=LeadingBar;
			if (LeadingBar && !p.SkipChar('|'))
			{
				return null;
			}

			// Create the row
			List<string> row = new List<string>();

			// Parse all columns except the last

			while (!p.eol)
			{
				// Find the next vertical bar
				p.Mark();
				while (!p.eol && p.current != '|')
					p.SkipEscapableChar(true);

				row.Add(p.Extract().Trim());

				bAnyBars|=p.SkipChar('|');
			}

			// Require at least one bar to continue the table
			if (!bAnyBars)
				return null;

			// Add missing columns
			while (row.Count < Columns.Count)
			{
				row.Add("&nbsp;");
			}

			p.SkipEol();
			return row;
		}

		internal void RenderRow(Markdown m, StringBuilder b, List<string> row, string type)
		{
			for (int i=0; i<row.Count; i++)
			{
				b.Append("\t<");
				b.Append(type);

				if (i < Columns.Count)
				{
					switch (Columns[i])
					{
						case ColumnAlignment.Left:
							b.Append(" align=\"left\"");
							break;
						case ColumnAlignment.Right:
							b.Append(" align=\"right\"");
							break;
						case ColumnAlignment.Center:
							b.Append(" align=\"center\"");
							break;
					}
				}

				b.Append(">");
				m.SpanFormatter.Format(b, row[i]);
				b.Append("</");
				b.Append(type);
				b.Append(">\n");
			}
		}
	
		public void Render(Markdown m, StringBuilder b)
		{
			b.Append("<table>\n");
			if (Headers != null)
			{
				b.Append("<thead>\n<tr>\n");
				RenderRow(m, b, Headers, "th");
				b.Append("</tr>\n</thead>\n");
			}

			b.Append("<tbody>\n");
			foreach (var row in Rows)
			{
				b.Append("<tr>\n");
				RenderRow(m, b, row, "td");
				b.Append("</tr>\n");
			}
			b.Append("</tbody>\n");

			b.Append("</table>\n");
		}

		public static TableSpec Parse(StringScanner p)
		{
			// Leading line space allowed
			p.SkipLinespace();

			// Quick check for typical case
			if (p.current != '|' && p.current != ':' && p.current != '-')
				return null;

			// Don't create the spec until it at least looks like one
			TableSpec spec = null;

			// Leading bar, looks like a table spec
			if (p.SkipChar('|'))
			{
				spec=new TableSpec();
				spec.LeadingBar=true;
			}


			// Process all columns
			while (true)
			{
				// Parse column spec
				p.SkipLinespace();

				// Must have something in the spec
				if (p.current == '|')
					return null;

				bool AlignLeft = p.SkipChar(':');
				while (p.current == '-')
					p.SkipForward(1);
				bool AlignRight = p.SkipChar(':');
				p.SkipLinespace();

				// Work out column alignment
				ColumnAlignment col = ColumnAlignment.NA;
				if (AlignLeft && AlignRight)
					col = ColumnAlignment.Center;
				else if (AlignLeft)
					col = ColumnAlignment.Left;
				else if (AlignRight)
					col = ColumnAlignment.Right;

				if (p.eol)
				{
					// Not a spec?
					if (spec == null)
						return null;

					// Add the final spec?
					spec.Columns.Add(col);
					return spec;
				}

				// We expect a vertical bar
				if (!p.SkipChar('|'))
					return null;

				// Create the table spec
				if (spec==null)
					spec=new TableSpec();

				// Add the column
				spec.Columns.Add(col);

				// Check for trailing vertical bar
				p.SkipLinespace();
				if (p.eol)
				{
					spec.TrailingBar = true;
					return spec;
				}

				// Next column
			}
		}
	}
}
