using System;
using System.Collections.Generic;

namespace TestsGenerator
{
    public class TestsGeneratorConfig
    {
        protected int processThreadCount;
        protected int readThreadCount;
        protected int writeThreadCount;
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

        public int ReadThreadCount
        {
            get => readThreadCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("There should be at least 1 thread");
                }
                readThreadCount = value;
            }
        }

        public int WriteThreadCount
        {
            get => writeThreadCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("There should be at least 1 thread");
                }
                writeThreadCount = value;
            }
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
            readThreadCount = 1;
            writeThreadCount = 1;
            readPaths = new List<string>();
            outputDirectoryPath = string.Empty;
        }
    }
}
