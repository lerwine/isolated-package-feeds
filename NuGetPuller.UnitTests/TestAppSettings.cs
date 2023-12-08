using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NuGetPuller.UnitTests;

public class TestAppSettings : ISharedAppSettings
{
    public string UpstreamServiceIndex { get; set; } = string.Empty;

    public string? OverrideUpstreamServiceIndex { get; set; }

    public string LocalRepository { get; set; } = string.Empty;

    public string? OverrideLocalRepository { get; set; }

    public string GlobalPackagesFolder { get; set; } = string.Empty;

    public string? OverrideGlobalPackagesFolder { get; set; }

    public ValidatedTestAppSettings Validated => new();

    IValidatedSharedAppSettings ISharedAppSettings.Validated => Validated;
}

public class TestAppSettingsValidatorService(ILogger<TestAppSettingsValidatorService> logger, IHostEnvironment hostEnvironment) : SharedAppSettingsValidatorService<TestAppSettings>(logger, hostEnvironment)
{
    protected override void Validate(TestAppSettings options, List<ValidationResult> validationResults) { }
}