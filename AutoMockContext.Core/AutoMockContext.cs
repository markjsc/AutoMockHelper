namespace AutoMockHelper.Core
{
	using Moq;
	using Moq.AutoMock;

	public abstract class AutoMockContext<TClassUnderTest>
		where TClassUnderTest : class
	{
		protected AutoMocker _autoMocker;
		private bool _isInstanceCreated;

		public TClassUnderTest ClassUnderTest
		{
			get
			{
				this.EnsureClassUnderTestIsCreated();
				return this._autoMocker.Get<TClassUnderTest>();
			}
		}

		public Mock<TDependency> MockFor<TDependency>()
			where TDependency : class
		{
			return this._autoMocker.GetMock<TDependency>();
		}

		public Mock<TImplementation> Use<TImplementation>(Mock<TImplementation> instance)
			where TImplementation : class
		{
			this._autoMocker.Use(instance);
			return instance;
		}

		public TImplementation Use<TImplementation>(TImplementation instance)
		{
			this._autoMocker.Use(instance);
			return instance;
		}

		public TImplementation Use<TInterface, TImplementation>()
			where TInterface : class
			where TImplementation : class, TInterface
		{
			var instance = this.CreateInstance<TImplementation>();
			this._autoMocker.Use<TInterface>(instance);
			return instance;
		}

		public void VerifyCallsFor<TDependency>()
			where TDependency : class
		{
			this.MockFor<TDependency>().VerifyAll();
		}

		/// <summary>
		/// Call Setup with your test framework's Test Initialize/Setup routine.
		/// </summary>
		public virtual void Setup()
		{
			this._autoMocker = new AutoMocker();
		}

		protected void VerifyAll()
		{
			this._autoMocker.VerifyAll();
		}

		protected void StrictMock<TDependency>()
			where TDependency : class
		{
			this._autoMocker.Use(mockedService: new Mock<TDependency>(MockBehavior.Strict));
		}

		protected void EnsureClassUnderTestIsCreated()
		{
			if (this._isInstanceCreated != true)
			{
				var instance = this.CreateInstance<TClassUnderTest>();
				this._autoMocker.Use(instance);
				this._isInstanceCreated = true;
			}
		}

		protected TClassToCreate CreateInstance<TClassToCreate>()
			where TClassToCreate : class
		{
			return this._autoMocker.CreateInstance<TClassToCreate>();
		}
	}
}