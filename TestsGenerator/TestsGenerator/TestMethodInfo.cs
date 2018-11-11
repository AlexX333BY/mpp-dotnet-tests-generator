﻿using System;

namespace TestsGenerator
{
    public class TestMethodInfo
    {
        public string Name
        { get; protected set; }

        public TestMethodInfo(string name)
        {
            Name = name ?? throw new ArgumentException("Name shouldn't be null");
        }
    }
}