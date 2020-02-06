using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 文件夹病毒专杀工具
{
    interface HookManager
    {
        bool Statue { get; }
        bool DisableHook();
        bool EnableHook();
    }
}
