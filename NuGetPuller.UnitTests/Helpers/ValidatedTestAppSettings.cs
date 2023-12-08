using Microsoft.Extensions.Options;

namespace NuGetPuller.UnitTests.Helpers;

public class ValidatedTestAppSettings(IOptions<TestAppSettings> options) : ValidatedSharedAppSettings<TestAppSettings>(options.Value)
{
}
