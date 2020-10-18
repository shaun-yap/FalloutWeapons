using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FalloutGasEmitter
{
    public class GasEmitterBuilding : Building
    {
        public override bool DeconstructibleBy(Faction faction)
        {
            if (this.GetComp<GasEmitter>().Props.allowDeconstruction)
                return base.DeconstructibleBy(faction);
            else
                return false;
        }

        public override bool ClaimableBy(Faction by)
        {
            if (this.GetComp<GasEmitter>().Props.allowClaim)
                return base.ClaimableBy(by);
            else
                return false;

        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (this.GetComp<GasEmitter>().Props.allowDestruction)
                base.PostApplyDamage(dinfo, totalDamageDealt);
            else
                this.HitPoints = this.MaxHitPoints;
        }
    }
}

