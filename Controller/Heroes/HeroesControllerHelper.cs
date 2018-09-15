using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Input.Common;
using Reloaded.Input.Common.ControllerInputs;
using Reloaded.Input.Common.Controller_Inputs_Substructures;
using Reloaded_Mod_Template.Config;

namespace Reloaded_Mod_Template.Controller.Heroes
{
    /// <summary>
    /// Provides a simple abstraction in helping convert some Reloaded Controller functionality into Heroes' inputs.
    /// </summary>
    public unsafe class HeroesControllerHelper
    {
        /// <summary>
        /// The active Heroes Controller pointer to be manipulated by this class.
        /// </summary>
        public HeroesController* HeroesController { get; set; }

        /// <summary>
        /// Stores the settings for the current controller.
        /// </summary>
        public ControllerMapping ControllerMapping { get; private set; }

        /// <summary>
        /// Used to fill the <see cref="HeroesController"/>'s individual fields for new button press and releases.
        /// </summary>
        private ButtonFlags _lastFrameFlags = 0;

        /// <summary>
        /// Assigns a Sonic Heroes button to functions which obtain whether a button is pressed.
        /// </summary>
        private Dictionary<ButtonFlags, Func<JoystickButtons, bool>> _controllerMappingDictionary;

        /* Constructor/s */
        public HeroesControllerHelper(ControllerMapping controllerMapping, HeroesController* heroesController)
        {
            ControllerMapping = controllerMapping;
            _controllerMappingDictionary = ControllerMappingDictionaryFactory.GetDictionary(controllerMapping);
            HeroesController = heroesController;
        }

        /// <summary>
        /// Passes on the controls to the individual game.
        /// </summary>
        /// <param name="controllerInputs"></param>
        public void SetControls(ref ControllerInputs controllerInputs)
        {
            SetButtonFlags(ref controllerInputs);
            SetSticks(ref controllerInputs);
        }


        /* Methods (Private) */

        /// <summary>
        /// Iterates over the set of mapping key value pairs and gets the currently enabled Heroes button flags.
        /// Updates the newly set and released buttons entries.
        /// </summary>
        private void SetButtonFlags(ref ControllerInputs controllerInputs)
        {
            ButtonFlags currentFlags = 0;
            JoystickButtons controllerButtons = controllerInputs.ControllerButtons;

            // Get the button flags.
            foreach (var keyValue in _controllerMappingDictionary)
                if (keyValue.Value(controllerButtons))
                    currentFlags |= keyValue.Key;

            // Update struct and last set buttons.
            (*HeroesController).SetPressedButtons(_lastFrameFlags, currentFlags);
            (*HeroesController).SetReleasedButtons(_lastFrameFlags, currentFlags);
            (*HeroesController).SetMinusOneButtonFlags(currentFlags);
            (*HeroesController).ButtonFlags = currentFlags;
            _lastFrameFlags = currentFlags;
        }

        /// <summary>
        /// Sets the analog stick values for the individual <see cref="HeroesController"/>.
        /// </summary>
        private void SetSticks(ref ControllerInputs controllerInputs)
        {
            var leftStick = controllerInputs.LeftStick;
            var rightStick = controllerInputs.RightStick;
            float maxValue = ControllerCommon.AxisMaxValueF;

            // Get sticks.
            float leftStickX = (*HeroesController).GetScaledAnalogValue(leftStick.GetX(), maxValue);
            float leftStickY = (*HeroesController).GetScaledAnalogValue(leftStick.GetY(), maxValue);
            float rightStickX = (*HeroesController).GetScaledAnalogValue(rightStick.GetX(), maxValue);
            float rightStickY = (*HeroesController).GetScaledAnalogValue(rightStick.GetY(), maxValue);

            // Update struct.
            (*HeroesController).LeftStickX = leftStickX;
            (*HeroesController).LeftStickY = leftStickY;
            (*HeroesController).RightStickX = rightStickX;
            (*HeroesController).RightStickY = rightStickY;
        }
    }
}
