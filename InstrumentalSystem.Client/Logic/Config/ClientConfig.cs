using Library.General.Project;
using Library.General.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentalSystem.Client.Logic.Config
{
    public static class ClientConfig
    {
        public static Project Project { get; set; }
        
        public static User User { get; set; }

        public static void SetProject(Project project) => Project = project;

        public static void SetUser(User user) => User = user;
    }
}
