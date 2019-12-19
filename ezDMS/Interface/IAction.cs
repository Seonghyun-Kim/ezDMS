using SmartDSP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartDSP.Define.LogDefine;

namespace SmartDSP
{
    public interface IAction
    {
        int? target_idx { get; }

        eModule action_module { get; }

        int? action_module_idx { get; }
    }
}
