using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded_Mod_Template.Misc;

namespace Reloaded_Mod_Template.Controller.Heroes
{
    /// <summary>
    /// Returns individual Heroes controllers for player(s) 1/2/3.
    /// </summary>
    public static unsafe class HeroesControllerFactory
    {
        /// <summary>
        /// Contains the address of the first controller inputs.
        /// </summary>
        public static HeroesController* PlayerOnePtr = (HeroesController*)0x00A2F948;

        /// <summary>
        /// Returns a pointer to an individual Sonic Heroes controller structure.
        /// </summary>
        public static HeroesController* GetController(Player controllerPort)
        {
            return PlayerOnePtr + (int)controllerPort;
        }
    }
}
