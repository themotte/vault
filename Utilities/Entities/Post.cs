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
                // this is simultaneously easier and more horrible than I was expecting
                contents = reader.ReadInnerXml();
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

        private static readonly System.Text.RegularExpressions.Regex SpecialCharacterStripper = new System.Text.RegularExpressions.Regex(@"[^\w ]*");
        public string URLSlug
        {
            get
            {
                return SpecialCharacterStripper.Replace(Title, "").Replace(" ", "_").ToLower();
            }
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
