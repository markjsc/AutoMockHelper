# Auto Mock Helper Changelog

## 1.1.0

### Added

- Added Nuget package tags.

### Fixed

- Eliminated the need to call `base.Setup()` in order to initialize AutoMocker. (Not calling `base.Setup()` resulted in a `NullReferenceException` - this change fixes the problem) ([Issue #1](https://github.com/markjsc/AutoMockHelper/issues/1)).

## 1.0.0

Initial Release
