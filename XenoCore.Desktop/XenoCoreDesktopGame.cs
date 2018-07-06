using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Game;
using XenoCore.Engine.Input;

namespace XenoCore.Desktop
{
    public class XenoCoreDesktopGame : XenoCoreGame
    {
        public XenoCoreDesktopGame()
            : base(DesktopPlatform.LoggerProvider, DesktopPlatform.ThreadProvider, new EventTextInputProvider())
        {
            var textInputProvider = this.TextInputProvider as EventTextInputProvider;
            this.Window.TextInput += (s, a) => { textInputProvider.CharacterEntered(a.Character); };
        }
    }
}
