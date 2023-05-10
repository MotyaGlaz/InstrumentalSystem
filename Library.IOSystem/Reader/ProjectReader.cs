using Library.General.Project;

namespace Library.IOSystem.Reader
{
    public static class ProjectReader
    {
        public static Project ReadProject(string path)
        {
            var parsePath = path.Split("\\");
            var project = new Project(parsePath[parsePath.Length-1].Substring(0, parsePath[parsePath.Length - 1].Length - 7));
            var master = File.ReadAllText(path);
            var parseMaster = master.Split("\n");
            foreach (var modulePath in parseMaster)
            {
                var moduleInfo = modulePath.Split("|");
                if (moduleInfo.Length == 3)
                {
                    var module = new LogicModule(moduleInfo[1]);
                    var moduleInfoPath = moduleInfo[2].Replace("\r","");
                    module.SetContent(
                            File.ReadAllText(moduleInfoPath)
                        );
                    project.Add(moduleInfo[0], module);
                }
            }
            return project;
        }

        public static List<ProjectInfo> ReadProjectsInfo()
        {
            List<ProjectInfo> result = new List<ProjectInfo>();
            var projects = File.ReadAllText(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "user.data")).Split("\n");
            foreach(var project in projects)
            {
                if (project.Length == 0)
                    break;
                var parseProjectPath = project.Split("\\");
                result.Add(new ProjectInfo(
                        parseProjectPath.Last(),
                        project,
                        $"Последние изменения:\n {File.GetLastWriteTime(project).ToString()}"
                    ));
            }
            return result;
        }

    }
}
