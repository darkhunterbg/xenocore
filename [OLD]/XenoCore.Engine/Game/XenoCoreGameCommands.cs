using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;

namespace XenoCore.Engine.Game
{
    class XenoCoreGameCommands : ConsoleAutoRegister
    {
        private XenoCoreGame game;
        public XenoCoreGameCommands(XenoCoreGame game)
        {
            this.game = game;
            Register();
        }
        [ConsoleCommand(Name ="exit")]
        public void Exit()
        {
            game.Exit();
        }
    
    }
}
