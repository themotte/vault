using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace QCUtilities.Entities
{
    [Serializable]
    public class Post
    {
        [XmlElement("author")]
        public string Author { get; set; }

        [XmlElement("date")]
        public DateTime Date { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlArray("redirect")]
        [XmlArrayItem("li")]
        public List<string> Redirect { get; set; } = new List<string>();

        [XmlArray("category")]
        [XmlArrayItem("li")]
        public List<string> Category { get; set; } = new List<string>();

        public class VerbatimBlob : IXmlSerializable
        {
            public string contents;

            public VerbatimBlob() { }
            public VerbatimBlob(string contents) { this.contents = contents; }

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {
                // By default, System.Xml.XmlTextReader ignores pure-whitespace text nodes between tags.
                // We need these because some people like doing <a href="evidence">stuff</a> <a href="evidence">like</a> <a href="evidence">this</a>.
                // Without the whitespace, these get smooshed up against each other and it looks like absolute butt.
                // There's no setting that I can find in XmlSerializer, nor is there a setting in XmlReader.
                // Thankfully, this XmlReader is *always* an XmlTextReader, which does have a relevant setting, and we can just cast it to XmlTextReader and change the setting at runtime and make it work.
                // This will obviously break completely if this stops being an XmlTextReader but that's a problem for future me.
                var treader = reader as System.Xml.XmlTextReader;
                treader.WhitespaceHandling = System.Xml.WhitespaceHandling.All;
                contents = treader.ReadInnerXml();
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {
                writer.WriteString(contents);
            }
        }

        [XmlElement("context")]
        public VerbatimBlob Context { get; set; }

        [XmlElement("body")]
        public VerbatimBlob Body { get; set; }

        private string bodyCompiled = null;
        public string BodyCompiled
        {
            get
            {
                if (bodyCompiled == null)
                {
                    bodyCompiled = Body.contents;

                    if (Context != null)
                    {
                        // yes I know this is awful
                        bodyCompiled = $"<blockquote>{Context.contents}</blockquote>" + bodyCompiled;
                    }
                }

                return bodyCompiled;
            }
        }

        private string bodyExcerptText = null;
        public string BodyExcerptText
        {
            get
            {
                if (bodyExcerptText == null)
                {
                    bodyExcerptText = QCVault.Utilities.TextExcerptGenerator.Generate(Body.contents, 500);
                }

                return bodyExcerptHTML;
            }
        }

        private string bodyExcerptHTML = null;
        public string BodyExcerptHTML
        {
            get
            {
                if (bodyExcerptHTML == null)
                {
                    bodyExcerptHTML = QCVault.Utilities.HTMLExcerptGenerator.Generate(Body.contents, 1500);

                    bodyExcerptHTML += $"<p>Read more at <a href=\"https://www.vault.themotte.org{FullURL}\">The Vault</a></p>";
                }

                return bodyExcerptHTML;
            }
        }

        private static readonly System.Text.RegularExpressions.Regex SpecialCharacterStripper = new System.Text.RegularExpressions.Regex(@"[^\w ]*");
        public string URLSlug
        {
            get
            {
                return SpecialCharacterStripper.Replace(Title, "").Replace(" ", "_").ToLower();
            }
        }

        public IEnumerable<string> RedirectURLSlug
        {
            get
            {
                return Redirect.Select(r => SpecialCharacterStripper.Replace(r, "").Replace(" ", "_").ToLower());
            }
        }

        public string FullURL
        {
            get => "/post/" + URLSlug;
        }

        public void GenerateCachedData()
        {
            int bytes = 0;
            bytes += BodyCompiled.Length;
            bytes += BodyExcerptHTML.Length;

            // We don't actually care about the bytes, I just needed an excuse to call the properties.
        }

        public override bool Equals(object obj)
        {
            var item = obj as Post;
            var result =
                Author.Equals(item.Author) &&
                Date.Equals(item.Date) &&
                Link.Equals(item.Link) &&
                Title.Equals(item.Title) &&
                Body.contents.Equals(item.Body.contents);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
