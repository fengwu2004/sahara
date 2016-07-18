using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    internal class TestResultWriter : TextWriter
    {
        public class TestResultWriterEventArgs: EventArgs
        {
            public BaseTestResult TestResult { get; set; }
        }

        public delegate void TestResultWriterEventHandler(object sender, TestResultWriterEventArgs args);
        public event TestResultWriterEventHandler OnWrite;

        public ITestResultBuilder Builder { get; private set; }

        public Type TestResultType { get; private set; }

        public TestResultWriter(ITestResultBuilder builder, Type testResultType)
        {
            this.TestResultType = testResultType;
            this.Builder = builder;
        }

        public override void Write(string value)
        {
            // Check if value is printable character
            if (value != "\r\n")
            {
                base.Write(value);
                var result = Activator.CreateInstance(TestResultType, value) as BaseTestResult;
                if (result != null)
                {
                    OnWrite?.Invoke(this, new TestResultWriterEventArgs()
                    {
                        TestResult = result
                    });
                    this.Builder.AddResult(result);
                }
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
