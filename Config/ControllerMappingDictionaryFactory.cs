using System;
using System.Collections.Generic;
using Reloaded.Input.Common.Controller_Inputs_Substructures;
using Reloaded_Mod_Template.Controller.Heroes;
using static Reloaded_Mod_Template.Config.ControllerButtonFunctionBuilder;

namespace Reloaded_Mod_Template.Config
{
    /// <summary>
    /// Builds dictionaries that map individual buttons of <see cref="ControllerMapping"/> to functions which retrieve
    /// the relevant buttons from the Reloaded controller structure.
    /// </summary>
    public static class ControllerMappingDictionaryFactory
    {

        /// <summary>
        /// Builds mapping of <see cref="ControllerMapping"/> buttons to functions which retrieve
        /// the relevant buttons from a Reloaded <see cref="JoystickButtons"/>.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<ButtonFlags, Func<JoystickButtons, bool>> GetDictionary(ControllerMapping controllerMapping)
        {
            var buttonDictionary = new Dictionary<ButtonFlags, Func<JoystickButtons, bool>>(32);

            buttonDictionary[ButtonFlags.Jump] = FromControllerButtons(controllerMapping.Jump);
            buttonDictionary[ButtonFlags.Action] = FromControllerButtons(controllerMapping.Action);
            buttonDictionary[ButtonFlags.FormationR] = FromControllerButtons(controllerMapping.FormationR);
            buttonDictionary[ButtonFlags.FormationL] = FromControllerButtons(controllerMapping.FormationL);

            buttonDictionary[ButtonFlags.DpadUp] = FromControllerButtons(controllerMapping.DpadUp);
            buttonDictionary[ButtonFlags.DpadDown] = FromControllerButtons(controllerMapping.DpadDown);
            buttonDictionary[ButtonFlags.DpadLeft] = FromControllerButtons(controllerMapping.DpadLeft);
            buttonDictionary[ButtonFlags.DpadRight] = FromControllerButtons(controllerMapping.DpadRight);

            buttonDictionary[ButtonFlags.CameraL] = FromControllerButtons(controllerMapping.CameraL);
            buttonDictionary[ButtonFlags.CameraR] = FromControllerButtons(controllerMapping.CameraR);
            buttonDictionary[ButtonFlags.TeamBlast] = FromControllerButtons(controllerMapping.TeamBlast);
            buttonDictionary[ButtonFlags.Start] = FromControllerButtons(controllerMapping.Start);
            return buttonDictionary;
        }
    }
}
