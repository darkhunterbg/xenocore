using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Console
{
    public abstract class ConsoleAutoRegister : IDisposable
    {
        protected void Register()
        {
            ConsoleService.LoadFromObject(this);
        }

        public void Dispose()
        {
            ConsoleService.UnloadFromObject(this);
        }
    }
}
