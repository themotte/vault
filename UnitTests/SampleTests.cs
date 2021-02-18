using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;

namespace QCVault.UnitTests
{
    [TestFixture]
    public class SampleTests
    {
        [TestCase("test")]
        public void Nunit_SimpleTest_WillRun(string test)
        {
            string result = test;
            Assert.IsTrue(result == test);
        }

        [TestCase("arg")]
        public void Nsub_SimpleFake_WillRun(string arg)
        {
            var testFake = Substitute.For<ISampleMockInterface>();
            testFake.ChecksSomething(Arg.Any<string>()).Returns(true);

            bool result = testFake.ChecksSomething(arg);

            Assert.IsTrue(result);

        }

        [TestCase("arg")]
        public void Nsub_MockAndFake_WillRun(string arg)
        {

            string result = string.Concat(arg,"5");

            var mock = Substitute.For<ISampleMockInterface>();
            var stub = Substitute.For<ISampleStubInterface>();
            stub.ReturnsSomething().Returns(result);

            var dummy = new DummyObjectToTest(mock, stub);

            dummy.DoSomethingWithDependency();

            mock.Received().ChecksSomething(Arg.Is<string>(s=>s.Equals(result)));


        }

        public class DummyObjectToTest
        {
            private readonly ISampleMockInterface toMock;
            private readonly ISampleStubInterface toStub;

            public DummyObjectToTest(ISampleMockInterface _mock,ISampleStubInterface _stub)
            {
                toMock = _mock;
                toStub = _stub;
            }

            public void DoSomethingWithDependency()
            {
                //does something
                string input = toStub.ReturnsSomething();
                toMock.ChecksSomething(input);
                //
            }

            
        }

        public class MockbjectDependency : ISampleMockInterface
        {
            public bool ChecksSomething(string randomArgument)
            {
                return true;
            }
        }

        public class FakeObjectDependency : ISampleStubInterface
        {

            public string ReturnsSomething()
            {
                return "something";
            }
        }

        public interface ISampleMockInterface
        {
            public bool ChecksSomething(string randomArgument);
        }

        public interface ISampleStubInterface
        {
            public string ReturnsSomething();
        }
    }
}
