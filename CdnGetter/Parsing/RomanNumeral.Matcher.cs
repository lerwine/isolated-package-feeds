using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CdnGetter.Parsing;

#pragma warning disable CA2231
public readonly partial struct RomanNumeral
#pragma warning restore CA2231
{
    internal const char ROMAN_NUM_1000_UC = 'M';
    internal const char ROMAN_NUM_1000_LC = 'm';
    internal const ushort ROMAN_NUM_MMM = 3000;
    internal const string ROMAN_NUM_3000 = "MMM";
    internal const ushort ROMAN_NUM_MM = 2000;
    internal const string ROMAN_NUM_2000 = "MM";
    internal const ushort ROMAN_NUM_M = 1000;
    internal const string ROMAN_NUM_1000 = "M";
    internal const ushort ROMAN_NUM_CM = 900;
    internal const string ROMAN_NUM_900 = "CM";
    internal const ushort ROMAN_NUM_DCCC = 800;
    internal const string ROMAN_NUM_800 = "DCCC";
    internal const ushort ROMAN_NUM_DCC = 700;
    internal const string ROMAN_NUM_700 = "DCC";
    internal const ushort ROMAN_NUM_DC = 600;
    internal const string ROMAN_NUM_600 = "DC";
    internal const char ROMAN_NUM_500_UC = 'D';
    internal const char ROMAN_NUM_500_LC = 'd';
    internal const ushort ROMAN_NUM_D = 500;
    internal const string ROMAN_NUM_500 = "D";
    internal const ushort ROMAN_NUM_CD = 400;
    internal const string ROMAN_NUM_400 = "CD";
    internal const ushort ROMAN_NUM_CCC = 300;
    internal const string ROMAN_NUM_300 = "CCC";
    internal const ushort ROMAN_NUM_CC = 200;
    internal const string ROMAN_NUM_200 = "CC";
    internal const char ROMAN_NUM_100_UC = 'C';
    internal const char ROMAN_NUM_100_LC = 'c';
    internal const ushort ROMAN_NUM_C = 100;
    internal const string ROMAN_NUM_100 = "C";
    internal const ushort ROMAN_NUM_XC = 90;
    internal const string ROMAN_NUM_90 = "XC";
    internal const ushort ROMAN_NUM_LXXX = 80;
    internal const string ROMAN_NUM_80 = "LXXX";
    internal const ushort ROMAN_NUM_LXX = 70;
    internal const string ROMAN_NUM_70 = "LXX";
    internal const ushort ROMAN_NUM_LX = 60;
    internal const string ROMAN_NUM_60 = "LX";
    internal const char ROMAN_NUM_50_UC = 'L';
    internal const char ROMAN_NUM_50_LC = 'l';
    internal const ushort ROMAN_NUM_L = 50;
    internal const string ROMAN_NUM_50 = "L";
    internal const ushort ROMAN_NUM_XL = 40;
    internal const string ROMAN_NUM_40 = "XL";
    internal const ushort ROMAN_NUM_XXX = 30;
    internal const string ROMAN_NUM_30 = "XXX";
    internal const ushort ROMAN_NUM_XX = 20;
    internal const string ROMAN_NUM_20 = "XX";
    internal const char ROMAN_NUM_10_UC = 'X';
    internal const char ROMAN_NUM_10_LC = 'x';
    internal const ushort ROMAN_NUM_X = 10;
    internal const string ROMAN_NUM_10 = "X";
    internal const ushort ROMAN_NUM_IX = 9;
    internal const string ROMAN_NUM_9 = "IX";
    internal const ushort ROMAN_NUM_VIII = 8;
    internal const string ROMAN_NUM_8 = "VIII";
    internal const ushort ROMAN_NUM_VII = 7;
    internal const string ROMAN_NUM_7 = "VII";
    internal const ushort ROMAN_NUM_VI = 6;
    internal const string ROMAN_NUM_6 = "VI";
    internal const char ROMAN_NUM_5_UC = 'V';
    internal const char ROMAN_NUM_5_LC = 'v';
    internal const ushort ROMAN_NUM_V = 5;
    internal const string ROMAN_NUM_5 = "V";
    internal const ushort ROMAN_NUM_IV = 4;
    internal const string ROMAN_NUM_4 = "IV";
    internal const ushort ROMAN_NUM_III = 3;
    internal const string ROMAN_NUM_3 = "III";
    internal const ushort ROMAN_NUM_II = 2;
    internal const string ROMAN_NUM_2 = "II";
    internal const char ROMAN_NUM_1_UC = 'I';
    internal const char ROMAN_NUM_1_LC = 'i';
    internal const ushort ROMAN_NUM_I = 1;
    internal const string ROMAN_NUM_1 = "I";
    internal const int MAX_STRING_LENGTH = 15;

    /// <summary>
    /// Gets the index after the last character of the current roman numeral which starts with <c>'I'</c>.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'I'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <returns>The next index after the last roman numeral character.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'I'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static int MoveFromI(ReadOnlySpan<char> source, int startIndex, int endIndex) => (++startIndex < endIndex) ? source[startIndex] switch
    {
        ROMAN_NUM_10_UC or ROMAN_NUM_10_LC or ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => startIndex + 1,
        ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => startIndex + 1,
            _ => startIndex
        } : startIndex,
        _ => startIndex
    } : startIndex;
    
    /// <summary>
    /// Gets the index after the last character of the current roman numeral which starts with <c>'V'</c>.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'V'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <returns>The next index after the last roman numeral character.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'V'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static int MoveFromV(ReadOnlySpan<char> source, int startIndex, int endIndex) => (++startIndex < endIndex) ? source[startIndex] switch
    {
        ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => (++startIndex < endIndex) ? source[startIndex] switch
            {
                ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => startIndex + 1,
                _ => startIndex
            } : startIndex,
            _ => startIndex
        } : startIndex,
        _ => startIndex
    } : startIndex;
    
    /// <summary>
    /// Gets the index after the last character of the current roman numeral which starts with <c>'X'</c>.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'X'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <returns>The next index after the last roman numeral character.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'X'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static int MoveFromX(ReadOnlySpan<char> source, int startIndex, int endIndex) => (++startIndex < endIndex) ? source[startIndex] switch
    {
        ROMAN_NUM_100_UC or ROMAN_NUM_100_LC or ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
            _ => startIndex
        } : startIndex,
        ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => (++startIndex < endIndex) ? source[startIndex] switch
            {
                ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
                ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
                _ => startIndex
            } : startIndex,
            ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
            _ => startIndex
        } : startIndex,
        ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
        ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
        _ => startIndex
    } : startIndex;
    
    /// <summary>
    /// Gets the index after the last character of the current roman numeral which starts with <c>'L'</c>.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'L'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <returns>The next index after the last roman numeral character.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'L'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static int MoveFromL(ReadOnlySpan<char> source, int startIndex, int endIndex) => (++startIndex < endIndex) ? source[startIndex] switch
    {
        ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => (++startIndex < endIndex) ? source[startIndex] switch
            {
                ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => (++startIndex < endIndex) ? source[startIndex] switch
                {
                    ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
                    ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
                    _ => startIndex
                } : startIndex,
                ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
                ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
                _ => startIndex
            } : startIndex,
            ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
            _ => startIndex
        } : startIndex,
        ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
        ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
        _ => startIndex
    } : startIndex;
    
    /// <summary>
    /// Gets the index after the last character of the current roman numeral which starts with <c>'C'</c>.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'C'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <returns>The next index after the last roman numeral character.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'C'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static int MoveFromC(ReadOnlySpan<char> source, int startIndex, int endIndex) => (++startIndex < endIndex) ? source[startIndex] switch
    {
        ROMAN_NUM_1000_UC or ROMAN_NUM_1000_LC or ROMAN_NUM_500_UC or ROMAN_NUM_500_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
            ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
            _ => startIndex
        } : startIndex,
        ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => (++startIndex < endIndex) ? source[startIndex] switch
            {
                ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
                ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
                ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
                ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
                _ => startIndex
            } : startIndex,
            ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
            ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
            _ => startIndex
        } : startIndex,
        ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
        ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
        ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
        ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
        _ => startIndex
    } : startIndex;
    
    /// <summary>
    /// Gets the index after the last character of the current roman numeral which starts with <c>'D'</c>.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'D'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <returns>The next index after the last roman numeral character.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'D'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static int MoveFromD(ReadOnlySpan<char> source, int startIndex, int endIndex) => (++startIndex < endIndex) ? source[startIndex] switch
    {
        ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => (++startIndex < endIndex) ? source[startIndex] switch
            {
                ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => (++startIndex < endIndex) ? source[startIndex] switch
                {
                    ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
                    ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
                    ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
                    ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
                    _ => startIndex
                } : startIndex,
                ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
                ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
                ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
                ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
                _ => startIndex
            } : startIndex,
            ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
            ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
            _ => startIndex
        } : startIndex,
        ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
        ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
        ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
        ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
        _ => startIndex
    } : startIndex;
    
    /// <summary>
    /// Gets the index after the last character of the current roman numeral which starts with <c>'M'</c>.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'M'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <returns>The next index after the last roman numeral character.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'M'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static int MoveFromM(ReadOnlySpan<char> source, int startIndex, int endIndex) => (++startIndex < endIndex) ? source[startIndex] switch
    {
        ROMAN_NUM_1000_UC or ROMAN_NUM_1000_LC => (++startIndex < endIndex) ? source[startIndex] switch
        {
            ROMAN_NUM_1000_UC or ROMAN_NUM_1000_LC => (++startIndex < endIndex) ? source[startIndex] switch
            {
                // ROMAN_NUM_1000_UC or ROMAN_NUM_1000_LC => (++startIndex < endIndex) ? source[startIndex] switch
                // {
                //     ROMAN_NUM_500_UC or ROMAN_NUM_500_LC => MoveFromD(source, startIndex, endIndex),
                //     ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => MoveFromC(source, startIndex, endIndex),
                //     ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
                //     ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
                //     ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
                //     ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
                //     _ => startIndex
                // } : startIndex,
                ROMAN_NUM_500_UC or ROMAN_NUM_500_LC => MoveFromD(source, startIndex, endIndex),
                ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => MoveFromC(source, startIndex, endIndex),
                ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
                ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
                ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
                ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
                _ => startIndex
            } : startIndex,
            ROMAN_NUM_500_UC or ROMAN_NUM_500_LC => MoveFromD(source, startIndex, endIndex),
            ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => MoveFromC(source, startIndex, endIndex),
            ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
            ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
            ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
            ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
            _ => startIndex
        } : startIndex,
        ROMAN_NUM_500_UC or ROMAN_NUM_500_LC => MoveFromD(source, startIndex, endIndex),
        ROMAN_NUM_100_UC or ROMAN_NUM_100_LC => MoveFromC(source, startIndex, endIndex),
        ROMAN_NUM_50_UC or ROMAN_NUM_50_LC => MoveFromL(source, startIndex, endIndex),
        ROMAN_NUM_10_UC or ROMAN_NUM_10_LC => MoveFromX(source, startIndex, endIndex),
        ROMAN_NUM_5_UC or ROMAN_NUM_5_LC => MoveFromV(source, startIndex, endIndex),
        ROMAN_NUM_1_UC or ROMAN_NUM_1_LC => MoveFromI(source, startIndex, endIndex),
        _ => startIndex
    } : startIndex;

    /// <summary>
    /// Gets the value of the current roman numeral starting with the <c>'I'</c> character.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'I'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <param name="nextIndex">Returns the next index after the last roman numeral character.</param>
    /// <returns>The numeric value of the parsed roman numeral.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'I'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static ushort ParseFromI(ReadOnlySpan<char> source, int startIndex, int endIndex, out int nextIndex)
    {
        if ((nextIndex = startIndex + 1) < endIndex)
            switch (source[nextIndex])
            {
                case ROMAN_NUM_10_UC:
                case ROMAN_NUM_10_LC:
                    nextIndex++;
                    return ROMAN_NUM_IX;
                case ROMAN_NUM_5_UC:
                case ROMAN_NUM_5_LC:
                    nextIndex++;
                    return ROMAN_NUM_IV;
                case ROMAN_NUM_1_UC:
                case ROMAN_NUM_1_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                nextIndex++;
                                return ROMAN_NUM_III;
                        }
                    return ROMAN_NUM_II;
            }
        return ROMAN_NUM_I;
    }

    /// <summary>
    /// Gets the value of the current roman numeral starting with the <c>'V'</c> character.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'V'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <param name="nextIndex">Returns the next index after the last roman numeral character.</param>
    /// <returns>The numeric value of the parsed roman numeral.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'V'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static ushort ParseFromV(ReadOnlySpan<char> source, int startIndex, int endIndex, out int nextIndex)
    {
        if ((nextIndex = startIndex + 1) < endIndex)
            switch (source[nextIndex])
            {
                case ROMAN_NUM_1_UC:
                case ROMAN_NUM_1_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                if (++nextIndex < endIndex)
                                    switch (source[nextIndex])
                                    {
                                        case ROMAN_NUM_1_UC:
                                        case ROMAN_NUM_1_LC:
                                            nextIndex++;
                                            return ROMAN_NUM_VIII;
                                    }
                                return ROMAN_NUM_VII;
                        }
                    return ROMAN_NUM_VI;
            }
        return ROMAN_NUM_V;
    }

    /// <summary>
    /// Gets the value of the current roman numeral starting with the <c>'X'</c> character.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'X'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <param name="nextIndex">Returns the next index after the last roman numeral character.</param>
    /// <returns>The numeric value of the parsed roman numeral.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'X'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static ushort ParseFromX(ReadOnlySpan<char> source, int startIndex, int endIndex, out int nextIndex)
    {
        if ((nextIndex = startIndex + 1) < endIndex)
            switch (source[nextIndex])
            {
                case ROMAN_NUM_100_UC:
                case ROMAN_NUM_100_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_XC + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_XC + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_XC;
                case ROMAN_NUM_50_UC:
                case ROMAN_NUM_50_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_XL + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_XL + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_XL;
                case ROMAN_NUM_10_UC:
                case ROMAN_NUM_10_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_10_UC:
                            case ROMAN_NUM_10_LC:
                                if (++nextIndex < endIndex)
                                    switch (source[nextIndex])
                                    {
                                        case ROMAN_NUM_5_UC:
                                        case ROMAN_NUM_5_LC:
                                            return (ushort)(ROMAN_NUM_XXX + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_1_UC:
                                        case ROMAN_NUM_1_LC:
                                            return (ushort)(ROMAN_NUM_XXX + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                                    }
                                return ROMAN_NUM_XXX;
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_XX + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_XX + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_XX;
                case ROMAN_NUM_5_UC:
                case ROMAN_NUM_5_LC:
                    return (ushort)(ROMAN_NUM_X + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_1_UC:
                case ROMAN_NUM_1_LC:
                    return (ushort)(ROMAN_NUM_X + ParseFromI(source, nextIndex, endIndex, out nextIndex));
            }
        return ROMAN_NUM_X;
    }

    /// <summary>
    /// Gets the value of the current roman numeral starting with the <c>'L'</c> character.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'L'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <param name="nextIndex">Returns the next index after the last roman numeral character.</param>
    /// <returns>The numeric value of the parsed roman numeral.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'L'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static ushort ParseFromL(ReadOnlySpan<char> source, int startIndex, int endIndex, out int nextIndex)
    {
        if ((nextIndex = startIndex + 1) < endIndex)
            switch (source[nextIndex])
            {
                case ROMAN_NUM_10_UC:
                case ROMAN_NUM_10_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_10_UC:
                            case ROMAN_NUM_10_LC:
                                if (++nextIndex < endIndex)
                                    switch (source[nextIndex])
                                    {
                                        case ROMAN_NUM_10_UC:
                                        case ROMAN_NUM_10_LC:
                                            if (++nextIndex < endIndex)
                                                switch (source[nextIndex])
                                                {
                                                    case ROMAN_NUM_5_UC:
                                                    case ROMAN_NUM_5_LC:
                                                        return (ushort)(ROMAN_NUM_LXXX + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                                                    case ROMAN_NUM_1_UC:
                                                    case ROMAN_NUM_1_LC:
                                                        return (ushort)(ROMAN_NUM_LXXX + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                                                }
                                            return ROMAN_NUM_LXXX;
                                        case ROMAN_NUM_5_UC:
                                        case ROMAN_NUM_5_LC:
                                            return (ushort)(ROMAN_NUM_LXX + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_1_UC:
                                        case ROMAN_NUM_1_LC:
                                            return (ushort)(ROMAN_NUM_LXX + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                                    }
                                return ROMAN_NUM_LXX;
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_LX + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_LX + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_LX;
                case ROMAN_NUM_5_UC:
                case ROMAN_NUM_5_LC:
                    return (ushort)(ROMAN_NUM_L + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_1_UC:
                case ROMAN_NUM_1_LC:
                    return (ushort)(ROMAN_NUM_L + ParseFromI(source, nextIndex, endIndex, out nextIndex));
            }
        return ROMAN_NUM_L;
    }

    /// <summary>
    /// Gets the value of the current roman numeral starting with the <c>'C'</c> character.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'C'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <param name="nextIndex">Returns the next index after the last roman numeral character.</param>
    /// <returns>The numeric value of the parsed roman numeral.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'C'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static ushort ParseFromC(ReadOnlySpan<char> source, int startIndex, int endIndex, out int nextIndex)
    {
        if ((nextIndex = startIndex + 1) < endIndex)
            switch (source[nextIndex])
            {
                case ROMAN_NUM_1000_UC:
                case ROMAN_NUM_1000_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_50_UC:
                            case ROMAN_NUM_50_LC:
                                return (ushort)(ROMAN_NUM_CM + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_10_UC:
                            case ROMAN_NUM_10_LC:
                                return (ushort)(ROMAN_NUM_CM + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_CM + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_CM + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_CM;
                case ROMAN_NUM_500_UC:
                case ROMAN_NUM_500_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_50_UC:
                            case ROMAN_NUM_50_LC:
                                return (ushort)(ROMAN_NUM_CD + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_10_UC:
                            case ROMAN_NUM_10_LC:
                                return (ushort)(ROMAN_NUM_CD + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_CD + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_CD + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_CD;
                case ROMAN_NUM_100_UC:
                case ROMAN_NUM_100_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_100_UC:
                            case ROMAN_NUM_100_LC:
                                if (++nextIndex < endIndex)
                                    switch (source[nextIndex])
                                    {
                                        case ROMAN_NUM_50_UC:
                                        case ROMAN_NUM_50_LC:
                                            return (ushort)(ROMAN_NUM_CCC + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_10_UC:
                                        case ROMAN_NUM_10_LC:
                                            return (ushort)(ROMAN_NUM_CCC + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_5_UC:
                                        case ROMAN_NUM_5_LC:
                                            return (ushort)(ROMAN_NUM_CCC + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_1_UC:
                                        case ROMAN_NUM_1_LC:
                                            return (ushort)(ROMAN_NUM_CCC + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                                    }
                                return ROMAN_NUM_CCC;
                            case ROMAN_NUM_50_UC:
                            case ROMAN_NUM_50_LC:
                                return (ushort)(ROMAN_NUM_CC + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_10_UC:
                            case ROMAN_NUM_10_LC:
                                return (ushort)(ROMAN_NUM_CC + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_CC + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_CC + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_CC;
                case ROMAN_NUM_50_UC:
                case ROMAN_NUM_50_LC:
                    return (ushort)(ROMAN_NUM_C + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_10_UC:
                case ROMAN_NUM_10_LC:
                    return (ushort)(ROMAN_NUM_C + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_5_UC:
                case ROMAN_NUM_5_LC:
                    return (ushort)(ROMAN_NUM_C + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_1_UC:
                case ROMAN_NUM_1_LC:
                    return (ushort)(ROMAN_NUM_C + ParseFromI(source, nextIndex, endIndex, out nextIndex));
            }
        return ROMAN_NUM_C;
    }

    /// <summary>
    /// Gets the value of the current roman numeral starting with the <c>'D'</c> character.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'D'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <param name="nextIndex">Returns the next index after the last roman numeral character.</param>
    /// <returns>The numeric value of the parsed roman numeral.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'D'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static ushort ParseFromD(ReadOnlySpan<char> source, int startIndex, int endIndex, out int nextIndex)
    {
        if ((nextIndex = startIndex + 1) < endIndex)
            switch (source[nextIndex])
            {
                case ROMAN_NUM_100_UC:
                case ROMAN_NUM_100_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_100_UC:
                            case ROMAN_NUM_100_LC:
                                if (++nextIndex < endIndex)
                                    switch (source[nextIndex])
                                    {
                                        case ROMAN_NUM_100_UC:
                                        case ROMAN_NUM_100_LC:
                                            if (++nextIndex < endIndex)
                                                switch (source[nextIndex])
                                                {
                                                    case ROMAN_NUM_50_UC:
                                                    case ROMAN_NUM_50_LC:
                                                        return (ushort)(ROMAN_NUM_DCCC + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                                                    case ROMAN_NUM_10_UC:
                                                    case ROMAN_NUM_10_LC:
                                                        return (ushort)(ROMAN_NUM_DCCC + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                                                    case ROMAN_NUM_5_UC:
                                                    case ROMAN_NUM_5_LC:
                                                        return (ushort)(ROMAN_NUM_DCCC + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                                                    case ROMAN_NUM_1_UC:
                                                    case ROMAN_NUM_1_LC:
                                                        return (ushort)(ROMAN_NUM_DCCC + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                                                }
                                            return ROMAN_NUM_DCCC;
                                        case ROMAN_NUM_50_UC:
                                        case ROMAN_NUM_50_LC:
                                            return (ushort)(ROMAN_NUM_DCC + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_10_UC:
                                        case ROMAN_NUM_10_LC:
                                            return (ushort)(ROMAN_NUM_DCC + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_5_UC:
                                        case ROMAN_NUM_5_LC:
                                            return (ushort)(ROMAN_NUM_DCC + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_1_UC:
                                        case ROMAN_NUM_1_LC:
                                            return (ushort)(ROMAN_NUM_DCC + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                                    }
                                return ROMAN_NUM_DCC;
                            case ROMAN_NUM_50_UC:
                            case ROMAN_NUM_50_LC:
                                return (ushort)(ROMAN_NUM_DC + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_10_UC:
                            case ROMAN_NUM_10_LC:
                                return (ushort)(ROMAN_NUM_DC + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_DC + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_DC + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_DC;
                case ROMAN_NUM_50_UC:
                case ROMAN_NUM_50_LC:
                    return (ushort)(ROMAN_NUM_D + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_10_UC:
                case ROMAN_NUM_10_LC:
                    return (ushort)(ROMAN_NUM_D + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_5_UC:
                case ROMAN_NUM_5_LC:
                    return (ushort)(ROMAN_NUM_D + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_1_UC:
                case ROMAN_NUM_1_LC:
                    return (ushort)(ROMAN_NUM_D + ParseFromI(source, nextIndex, endIndex, out nextIndex));
            }
        return ROMAN_NUM_D;
    }

    /// <summary>
    /// Gets the value of the current roman numeral starting with the <c>'M'</c> character.
    /// </summary>
    /// <param name="source">The characters being parsed.</param>
    /// <param name="startIndex">The index of the current <c>'M'</c> character.</param>
    /// <param name="endIndex">The exclusive end index of the parsing character range.</param>
    /// <param name="nextIndex">Returns the next index after the last roman numeral character.</param>
    /// <returns>The numeric value of the parsed roman numeral.</returns>
    /// <remarks>This only assumes that the character at the <paramref name="startIndex" /> of the <paramref name="source" /> is the <c>'M'</c> character, and only checks following characters.
    /// <para>This will stop parsing when it reaches the first character that's not part of a valid roman numeral, irregardless of what follows.</para></remarks>
    public static ushort ParseFromM(ReadOnlySpan<char> source, int startIndex, int endIndex, out int nextIndex)
    {
        if ((nextIndex = startIndex + 1) < endIndex)
            switch (source[nextIndex])
            {
                case ROMAN_NUM_1000_UC:
                case ROMAN_NUM_1000_LC:
                    if (++nextIndex < endIndex)
                        switch (source[nextIndex])
                        {
                            case ROMAN_NUM_1000_UC:
                            case ROMAN_NUM_1000_LC:
                                if (++nextIndex < endIndex)
                                    switch (source[nextIndex])
                                    {
                                        case ROMAN_NUM_500_UC:
                                        case ROMAN_NUM_500_LC:
                                            return (ushort)(ROMAN_NUM_MMM + ParseFromD(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_100_UC:
                                        case ROMAN_NUM_100_LC:
                                            return (ushort)(ROMAN_NUM_MMM + ParseFromC(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_50_UC:
                                        case ROMAN_NUM_50_LC:
                                            return (ushort)(ROMAN_NUM_MMM + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_10_UC:
                                        case ROMAN_NUM_10_LC:
                                            return (ushort)(ROMAN_NUM_MMM + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_5_UC:
                                        case ROMAN_NUM_5_LC:
                                            return (ushort)(ROMAN_NUM_MMM + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                                        case ROMAN_NUM_1_UC:
                                        case ROMAN_NUM_1_LC:
                                            return (ushort)(ROMAN_NUM_MMM + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                                    }
                                return ROMAN_NUM_MMM;
                            case ROMAN_NUM_500_UC:
                            case ROMAN_NUM_500_LC:
                                return (ushort)(ROMAN_NUM_MM + ParseFromD(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_100_UC:
                            case ROMAN_NUM_100_LC:
                                return (ushort)(ROMAN_NUM_MM + ParseFromC(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_50_UC:
                            case ROMAN_NUM_50_LC:
                                return (ushort)(ROMAN_NUM_MM + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_10_UC:
                            case ROMAN_NUM_10_LC:
                                return (ushort)(ROMAN_NUM_MM + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_5_UC:
                            case ROMAN_NUM_5_LC:
                                return (ushort)(ROMAN_NUM_MM + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                            case ROMAN_NUM_1_UC:
                            case ROMAN_NUM_1_LC:
                                return (ushort)(ROMAN_NUM_MM + ParseFromI(source, nextIndex, endIndex, out nextIndex));
                        }
                    return ROMAN_NUM_MM;
                case ROMAN_NUM_500_UC:
                case ROMAN_NUM_500_LC:
                    return (ushort)(ROMAN_NUM_M + ParseFromD(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_100_UC:
                case ROMAN_NUM_100_LC:
                    return (ushort)(ROMAN_NUM_M + ParseFromC(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_50_UC:
                case ROMAN_NUM_50_LC:
                    return (ushort)(ROMAN_NUM_M + ParseFromL(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_10_UC:
                case ROMAN_NUM_10_LC:
                    return (ushort)(ROMAN_NUM_M + ParseFromX(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_5_UC:
                case ROMAN_NUM_5_LC:
                    return (ushort)(ROMAN_NUM_M + ParseFromV(source, nextIndex, endIndex, out nextIndex));
                case ROMAN_NUM_1_UC:
                case ROMAN_NUM_1_LC:
                    return (ushort)(ROMAN_NUM_M + ParseFromI(source, nextIndex, endIndex, out nextIndex));
            }
        return ROMAN_NUM_M;
    }

    public static string? ToRomanNumeral(ushort value)
    {
        if (value > ParsingExtensionMethods.ROMAN_NUMERAL_MAX_VALUE)
            throw new ArgumentOutOfRangeException(nameof(value));
        switch (value)
        {
            case 0: return null;
            case ROMAN_NUM_I: return ROMAN_NUM_1;
            case ROMAN_NUM_II: return ROMAN_NUM_2;
            case ROMAN_NUM_III: return ROMAN_NUM_3;
            case ROMAN_NUM_IV: return ROMAN_NUM_4;
            case ROMAN_NUM_V: return ROMAN_NUM_5;
            case ROMAN_NUM_VI: return ROMAN_NUM_6;
            case ROMAN_NUM_VII: return ROMAN_NUM_7;
            case ROMAN_NUM_VIII: return ROMAN_NUM_8;
            case ROMAN_NUM_IX: return ROMAN_NUM_9;
            case ROMAN_NUM_X: return ROMAN_NUM_10;
            case ROMAN_NUM_XX: return ROMAN_NUM_20;
            case ROMAN_NUM_XXX: return ROMAN_NUM_30;
            case ROMAN_NUM_XL: return ROMAN_NUM_40;
            case ROMAN_NUM_L: return ROMAN_NUM_50;
            case ROMAN_NUM_LX: return ROMAN_NUM_60;
            case ROMAN_NUM_LXX: return ROMAN_NUM_70;
            case ROMAN_NUM_LXXX: return ROMAN_NUM_80;
            case ROMAN_NUM_XC: return ROMAN_NUM_90;
            case ROMAN_NUM_C: return ROMAN_NUM_100;
            case ROMAN_NUM_CC: return ROMAN_NUM_200;
            case ROMAN_NUM_CCC: return ROMAN_NUM_300;
            case ROMAN_NUM_CD: return ROMAN_NUM_400;
            case ROMAN_NUM_D: return ROMAN_NUM_500;
            case ROMAN_NUM_DC: return ROMAN_NUM_600;
            case ROMAN_NUM_DCC: return ROMAN_NUM_700;
            case ROMAN_NUM_DCCC: return ROMAN_NUM_800;
            case ROMAN_NUM_CM: return ROMAN_NUM_900;
            case ROMAN_NUM_M: return ROMAN_NUM_1000;
            case ROMAN_NUM_MM: return ROMAN_NUM_2000;
            case ROMAN_NUM_MMM: return ROMAN_NUM_3000;
        }
        
        StringBuilder sb = new();
        while (value > ROMAN_NUM_M)
        {
            value -= ROMAN_NUM_M;
            sb.Append(ROMAN_NUM_1000_UC);
        }
        if (value >= ROMAN_NUM_CM)
        {
            value -= ROMAN_NUM_CM;
            sb.Append(ROMAN_NUM_900);
        }
        else if (value >= ROMAN_NUM_D)
        {
            value -= ROMAN_NUM_D;
            sb.Append(ROMAN_NUM_500_UC);
            while (value >= ROMAN_NUM_C)
            {
                value -= ROMAN_NUM_C;
                sb.Append(ROMAN_NUM_100_UC);
            }
        }
        else if (value >= ROMAN_NUM_CD)
        {
            value -= ROMAN_NUM_CD;
            sb.Append(ROMAN_NUM_400);
        }
        else
            while (value >= ROMAN_NUM_C)
            {
                value -= ROMAN_NUM_C;
                sb.Append(ROMAN_NUM_100_UC);
            }
        if (value >= ROMAN_NUM_XC)
        {
            value -= ROMAN_NUM_XC;
            sb.Append(ROMAN_NUM_90);
        }
        else if (value >= ROMAN_NUM_L)
        {
            value -= ROMAN_NUM_L;
            sb.Append(ROMAN_NUM_50_UC);
            while (value >= ROMAN_NUM_X)
            {
                value -= 10;
                sb.Append(ROMAN_NUM_10_UC);
            }
        }
        else if (value >= ROMAN_NUM_XL)
        {
            value -= ROMAN_NUM_XL;
            sb.Append(ROMAN_NUM_40);
        }
        else
            while (value >= ROMAN_NUM_X)
            {
                value -= ROMAN_NUM_X;
                sb.Append(ROMAN_NUM_10_UC);
            }
        if (value == ROMAN_NUM_IX)
            return sb.Append(ROMAN_NUM_9).ToString();
        if (value >= ROMAN_NUM_V)
        {
            value -= ROMAN_NUM_V;
            sb.Append(ROMAN_NUM_5_UC);
            while (value > 0)
            {
                value--;
                sb.Append(ROMAN_NUM_1_UC);
            }
        }
        else
        {
            if (value == ROMAN_NUM_IV)
                return sb.Append(ROMAN_NUM_4).ToString();
            while (value > 0)
            {
                value--;
                sb.Append(ROMAN_NUM_1_UC);
            }
        }
        return sb.ToString();
    }
    
    public static bool TryParse(ReadOnlySpan<char> span, int startIndex, int endIndex, out RomanNumeral result, out int nextIndex)
    {
        if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
        {
            nextIndex = startIndex;
            result = default;
            return false;
        }
        switch (span[startIndex])
        {
            case ROMAN_NUM_1000_UC:
            case ROMAN_NUM_1000_LC:
                nextIndex = MoveFromM(span, startIndex, endIndex);
                break;
            case ROMAN_NUM_500_UC:
            case ROMAN_NUM_500_LC:
                nextIndex = MoveFromD(span, startIndex, endIndex);
                break;
            case ROMAN_NUM_100_UC:
            case ROMAN_NUM_100_LC:
                nextIndex = MoveFromC(span, startIndex, endIndex);
                break;
            case ROMAN_NUM_50_UC:
            case ROMAN_NUM_50_LC:
                nextIndex = MoveFromL(span, startIndex, endIndex);
                break;
            case ROMAN_NUM_10_UC:
            case ROMAN_NUM_10_LC:
                nextIndex = MoveFromX(span, startIndex, endIndex);
                break;
            case ROMAN_NUM_5_UC:
            case ROMAN_NUM_5_LC:
                nextIndex = MoveFromV(span, startIndex, endIndex);
                break;
            case ROMAN_NUM_1_UC:
            case ROMAN_NUM_1_LC:
                nextIndex = MoveFromI(span, startIndex, endIndex);
                break;
            default:
                nextIndex = startIndex;
                result = default;
                return false;
        }
        result = new(new string(span[startIndex..nextIndex]));
        return true;
    }
    
    public sealed class Matcher : IMatcher
    {
        public static readonly Matcher Instance = new();

        private Matcher() { }

        /// <summary>
        /// Tests whether a roman numeral token can be parsed from one or more characters starting from the specified index.
        /// </summary>
        /// <param name="span">The source sequence of characters.</param>
        /// <param name="startIndex">The index of the first character to be tested.</param>
        /// <param name="endIndex">The exclusive index of the end of the range of characters to be tested.</param>
        /// <param name="nextIndex">Returns the index following the last matched character or the value of <paramref name="startIndex" /> if there is no match.</param>
        /// <returns><see langword="true" /> if the current <see cref="IMatcher" /> can parse a <see cref="RomanNumeral" /> token from one or more characters starting from the specified <paramref name="startIndex" />; otherwise, <see langword="false" />.</returns>
        public bool Match(ReadOnlySpan<char> span, int startIndex, int endIndex, out int nextIndex)
        {
            if (span.ValidateExtentsIsEmpty(ref startIndex, ref endIndex))
            {
                nextIndex = startIndex;
                return false;
            }
            switch (span[startIndex])
            {
                case ROMAN_NUM_1000_UC:
                case ROMAN_NUM_1000_LC:
                    nextIndex = MoveFromM(span, startIndex, endIndex);
                    break;
                case ROMAN_NUM_500_UC:
                case ROMAN_NUM_500_LC:
                    nextIndex = MoveFromD(span, startIndex, endIndex);
                    break;
                case ROMAN_NUM_100_UC:
                case ROMAN_NUM_100_LC:
                    nextIndex = MoveFromC(span, startIndex, endIndex);
                    break;
                case ROMAN_NUM_50_UC:
                case ROMAN_NUM_50_LC:
                    nextIndex = MoveFromL(span, startIndex, endIndex);
                    break;
                case ROMAN_NUM_10_UC:
                case ROMAN_NUM_10_LC:
                    nextIndex = MoveFromX(span, startIndex, endIndex);
                    break;
                case ROMAN_NUM_5_UC:
                case ROMAN_NUM_5_LC:
                    nextIndex = MoveFromV(span, startIndex, endIndex);
                    break;
                case ROMAN_NUM_1_UC:
                case ROMAN_NUM_1_LC:
                    nextIndex = MoveFromI(span, startIndex, endIndex);
                    break;
                default:
                    nextIndex = startIndex;
                    return false;
            }
            return true;
        }

        public bool TryParse(ReadOnlySpan<char> span, int startIndex, int endIndex, [NotNullWhen(true)] out IToken? result, out int nextIndex)
        {
            if (RomanNumeral.TryParse(span, startIndex, endIndex, out RomanNumeral rn, out nextIndex))
            {
                result = rn;
                return true;
            }
            result = null;
            return false;
        }
    }
}
