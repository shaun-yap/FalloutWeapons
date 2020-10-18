using System.Collections.Generic;
using RimWorld;
using Verse;

namespace FalloutCollaborationProjectGas
{
    public class ThingThatAddsHediffDef : ThingDef
    {
        public HediffDef hediffToAdd;
        public int ticksBetweenApplication = 60;
        public float severityPerApplication = .01f;
    }

}

