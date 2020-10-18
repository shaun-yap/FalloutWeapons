using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace FalloutCore
{
    public class Hediff_ImplantTamer : Hediff_Implant
    {
        public override void PostMake()
        {
            base.PostMake();
            this.pawn.SetFaction(Faction.OfPlayer); 
            if (pawn.guest != null)
            {
                pawn.guest.SetGuestStatus(null, false);
            }
            if (this.def.HasModExtension<TamingOption>())
            {
                var options = this.def.GetModExtension<TamingOption>();
                Find.LetterStack.ReceiveLetter(options.letterTitle.Translate(pawn.Named("PAWN")).CapitalizeFirst(), 
                    options.letterDesc.Translate(pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent, pawn);
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(options.thoughtToGive);
            }
            else
            {
                string value = pawn.LabelIndefinite();
                bool flag = pawn.Name != null;
                string str;
                if (!flag && pawn.Name != null)
                {
                    if (pawn.Name.Numerical)
                    {
                        str = "LetterAnimalSelfTameAndNameNumerical".Translate(value, pawn.Name.ToStringFull, pawn.Named("ANIMAL")).CapitalizeFirst();
                    }
                    else
                    {
                        str = "LetterAnimalSelfTameAndName".Translate(value, pawn.Name.ToStringFull, pawn.Named("ANIMAL")).CapitalizeFirst();
                    }
                }
                else
                {
                    str = "LetterAnimalSelfTame".Translate(pawn).CapitalizeFirst();
                }
                Find.LetterStack.ReceiveLetter("LetterAnimalSelfTame".Translate(pawn).CapitalizeFirst()
                    , str, LetterDefOf.PositiveEvent, pawn);
            }

        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            this.pawn.SetFaction(null);
        }

    }
}

