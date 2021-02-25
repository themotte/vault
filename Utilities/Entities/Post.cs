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
        [XmlElement(nameof(Author))]
        public string Author { get; set; }

        [XmlElement(nameof(Date))]
        public DateTime Date { get; set; }

        [XmlElement(nameof(Link))]
        public string Link { get; set; }

        [XmlElement(nameof(Title))]
        public string Title { get; set; }

        [XmlElement(nameof(Body))]
        public string Body { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as Post;
            var result=
            Author.Equals(item.Author)
                &&
                Date.Equals(item.Date)
                &&
                Link.Equals(item.Link)
                &&
                Title.Equals(item.Title)
                &&
                Body.Equals(item.Body)
                ;
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    [XmlRoot("Posts")]

    public class PostCollection
    {
        [XmlElement(nameof(Post))]
        public Post[] Posts { get; set; }

        public List<Post> ToList()
        {
            return Posts.ToList();
        }
    }
}
