using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.IndexingRelationships
{
    public class Models
    {
        public class User
        {
            public string Id { get; set; }

            public string Name { get; set; }
        }

        public class UserActivity
        {
            public string Id { get; set; }

            public string User { get; set; }

            public string Content { get; set; }
        }

        public class BlogPost
        {
            public string Id { get; set; }

            public string Title { get; set; }

            public int ReadingTime { get; set; }
        }

        public class Video
        {
            public string Id { get; set; }

            public string Title { get; set; }

            public int WatchingTime { get; set; }
        }
    }
}
