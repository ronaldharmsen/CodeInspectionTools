﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspector.CodeMetrics
{
    public class NestingLevelScore : MethodScore
    {
        public Dictionary<int, int> LineCountPerLevel { get; internal set; }
    }
}