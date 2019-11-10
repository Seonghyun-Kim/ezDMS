using ezDMS.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ezDMS.Define.LogDefine;

namespace ezDMS
{
    public interface IAction
    {
        int? target_idx { get; }

        eModule action_module { get; }

        int? action_module_idx { get; }
    }
}
