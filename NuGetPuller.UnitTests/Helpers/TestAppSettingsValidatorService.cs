using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NuGetPuller.UnitTests.Helpers;

public class TestAppSettingsValidatorService(ILogger<TestAppSettingsValidatorService> logger, ValidatedTestAppSettings validatedSettings, IHostEnvironment hostEnvironment) : SharedAppSettingsValidatorService<TestAppSettings, ValidatedTestAppSettings>(logger, validatedSettings, hostEnvironment)
{
    protected override void Validate(TestAppSettings options, ValidatedTestAppSettings validatedSettings, List<ValidationResult> validationResults) { }
}
