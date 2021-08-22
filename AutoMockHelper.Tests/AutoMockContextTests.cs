namespace AutoMockHelper.Tests
{
    using AutoMockHelper.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Diagnostics.CodeAnalysis;

    #region Sample Classes

    public class SampleLogic
    {
        private readonly ISampleService _sampleService;
        private readonly ISampleService2 _sampleService2;

        public SampleLogic(ISampleService sampleService, ISampleService2 sampleService2)
        {
            this._sampleService = sampleService;
            this._sampleService2 = sampleService2;
        }

        public void CallSampleServiceMethods()
        {
            this._sampleService.SampleServiceMethod();
            this._sampleService2.SampleServiceMethod2();
        }
    }

    public interface ISampleService
    {
        void SampleServiceMethod();
    }

    public interface ISampleService2
    {
        void SampleServiceMethod2();
    }

    public sealed class SampleExternalService
    {
        public SampleExternalService(ISampleService sampleService)
        {
        }
    }

    public class AlternateSampleServiceImplementation : ISampleService {
        public void SampleServiceMethod()
        {
        }
    }

    public class SampleLogicTests : AutoMockContext<SampleLogic>
    {
        public new Mock<TDependency> StrictMock<TDependency>()
            where TDependency : class
        {
            return base.StrictMock<TDependency>();
        }

        public new TClassToCreate CreateInstance<TClassToCreate>()
            where TClassToCreate : class
        {
            return base.CreateInstance<TClassToCreate>();
        }

        public new void VerifyAll()
        {
            base.VerifyAll();
        }

        public void TestMethodThatCallsSampleServiceSampleServiceMethod()
        {
            //Arrange
            //Act
            this.ClassUnderTest.CallSampleServiceMethods();

            //Assert
            //This is handled by the calling test class
        }
    }

    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AutoMockContextTests
    {
        private SampleLogicTests GetInitializedTestClassInstance()
        {
            return new SampleLogicTests();
        }

        [TestMethod]
        public void IsClassUnderTestInitializedBeforeSetUp()
        {
            //Arrange
            var instance = new SampleLogicTests();

            //Act/Assert
            Assert.IsNotNull(instance.ClassUnderTest);
        }

        [TestMethod]
        public void SetupInitializesAutoMocker()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();

            //Act
            
            var isAutoMockerInitializedAfterSetup = instance.ClassUnderTest != null;

            //Asesrt
            Assert.IsTrue(isAutoMockerInitializedAfterSetup);
        }

        [TestMethod]
        public void MockForReturnsMockOfSpecifiedType()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();

            //Act
            var actual = instance.MockFor<SampleLogic>();

            //Assert
            Assert.IsInstanceOfType(actual, typeof(Mock<SampleLogic>));
        }

        [TestMethod]
        public void StrictMockEnablesStrictMockForSpecifiedType()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();
            var mock = instance.StrictMock<ISampleService>();

            //Act
            var actual = instance.MockFor<ISampleService>();

            //Assert
            Assert.AreEqual(mock, actual);
            Assert.AreEqual(MockBehavior.Strict, actual.Behavior);
        }

        [TestMethod]
        public void CreateInstanceCreatesNewInstanceOfSpecifiedType()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();

            //Act
            var actual = instance.CreateInstance<SampleExternalService>();

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(SampleExternalService));
        }

        [TestMethod]
        public void UseImplementationUsesMockOfSpecifiedImplementation()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();
            var mockToUse = new Mock<ISampleService2>();

            //Act
            var actual = instance.Use(mockToUse);

            //Assert
            Assert.AreSame(mockToUse, actual);
        }

        [TestMethod]
        public void UseImplementationUsesSpecifiedInstance()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();
            var instanceToUse = new SampleExternalService(new Mock<ISampleService>().Object);

            //Act
            var actual = instance.Use(instanceToUse);

            //Assert
            Assert.AreSame(instanceToUse, actual);
        }

        [TestMethod]
        public void UseTypeInstantiatesSpecifiedType()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();
            
            //Act
            var actual = instance.Use<ISampleService, AlternateSampleServiceImplementation>();
            actual.SampleServiceMethod(); //Call this just to reach full code coverage (not that it really matters here).

            //Assert
            Assert.IsInstanceOfType(actual, typeof(AlternateSampleServiceImplementation));
        }

        [TestMethod]
        public void VerifyCallsForTypeVerifiesOnlyCallsForThatType()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();
            var sampleServiceMock = new Mock<ISampleService>(MockBehavior.Strict);
            sampleServiceMock.Setup(x => x.SampleServiceMethod());
            instance.Use(sampleServiceMock);

            //Act
            instance.TestMethodThatCallsSampleServiceSampleServiceMethod();

            //Assert
            instance.VerifyCallsFor<ISampleService>();
        }

        [TestMethod]
        public void VerifyAllCallsVerifyAll()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();
            instance.StrictMock<ISampleService>();
            instance.StrictMock<ISampleService2>();
            instance.MockFor<ISampleService>().Setup(x => x.SampleServiceMethod());
            instance.MockFor<ISampleService2>().Setup(x => x.SampleServiceMethod2());

            //Act
            instance.TestMethodThatCallsSampleServiceSampleServiceMethod();

            //Assert
            instance.VerifyAll();
        }

        [TestMethod]
        public void VerifyForCallsVerifyFor()
        {
            //Arrange
            var instance = this.GetInitializedTestClassInstance();
            instance.StrictMock<ISampleService>();
            instance.StrictMock<ISampleService2>();
            instance.MockFor<ISampleService>().Setup(x => x.SampleServiceMethod());
            instance.MockFor<ISampleService2>().Setup(x => x.SampleServiceMethod2());

            //Act
            instance.TestMethodThatCallsSampleServiceSampleServiceMethod();

            //Assert
            instance.Verify<ISampleService>(x => x.SampleServiceMethod(), Times.Once()); //Using the Times for the Times parameter
            instance.Verify<ISampleService2>(x => x.SampleServiceMethod2(), Times.Once); //Using the Func<Times> overload for the Times parameter
        }
    }
}
