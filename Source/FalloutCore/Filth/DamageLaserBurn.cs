﻿using UnityEngine;
using RimWorld;
using Verse;
using System.Linq;
using System.Collections.Generic;
using Verse.AI.Group;

namespace FCPEnergyFilth
{
    public class LaserBurn : DamageWorker_AddInjury
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            var result = base.Apply(dinfo, thing);
            Log.Message("TEst: " + dinfo.Instigator);
            if (thing is Pawn pawn)
            {
                if (dinfo.Weapon != null && dinfo.Weapon.HasModExtension<DeathEffectModExtension>())
                {
                    var comp = pawn.TryGetComp<CompDeathFilth>();
                    if (comp == null)
                    {
                        comp = new CompDeathFilth();
                        comp.Initialize(null);
                        comp.parent = pawn;
                        comp.chance = dinfo.Weapon.GetModExtension<DeathEffectModExtension>().effectChance * 100;
                        comp.thingToSpawn = "FG_Filth_AshPile";
                        pawn.AllComps.Add(comp);
                        comp.TryToMakeDamageFilth();
                    }
                    else
                    {
                        comp.TryToMakeDamageFilth();
                    }
                }
                else if (dinfo.Instigator is Pawn instigator)
                {
                    foreach (var hediff in instigator.health?.hediffSet?.hediffs)
                    {
                        if (hediff.def.HasModExtension<DeathEffectModExtension>())
                        {
                            var comp = pawn.TryGetComp<CompDeathFilth>();
                            if (comp == null)
                            {
                                comp = new CompDeathFilth();
                                comp.Initialize(null);
                                comp.parent = pawn;
                                comp.chance = hediff.def.GetModExtension<DeathEffectModExtension>().effectChance * 100;
                                comp.thingToSpawn = "FG_Filth_AshPile";
                                pawn.AllComps.Add(comp);
                                comp.TryToMakeDamageFilth();
                            }
                            else
                            {
                                comp.TryToMakeDamageFilth();
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}

