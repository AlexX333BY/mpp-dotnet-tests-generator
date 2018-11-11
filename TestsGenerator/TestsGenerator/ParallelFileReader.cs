using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestsGenerator
{
    public class ParallelFileReader : IParallelReader
    {
        protected List<string> successfullyReadText;
        protected int threadsCount;

        public IEnumerable<string> SuccessfullyReadText => new List<string>(successfullyReadText);

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

        public IEnumerable<string> ReadText(IEnumerable<string> paths)
        {
            var result = new ConcurrentBag<string>();
            var exceptions = new ConcurrentBag<Exception>();
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = ThreadsCount
            };

            if (paths == null)
            {
                throw new ArgumentException("Paths shouldn't be null");
            }

            Parallel.ForEach(paths, options, path =>
                {
                    try
                    {
                        result.Add(File.ReadAllText(path));
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }
            );
            successfullyReadText = new List<string>(result);

            if (!exceptions.IsEmpty)
            {
                throw new AggregateException(exceptions);
            }
            return SuccessfullyReadText;
        }

        public ParallelFileReader() 
        {
            threadsCount = 1;
            successfullyReadText = new List<string>();
        }
    }
}
