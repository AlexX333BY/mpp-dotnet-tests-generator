﻿using System;
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

            var readTransform = new TransformBlock<string, string>((readPath) => config.Reader.ReadText(readPath), readOptions);
            var sourceToTestfileTransform = new TransformManyBlock<string, PathContentPair>((sourceText) => config.TemplateGenerator.Generate(sourceText), processOptions);
            var writeAction = new ActionBlock<PathContentPair>((pathTextPair) => config.Writer.WriteText(pathTextPair), writeOptions);

            readTransform.LinkTo(sourceToTestfileTransform, linkOptions);
            sourceToTestfileTransform.LinkTo(writeAction, linkOptions);

            Parallel.ForEach(config.ReadPaths, async (readPath) =>
            {
                await readTransform.SendAsync(readPath);
            });

            readTransform.Complete();
            writeAction.Completion.Wait();
        }

        public TestsGenerator(TestsGeneratorConfig config)
        {
            this.config = config ?? throw new ArgumentException("Config shouldn't be null");
        }
    }
}
