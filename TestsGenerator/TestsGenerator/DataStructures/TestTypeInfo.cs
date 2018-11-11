using System;
using System.Collections.Generic;

namespace TestsGenerator.DataStructures
{
    public class TestTypeInfo
    {
        public List<TestMethodInfo> Methods
        { get; protected set; }

        public string Namespace
        { get; protected set; }

        public string Name
        { get; protected set; }

        public TestTypeInfo(string name, string @namespace)
        {
            Name = name ?? throw new ArgumentException("Name shouldn't be null");
            Namespace = @namespace ?? throw new ArgumentException("Namespace shouldn't be null");
            Methods = new List<TestMethodInfo>();
        }
    }
}
