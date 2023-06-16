using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

public class AlternateMatcher<TInput, TOutput> : IMatcher<TInput, TOutput>
{
    public ReadOnlyCollection<IMatcher<TInput, TOutput>> Options { get; }
    
    public Func<TOutput>? NoMatchFactory { get; }
    
    public AlternateMatcher(IEnumerable<IMatcher<TInput, TOutput>> options, Func<TOutput>? noMatchFactory = null) => (Options, NoMatchFactory) = (new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput, TOutput>>()), noMatchFactory);
    
    public AlternateMatcher(Func<TOutput> noMatchFactory, params IMatcher<TInput, TOutput>[] options) => (NoMatchFactory, Options) = (noMatchFactory, new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput, TOutput>>()));
    
    public AlternateMatcher(params IMatcher<TInput, TOutput>[] options) => (NoMatchFactory, Options) = (null, new(options?.Where(p => p is not null).ToArray() ?? Array.Empty<IMatcher<TInput, TOutput>>()));
    
    public bool Match(ReadOnlySpan<TInput> span, int startIndex, int endIndex, out int nextIndex)
    {
        if (span.ValidateExtentsNotEmpty(ref startIndex, ref endIndex))
            foreach (IMatcher<TInput, TOutput> matcher in Options)
                if (matcher.Match(span, startIndex, endIndex, out nextIndex))
                    return true;
        nextIndex = startIndex;
        return NoMatchFactory is not null;
    }

    public bool TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [MaybeNullWhen(false)] out TOutput result, out int nextIndex)
    {
        if (span.ValidateExtentsNotEmpty(ref startIndex, ref endIndex))
            foreach (IMatcher<TInput, TOutput> matcher in Options)
                if (matcher.TryParse(span, startIndex, endIndex, out result, out nextIndex))
                    return true;
        nextIndex = startIndex;
        if (NoMatchFactory is null)
        {
            result = default;
            return false;
        }
        result = NoMatchFactory();
        return true;
    }
}
