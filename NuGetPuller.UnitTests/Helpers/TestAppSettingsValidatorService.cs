using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NuGetPuller.UnitTests.Helpers;

public class TestAppSettingsValidatorService(ILogger<TestAppSettingsValidatorService> logger, IHostEnvironment hostEnvironment) : SharedAppSettingsValidatorService<TestAppSettings>(logger, hostEnvironment)
{
    protected override void Validate(TestAppSettings options, List<ValidationResult> validationResults) { }
}