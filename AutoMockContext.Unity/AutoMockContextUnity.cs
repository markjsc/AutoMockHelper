using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AutoMockContext.Core;
using Moq.AutoMock;
using Unity.Attributes;

namespace AutoMockContext.Unity
{
    public class AutoMockContextUnity<TClassUnderTest> : AutoMockContext<TClassUnderTest>
        where TClassUnderTest : class
    {
        protected override void PopulateInjectionFrameworkSpecificItems<TClassToCreate>(TClassToCreate classToCreate)
        {
            // This is a hack.  Because of the restrictions of the AutoMocker class that is a nuget package and
            // not able to be modified, we need to do some funny business to populate [Dependency] attributed 
            // properties with instances.  For now, we default this to a new instance of the dependency for each
            // attribute; however in the future it may be appropriate to use Mock<T> instances instead.
            try
            {
                var dependencyProperties = typeof(TClassToCreate)
                                           .GetProperties(bindingAttr: BindingFlags.Instance | BindingFlags.Public)
                                           .Where(prop => prop.IsDefined(attributeType: typeof(DependencyAttribute), inherit: true));
                foreach (var property in dependencyProperties)
                {
                    // AutoMocker did not expose Type object parameters, only generic methods, so we have to make a
                    // custom generic method via reflection for each type.
                    var propertyValue = typeof(AutoMocker)
                                        .GetMethod(name: nameof(this.CreateInstance), types: new Type[] { })
                                        ?.MakeGenericMethod(property.PropertyType)
                                        .Invoke(this._autoMocker, parameters: new object[] { });

                    // This may not work for all dependencies, so if an exception occurs here, there may be a need to
                    // revisit this logic.
                    property.SetValue(classToCreate, propertyValue);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
