using RimWorld;
using Verse;

namespace FalloutGasEmitter
{
    public class GasEmitterProperties : CompProperties
    {
        public ThingDef gasDef = ThingDefOf.Gas_Smoke;
        public bool ignoreWalls = true;
        public bool allowDeconstruction = false;
        public bool allowClaim = false;
        public int ticksBetweenChecks = 300;
        public float rangeToSpread = 4.5f;
        public bool allowDestruction = false;

        public GasEmitterProperties()
        {
            this.compClass = typeof(GasEmitter);
        }
    }

}

