using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SourceDocs.Core.Services
{
    public class SourceFileTransformer : IFileTransformer
    {
        public string Transform(FileInfo fileInfo)
        {
            Language language;
            if (Languages.TryGetValue(fileInfo.Extension, out language))
            {
                var sections = Parse(language, File.ReadAllLines(fileInfo.FullName));

                Hightlight(sections);
                return GenerateHtml(language, sections);
            }

            return null;
        }

        private static IList<Section> Parse(Language language, string[] lines)
        {
            var sections = new List<Section>();
            var docsText = new StringBuilder();
            var codeText = new StringBuilder();

            Action<string, string> save = (docs, code) => sections.Add(new Section { DocsHtml = docs, CodeHtml = code });
            Func<string, string> mapToMarkdown = docs =>
            {
                if (language.MarkdownMaps != null)
                {
                    foreach (var map in language.MarkdownMaps)
                    {
                        docs = Regex.Replace(docs, map.Key, map.Value, RegexOptions.Multiline);
                    }
                }
                return docs;
            };

            var hasCode = false;

            foreach (var line in lines)
            {
                if (language.CommentMatcher.IsMatch(line) && !language.CommentFilter.IsMatch(line))
                {
                    if (hasCode)
                    {
                        save(mapToMarkdown(docsText.ToString()), codeText.ToString());
                        hasCode = false;
                        docsText = new StringBuilder();
                        codeText = new StringBuilder();
                    }
                    docsText.AppendLine(language.CommentMatcher.Replace(line, ""));
                }
                else
                {
                    hasCode = true;
                    codeText.AppendLine(line);
                }
            }

            save(mapToMarkdown(docsText.ToString()), codeText.ToString());

            return sections;
        }

        private static void Hightlight(IList<Section> sections)
        {
            var md = new MarkdownDeep.Markdown
            {
                ExtraMode = true,
                SafeMode = false
            };

            foreach (var section in sections)
            {
                section.DocsHtml = md.Transform(section.DocsHtml);
                section.CodeHtml = System.Web.HttpUtility.HtmlEncode(section.CodeHtml);
            }
        }

        // todo: use Nustache
        private static string GenerateHtml(Language language, IList<Section> sections)
        {
            var result = new StringBuilder();

            result.AppendLine(@"<div class=""container-fluid"">");

            for (var index = 0; index < sections.Count; index++)
            {
                var section = sections[index];

                result.AppendFormat(@"
<div class=""row"" id=""section_{0}"">
<div class=""col-md-4"">{1}</div>
<div class=""col-md-8""><pre><code class=""hljs {3}"">{2}</code></pre></div>
</div>", index, section.DocsHtml, section.CodeHtml, language.Name);
            }

            result.AppendLine(@"</div>");

            return result.ToString();
        }

        private static readonly Dictionary<string, Language> Languages = new Dictionary<string, Language>
        {
			{ ".js", new Language {
				Name = "javascript",
				Symbol = "//"
			}},
			{ ".cs", new Language {
				Name = "csharp",
				Symbol = "///?",
				MarkdownMaps = new Dictionary<string, string> {
					{ @"<c>([^<]*)</c>", "`$1`" },
					{ @"<param[^\>]*>([^<]*)</param>", "" },
					{ @"<returns>([^<]*)</returns>", "" },
					{ @"<see\s*cref=""([^""]*)""\s*/>", "see `$1`"},
					{ @"(</?example>|</?summary>|</?remarks>)", "" },
				}
			}},
			{ ".vb", new Language {
				Name = "vbnet",
				Symbol = "'+",
				MarkdownMaps = new Dictionary<string, string> {
					{ @"<c>([^<]*)</c>", "`$1`" },
					{ @"<param[^\>]*>([^<]*)</param>", "" },
					{ @"<returns>([^<]*)</returns>", "" },
					{ @"<see\s*cref=""([^""]*)""\s*/>", "see `$1`"},
					{ @"(</?example>|</?summary>|</?remarks>)", "" },
				}
			}}
		};
    }

    public class Section
    {
        public string CodeHtml;
        public string DocsHtml;
    }

    public class Language
    {
        public string Name;
        public string Symbol;
        public Regex CommentMatcher { get { return new Regex(@"^\s*" + Symbol + @"\s?"); } }
        public Regex CommentFilter { get { return new Regex(@"(^#![/]|^\s*#\{)"); } }
        public IDictionary<string, string> MarkdownMaps;
    }
}