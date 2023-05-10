using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Workspace
{
    public interface IPrefix
    {
        IReadOnlyList<PrefixCouple> PrefixCouples { get; }

        bool IsNeedToDefine(string id);
    }
}
