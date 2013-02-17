﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SwarmEngine;

namespace SwarmAnalysisEngine
{
    public interface IAnalysisModule
    {
        string ModuleName{get;}
        List<AnalysisResult> Analyze(List<Individual> indvds);
    }
}
