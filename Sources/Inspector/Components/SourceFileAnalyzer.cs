﻿using Inspector.CodeMetrics;
using Inspector.CodeMetrics.Scores;
using System.Collections.Generic;
using System.Linq;

namespace Inspector.Components
{

    class SourceFileAnalyzer
    {
        private readonly ICollection<ICodeMetricAnalyzer> analyzers = new List<ICodeMetricAnalyzer>();

        protected void AddAnalyzer(ICodeMetricAnalyzer analyzer)
        {
            analyzers.Add(analyzer);
        }

        public Dictionary<SourceFile, IEnumerable<CodeMetricScore>> CalculateCodeMetrics(IEnumerable<SourceFile> sourceFiles)
        {
            return sourceFiles.ToDictionary(sf => sf, sf => sf.CalculateMetricsWith(analyzers));
        }                                                 
    }
}
