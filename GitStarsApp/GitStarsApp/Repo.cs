namespace GitStarsApp
{
    public class Repo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public string AvatarUrl { get; set; }
        public int Stars { get; set; }
        public string Language { get; set; }
        public string Url { get; internal set; }
    }
}