using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoMockHelper.Tests
{
	using System;
	using AutoMockHelper.Core;
	using Moq;

	public class SampleLogic { }

    public interface ISampleService { }

    public class SampleLogicTests : AutoMockContext<SampleLogic>
    {
        public SampleLogicTests(ISampleService sampleService)
        {
        }
    }

	[TestClass]
	public class AutoMockContextTests
	{
	    [TestMethod, ExpectedException(typeof(NullReferenceException))]
	    public void IsClassUnderTestNullBeforeSetUp()
	    {
            //Arrange
	        var instance = new SampleLogicTests(new Mock<ISampleService>().Object);

            //Act/Assert (Expected NullReferenceException)
	        var classUnderTest = instance.ClassUnderTest;
	    }

		[TestMethod]
		public void SetupInitializesAutoMocker()
		{
            //Arrange
		    var instance = new SampleLogicTests(new Mock<ISampleService>().Object);

		    //Act
		    instance.Setup();
		    var isAutoMockerInitializedAfterSetup = instance.ClassUnderTest != null;

            //Asesrt
		    Assert.IsTrue(isAutoMockerInitializedAfterSetup);
		}
	}
}
