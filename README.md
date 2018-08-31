# Auto Mock Helper

If you use Dependency Injection (DI) in your applications and [Moq](https://github.com/moq/moq) to help with your Unit Tests, this project will help you write unit tests faster.

## Assumptions

This project currently targets .NET Standard 2.0. Support for earlier versions of .NET Standard and even .NET Framework will be considered in the near future.

This project assumes the use of [Visual Studio 2017](https://visualstudio.microsoft.com/vs/) or [VS Code](https://code.visualstudio.com/) along with [Moq](https://github.com/moq/moq) and [Moq's AutoMocker](https://github.com/moq/Moq.AutoMocker).

AutoMockHelper supports MSTest, NUnit, and xUnit.

## Getting Started

- Add the AutoMockHelper Nuget Package (**TODO: NEED LINK**) to your test project.
- Create a test class and inherit from AutoMockContext, using the type of the class under test for the generic parameter:

```c#
public class OrderProcessorTests : AutoMockContext<OrderProcessor>
```

- Never worry about manually creating mocks for all of the dependencies in the tested class again!

## Why do I need this

The most common way to create an instance of a class in a Unit Test is to call the constructor and provide either concrete or mocked instances of the necessary dependencies.

Here's an example of creating an instance of the sample [Order Processor](./AutoMockHelper.Samples.Logic/OrderProcessor/OrderProcessor.cs):

```c#
//Inside the Arrange portion of a Unit Test

//Create mock or test double for each dependency
var mockNotificationService = new Mock<INotificationService>();
mockNotificationService.Setup(x => x.NotifyCustomerOfFailedOrder(testCustomer.CustomerId, NewOrderNumber));
var mockInventoryService = new Mock<InventoryService>();
//...Configure mock objects with other setup/returns/etc.
var mockOrderRepository = new Mock<IOrderRepository>();
var testOrderNumberGeneratorService = new TestOrderNumberGeneratorService();
var logger = new Mock<ILogger>();
logger.Setup(x => x.Info(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}"))));

//Create instance of class to test, passing in all of the required dependency
var orderProcessor = new OrderProcessor(mockNotificationService.Object,
                                        mockInventoryService.Object,
                                        mockOrderRepository.Object,
                                        testOrderNumberGeneratorService,
                                        logger.Object);
```

That's not so bad, right? But what happens when you introduce a new dependency to `OrderProcessor`? **All of the Unit Tests that create an instance of OrderProcessor will fail to compile!**

Another potential issue is the repetition of code involved in creating the necessary mocked dependencies. Sure, you could abstract this logic into a helper method. But there's still a degree of unnecessary duplication.

## Show me a better way

Glad you asked! This project aims to solve the problems mentioned above, combined with the goodness of [Moq's AutoMocker](https://github.com/moq/Moq.AutoMocker).

Here's an example that includes the same `OrderProcessor` class from above, except using `AutoMockContext` as the base class (assuming we're testing `OrderProcessor`):

```c#
//Assuming MSTest, but it also works with NUnit and xUnit

[TestClass]
public class OrderProcessorTests : AutoMockContext<OrderProcessor>
{
    [TestMethod]
    public async Task CreateNewOrderLogsException()
    {
        //Arrange
        this.MockFor<ILogger>()
            .Setup(x => x.Error(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}")), It.IsAny<Exception>()));

        this.MockFor<IOrderRepository>()
            .Setup(x => x.SaveNewOrderAsync(It.IsAny<int>(), It.IsAny<List<OrderItem>>(), It.IsAny<Customer>()))
            .Throws(new ApplicationException("Order Repository is broken!"));

        //Act
        await this.ClassUnderTest.CreateNewOrder(It.IsAny<List<OrderItem>>(), It.IsAny<Customer>());

        //Assert
        this.VerifyAll();
    }
}
```

## What else can AutoMockHelper do

The available members exposed by AutoMockContext are:

- **ClassUnderTest** - an instance of the class specified as the generic type parameter of the test class (i.e. AutoMockContext&lt;YourClassUnderTest&gt;)
- **CreateInstance&lt;TAnyClass&gt;()** - Creates an instance of the specified type, providing the registered test doubles or mocks for all dependencies
- **MockFor&lt;TAnyInterfaceOrClass&gt;()** - Creates, registers, and returns a mock instance of the provided interface or class using the default behavior (Loose - never throws exceptions, returning default values when necessary)
- **StrictMock&lt;TAnyInterfaceOrClass&gt;()** - Creates, registers and returns a mock instance of the provided interface or class using Strict behavior (causes the mock to always throw an exception for invocations that don't have a corresponding setup)
- **Use&lt;&gt;()** (multiple overloads) - Registers and returns an instance of the provided type
- **VerifyAll()** - Calls VerifyAll() for all registered mocks
- **VerifyCallsFor&lt;TMock&gt;()** - Calls VerifyAll() for the specified mock

## Included Resources

The following projects are included:

- **AutoMockHelper.Core** - This includes the AutoMockContext class and is really the only resource you need in order to implement the Auto Mock Helper.
- **AutoMockHelper.Samples.Logic** - This includes some sample logic that is used to demonstrate how AutoMockContext can be applied.
- **AutoMockHelper.Samples.MSTest** - This includes sample unit tests using the MSTest framework.
- **AutoMockHelper.Samples.NUnit** - This includes sample unit tests using the NUnit framework.
- **AutoMockHelper.Samples.xUnit** - This includes sample unit tests using the xUnit framework.

## Notes

- This only supports resolving dependencies that are supplied as Constructor parameters. Although many DI frameworks support Dependency properties, this is not a recommended practice. Also, adding support for Dependency properties would require a reference to the specific DI framework, making this project less generic.

## Next Steps

- Create an automated build, likely using [AppVeyor](https://www.appveyor.com)
- Create a Nuget package and submit to the public Nuget repository
- Add support for .NET Standard 1.x and .NET Framework
- Add improvements and fixes, as needed
