using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using Reloaded;
using Reloaded.Assembler;
using Reloaded.Input;
using Reloaded.Process;
using Reloaded.Process.Buffers;
using Reloaded.Process.Functions.X86Functions;
using Reloaded.Process.Functions.X86Hooking;
using Reloaded.Process.Memory;
using Reloaded.Process.Native;
using Reloaded_Mod_Template.Controller;
using Reloaded_Mod_Template.Controller.Heroes;
using Reloaded_Mod_Template.Misc;
using static Reloaded.Process.Functions.X86Functions.ReloadedFunctionAttribute;
using CallingConventions = Reloaded.Process.Functions.X86Functions.CallingConventions;

namespace Reloaded_Mod_Template
{
    public static unsafe class Program
    {
        #region Mod Loader Template Description & Explanation | Your first time? Read this.
        /*
         *  Reloaded Mod Loader DLL Modification Template
         *  Sewer56, 2018 ©
         *
         *  -------------------------------------------------------------------------------
         *
         *  Here starts your own mod loader DLL code.
         *
         *  The Init function below is ran at the initialization stage of the game.
         *
         *  The game at this point suspended and frozen in memory. There is no execution
         *  of game code currently ongoing.
         *
         *  This is where you do your hot-patches such as graphics stuff, patching the
         *  window style of the game to borderless, setting up your initial variables, etc.
         *
         *  -------------------------------------------------------------------------------
         *
         *  Important Note:
         *
         *  This function is executed once during startup and SHOULD return as the
         *  mod loader awaits successful completion of the main function.
         *
         *  If you want your mod/code to sit running in the background, please initialize
         *  another thread and run your code in the background on that thread, otherwise
         *  please remember to return from the function.
         *
         *  There is also some extra code, including DLL stubs for Reloaded, classes
         *  to interact with the Mod Loader Server as well as other various loader related
         *  utilities available.
         *
         *  -------------------------------------------------------------------------------
         *  Extra Tip:
         *
         *  For Reloaded mod development, there are also additional libraries and packages
         *  available on NuGet which provide you with extra functionality.
         *
         *  Examples include:
         *  [Input] Reading controller information using Reloaded's input stack.
         *  [IO] Accessing the individual Reloaded config files.
         *  [Overlays] Easy to use D3D and external overlay code.
         *
         *  Simply search libReloaded on NuGet to find those extras and refer to
         *  Reloaded-Mod-Samples subdirectory on Github for examples of using them (and
         *  sample mods showing how Reloaded can be used).
         *
         *  -------------------------------------------------------------------------------
         *
         *  [Template] Brief Walkthrough:
         *
         *  > ReloadedTemplate/Initializer.cs
         *      Stores Reloaded Mod Loader DLL Template/Initialization Code.
         *      You are not required/should not (need) to modify any of the code.
         *
         *  > ReloadedTemplate/Client.cs
         *      Contains various pieces of code to interact with the mod loader server.
         *
         *      For convenience it's recommended you import Client static(ally) into your
         *      classes by doing it as such `Reloaded_Mod_Template.Reloaded_Code.Client`.
         *
         *      This will avoid you typing the full class name and let you simply type
         *      e.g. Print("SomeTextToConsole").
         *
         *  -------------------------------------------------------------------------------
         *
         *  If you like Reloaded, please consider giving a helping hand. This has been
         *  my sole project taking up most of my free time for months. Being the programmer,
         *  artist, tester, quality assurance, alongside various other roles is pretty hard
         *  and time consuming, not to mention that I am doing all of this for free.
         *
         *  Well, alas, see you when Reloaded releases.
         *
         *  Please keep this notice here for future contributors or interested parties.
         *  If it bothers you, consider wrapping it in a #region.
        */
        #endregion Mod Loader Template Description

        #region Reloaded Mod Template Variables
        /*
            Default Variables:
            These variables are automatically assigned by the mod template, you do not
            need to assign those manually.
        */

        /// <summary>
        /// Holds the game process for us to manipulate.
        /// Allows you to read/write memory, perform pattern scans, etc.
        /// See libReloaded/GameProcess (folder)
        /// </summary>
        public static ReloadedProcess GameProcess;

