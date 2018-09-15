using Reloaded.Native.Functions;

namespace Reloaded_Mod_Template.Controller.Heroes
{
    /// <summary>
    /// Describes a Sonic Heroes controller structure.
    /// </summary>
    public struct HeroesController
    {
        /// <summary>
        /// Contains the currently pressed buttons at any point.
        /// </summary>
        public ButtonFlags ButtonFlags;

        /// <summary>
        /// This value is (-1 - _buttonFlags).
        /// This value is also 0 when the window is not in focus.
        /// </summary>
        public int MinusOneMinusButtonFlags;

        /// <summary>
        /// If a button is pressed and it was not pressed the last frame,
        /// set the <see cref="ButtonFlags"/> of said button.
        /// </summary>
        public ButtonFlags OneFramePressButtonFlag;

        /// <summary>
        /// If a button is released and it was pressed the last frame,
        /// set the <see cref="ButtonFlags"/> of said button.
        /// </summary>
        public ButtonFlags OneFrameReleaseButtonFlag;

        /* Range: -1.0 to 1.0 */
        public float LeftStickX;
        public float LeftStickY;
        public float RightStickX;
        public float RightStickY;

        /// <summary>
        /// This value never seems to change. It does not have any effect on the game.
        /// </summary>
        public int ProbablyIsEnabled;


        /*
            ------------------
            Functions (Public)
            ------------------
        */

        /// <summary>
        /// Sets the newly released buttons into the <see cref="OneFrameReleaseButtonFlag"/> member of the struct.
        /// </summary>
        /// <param name="before">The inputs that were pressed on the last frame.</param>
        /// <param name="after">The inputs pressed on the current frame.</param>
        public void SetReleasedButtons(ButtonFlags before, ButtonFlags after)
        {
            OneFrameReleaseButtonFlag = GetReleasedButtons(before, after);
        }

        /// <summary>
        /// Sets the newly released buttons into the <see cref="OneFramePressButtonFlag"/> member of the struct.
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public void SetPressedButtons(ButtonFlags before, ButtonFlags after)
        {
            OneFramePressButtonFlag = GetPressedButtons(before, after);
        }

        /// <summary>
        /// Seta the <see cref="MinusOneMinusButtonFlags"/> property based on the currently pressed input buttons.
        /// </summary>
        /// <param name="buttonFlags">The currently pressed buttons at this moment in time.</param>
        public void SetMinusOneButtonFlags(ButtonFlags buttonFlags)
        {
            MinusOneMinusButtonFlags = GetMinusOneButtonFlags(buttonFlags);
        }

        /// <summary>
        /// Converts the current analog stick value to a scaled value which you
        /// can assign directly to the structure.
        /// </summary>
        /// <param name="value">The current value of your analog stick.</param>
        /// <param name="maxValue">The maximum value your analog stick can have.</param>
        public float GetScaledAnalogValue(float value, float maxValue)
        {
            return (float)((value / maxValue) * 1F);
        }

        /// <summary>
        /// Resets the structure of currently pressed controls to 0.
        /// Used before we apply our own controls.
        /// </summary>
        public void Reset()
        {
            ButtonFlags                 = 0;
            MinusOneMinusButtonFlags    = -1;
            OneFramePressButtonFlag     = 0;
            OneFrameReleaseButtonFlag   = 0;
            LeftStickX                  = 0.0F;
            LeftStickY                  = 0.0F;
            RightStickX                 = 0.0F;
            RightStickY                 = 0.0F;
            ProbablyIsEnabled           = 1;
        }

        /*
            -------------------
            Functions (Private)
            -------------------
        */

        /// <summary>
        /// Returns the buttons that have been pressed <see cref="before"/> but not pressed <see cref="after"/>.
        /// </summary>
        private ButtonFlags GetReleasedButtons(ButtonFlags before, ButtonFlags after)
        {
            // Return B and NOT A
            // "Return those before without the ones after"
            return before & (~after);
        }

        /// <summary>
        /// Returns the buttons that have been pressed in <see cref="after"/> but not pressed <see cref="before"/>.
        /// </summary>
        private ButtonFlags GetPressedButtons(ButtonFlags before, ButtonFlags after)
        {
            // Return A and NOT B
            // "Return those after without the ones before"
            return after & (~before);
        }

        /// <summary>
        /// Retrieves the value to set to the <see cref="MinusOneMinusButtonFlags"/> property.
        /// </summary>
        /// <param name="buttonFlags">The currently pressed buttons at this moment in time.</param>
        private int GetMinusOneButtonFlags(ButtonFlags buttonFlags)
        {
            // This function returns 0 if no window is active.
            if (WindowProperties.IsWindowActivated(Program.GameProcess.Process.MainWindowHandle))
            {
                return (-1 - (int)buttonFlags);
            }

            return 0;
        }


    }
}
