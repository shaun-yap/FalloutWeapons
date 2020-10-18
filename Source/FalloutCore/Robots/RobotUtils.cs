using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RobotStuff
{
    /// <summary>
    /// Tweaks ThingDefs after the game has been made.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class RobotUtils
    {
        static RobotUtils()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                //If the Def has a RobotEdit do blank, otherwise does nothing.
                RobotEdit tweaker = thingDef.GetModExtension<RobotEdit>();
                if (tweaker != null)
                {
                    ThingDef corpseDef = thingDef?.race?.corpseDef;
                    if (corpseDef != null)
                    {
                        //Removes the corpse rotting.
                        if (tweaker.removeCorpseRot)
                        {
                            corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_Rottable);
                            corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_SpawnerFilth);
                        }

                    }
                }
            }
        }

        public static List<string> robotRaces = new List<string>
        {
            "MrHandy",
            "MisterGutsy",
            "NurseHandy",
            "Protectetron",
            "ScrapBot",
            "Securitron",
            "Sentrybot",
            "Assaultron"
        };
        public static bool IsRobot(this Pawn pawn)
        {
            if (robotRaces.Contains(pawn.def.defName))
            {
                return true;
            }
            return false;
        }
    }
}