using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Input;
using Reloaded.Input.Common;
using Reloaded.Input.Common.ControllerInputs;
using Reloaded.Input.Common.Controller_Inputs_Substructures;
using Reloaded_Mod_Template.Config;
using Reloaded_Mod_Template.Controller.Heroes;
using Reloaded_Mod_Template.Misc;

namespace Reloaded_Mod_Template.Controller
{
    /// <summary>
    /// <see cref="ReloadedController"/> is an abstraction that writes Reloaded controls to Heroes' controller input struct.
    /// </summary>
    public unsafe class ReloadedController
    {
        /// <summary>
        /// The individual function used to retrieve the inputs of the individual controller.
        /// </summary>
        private Func<ControllerInputs> _getInputs;

        /// <summary>
        /// Performs all of the Reloaded Controller to Heroes Controller translations and maintains active last pressed
        /// button backups.
        /// </summary>
        private HeroesControllerHelper _heroesControllerHelper;

        /*
            ------------
            Constructors
            ------------
        */

        /// <summary>
        /// Creates a new <see cref="ReloadedController"/> that writes Reloaded controls to Heroes' controller input struct.
        /// </summary>
        /// <param name="player">The individual player for which the Reloaded Controller should be created for.</param>
        /// <param name="controllerManager">Reloaded's standard Controller Manager.</param>
        public ReloadedController(Player player, ControllerManager controllerManager)
        {
            var mappingFactory = new ControllerMappingFactory(Program.ModDirectory);
            var mapping        = mappingFactory.GetInstance(player);
            var controllerPtr  = HeroesControllerFactory.GetController(player);

            _getInputs = () => controllerManager.GetInput(mapping.ControllerPort);
            _heroesControllerHelper = new HeroesControllerHelper(mapping, controllerPtr);
        }

        /*
            ---------
            Functions
            ---------
        */

        /// <summary>
        /// Retrieves the current inputs for the Reloaded controller and writes them into game memory.
        /// Intended to be called inside the controller button get hook.
        /// </summary>
        public void SendInputs()
        {
            var inputs = _getInputs();
            (*_heroesControllerHelper.HeroesController).Reset();
            _heroesControllerHelper.SetControls(ref inputs);
        }

        /// <summary>
        /// Sets the individual left and right trigger pressures to a Heroes peri controller "SkyPad" instance.
        /// </summary>
        /// <param name="skypad"></param>
        public void SetTriggerRotations(SkyPad* skypad)
        {
            if (_heroesControllerHelper.ControllerMapping.TriggerOptions.EnableTriggerRotation)
            {
                // Do not mix triggers and buttons.
                if ((*skypad).TriggerPressureL != 0 || (*skypad).TriggerPressureR != 0)
                    return;
                
                // Get trigger pressures scaled to Heroes' 255 max.
                var triggerPressures = GetTriggerPressures(255F);
                short leftTriggerPressure = (short)Math.Round(triggerPressures.leftPressure);
                short rightTriggerPressure = (short)Math.Round(triggerPressures.rightPressure);

                // Override bumper buttons if the triggers are pressed
                if (leftTriggerPressure > 0 || rightTriggerPressure > 0)
                {
                    // Get pressures to set.
                    if (_heroesControllerHelper.ControllerMapping.TriggerOptions.SwapTriggers)
                        Swap(ref leftTriggerPressure, ref rightTriggerPressure);

                    // Apply trigger pressures.
                    (*skypad).TriggerPressureL = leftTriggerPressure;
                    (*skypad).TriggerPressureR = rightTriggerPressure;

                    // Simulate button press if necessary.
                    if (leftTriggerPressure > 0)
                        (*skypad).ButtonFlags |= ButtonFlags.CameraL;
                    if (rightTriggerPressure > 0)
                        (*skypad).ButtonFlags |= ButtonFlags.CameraR;
                }
            }
        }

        /// <summary>
        /// Retrieves the current applied left and right trigger pressure scaled to a specified maximum value.
        /// </summary>
        /// <param name="maxValue">Scales the maximum value of the trigger pressures to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (float leftPressure, float rightPressure) GetTriggerPressures(float maxValue)
        {
            var inputs = _getInputs();
            float leftPressure = inputs.GetLeftTriggerPressure();
            float rightPressure = inputs.GetRightTriggerPressure();

            leftPressure = (leftPressure / ControllerCommon.AxisMaxValueF) * maxValue;
            rightPressure = (rightPressure / ControllerCommon.AxisMaxValueF) * maxValue;

            return (leftPressure, rightPressure);
        }

        /// <summary>
        /// Swaps two elements.
        /// </summary>
        private static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
