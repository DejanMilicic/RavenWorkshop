using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.IndexingRelationships
{
    public class Users_ByContent2 : AbstractMultiMapIndexCreationTask<Users_ByContent2.Entry>
    {
        public class Entry
        {
            public string Name { get; set; }

            public string Title { get; set; }

            public int Time { get; set; }
        }

        public Users_ByContent2()
        {
            // blog post activities
            AddMap<Models.UserActivity>(
                activities => from activity in activities
                              where LoadDocument<Models.BlogPost>(activity.Content) != null
                              let user = LoadDocument<Models.User>(activity.User)
                              let blogPost = LoadDocument<Models.BlogPost>(activity.Content)
                              select new Entry
                              {
                                  Name = user.Name,
                                  Title = blogPost.Title,
                                  Time = blogPost.ReadingTime
                              }
            );

            // video activities
            AddMap<Models.UserActivity>(
                activities => from activity in activities
                              where LoadDocument<Models.Video>(activity.Content) != null
                              let user = LoadDocument<Models.User>(activity.User)
                              let video = LoadDocument<Models.Video>(activity.Content)
                              select new Entry
                              {
                                  Name = user.Name,
                                  Title = video.Title,
                                  Time = video.WatchingTime
                              }
            );
        }
    }
}
