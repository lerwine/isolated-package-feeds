using System.Diagnostics.CodeAnalysis;

namespace CdnGetter.Parsing;

public interface IMatcher<TInput>
{
    bool Match(ReadOnlySpan<TInput> span, int startIndex, int endIndex, out int nextIndex);

    bool TryParse(ReadOnlySpan<TInput> span, int startIndex, int endIndex, [NotNullWhen(true)] out IToken? result, out int nextIndex);
}
