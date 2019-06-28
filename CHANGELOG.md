# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.0.0] - YYYY-MM-DD

### Added
- Set up CI with Azure Pipelines.
- New property `LabeledTickBar.ValueNumericFormat`.
- New `TickLabelNumericFormat` property in range sliders.
- New handy `NullableInt64ToTimeSpanConverter`, `SolidColorBrushToColorConverter`.
- New `ZoomBar.ThumbSize` property. `StartThumb` and `EndThumb` size now not fixed to 10, but can be customized.
- Add `LocalizationResource.resx` to make library localizable.

### Changed
- Migrate from `Blend SDK` to `Microsoft.Xaml.Behaviors`.
- `VisualTest1` project replaced by more advanced `Samples` project.
- Change NeutralResourcesLanguageAttribute to `en-US` from `ru`.
- Translate all russian comments (Fixes #7).
- Remove WPF version from names, comments, properties, etc. I.e. `WPF4` now comes to `WPF`.

### Deprecated
- `GridSplitter` marked `Obsolete` (Fixes #15).

### Removed
- ???.

### Fixed
- Improve `DateTimeCollection` convertion from and to string.
- Fix bug in `OnThumbDragStarted` in range sliders (appear if `AutoToolTipPlacement` is `null`).
- Some `ZoomBar`'s properties category changed from `Brushes` to `Brush`.

### Security
- ???.


## [1.0.3-abandoned] - Never released
Exists only as artifact of migration to GitHub


[Unreleased]: https://github.com/SaneDevelopment/WPF-Controls/compare/v1.0.3-abandoned...HEAD
[1.0.3-abandoned]: https://github.com/SaneDevelopment/WPF-Controls/releases/tag/v1.0.3-abandoned
