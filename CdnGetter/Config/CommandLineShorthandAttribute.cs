using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CdnGetter.Config
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class CommandLineShorthandAttribute(params string[] switches) : Attribute
    {
        public ImmutableArray<string> Switches { get; } = switches?.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToImmutableArray() ?? [];
    }
}