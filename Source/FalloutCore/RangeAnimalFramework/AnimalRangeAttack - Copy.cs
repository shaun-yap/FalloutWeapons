using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace FalloutCore
{
    public class FireProjectile : Projectile
    {

        public override void Tick()
        {
            base.Tick();
            tickAmount++;
        }
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (tickAmount > 3)
            {
                base.DrawAt(drawLoc, flip);
            }
        }

        public override void Draw()
        {
            if (tickAmount > 3)
            {
                base.Draw();
            }
        }
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact =
                new BattleLogEntry_RangedImpact(this.launcher, hitThing,
                this.intendedTarget.Thing, ThingDef.Named("Gun_Autopistol"), this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            bool flag = hitThing != null;
            if (flag)
            {
                DamageDef damageDef = this.def.projectile.damageDef;
                float num = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo damageInfo = new DamageInfo(damageDef, num, armorPenetration, y, launcher, null, null, 0, this.intendedTarget.Thing); ;
                hitThing.TakeDamage(damageInfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn = hitThing as Pawn;
                bool flag2 = pawn != null && pawn.stances != null &&
                    pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f;
                if (flag2)
                {
                    pawn.stances.StaggerFor(95);
                }

                float ignitionChance = def.projectile.explosionChanceToStartFire;
                var radius = def.projectile.explosionRadius;
                var cellsToAffect = SimplePool<List<IntVec3>>.Get();
                cellsToAffect.Clear();
                cellsToAffect.AddRange(def.projectile.damageDef.Worker.ExplosionCellsToHit(Position, map, radius));
                for (int i = 0; i < 4; i++)
                {
                    MoteMaker.ThrowSmoke(Position.ToVector3Shifted() + Gen.RandomHorizontalVector(radius * 0.7f), map, radius * 0.6f);
                }

                if (Rand.Chance(ignitionChance))
                    foreach (var vec3 in cellsToAffect)
                    {
                        var fireSize = radius - vec3.DistanceTo(Position);
                        if (fireSize > 0.1f)
                        {
                            FireUtility.TryStartFireIn(vec3, map, fireSize);
                        }
                    }

                //Fire explosion should be tiny.
                if (this.def.projectile.explosionEffect != null)
                {
                    Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                    effecter.Trigger(new TargetInfo(this.Position, map, false), new TargetInfo(this.Position, map, false));
                    effecter.Cleanup();
                }
            }
            else
            {
                SoundStarter.PlayOneShot(SoundDefOf.BulletImpact_Ground, new TargetInfo(base.Position, map, false));
                //MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                bool takeSplashes = GridsUtility.GetTerrain(base.Position, map).takeSplashes;
                if (takeSplashes)
                {
                    //MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.tickAmount, "tickAmount", 0, true); 
        }

        public int tickAmount = 0;
    }
}

