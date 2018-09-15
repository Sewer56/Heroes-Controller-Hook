using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reloaded_Mod_Template.Controller.Heroes
{
    public struct SkyPad
    {
        public ButtonFlags ButtonFlags;
        public int MinusOneMinusButtonFlags;
        public int OneFramePressButtonFlags;
        public int OneFrameReleaseButtonFlags;
        public short TriggerPressureL; // Up to 255
        public short TriggerPressureR; // Up to 255
        public int Field14;
        public int Field18;
        public int LeftAnalogStickVector;

        /// <summary>
        /// Max 1.0F
        /// </summary>
        public float LeftAnalogStickForce;
        public int RightAnalogStickVector;

        /// <summary>
        /// Max 1.0F
        /// </summary>
        public float RightAnalogStickForce;
        public int Field2C;
        public int Field30;
        public int Field34;
        public int Field38;
        public int Field3C;
        public int Field40;
        public int Field44;
        public int Field48;
    }
}
