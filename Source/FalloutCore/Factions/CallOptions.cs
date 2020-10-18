using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace FalloutCore
{
    public class CallOptions : DefModExtension
    {
        public List<CallOption> options;
    }

    public class CallOption
    {
        public IncidentDef callIncidentDef;
        public int goodwillCost;
        public string text;
        public string message;
    }
}

