using System;
using System.Collections.Generic;
using System.Linq;
using MutationTransformation;
using RimWorld;
using Verse;

namespace FalloutCore
{
    public class Hediff_GhoulMutation : Hediff_Implant
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            var newPawn = FCUtils.GetPawnDuplicate(pawn, PawnKindDef.Named("Ghoul_Colonist"));
            GenSpawn.Spawn(newPawn, pawn.Position, pawn.Map);
            pawn.Destroy(DestroyMode.Vanish);
            Find.LetterStack.ReceiveLetter("FC_PawnMutatesToGhoul".Translate(pawn.Named("PAWN")), "FC_PawnMutatesToGhoulDesc".Translate(pawn.Named("PAWN")), 
                LetterDefOf.NegativeEvent, newPawn);
        }
    }
}

