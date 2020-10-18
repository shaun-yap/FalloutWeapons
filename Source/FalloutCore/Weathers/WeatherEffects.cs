using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace FalloutCore
{
    public class WeatherEffects : DefModExtension
    {

        public List<string> hediffDefnames;

        public int ticksInterval;

        public float severity;

        public bool causesRotting;

        public bool killingPlants;

    }
}

