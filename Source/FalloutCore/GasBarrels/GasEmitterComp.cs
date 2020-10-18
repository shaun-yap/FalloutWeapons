using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace FalloutGasEmitter
{
    public class GasEmitter : ThingComp
    {
        List<IntVec3> cellsInArea;
        private int ticksToNext = 0;
        private int ticksBetweenChecks;
        private float range = 0;
        public GasEmitterProperties Props
        {
            get
            {
                return this.props as GasEmitterProperties;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            cellsInArea = new List<IntVec3>();
            ticksBetweenChecks = Props.ticksBetweenChecks;
            range = Props.rangeToSpread;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (Find.TickManager.TicksGame % Rand.RangeInclusive(60, 100) == 0)
            {
                foreach (var cell in cellsInArea)
                {
                    for (int i = this.parent.Map.thingGrid.ThingsListAt(cell).Count - 1; i >= 0; i--)
                    {
                        if (this.parent.Map.thingGrid.ThingsListAt(cell)[i] is Pawn pawn)
                        {
                            Job job = null;
                            IntVec3 intVec;
                            if (pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Flee)
                            {
                                intVec = pawn.CurJob.targetA.Cell;
                            }
                            else
                            {
                                var dangers = new List<Thing>();
                                foreach (var dir in GenRadial.RadialCellsAround(pawn.Position, 5, true))
                                {
                                    if (GenGrid.InBounds(dir, pawn.Map))
                                    {
                                        for (int j = pawn.Map.thingGrid.ThingsListAt(dir).Count - 1; j >= 0; j--)
                                        {
                                            var t = pawn.Map.thingGrid.ThingsListAt(dir)[j];
                                            if (t.def == Props.gasDef)
                                            {
                                                dangers.Add(t);
                                            }
                                        }
                                    }
                                }
                                intVec = CellFinderLoose.GetFleeDest(pawn, dangers, 24f);
                            }
                            if (intVec == pawn.Position)
                            {
                                intVec = GenRadial.RadialCellsAround(pawn.Position, 6, 6).RandomElement();
                            }
                            if (intVec != pawn.Position)
                            {
                                job = JobMaker.MakeJob(JobDefOf.Flee, intVec, this.parent);
                            }
                            if (job != null)
                            {
                                pawn.jobs.TryTakeOrderedJob(job);
                            }
                        }
                    }
                }
            }

            ticksToNext--;
            if (ticksToNext <= 0)
            {
                CheckEmissionArea();
                ticksToNext = Props.ticksBetweenChecks;
            }
        }

        private void CheckEmissionArea()
        {

            Map map = base.parent.Map;
            cellsInArea = GetAreaCells(base.parent.Position, Props.rangeToSpread, Props.ignoreWalls);

            if (cellsInArea.Count < 1)
                return;
            Thing gasThing;
            for (int i = 0; i < cellsInArea.Count; i++)
            {

                if (!DoesCellContain(cellsInArea[i], Props.gasDef))
                {
                    if (Props.gasDef != null)
                    {
                        gasThing = ThingMaker.MakeThing(Props.gasDef);
                        gasThing.stackCount = 1;

                    }
                    else
                    {
                        Log.Warning("Something is wrong here");
                        Log.Warning(Props.gasDef + " is null");
                        gasThing = ThingMaker.MakeThing(ThingDefOf.Filth_Blood);
                        gasThing.stackCount = 1;
                    }

                    GenSpawn.Spawn(gasThing, cellsInArea[i], map);
                }
            }

        }

        private bool DoesCellContain(IntVec3 intVec3, ThingDef gasToSpread)
        {
            Thing thing = intVec3.GetFirstThing(base.parent.Map, gasToSpread);
            if (thing == null)
                return false;
            return true;
        }

        public List<IntVec3> GetAreaCells(IntVec3 currentPosistion, float radius, bool ignoreWalls = true)
        {
            List<IntVec3> cells = new List<IntVec3>();
            int debugcount = 0;
            for (int x = -Mathf.RoundToInt(radius); x < radius; x++)
            {
                for (int z = -Mathf.RoundToInt(radius); z < radius; z++)
                {
                    IntVec3 cell = new IntVec3(x, 0, z);
                    cell += currentPosistion;
                    if (cell.InHorDistOf(currentPosistion, radius))
                        cells.Add(cell);

                    debugcount++;

                }
            }

            return cells;
        }

        public List<IntVec3> GetRegionCells(IntVec3 currentPosistion, float radius)
        {

            Map map = base.parent.Map;
            cellsInArea.Clear();
            if (!currentPosistion.InBounds(map))
            {
                return cellsInArea;
            }

            Region region = currentPosistion.GetRegion(map);
            if (region == null)
            {
                return cellsInArea;
            }

            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null,
           delegate (Region r)
           {
               foreach (IntVec3 cell in r.Cells)
               {
                   if (cell.InHorDistOf(currentPosistion, range))
                   {
                       cellsInArea.Add(cell);
                   }
               }
               return false;
           }, 16);

            return cellsInArea;
        }
    }

}

