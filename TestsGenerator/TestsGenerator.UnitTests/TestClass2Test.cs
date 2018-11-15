using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestClassNamespace.Class2;

namespace TestClassNamespace.Class2.Test
{
    [TestClass]
    public class TestClass2Test
    {
        private TestClass2 testClass2TestInstance; 

        [TestInitialize]
        public void TestInitialize()
        {
            IMyTestInterface myTestInterface = new Mock<IMyTestInterface>();
            testClass2TestInstance = new TestClass2(myTestInterface.Object);
        }

        [TestMethod]
        public void GetInterfaceTest()
        {
            IMyTestInterface actual = testClass2TestInstance.GetInterface(); 

            IMyTestInterface expected = new Mock<IMyTestInterface>();
            Assert.AreEqual(expected.Object, actual);
            Assert.Fail("autogenerated");
        }

        [TestMethod]
        public void SetInterfaceTest()
        {
            IMyTestInterface myInt = new Mock<IMyTestInterface>(); 

            Assert.Fail("autogenerated");
        }
    }
}