using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestsGenerator.IO
{
    public class ParallelFileWriter : IParallelWriter
    {
        protected int threadsCount;

        public int ThreadsCount
        {
            get => threadsCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("There should be at least 1 thread");
                }
                threadsCount = value;
            }
        }

        public void WriteText(IDictionary<string, string> pathContentPairs)
        {
            var exceptions = new ConcurrentBag<Exception>();
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = ThreadsCount
            };

            if (pathContentPairs == null)
            {
                throw new ArgumentException("Arguments shouldn't be null");
            }

            Parallel.ForEach(pathContentPairs, options, nameContentPair =>
                {
                    try
                    {
                        string directoryName = Path.GetDirectoryName(Path.GetFullPath(nameContentPair.Key));
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                        File.WriteAllText(nameContentPair.Key, nameContentPair.Value);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }
            );

            if (!exceptions.IsEmpty)
            {
                throw new AggregateException(exceptions);
            }
        }

        public ParallelFileWriter()
        {
            threadsCount = 1;
        }
    }
}
