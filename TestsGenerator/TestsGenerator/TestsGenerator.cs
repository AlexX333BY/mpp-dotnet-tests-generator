using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TestsGenerator
{
    public class TestsGenerator : ITestsGenerator
    {
        protected readonly TestsGeneratorConfig config;

        public void Generate()
        {
            var exceptions = new List<Exception>();
            DataflowLinkOptions linkOptions = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };
            ExecutionDataflowBlockOptions readOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = config.ReadThreadCount
            };
            ExecutionDataflowBlockOptions writeOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = config.WriteThreadCount
            };
            ExecutionDataflowBlockOptions processOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = config.ProcessThreadCount
            };

            var sourceToTestTransform = new TransformBlock<string, KeyValuePair<string, string>>(new Func<string, KeyValuePair<string, string>>(null), processOptions);
            var writeAction = new ActionBlock<KeyValuePair<string, string>>((pathTextPair) => config.Writer.WriteText(pathTextPair), writeOptions);

            sourceToTestTransform.LinkTo(writeAction, linkOptions);

            Parallel.ForEach(config.ReadPaths, (readPath) =>
            {
                // read and send to transform
            });
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
