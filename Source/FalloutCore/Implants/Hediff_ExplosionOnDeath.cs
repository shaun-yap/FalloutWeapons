using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace FalloutCore
{
    public class Hediff_ExplosionOnDeath : Hediff_Implant
    {
        public bool activated = false;
        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            var option = this.def.GetModExtension<ExplosionOption>();
            if (option != null && Rand.Range(0f, 100f) <= option.chance * 100f)
            {
                GenExplosion.DoExplosion(this.pawn.Position, this.pawn.Map, option.radius, DamageDefOf.Bomb, null);
                this.pawn.health.RemoveHediff(this);
            }
        }
    }
}

