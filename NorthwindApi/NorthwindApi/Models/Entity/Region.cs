namespace NorthwindApi.Models.Entity
{
    public class Region
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Territory> Territories { get; set; }
    }

    public class Territory
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
    }
}
