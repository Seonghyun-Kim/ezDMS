using IS_PODS.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IS_PODS.Define.LogDefine;

namespace IS_PODS
{
    public interface IAction
    {
        int? target_idx { get; }

        eModule action_module { get; }

        int? action_module_idx { get; }
    }
}
