using System;
using System.Collections.Generic;
using TestsGenerator.IO;

namespace TestsGenerator
{
    public class TestsGeneratorConfig
    {
        protected int processThreadCount;
        protected IParallelReader reader;
        protected IParallelWriter writer;
        protected IEnumerable<string> readPaths;
        protected string outputDirectoryPath;

        public int ProcessThreadCount
        {
            get => processThreadCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("There should be at least 1 thread");
                }
                processThreadCount = value;
            }
        }

        public IParallelReader Reader
        {
            get => reader;
            set => reader = value ?? throw new ArgumentException("Reader shouldn't be null");
        }

        public IParallelWriter Writer
        {
            get => writer;
            set => writer = value ?? throw new ArgumentException("Writer shouldn't be null");
        }

        public IEnumerable<string> ReadPaths
        {
            get => new List<string>(readPaths);
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Paths shouldn't be null");
                }
                readPaths = new List<string>(value);
            }
        }

        public string OutputDirectoryPath
        {
            get => outputDirectoryPath;
            set => outputDirectoryPath = value ?? throw new ArgumentException("Path shouldn't be null");
        }

        public TestsGeneratorConfig()
        {
            processThreadCount = Environment.ProcessorCount;
            reader = null;
            writer = null;
            readPaths = new List<string>();
            outputDirectoryPath = string.Empty;
        }
    }
}
