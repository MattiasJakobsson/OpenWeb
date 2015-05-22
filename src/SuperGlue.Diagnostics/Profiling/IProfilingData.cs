﻿using System;
using System.Collections.Generic;

namespace SuperGlue.Diagnostics.Profiling
{
    public interface IProfilingData
    {
        void AddContextFor(Guid id, IProfilingContext context);
        IEnumerable<ProfilingInformation> GetFor(Guid id);
    }
}