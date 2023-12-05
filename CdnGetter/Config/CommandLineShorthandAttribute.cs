using System.Collections.Immutable;

namespace CdnGetter.Config
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class CommandLineShorthandAttribute(params string[] switches) : Attribute
    {
        public ImmutableArray<string> Switches { get; } = switches?.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToImmutableArray() ?? [];
    }
}