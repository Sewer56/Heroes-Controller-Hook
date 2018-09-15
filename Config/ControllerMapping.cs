using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Reloaded.Input.Common.ControllerInputs;

namespace Reloaded_Mod_Template.Config
{
    public class ControllerMapping
    {
        /* Main Properties */
        public int ControllerPort = 0;

        /* Buttons */

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton Jump       = ControllerButton.ButtonA;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton Action = ControllerButton.ButtonB;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton FormationL = ControllerButton.ButtonY;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton FormationR = ControllerButton.ButtonX;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton CameraL = ControllerButton.ButtonLb;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton CameraR = ControllerButton.ButtonRb;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton TeamBlast = ControllerButton.ButtonBack;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton Start = ControllerButton.ButtonStart;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton DpadUp     = ControllerButton.DpadUp;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton DpadDown   = ControllerButton.DpadDown;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton DpadLeft   = ControllerButton.DpadLeft;

        [JsonConverter(typeof(StringEnumConverter))]
        public ControllerButton DpadRight  = ControllerButton.DpadRight;

        /* Axis */
        public TriggerSettings   TriggerOptions = new TriggerSettings();

        /* Functions (Public)*/
        public static ControllerMapping FromFile(string filePath)
        {
            if (! File.Exists(filePath))
                new ControllerMapping().ToFile(filePath);

            return JsonConvert.DeserializeObject<ControllerMapping>(File.ReadAllText(filePath));
        }

        public void ToFile(string filePath)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            json = CommentMappingFile(json);
            File.WriteAllText(filePath, json);
        }

        /* Functions (Private) */

        // Adds additional comments at the end of the JSON mapping file.
        private string CommentMappingFile(string json)
        {
            StringBuilder builder = new StringBuilder(json);

            builder.Append("\n");
            builder.Append("/*");
            builder.Append("\n\n");

            builder.Append("ControllerPort: Defines which Reloaded controller slot/number to read from.");
            builder.Append("\n\n");

            builder.Append("Available Buttons:\n");
            foreach (var enumName in Enum.GetNames(typeof(ControllerButton)))
                builder.Append($"{enumName}\n");

            builder.Append("\n\n");
            builder.Append("*/");
            builder.Append("\n");
            return builder.ToString();
        }

        /* Miscellaneous Classes */
        public class TriggerSettings
        {
            public bool EnableTriggerRotation   = true;
            public bool SwapTriggers  = false;
        }
    }
}
