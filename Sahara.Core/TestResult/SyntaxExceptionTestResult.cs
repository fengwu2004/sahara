using Microsoft.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    public class SyntaxExceptionTestResult : ExceptionTestResult
    {
        public SyntaxExceptionTestResult(SyntaxErrorException ex)
            : base(ex)
        {
            this.Line = ex.Line;
            this.Column = ex.Column;
        }

        public int Line { get; private set; }

        public int Column { get; private set; }

        public override string Message
        {
            get
            {
                return String.Format("Line: {0}, Column: {1}, Error: {2}", this.Line, this.Column, base.Message);
            }
        }
    }
}
