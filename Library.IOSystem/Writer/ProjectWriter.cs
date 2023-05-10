using Library.General.Project;
using Library.IOSystem.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.IOSystem.Writer
{
    public class ProjectWriter : IPathable
    {
        public string Path { get; private set; }

        public ProjectWriter(string path)
        {
            Path = path;
        }

        public void WriteProject(Project project)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
            foreach (var @namespace in project.Namespaces)
            {
                foreach (var level in @namespace.Levels)
                {
                    if (!Directory.Exists($"{Path}/{@namespace.Name}"))
                    {
                        Directory.CreateDirectory($"{Path}/{@namespace.Name}");
                    }
                    File.WriteAllText($"{Path}\\{@namespace.Name}\\{level.Name}.logic", level.Content);
                    stringBuilder.AppendLine($"{@namespace.Name}|{level.Name}|{Path}\\{@namespace.Name}\\{level.Name}.logic");
                }
            }
            File.WriteAllText($"{Path}/{project.Name}.master", stringBuilder.ToString());
            WriteUserData();
        }

        private void WriteUserData()
        {
            var content = File.ReadAllText($"{Directory.GetCurrentDirectory()}/user.data");
            foreach (var str in content.Split("\n"))
            {
                if (str.Equals(Path))
                    return;
            }

            File.AppendAllText($"{Directory.GetCurrentDirectory()}/user.data", $"{Path}\n");
        }
    }
}
