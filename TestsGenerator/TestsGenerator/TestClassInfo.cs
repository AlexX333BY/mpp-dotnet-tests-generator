using System;
using System.Collections.Generic;

namespace TestsGenerator
{
    public class TestClassInfo
    {
        public List<TestMethodInfo> Methods
        { get; protected set; }

        public string Namespace
        { get; protected set; }

        public string Name
        { get; protected set; }

        public TestClassInfo(string name, string @namespace)
        {
            Name = name ?? throw new ArgumentException("Name shouldn't be null");
            Namespace = @namespace ?? throw new ArgumentException("Namespace shouldn't be null");
            Methods = new List<TestMethodInfo>();
        }
    }
}
