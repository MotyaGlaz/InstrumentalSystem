

namespace Library.General.Project
{
    public class ProjectInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Date { get; set; }

        public string Owner { get; set; }
        public string Picture { get; set; }
        public bool IsCompleted { get; set; }

        public ProjectInfo(string name, string path, string date)
        {
            Name = name;
            Path = path;
            Date = date;
            IsCompleted = false;
        }

        public ProjectInfo(string name, string owner, string date, string picture)
        {
            Name = name;
            Owner = owner;
            Date = date;
            Picture = picture;
        }
    }
}
