﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Analyzer.Runtime
{
    public class ParseRunnerOptions
    {
        public bool EnableErrorRecovery { get; }

        public ParseRunnerOptions(bool enableErrorRecovery = false)
        {
            EnableErrorRecovery = enableErrorRecovery;
        }
    }
}
