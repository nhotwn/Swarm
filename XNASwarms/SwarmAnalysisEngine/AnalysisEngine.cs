﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SwarmEngine;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace SwarmAnalysisEngine
{
    public abstract class AnalysisEngine
    {
        private List<AnalysisModule> Modules;

        public AnalysisEngine(List<AnalysisModule> modules)
        {
            Modules = modules;
        }
    
        public List<Analysis> Run(List<Individual> indvds, float gametime, bool sendaudiodata)
        {
            List<Analysis> analysis = new List<Analysis>();
            for (int i = 0; i < Modules.Count; i++ )
            {
                analysis.Add(Modules[i].TryAnalysis(indvds, gametime, sendaudiodata));
            }
            return analysis;
        }

    }
}
