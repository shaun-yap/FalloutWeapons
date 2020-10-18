using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace ArmorWithHediffs
{
    public class ArmorWithHediffs : Apparel
    {
        public Pawn wearer = null;
        public override void Tick()
        {
            base.Tick();
            //Log.Message("wearer: " + wearer + " - base.Wearer: " + base.Wearer, true);
            if (this.def.HasModExtension<HediffListModExtension>() && base.Wearer != wearer)
            {
                var hediffStrings = this.def.GetModExtension<HediffListModExtension>().hediffDefnames;
                if (hediffStrings != null && hediffStrings.Count > 0)
                {
                    if (wearer != null) // it removes the hediffs from the previous owner
                    {
                        foreach (var defName in hediffStrings)
                        {
                            var hediffDef = HediffDef.Named(defName);
                            if (hediffDef != null)
                            {
                                var hediff = wearer.health.hediffSet.hediffs
                                .Where(x => x.def.defName == defName).FirstOrDefault();
                                if (hediff != null) wearer.health.hediffSet.hediffs.Remove(hediff);
                            }
                        }
                    }
                    if (base.Wearer != null)
                    {
                        foreach (var defName in hediffStrings) //it add the hediffs to the new owner
                        {
                            var hediff = HediffDef.Named(defName);
                            if (hediff != null && base.Wearer.health.hediffSet.hediffs
                                    .Where(x => x.def.defName == defName).Count() == 0)
                            {
                                base.Wearer.health.AddHediff(hediff);
                            }
                        }
                    }
                }
                wearer = base.Wearer;
            }
        }
    }
}

