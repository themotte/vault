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

        [XmlElement("body")]
        public string Body { get; set; }

        private static System.Text.RegularExpressions.Regex SpecialCharacterStripper = new System.Text.RegularExpressions.Regex(@"[^\w ]*");
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
                Body.Equals(item.Body);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    [XmlRoot("posts")]
    public class PostCollection
    {
        

        public PostCollection()
        {
            Posts = new List<Post>();
        }

        [XmlElement("post")]
        public List<Post> Posts { get; set; }
    }
}
