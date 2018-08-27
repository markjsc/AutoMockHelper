# Auto Mock Helper

If you use Dependency Injection (DI), this project will help you write unit tests faster.

After adding this NuGet Package (**NEED LINK**), simply inherit from AutoMockContext in your unit tests and you'll never have to manually create mocks for all of the dependencies in the tested class again.

## Show Me How
```c#
//TODO: Add lots of stuff
//Example code here
```

## Notes

- This only supports resolving dependencies that are supplied as Constructor parameters. Although many DI frameworks support Dependency properties, this is not a recommended practice. Also, adding support for Dependency properties would require a reference to the specific DI framework, making this project less generic.