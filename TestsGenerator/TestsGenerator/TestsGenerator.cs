using System;
using System.Collections.Generic;

namespace TestsGenerator
{
    public class TestsGenerator : ITestsGenerator
    {
        protected readonly TestsGeneratorConfig config;

        public void Generate()
        {
            var exceptions = new List<Exception>();
            IEnumerable<string> readSource;
        }

        public TestsGenerator(TestsGeneratorConfig config)
        {
            if (config == null)
            {
                throw new ArgumentException("Config shouldn't be null");
            }
            if ((config.Reader == null) || (config.Writer == null))
            {
                throw new ArgumentException("Config isn't initialized with reader or writer");
            }

            this.config = config;
        }
    }
}
