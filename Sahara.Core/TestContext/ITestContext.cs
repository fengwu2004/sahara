﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public interface ITestContext<TResult>
        where TResult : ITestResult
    {
        TResult Execute();
    }
}