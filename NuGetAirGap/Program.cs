using Microsoft.Extensions.Hosting;

namespace NuGetAirGap;

class Program
{
    static void Main(string[] args)
    {
        AppHost.Initialize(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, args).Run();
    }
}
