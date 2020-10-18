using System;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using RimWorld;

namespace FalloutCore
{
    public class Filth_ToxicFilth : Filth
    {
        public int nextTick = 0;
        public override void Tick()
        {
            List<Thing> list = this.Map.thingGrid.ThingsListAt(this.Position);
            if (list != null && list.Count > 0)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i] is Pawn pawn && !pawn.def.defName.Contains("Gecko") 
                        && Find.TickManager.TicksGame > nextTick)
                    {
                        float severity = 0.05f * pawn.GetStatValue(StatDefOf.ToxicSensitivity, true);
                        float damage = 5 * pawn.GetStatValue(StatDefOf.ToxicSensitivity, true);
                        HealthUtility.AdjustSeverity(pawn, HediffDefOf.ToxicBuildup, severity);
                        pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, damage));
                        if (Rand.Chance(0.1f) && pawn.GetStatValue(StatDefOf.ToxicSensitivity, true) > 0)
                            pawn.pather.StopDead();
                        MoteMaker.ThrowSmoke(this.Position.ToVector3Shifted(), this.Map, 1f);
                        nextTick = Find.TickManager.TicksGame + 60;
                    }
                    else if (list[i] is Plant plant)
                    {
                        if (Rand.Chance(0.1f)) MoteMaker.ThrowSmoke(this.Position.ToVector3Shifted(), this.Map, 0.3f);
                        plant.TakeDamage(new DamageInfo(DamageDefOf.Burn, 1));
                    }
                }
            }
        }
    }
}

