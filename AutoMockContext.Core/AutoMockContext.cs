namespace AutoMockContext.Core
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.CompilerServices;
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
			var classToTest = this._autoMocker.CreateInstance<TClassToCreate>();

			// This is a hack.  Because of the restrictions of the AutoMocker class that is a nuget package and
			// not able to be modified, we need to do some funny business to populate [Dependency] attributed 
			// properties with instances.  For now, we default this to a new instance of the dependency for each
			// attribute; however in the future it may be appropriate to use Mock<T> instances instead.
			try
			{
				var dependencyProperties = typeof(TClassToCreate)
				                           .GetProperties(bindingAttr: BindingFlags.Instance | BindingFlags.Public)
				                           .Where(prop => prop.IsDefined(attributeType: typeof(DependencyAttribute), inherit: true) && prop.PropertyType.IsClass);
				foreach (var property in dependencyProperties)
				{
					// AutoMocker did not expose Type object parameters, only generic methods, so we have to make a
					// custom generic method via reflection for each type.
					var propertyValue = typeof(AutoMocker)
					                    .GetMethod(name: nameof(this.CreateInstance))
					                    ?.MakeGenericMethod(property.PropertyType)
					                    .Invoke(this._autoMocker, parameters: new object[] { });

					// This may not work for all dependencies, so if an exception occurs here, there may be a need to
					// revisit this logic.
					property.SetValue(classToTest, propertyValue);
				}
			}
			catch (Exception ex)
			{
				Debug.Write(ex);
			}

			return classToTest;
		}
	}
}