using System;
using System.Collections.Generic;
using System.IO;

namespace TestsGenerator
{
    public class TestsGenerator : ITestsGenerator
    {
        protected readonly TestsGeneratorConfig config;

        protected Dictionary<string, string> CreateOutputPaths(string directory, IDictionary<string, string> sourcePaths)
        {
            var outputPaths = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> keyValuePair in sourcePaths)
            {
                outputPaths[directory + Path.DirectorySeparatorChar + keyValuePair.Key] = keyValuePair.Value;
            }

            return outputPaths;
        }

        public void Generate()
        {
            var exceptions = new List<Exception>();
            IDictionary<string, string> readSource;

            try
            {
                config.Reader.ReadText(config.ReadPaths);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            readSource = config.Reader.SuccessfullyReadText;

            // TODO: generation

            try
            {
                config.Writer.WriteText(CreateOutputPaths(config.OutputDirectoryPath, readSource));
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }

            if (exceptions.Count != 0)
            {
                throw new AggregateException(exceptions);
            }
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
