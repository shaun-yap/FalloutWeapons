using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using System;

namespace RobotRamRod
{
    public class RobotCompProperties_Hatcher : CompProperties
    {
        public float hatcherDaystoHatch = 1f;

        public PawnKindDef hatcherPawn;

        public RobotCompProperties_Hatcher()
        {
            this.compClass = typeof(RobotCompHatcher);
        }
    }
}

