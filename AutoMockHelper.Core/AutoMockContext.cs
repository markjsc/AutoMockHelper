namespace AutoMockHelper.Core
{
	using Moq;
	using Moq.AutoMock;

	/// <summary>
	/// Use this as a base class for your unit tests.
	/// </summary>
	/// <typeparam name="TClassUnderTest">Use the type being tested as the generic TClassUnderTest parameter</typeparam>
	public abstract class AutoMockContext<TClassUnderTest>
		where TClassUnderTest : class
	{
	    private AutoMocker _autoMocker;
		private bool _isInstanceCreated;

        /// <summary>
		/// Use ClassUnderTest within the unit test class to access the class that is under test.
		/// </summary>
		public TClassUnderTest ClassUnderTest
		{
			get
			{
				this.EnsureClassUnderTestIsCreated();
				return this.AutoMocker.Get<TClassUnderTest>();
			}
		}

        /// <summary>
		/// Use MockFor() to create or retrieve an instance of the mocked TDependency.
		/// </summary>
		/// <typeparam name="TDependency"></typeparam>
		public Mock<TDependency> MockFor<TDependency>()
			where TDependency : class
		{
			return this.AutoMocker.GetMock<TDependency>();
		}

        /// <summary>
		/// Use this overload of Use() to supply an existing Mock instance of TImplementation.
		/// This is helpful if you've abstracted the creation and configuration of a Mock.
		/// For example, if you've added a method to a helper class to create and configure
		/// a Mock that can be used by multiple unit test classes, this would be helpful.
		/// </summary>
		/// <typeparam name="TImplementation"></typeparam>
		/// <param name="instance"></param>
		public Mock<TImplementation> Use<TImplementation>(Mock<TImplementation> instance)
			where TImplementation : class
		{
			this.AutoMocker.Use(instance);
			return instance;
		}
        
        /// <summary>
		/// Use this overload of Use() to supply a concrete TImplementation.
		/// This is helpful for cases where you've created a test double and
		/// would like to provide it to the test at runtime.
		/// </summary>
		/// <typeparam name="TImplementation"></typeparam>
		/// <param name="instance"></param>
		public TImplementation Use<TImplementation>(TImplementation instance)
		{
			this.AutoMocker.Use(instance);
			return instance;
		}

        /// <summary>
		/// Use this overload of Use() to supply an TInterface type and TImplementation
		/// types, allowing Mock to worry about the details.
		/// This is helpful for cases when you need to test a specific implemtnation
		/// type but don't need to instantiate it before providing it to the test.
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <typeparam name="TImplementation"></typeparam>
		/// <returns></returns>
		public TImplementation Use<TInterface, TImplementation>()
			where TInterface : class
			where TImplementation : class, TInterface
		{
			var instance = this.CreateInstance<TImplementation>();
			this.AutoMocker.Use<TInterface>(instance);
			return instance;
		}

        /// <summary>
		/// Use VerifyCallsFor() to verify that all expectations have been met for the specified TDependency type.
		/// </summary>
		/// <typeparam name="TDependency"></typeparam>
		public void VerifyCallsFor<TDependency>()
			where TDependency : class
		{
			this.MockFor<TDependency>().VerifyAll();
		}

		/// <summary>
		/// Use Setup() to initialize the AutoMocker.
		/// This should be called by your test framework's Test Initialize/Setup routine:
		/// - MSTest: Call Setup() from a method decorated with the [TestInitialize] attribute.
		/// - NUnit: Call Setup() from a method decorated with the [SetUp] attribute.
		/// - xUnit: Call Setup() from the constructor.
		/// </summary>
		public virtual void Setup()
		{
			this._autoMocker = new AutoMocker();
		}

        /// <summary>
		/// Use Cleanup() to reset the AutoMocker.
		/// This isn't needed by all test frameworks, but it's probably a good idea to call it
		/// during your test framework's Cleanup/TearDown method.
		/// Specifically NUnit would not work correctly without using this method, but MSTest and
		/// xUnit do.
		/// - MSTets: Call Cleanup() from a method decorated with the [TestCleanup] attribute.
		/// - NUnit Call Cleanup() from a method decorated with the [TearDown] attribute.
		/// - xUnit: Call Cleanup() in the deconstructor/dispose method.
		/// </summary>
	    public virtual void Cleanup()
	    {
	        this._isInstanceCreated = false;
	    }

	    protected AutoMocker AutoMocker
	    {
	        get
	        {
	            return this._autoMocker ?? (this._autoMocker = new AutoMocker());
	        }
	    }

        /// <summary>
		/// Use VerifyAll() to verify that all expectations for all mocks have been satisfied.
		/// </summary>
		protected void VerifyAll()
		{
			this.AutoMocker.VerifyAll();
		}
        
        /// <summary>
		/// Use StrictMock() to create a mock with behavior of Strict.
		/// This will cause the Mock to throw an error if any member is accessed without first being setup.
		/// </summary>
		/// <typeparam name="TDependency"></typeparam>
		protected void StrictMock<TDependency>()
			where TDependency : class
		{
			this.AutoMocker.Use(mockedService: new Mock<TDependency>(MockBehavior.Strict));
		}

        /// <summary>
		/// Use EnsureClassUnderTestIsCreated() to trigger initializing the class (i.e. calling its constructor)
		/// during a test.
		/// </summary>
		protected void EnsureClassUnderTestIsCreated()
		{
			if (this._isInstanceCreated != true)
			{
				var instance = this.CreateInstance<TClassUnderTest>();
				this.AutoMocker.Use(instance);
				this._isInstanceCreated = true;
			}
		}

        /// <summary>
		/// Use CreateInstance() to allow AutoMocker to create an instance of the specified TClassToCreate.
		/// This will automatically supply any dependencies required by the TClassToCreate.
		/// </summary>
		/// <typeparam name="TClassToCreate"></typeparam>
		protected TClassToCreate CreateInstance<TClassToCreate>()
			where TClassToCreate : class
		{
			return this.AutoMocker.CreateInstance<TClassToCreate>();
		}
	}
}