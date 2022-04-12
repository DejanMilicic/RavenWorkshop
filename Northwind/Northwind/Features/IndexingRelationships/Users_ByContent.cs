using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.IndexingRelationships
{
    public class Users_ByContent : AbstractMultiMapIndexCreationTask<Users_ByContent.Entry>
    {
        public class Entry
        {
            public string Name { get; set; }

            public string Title { get; set; }

            public int Time { get; set; }
        }

        public Users_ByContent()
        {
            AddMap<Models.UserActivity>(
                activities => from activity in activities
                    let user = LoadDocument<Models.User>(activity.User)
                    let blogPost = LoadDocument<Models.BlogPost>(activity.Content)
                    let video = LoadDocument<Models.Video>(activity.Content)
                    select new Entry
                    {
                        Name = user.Name,
                        Title = blogPost.Title ?? video.Title,
                        Time = (blogPost != null) ? blogPost.ReadingTime : video.WatchingTime
                    }
            );
        }
    }
}
