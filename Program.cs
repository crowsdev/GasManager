using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.IO;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        List<IMyAirVent> airVentList;
        List<IMyGasTank> gasTankList;
        List<IMyGasGenerator> gasGeneratorList;
        List<ITerminalAction> gasGeneratorActions;


        float LowOxygenAirVent;
        double LowGasTanks;

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.

            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            LowOxygenAirVent = 0.35f;
            LowGasTanks = 0.35f;


            airVentList = new List<IMyAirVent>();
            gasTankList = new List<IMyGasTank>();
            gasGeneratorList = new List<IMyGasGenerator>();
            gasGeneratorActions = new List<ITerminalAction>();

        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main()
        {
            GridTerminalSystem.GetBlocksOfType<IMyAirVent>(airVentList, null);
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(gasTankList, null);
            GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(gasGeneratorList, null);

            if (CheckAirVents(airVentList) || CheckGasTanks(gasTankList))
            {
                foreach (var gg in gasGeneratorList)
                {
                    
                    gg.Enabled = true;
                }
            }
            else if (ElseCheckAirVent(airVentList) && ElseCheckGasTank(gasTankList))
            {
                foreach (var gg in gasGeneratorList)
                {
                    gg.Enabled = false;
                }
            }
        }

        private bool Func(IMyTerminalBlock arg)
        {
            if (arg.CustomName.Contains(@"[SUSS]")) return true;
            return false;
        }

        bool CheckAirVents(List<IMyAirVent> _myAirVents)
        {
            if (_myAirVents.Count == 0) return false;

            foreach (var av in _myAirVents)
            {
                if (av.GetOxygenLevel() <= LowOxygenAirVent)
                {
                    string toLog = av.GetOxygenLevel().ToString();
                    Echo("NEW-OBJECT-DETECTED-------------------------");
                    Echo("Detected Low AirVent OxygenLevel::" + toLog);
                    toLog = "CustomName::" + av.CustomName;
                    Echo(toLog);
                    toLog = "EntityId::" + av.CubeGrid.EntityId.ToString();
                    Echo(toLog);
                    toLog = av.DetailedInfo;
                    Echo("DetailedInfo::" + toLog);
                    Echo("--------------------------------------------");
                    return true;
                }
            }

            return false;
        }

        bool CheckGasTanks(List<IMyGasTank> _myGasTanks)
        {
            if (_myGasTanks.Count == 0) return false;

            foreach (var gt in _myGasTanks)
            {
                if (gt.FilledRatio <= LowGasTanks)
                {
                    string toLog = gt.FilledRatio.ToString();
                    Echo("NEW-OBJECT-DETECTED-------------------------");
                    Echo("Detected Low GasTank FilledRatio::" + toLog);
                    toLog = "CustomName::" + gt.CustomName;
                    Echo(toLog);
                    toLog = "EntityId::" + gt.CubeGrid.EntityId.ToString();
                    Echo(toLog);
                    toLog = gt.DetailedInfo;
                    Echo("DetailedInfo::" + toLog);
                    Echo("--------------------------------------------");
                    return true;
                }
            }

            return false;
        }

        bool ElseCheckAirVent(List<IMyAirVent> _myAirVents)
        {
            if (_myAirVents.Count == 0) return false;

            foreach (var av in _myAirVents)
            {
                if (av.GetOxygenLevel() < 0.9f) return false;
            }

            return true;

        }

        bool ElseCheckGasTank(List<IMyGasTank> _myGasTanks)
        {
            if (_myGasTanks.Count == 0) return false;

            foreach (var gt in _myGasTanks)
            {
                if (gt.FilledRatio < 0.9f) return false;
            }

            return true;

        }
    }
}
