using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded_Mod_Template.Controller;
using Reloaded_Mod_Template.Misc;

namespace Reloaded_Mod_Template.Config
{
    public class ControllerMappingFactory
    {
        /* Relative to mod folder. */
        private const string PlayerOneFileName = "PlayerOne.json";
        private const string PlayerTwoFileName = "PlayerTwo.json";
        private const string PlayerThreeFileName = "PlayerThree.json";
        private const string PlayerFourFileName = "PlayerFour.json";

        /* Contains the individual Sonic Heroes players. */
        private static ControllerMapping _playerOne;
        private static ControllerMapping _playerTwo;
        private static ControllerMapping _playerThree;
        private static ControllerMapping _playerFour;
        private readonly string _sourceFolder;

        /* Constructor */
        public ControllerMappingFactory(string sourceFolder)
        {
            _sourceFolder = sourceFolder;
            GetControllers();
        }

        /* Methods */

        /// <summary>
        /// Returns a mapping for either player one or player two.
        /// </summary>
        public ControllerMapping GetInstance(Player player)
        {
            switch (player)
            {
                case Player.PlayerOne: return _playerOne;
                case Player.PlayerTwo: return _playerTwo;
                case Player.PlayerThree: return _playerThree;
                case Player.PlayerFour: return _playerFour;
            }

            return null;
        }

        /// <summary>
        /// Populates the <see cref="_playerTwo"/> and <see cref="_playerOne"/> fields with individual controller mappings.
        /// </summary>
        public void GetControllers()
        {
            _playerOne = ControllerMapping.FromFile($"{_sourceFolder}\\{PlayerOneFileName}");
            _playerTwo = ControllerMapping.FromFile($"{_sourceFolder}\\{PlayerTwoFileName}");
            _playerThree = ControllerMapping.FromFile($"{_sourceFolder}\\{PlayerThreeFileName}");
            _playerFour = ControllerMapping.FromFile($"{_sourceFolder}\\{PlayerFourFileName}");
        }
    }
}
