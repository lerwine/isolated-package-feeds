using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

public class AlternateMatcher<TInput> : IMatcher<TInput>
{
    public ReadOnlyCollection<IMatcher<TInput>> Options { get; }
    
    public Func<IToken>? NoMatchFactory { get; }
    
    public AlternateMatcher(IEnumerable<IMatcher<TInput>> options, Func<IToken>? noMatchFactory = null) => (Options, NoMatchFactory) = (new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>()), noMatchFactory);
    
    public AlternateMatcher(Func<IToken> noMatchFactory, params IMatcher<TInput>[] options) => (NoMatchFactory, Options) = (noMatchFactory, new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>()));
    
    public AlternateMatcher(params IMatcher<TInput>[] options) => (NoMatchFactory, Options) = (null, new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput>>()));
    
    public bool Match(ReadOnlySpan<TInput> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsNotEmpty(ref startIndex, ref endIndex))
            foreach (IMatcher<TInput> matcher in Options)
                if (matcher.Match(span, startIndex, endIndex, out nextIndex))
                    return true;
        nextIndex = startIndex;
        return NoMatchFactory is not null;
    }

    public bool TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [NotNullWhen(true)] out IToken? result, out int nextIndex)
    {
        if (span.ValidateExtentsNotEmpty(ref startIndex, ref endIndex))
            foreach (IMatcher<TInput> matcher in Options)
                if (matcher.TryParse(span, startIndex, endIndex, out result, out nextIndex))
                    return true;
        nextIndex = startIndex;
        if (NoMatchFactory is null)
        {
            result = null;
            return false;
        }
        result = NoMatchFactory();
        return true;
    }
}