        /// <summary>
        /// Stores the absolute executable location of the currently executing game or process.
        /// </summary>
        public static string ExecutingGameLocation;

        /// <summary>
        /// Specifies the full directory location that the current mod 
        /// is contained in.
        /// </summary>
        public static string ModDirectory;
        #endregion Reloaded Mod Template Variables

        private static ReloadedController playerOneController;
        private static ReloadedController playerTwoController;
        private static ReloadedController playerThreeController;
        private static ReloadedController playerFourController;

        private static FunctionHook<psPADServerPC> psPADServerHook;
        private static sGamePeri__MakeRepeatCount periMakeRepeatFunction;
        private static FunctionHook<sGamePeri__MakeRepeatCount> periMakeRepeatCountHook;

        /* Entry Point */
        public static unsafe void Init()
        {
#if DEBUG
            Debugger.Launch();
#endif

            // Setup controllers.
            var controllerManager = new ControllerManager();
            playerOneController = new ReloadedController(Player.PlayerOne, controllerManager);
            playerTwoController = new ReloadedController(Player.PlayerTwo, controllerManager);
            playerThreeController = new ReloadedController(Player.PlayerThree, controllerManager);
            playerFourController = new ReloadedController(Player.PlayerFour, controllerManager);

            // Hook get controls function.
            psPADServerHook = FunctionHook<psPADServerPC>.Create(0x444F30, PSPADServerImpl).Activate();

            // Copy the old function to a new place and create a function from it.
            byte[] periMakeRepeatBytes = GameProcess.ReadMemory((IntPtr) 0x00434FF0, 0xDD);
            IntPtr functionPtr = MemoryBufferManager.Add(periMakeRepeatBytes);

            periMakeRepeatFunction = FunctionWrapper.CreateWrapperFunction<sGamePeri__MakeRepeatCount>((long)functionPtr);
            periMakeRepeatCountHook = FunctionHook<sGamePeri__MakeRepeatCount>.Create(0x00434FF0, MakeRepeatCountImpl).Activate();
        }

        /// <summary>
        /// Sends the individual player controls directly to the game.
        /// </summary>
        /// <returns>Game does not use return value.</returns>
        private static int PSPADServerImpl()
        {
            if (Reloaded.Native.Functions.WindowProperties.IsWindowActivated())
            {
                playerOneController.SendInputs();
                playerTwoController.SendInputs();
                playerThreeController.SendInputs();
                playerFourController.SendInputs();
            }

            // Return value exists but the game does not use it.
            return 1;
        }

        /// <summary>
        /// Reimplements the original function which calculates/sets how much each button has been repeatedly pressed/tapped.
        /// </summary>
        /// <param name="skyPad"></param>
        /// <returns></returns>
        private static SkyPad* MakeRepeatCountImpl(SkyPad* skyPad)
        {
            // Do not mix trigger buttons and triggers.
            switch ((int)skyPad)
            {
                // 1P, 2P, 3P, 4P cases.
                case 0x00A23A68: playerOneController.SetTriggerRotations(skyPad); break;
                case 0x00A23AB4: playerTwoController.SetTriggerRotations(skyPad); break;
                case 0x00A23B00: playerThreeController.SetTriggerRotations(skyPad); break;
                case 0x00A23B4C: playerFourController.SetTriggerRotations(skyPad); break;
            }
            
            return periMakeRepeatFunction(skyPad);
        }

        [ReloadedFunction(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int psPADServerPC(); // 00444F30

        // The actual function we would want to hook to control the triggers; the only issue seems to be weird behaviour
        // When hooking the function - button inputs repeating over and over without user interaction and straight up 
        // crashes in C# method calls.
        // I've spent 6 hours messing with this function; even made a manual custom hook but no avail.
        // 0x004351A0
        [ReloadedFunction(new []{ Register.ebx, Register.edi, Register.esi}, Register.eax, StackCleanup.Caller)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate SkyPad* periConvertPadData(IntPtr unknownPtr, HeroesController* heroesControllerPointer, SkyPad* skyPad);

        // 00434FF0
        [ReloadedFunction(new[] { Register.eax }, Register.eax, StackCleanup.Caller)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate SkyPad* sGamePeri__MakeRepeatCount(SkyPad* skyPad);
    }
}
