using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentalSystem.Client.Logic.Task
{
    class ModuleCreationTask
    {
        public ModuleCreationTaskType Task { get { return _type; } }
        public bool InProgress { get; set; }
        private ModuleCreationTaskType _type;

        public ModuleCreationTask(ModuleCreationTaskType type)
        {
            _type = type;
            InProgress = true;
        }


    }
}
