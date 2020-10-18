using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Quests
{
    public class ArmorData : IExposable
    {
        public List<StatModifier> ArmorStats;
        public BodyPartGroupDef bodyPart;

        public void ExposeData()
        {
            Scribe_Defs.Look<BodyPartGroupDef>(ref this.bodyPart, "part");
            //Scribe_Values.Look<float>(ref this.ArmorRating_Sharp, "ArmorRating_Sharp", 0f, true);
            //Scribe_Values.Look<float>(ref this.ArmorRating_Blunt, "ArmorRating_Blunt", 0f, true);
            //Scribe_Values.Look<float>(ref this.ArmorRating_Heat, "ArmorRating_Heat", 0f, true);
        }

    }
}

