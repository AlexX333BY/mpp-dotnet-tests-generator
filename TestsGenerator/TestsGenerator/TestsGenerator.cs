using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestsGenerator.DataStructures;

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

            var sourceToTestfileTransform = new TransformBlock<string, PathContentPair>((sourceText) => (default(PathContentPair)), processOptions);
            var writeAction = new ActionBlock<PathContentPair>((pathTextPair) => config.Writer.WriteText(pathTextPair), writeOptions);
            var readTransform = new TransformBlock<string, string>((readPath) => config.Reader.ReadText(readPath), readOptions);

            readTransform.LinkTo(sourceToTestfileTransform, linkOptions);
            sourceToTestfileTransform.LinkTo(writeAction, linkOptions);

            Parallel.ForEach(config.ReadPaths, async (readPath) =>
            {
                await readTransform.SendAsync(readPath);
            });

            readTransform.Complete();
            sourceToTestfileTransform.Complete();
            writeAction.Complete();
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
