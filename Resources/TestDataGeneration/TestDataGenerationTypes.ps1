Add-Type -AssemblyName 'System.Text.Json' -ErrorAction Stop;

Add-Type -TypeDefinition @'
namespace TestDataGeneration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    public enum CharacterGroupType
    {
        LettersAndDigits,
        AsciiChars,
        AsciiLettersAndDigits,
        Letters,
        AsciiLetters,
        UpperChars,
        LowerChars,
        Consonants,
        AsciiLettersUpper,
        AsciiLettersLower,
        Numbers,
        HardConsonants,
        SoftConsonants,
        Vowels,
        AsciiHexDigits,
        Surrogates,
        Digits,
        ConsonantsUpper,
        ConsonantsLower,
        AsciiHexDigitsUpper,
        AsciiHexDigitsLower,
        WhiteSpaceChars,
        HighSurrogates,
        LowSurrogates,
        ControlChars,
        HardConsonantsUpper,
        HardConsonantsLower,
        SoftConsonantsUpper,
        SoftConsonantsLower,
        VowelsUpper,
        VowelsLower,
        Separators,
        PunctuationChars,
        Symbols,
        AsciiDigits,
        AsciiControlChars,
        AsciiPunctuation,
        AsciiSymbols,
        UriDataChars,
        CsIdentifierChars
    }
    
    public abstract class CharacterGroup
    {
        public static int GetRandomNumber(Random random, int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(max));
            if (min == max)
                return min;
            if (max == int.MaxValue)
            {
                if (min == int.MinValue)
                    return (random.Next(0, 2) == 1) ? random.Next(-1, int.MaxValue) + 1 : random.Next(int.MinValue, 0);
                return random.Next(min - 1, max) + 1;
            }
            return random.Next(min, max + 1);
        }
        public static IEnumerable<int> GetRandomNumbers(Random random, int minRepeat, int maxRepeat, int minValue, int maxValue) { return GetRandomNumbers(random, GetRandomNumber(random, minRepeat, maxRepeat), minValue, maxValue); }
        public static IEnumerable<int> GetRandomNumbers(Random random, int repeat, int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            if (minValue == maxValue)
            {
                for (int i = 0; i < repeat; i++)
                    yield return minValue;
            }
            else if (maxValue == int.MaxValue)
            {
                if (minValue == int.MinValue)
                {
                    for (int i = 0; i < repeat; i++)
                        yield return (random.Next(0, 2) == 1) ? random.Next(-1, int.MaxValue) + 1 : random.Next(int.MinValue, 0);
                }
                else
                    for (int i = 0; i < repeat; i++)
                        yield return random.Next(minValue - 1, maxValue) + 1;
            }
            else
                for (int i = 0; i < repeat; i++)
                    yield return random.Next(minValue, maxValue + 1);
        }
        public static void WriteRandomNumbers(TextWriter writer, Random random, int minRepeat, int maxRepeat, int minValue, int maxValue) { WriteRandomNumbers(writer, random, GetRandomNumber(random, minRepeat, maxRepeat), minValue, maxValue); }
        public static void WriteRandomNumbers(TextWriter writer, Random random, int repeat, int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            if (repeat < 1)
                return;
            if (minValue == maxValue)
            {
                writer.Write(minValue);
                for (int i = 1; i < repeat; i++)
                {
                    writer.Write(", ");
                    writer.Write(minValue);
                }
            }
            else if (maxValue == int.MaxValue)
            {
                if (minValue == int.MinValue)
                {
                    writer.Write((random.Next(0, 2) == 1) ? random.Next(-1, int.MaxValue) + 1 : random.Next(int.MinValue, 0));
                    for (int i = 1; i < repeat; i++)
                    {
                        writer.Write(", ");
                        writer.Write((random.Next(0, 2) == 1) ? random.Next(-1, int.MaxValue) + 1 : random.Next(int.MinValue, 0));
                    }
                }
                else
                    for (int i = 1; i < repeat; i++)
                    {
                        writer.Write(", ");
                        writer.Write(random.Next(minValue - 1, maxValue) + 1);
                    }
            }
            else
                for (int i = 1; i < repeat; i++)
                {
                    writer.Write(", ");
                    writer.Write(random.Next(minValue, maxValue + 1));
                }
        }
        private readonly CharacterGroupType _type;
        private ReadOnlyCollection<CharacterGroup> _includes;
        private ReadOnlyCollection<CharacterGroup> _includedBy;
        public CharacterGroupType Type { get { return _type; } }
        public ReadOnlyCollection<CharacterGroup> Includes { get { return _includes; } }
        public ReadOnlyCollection<CharacterGroup> IncludedBy { get { return _includedBy; } }
        public abstract bool Test(char c);

        public abstract IEnumerable<char> GetValues();

        protected CharacterGroup(CharacterGroupType type, Collection<CharacterGroup> includes, Collection<CharacterGroup> includedBy)
        {
            _type = type;
            _includes = new ReadOnlyCollection<CharacterGroup>(includes);
            _includedBy = new ReadOnlyCollection<CharacterGroup>(includedBy);
        }

        public static readonly ReadOnlyCollection<CharacterGroup> All;

        public static readonly ReadOnlyDictionary<CharacterGroupType, CharacterGroup> ByType;

        private static CharacterGroup AddDelegated(CharacterGroupType type, Dictionary<CharacterGroupType, CharacterGroup> byType,
            Dictionary<CharacterGroupType, Collection<CharacterGroup>> includes,
            Dictionary<CharacterGroupType, Collection<CharacterGroup>> includedBy, Predicate<char> test, Func<IEnumerable<char>> getValues)
        {
            Collection<CharacterGroup> i = new Collection<CharacterGroup>();
            Collection<CharacterGroup> b = new Collection<CharacterGroup>();
            includes.Add(type, i);
            includedBy.Add(type, b);
            CharacterGroup result = new DelgateCharacterGroup(type, i, b, test, getValues);
            byType.Add(type, result);
            return result;
        }

        private static CharacterGroup AddExplicit(CharacterGroupType type, string characters, Dictionary<CharacterGroupType, CharacterGroup> byType,
            Dictionary<CharacterGroupType, Collection<CharacterGroup>> includes,
            Dictionary<CharacterGroupType, Collection<CharacterGroup>> includedBy)
        {
            Collection<CharacterGroup> i = new Collection<CharacterGroup>();
            Collection<CharacterGroup> b = new Collection<CharacterGroup>();
            includes.Add(type, i);
            includedBy.Add(type, b);
            CharacterGroup result = new ExplicitCharacterGroup(type, characters, i, b);
            byType.Add(type, result);
            return result;
        }

        private static void SetIncludes(CharacterGroupType type, Dictionary<CharacterGroupType, CharacterGroup> byType,
            Dictionary<CharacterGroupType, Collection<CharacterGroup>> includes,
            Dictionary<CharacterGroupType, Collection<CharacterGroup>> includedBy, params CharacterGroupType[] includedTypes)
        {
            CharacterGroup p = byType[type];
            Collection<CharacterGroup> i = includes[type];
            foreach (CharacterGroupType n in includedTypes)
            {
                i.Add(byType[n]);
                includedBy[type].Add(p);
            }
        }
        public static IEnumerable<char> GetAsciiChars()
        {
            for (char c = char.MinValue; c < '\u0080'; c++) yield return c;
        }

        public static IEnumerable<char> GetAsciiLettersAndDigits()
        {
            for (char c = '0'; c <= '9'; c++) yield return c;
            for (char c = 'A'; c <= 'Z'; c++) yield return c;
            for (char c = 'a'; c <= 'z'; c++) yield return c;
        }

        public static IEnumerable<char> GetUriDataChars()
        {
            foreach (char c in GetAsciiLettersAndDigits()) yield return c;
            yield return '-';
            yield return '.';
            yield return '_';
            yield return '~';
        }

        public static IEnumerable<char> GetCsIdentifierChars()
        {
            foreach (char c in GetAsciiLettersAndDigits()) yield return c;
            yield return '_';
        }

        public static IEnumerable<char> GetAsciiLetters()
        {
            for (char c = 'A'; c <= 'Z'; c++) yield return c;
            for (char c = 'a'; c <= 'z'; c++) yield return c;
        }
        
        public static IEnumerable<char> GetAsciiLettersUpper()
        {
            for (char c = 'A'; c <= 'Z'; c++) yield return c;
        }
        
        public static IEnumerable<char> GetAsciiLettersLower()
        {
            for (char c = 'a'; c <= 'z'; c++) yield return c;
        }

        public static IEnumerable<char> GetAsciiHexDigits()
        {
            for (char c = '0'; c <= '9'; c++) yield return c;
            for (char c = 'A'; c <= 'F'; c++) yield return c;
            for (char c = 'a'; c <= 'f'; c++) yield return c;
        }

        public static IEnumerable<char> GetSurrogates()
        {
            for (char c = '\ud800'; c < '\udfff'; c++) yield return c;
        }

        public static IEnumerable<char> GetAsciiHexDigitsUpper()
        {
            for (char c = '0'; c <= '9'; c++) yield return c;
            for (char c = 'A'; c <= 'F'; c++) yield return c;
        }

        public static IEnumerable<char> GetAsciiHexDigitsLower()
        {
            for (char c = '0'; c <= '9'; c++) yield return c;
            for (char c = 'a'; c <= 'f'; c++) yield return c;
        }
        
        public static IEnumerable<char> GetWhiteSpaceChars()
        {
            for (char c = '	'; c < '\u000d'; c++) yield return c;
            yield return ' ';
            yield return '\u0085';
            yield return '\u00a0';
            yield return '\u1680';
            for (char c = '\u2000'; c < '\u200a'; c++) yield return c;
            yield return '\u2028';
            yield return '\u2029';
            yield return '\u202f';
            yield return '\u205f';
            yield return '\u3000';
        }

        public static IEnumerable<char> GetHighSurrogates()
        {
            for (char c = '\ud800'; c < '\udbff'; c++) yield return c;
        }

        public static IEnumerable<char> GetLowSurrogates()
        {
            for (char c = '\udc00'; c < '\udfff'; c++) yield return c;
        }
        
        public static IEnumerable<char> GetAsciiControlChars()
        {
            for (char c = '\u0000'; c < '\u001f'; c++) yield return c;
        }

        public static IEnumerable<char> GetControlChars()
        {
            for (char c = '\u0000'; c < '\u001f'; c++) yield return c;
            for (char c = '\u007f'; c < '\u009f'; c++) yield return c;
        }

        public static IEnumerable<char> GetSeparators()
        {
            yield return ' ';
            yield return '\u00a0';
            yield return '\u1680';
            for (char c = '\u2000'; c < '\u200a'; c++) yield return c;
            yield return '\u2028';
            yield return '\u2029';
            yield return '\u202f';
            yield return '\u205f';
            yield return '\u3000';
        }

        public static IEnumerable<char> GetAsciiDigits() { for (char c = '0'; c <= '9'; c++) yield return c; }

        public static IEnumerable<char> GetLetters()
        {
            foreach (char c in GetUpperChars()) yield return c;
            foreach (char c in GetLowerChars()) yield return c;
            yield return '\u00aa';
            yield return '\u00ba';
            yield return '\u01bb';
            for (char c = '\u01c0'; c <= '\u01c3'; c++) yield return c;
            yield return '\u01c5';
            yield return '\u01c8';
            yield return '\u01cb';
            yield return '\u01f2';
            yield return '\u0294';
            for (char c = '\u02b0'; c <= '\u02c1'; c++) yield return c;
            for (char c = '\u02c6'; c <= '\u02d1'; c++) yield return c;
            for (char c = '\u02e0'; c <= '\u02e4'; c++) yield return c;
            yield return '\u02ec';
            yield return '\u02ee';
            yield return '\u0374';
            yield return '\u037a';
            yield return '\u0559';
            for (char c = '\u05d0'; c <= '\u05ea'; c++) yield return c;
            for (char c = '\u05ef'; c <= '\u05f2'; c++) yield return c;
            for (char c = '\u0620'; c <= '\u064a'; c++) yield return c;
            yield return '\u066e';
            yield return '\u066f';
            for (char c = '\u0671'; c <= '\u06d3'; c++) yield return c;
            yield return '\u06d5';
            yield return '\u06e5';
            yield return '\u06e6';
            yield return '\u06ee';
            yield return '\u06ef';
            for (char c = '\u06fa'; c <= '\u06fc'; c++) yield return c;
            yield return '\u06ff';
            yield return '\u0710';
            for (char c = '\u0712'; c <= '\u072f'; c++) yield return c;
            for (char c = '\u074d'; c <= '\u07a5'; c++) yield return c;
            yield return '\u07b1';
            for (char c = '\u07ca'; c <= '\u07ea'; c++) yield return c;
            yield return '\u07f4';
            yield return '\u07f5';
            yield return '\u07fa';
            for (char c = '\u0800'; c <= '\u0815'; c++) yield return c;
            yield return '\u081a';
            yield return '\u0824';
            yield return '\u0828';
            for (char c = '\u0840'; c <= '\u0858'; c++) yield return c;
            for (char c = '\u0860'; c <= '\u086a'; c++) yield return c;
            for (char c = '\u0870'; c <= '\u0887'; c++) yield return c;
            for (char c = '\u0889'; c <= '\u088e'; c++) yield return c;
            for (char c = '\u08a0'; c <= '\u08c9'; c++) yield return c;
            for (char c = '\u0904'; c <= '\u0939'; c++) yield return c;
            yield return '\u093d';
            yield return '\u0950';
            for (char c = '\u0958'; c <= '\u0961'; c++) yield return c;
            for (char c = '\u0971'; c <= '\u0980'; c++) yield return c;
            for (char c = '\u0985'; c <= '\u098c'; c++) yield return c;
            yield return '\u098f';
            yield return '\u0990';
            for (char c = '\u0993'; c <= '\u09a8'; c++) yield return c;
            for (char c = '\u09aa'; c <= '\u09b0'; c++) yield return c;
            yield return '\u09b2';
            for (char c = '\u09b6'; c <= '\u09b9'; c++) yield return c;
            yield return '\u09bd';
            yield return '\u09ce';
            yield return '\u09dc';
            yield return '\u09dd';
            for (char c = '\u09df'; c <= '\u09e1'; c++) yield return c;
            yield return '\u09f0';
            yield return '\u09f1';
            yield return '\u09fc';
            for (char c = '\u0a05'; c <= '\u0a0a'; c++) yield return c;
            yield return '\u0a0f';
            yield return '\u0a10';
            for (char c = '\u0a13'; c <= '\u0a28'; c++) yield return c;
            for (char c = '\u0a2a'; c <= '\u0a30'; c++) yield return c;
            yield return '\u0a32';
            yield return '\u0a33';
            yield return '\u0a35';
            yield return '\u0a36';
            yield return '\u0a38';
            yield return '\u0a39';
            for (char c = '\u0a59'; c <= '\u0a5c'; c++) yield return c;
            yield return '\u0a5e';
            for (char c = '\u0a72'; c <= '\u0a74'; c++) yield return c;
            for (char c = '\u0a85'; c <= '\u0a8d'; c++) yield return c;
            for (char c = '\u0a8f'; c <= '\u0a91'; c++) yield return c;
            for (char c = '\u0a93'; c <= '\u0aa8'; c++) yield return c;
            for (char c = '\u0aaa'; c <= '\u0ab0'; c++) yield return c;
            yield return '\u0ab2';
            yield return '\u0ab3';
            for (char c = '\u0ab5'; c <= '\u0ab9'; c++) yield return c;
            yield return '\u0abd';
            yield return '\u0ad0';
            yield return '\u0ae0';
            yield return '\u0ae1';
            yield return '\u0af9';
            for (char c = '\u0b05'; c <= '\u0b0c'; c++) yield return c;
            yield return '\u0b0f';
            yield return '\u0b10';
            for (char c = '\u0b13'; c <= '\u0b28'; c++) yield return c;
            for (char c = '\u0b2a'; c <= '\u0b30'; c++) yield return c;
            yield return '\u0b32';
            yield return '\u0b33';
            for (char c = '\u0b35'; c <= '\u0b39'; c++) yield return c;
            yield return '\u0b3d';
            yield return '\u0b5c';
            yield return '\u0b5d';
            for (char c = '\u0b5f'; c <= '\u0b61'; c++) yield return c;
            yield return '\u0b71';
            yield return '\u0b83';
            for (char c = '\u0b85'; c <= '\u0b8a'; c++) yield return c;
            for (char c = '\u0b8e'; c <= '\u0b90'; c++) yield return c;
            for (char c = '\u0b92'; c <= '\u0b95'; c++) yield return c;
            yield return '\u0b99';
            yield return '\u0b9a';
            yield return '\u0b9c';
            yield return '\u0b9e';
            yield return '\u0b9f';
            yield return '\u0ba3';
            yield return '\u0ba4';
            for (char c = '\u0ba8'; c <= '\u0baa'; c++) yield return c;
            for (char c = '\u0bae'; c <= '\u0bb9'; c++) yield return c;
            yield return '\u0bd0';
            for (char c = '\u0c05'; c <= '\u0c0c'; c++) yield return c;
            for (char c = '\u0c0e'; c <= '\u0c10'; c++) yield return c;
            for (char c = '\u0c12'; c <= '\u0c28'; c++) yield return c;
            for (char c = '\u0c2a'; c <= '\u0c39'; c++) yield return c;
            yield return '\u0c3d';
            for (char c = '\u0c58'; c <= '\u0c5a'; c++) yield return c;
            yield return '\u0c5d';
            yield return '\u0c60';
            yield return '\u0c61';
            yield return '\u0c80';
            for (char c = '\u0c85'; c <= '\u0c8c'; c++) yield return c;
            for (char c = '\u0c8e'; c <= '\u0c90'; c++) yield return c;
            for (char c = '\u0c92'; c <= '\u0ca8'; c++) yield return c;
            for (char c = '\u0caa'; c <= '\u0cb3'; c++) yield return c;
            for (char c = '\u0cb5'; c <= '\u0cb9'; c++) yield return c;
            yield return '\u0cbd';
            yield return '\u0cdd';
            yield return '\u0cde';
            yield return '\u0ce0';
            yield return '\u0ce1';
            yield return '\u0cf1';
            yield return '\u0cf2';
            for (char c = '\u0d04'; c <= '\u0d0c'; c++) yield return c;
            for (char c = '\u0d0e'; c <= '\u0d10'; c++) yield return c;
            for (char c = '\u0d12'; c <= '\u0d3a'; c++) yield return c;
            yield return '\u0d3d';
            yield return '\u0d4e';
            for (char c = '\u0d54'; c <= '\u0d56'; c++) yield return c;
            for (char c = '\u0d5f'; c <= '\u0d61'; c++) yield return c;
            for (char c = '\u0d7a'; c <= '\u0d7f'; c++) yield return c;
            for (char c = '\u0d85'; c <= '\u0d96'; c++) yield return c;
            for (char c = '\u0d9a'; c <= '\u0db1'; c++) yield return c;
            for (char c = '\u0db3'; c <= '\u0dbb'; c++) yield return c;
            yield return '\u0dbd';
            for (char c = '\u0dc0'; c <= '\u0dc6'; c++) yield return c;
            for (char c = '\u0e01'; c <= '\u0e30'; c++) yield return c;
            yield return '\u0e32';
            yield return '\u0e33';
            for (char c = '\u0e40'; c <= '\u0e46'; c++) yield return c;
            yield return '\u0e81';
            yield return '\u0e82';
            yield return '\u0e84';
            for (char c = '\u0e86'; c <= '\u0e8a'; c++) yield return c;
            for (char c = '\u0e8c'; c <= '\u0ea3'; c++) yield return c;
            yield return '\u0ea5';
            for (char c = '\u0ea7'; c <= '\u0eb0'; c++) yield return c;
            yield return '\u0eb2';
            yield return '\u0eb3';
            yield return '\u0ebd';
            for (char c = '\u0ec0'; c <= '\u0ec4'; c++) yield return c;
            yield return '\u0ec6';
            for (char c = '\u0edc'; c <= '\u0edf'; c++) yield return c;
            yield return '\u0f00';
            for (char c = '\u0f40'; c <= '\u0f47'; c++) yield return c;
            for (char c = '\u0f49'; c <= '\u0f6c'; c++) yield return c;
            for (char c = '\u0f88'; c <= '\u0f8c'; c++) yield return c;
            for (char c = '\u1000'; c <= '\u102a'; c++) yield return c;
            yield return '\u103f';
            for (char c = '\u1050'; c <= '\u1055'; c++) yield return c;
            for (char c = '\u105a'; c <= '\u105d'; c++) yield return c;
            yield return '\u1061';
            yield return '\u1065';
            yield return '\u1066';
            for (char c = '\u106e'; c <= '\u1070'; c++) yield return c;
            for (char c = '\u1075'; c <= '\u1081'; c++) yield return c;
            yield return '\u108e';
            yield return '\u10fc';
            for (char c = '\u1100'; c <= '\u1248'; c++) yield return c;
            for (char c = '\u124a'; c <= '\u124d'; c++) yield return c;
            for (char c = '\u1250'; c <= '\u1256'; c++) yield return c;
            yield return '\u1258';
            for (char c = '\u125a'; c <= '\u125d'; c++) yield return c;
            for (char c = '\u1260'; c <= '\u1288'; c++) yield return c;
            for (char c = '\u128a'; c <= '\u128d'; c++) yield return c;
            for (char c = '\u1290'; c <= '\u12b0'; c++) yield return c;
            for (char c = '\u12b2'; c <= '\u12b5'; c++) yield return c;
            for (char c = '\u12b8'; c <= '\u12be'; c++) yield return c;
            yield return '\u12c0';
            for (char c = '\u12c2'; c <= '\u12c5'; c++) yield return c;
            for (char c = '\u12c8'; c <= '\u12d6'; c++) yield return c;
            for (char c = '\u12d8'; c <= '\u1310'; c++) yield return c;
            for (char c = '\u1312'; c <= '\u1315'; c++) yield return c;
            for (char c = '\u1318'; c <= '\u135a'; c++) yield return c;
            for (char c = '\u1380'; c <= '\u138f'; c++) yield return c;
            for (char c = '\u1401'; c <= '\u166c'; c++) yield return c;
            for (char c = '\u166f'; c <= '\u167f'; c++) yield return c;
            for (char c = '\u1681'; c <= '\u169a'; c++) yield return c;
            for (char c = '\u16a0'; c <= '\u16ea'; c++) yield return c;
            for (char c = '\u16f1'; c <= '\u16f8'; c++) yield return c;
            for (char c = '\u1700'; c <= '\u1711'; c++) yield return c;
            for (char c = '\u171f'; c <= '\u1731'; c++) yield return c;
            for (char c = '\u1740'; c <= '\u1751'; c++) yield return c;
            for (char c = '\u1760'; c <= '\u176c'; c++) yield return c;
            for (char c = '\u176e'; c <= '\u1770'; c++) yield return c;
            for (char c = '\u1780'; c <= '\u17b3'; c++) yield return c;
            yield return '\u17d7';
            yield return '\u17dc';
            for (char c = '\u1820'; c <= '\u1878'; c++) yield return c;
            for (char c = '\u1880'; c <= '\u1884'; c++) yield return c;
            for (char c = '\u1887'; c <= '\u18a8'; c++) yield return c;
            yield return '\u18aa';
            for (char c = '\u18b0'; c <= '\u18f5'; c++) yield return c;
            for (char c = '\u1900'; c <= '\u191e'; c++) yield return c;
            for (char c = '\u1950'; c <= '\u196d'; c++) yield return c;
            for (char c = '\u1970'; c <= '\u1974'; c++) yield return c;
            for (char c = '\u1980'; c <= '\u19ab'; c++) yield return c;
            for (char c = '\u19b0'; c <= '\u19c9'; c++) yield return c;
            for (char c = '\u1a00'; c <= '\u1a16'; c++) yield return c;
            for (char c = '\u1a20'; c <= '\u1a54'; c++) yield return c;
            yield return '\u1aa7';
            for (char c = '\u1b05'; c <= '\u1b33'; c++) yield return c;
            for (char c = '\u1b45'; c <= '\u1b4c'; c++) yield return c;
            for (char c = '\u1b83'; c <= '\u1ba0'; c++) yield return c;
            yield return '\u1bae';
            yield return '\u1baf';
            for (char c = '\u1bba'; c <= '\u1be5'; c++) yield return c;
            for (char c = '\u1c00'; c <= '\u1c23'; c++) yield return c;
            for (char c = '\u1c4d'; c <= '\u1c4f'; c++) yield return c;
            for (char c = '\u1c5a'; c <= '\u1c7d'; c++) yield return c;
            for (char c = '\u1ce9'; c <= '\u1cec'; c++) yield return c;
            for (char c = '\u1cee'; c <= '\u1cf3'; c++) yield return c;
            yield return '\u1cf5';
            yield return '\u1cf6';
            yield return '\u1cfa';
            for (char c = '\u1d2c'; c <= '\u1d6a'; c++) yield return c;
            yield return '\u1d78';
            for (char c = '\u1d9b'; c <= '\u1dbf'; c++) yield return c;
            for (char c = '\u1f88'; c <= '\u1f8f'; c++) yield return c;
            for (char c = '\u1f98'; c <= '\u1f9f'; c++) yield return c;
            for (char c = '\u1fa8'; c <= '\u1faf'; c++) yield return c;
            yield return '\u1fbc';
            yield return '\u1fcc';
            yield return '\u1ffc';
            yield return '\u2071';
            yield return '\u207f';
            for (char c = '\u2090'; c <= '\u209c'; c++) yield return c;
            for (char c = '\u2135'; c <= '\u2138'; c++) yield return c;
            yield return '\u2c7c';
            yield return '\u2c7d';
            for (char c = '\u2d30'; c <= '\u2d67'; c++) yield return c;
            yield return '\u2d6f';
            for (char c = '\u2d80'; c <= '\u2d96'; c++) yield return c;
            for (char c = '\u2da0'; c <= '\u2da6'; c++) yield return c;
            for (char c = '\u2da8'; c <= '\u2dae'; c++) yield return c;
            for (char c = '\u2db0'; c <= '\u2db6'; c++) yield return c;
            for (char c = '\u2db8'; c <= '\u2dbe'; c++) yield return c;
            for (char c = '\u2dc0'; c <= '\u2dc6'; c++) yield return c;
            for (char c = '\u2dc8'; c <= '\u2dce'; c++) yield return c;
            for (char c = '\u2dd0'; c <= '\u2dd6'; c++) yield return c;
            for (char c = '\u2dd8'; c <= '\u2dde'; c++) yield return c;
            yield return '\u2e2f';
            yield return '\u3005';
            yield return '\u3006';
            for (char c = '\u3031'; c <= '\u3035'; c++) yield return c;
            yield return '\u303b';
            yield return '\u303c';
            for (char c = '\u3041'; c <= '\u3096'; c++) yield return c;
            for (char c = '\u309d'; c <= '\u309f'; c++) yield return c;
            for (char c = '\u30a1'; c <= '\u30fa'; c++) yield return c;
            for (char c = '\u30fc'; c <= '\u30ff'; c++) yield return c;
            for (char c = '\u3105'; c <= '\u312f'; c++) yield return c;
            for (char c = '\u3131'; c <= '\u318e'; c++) yield return c;
            for (char c = '\u31a0'; c <= '\u31bf'; c++) yield return c;
            for (char c = '\u31f0'; c <= '\u31ff'; c++) yield return c;
            for (char c = '\u3400'; c <= '\u4dbf'; c++) yield return c;
            for (char c = '\u4e00'; c <= '\ua48c'; c++) yield return c;
            for (char c = '\ua4d0'; c <= '\ua4fd'; c++) yield return c;
            for (char c = '\ua500'; c <= '\ua60c'; c++) yield return c;
            for (char c = '\ua610'; c <= '\ua61f'; c++) yield return c;
            yield return '\ua62a';
            yield return '\ua62b';
            yield return '\ua66e';
            yield return '\ua67f';
            yield return '\ua69c';
            yield return '\ua69d';
            for (char c = '\ua6a0'; c <= '\ua6e5'; c++) yield return c;
            for (char c = '\ua717'; c <= '\ua71f'; c++) yield return c;
            yield return '\ua770';
            yield return '\ua788';
            yield return '\ua78f';
            for (char c = '\ua7f2'; c <= '\ua7f4'; c++) yield return c;
            for (char c = '\ua7f7'; c <= '\ua7f9'; c++) yield return c;
            for (char c = '\ua7fb'; c <= '\ua801'; c++) yield return c;
            for (char c = '\ua803'; c <= '\ua805'; c++) yield return c;
            for (char c = '\ua807'; c <= '\ua80a'; c++) yield return c;
            for (char c = '\ua80c'; c <= '\ua822'; c++) yield return c;
            for (char c = '\ua840'; c <= '\ua873'; c++) yield return c;
            for (char c = '\ua882'; c <= '\ua8b3'; c++) yield return c;
            for (char c = '\ua8f2'; c <= '\ua8f7'; c++) yield return c;
            yield return '\ua8fb';
            yield return '\ua8fd';
            yield return '\ua8fe';
            for (char c = '\ua90a'; c <= '\ua925'; c++) yield return c;
            for (char c = '\ua930'; c <= '\ua946'; c++) yield return c;
            for (char c = '\ua960'; c <= '\ua97c'; c++) yield return c;
            for (char c = '\ua984'; c <= '\ua9b2'; c++) yield return c;
            yield return '\ua9cf';
            for (char c = '\ua9e0'; c <= '\ua9e4'; c++) yield return c;
            for (char c = '\ua9e6'; c <= '\ua9ef'; c++) yield return c;
            for (char c = '\ua9fa'; c <= '\ua9fe'; c++) yield return c;
            for (char c = '\uaa00'; c <= '\uaa28'; c++) yield return c;
            for (char c = '\uaa40'; c <= '\uaa42'; c++) yield return c;
            for (char c = '\uaa44'; c <= '\uaa4b'; c++) yield return c;
            for (char c = '\uaa60'; c <= '\uaa76'; c++) yield return c;
            yield return '\uaa7a';
            for (char c = '\uaa7e'; c <= '\uaaaf'; c++) yield return c;
            yield return '\uaab1';
            yield return '\uaab5';
            yield return '\uaab6';
            for (char c = '\uaab9'; c <= '\uaabd'; c++) yield return c;
            yield return '\uaac0';
            yield return '\uaac2';
            for (char c = '\uaadb'; c <= '\uaadd'; c++) yield return c;
            for (char c = '\uaae0'; c <= '\uaaea'; c++) yield return c;
            for (char c = '\uaaf2'; c <= '\uaaf4'; c++) yield return c;
            for (char c = '\uab01'; c <= '\uab06'; c++) yield return c;
            for (char c = '\uab09'; c <= '\uab0e'; c++) yield return c;
            for (char c = '\uab11'; c <= '\uab16'; c++) yield return c;
            for (char c = '\uab20'; c <= '\uab26'; c++) yield return c;
            for (char c = '\uab28'; c <= '\uab2e'; c++) yield return c;
            for (char c = '\uab5c'; c <= '\uab5f'; c++) yield return c;
            yield return '\uab69';
            for (char c = '\uabc0'; c <= '\uabe2'; c++) yield return c;
            for (char c = '\uac00'; c <= '\ud7a3'; c++) yield return c;
            for (char c = '\ud7b0'; c <= '\ud7c6'; c++) yield return c;
            for (char c = '\ud7cb'; c <= '\ud7fb'; c++) yield return c;
            for (char c = '\uf900'; c <= '\ufa6d'; c++) yield return c;
            for (char c = '\ufa70'; c <= '\ufad9'; c++) yield return c;
            yield return '\ufb1d';
            for (char c = '\ufb1f'; c <= '\ufb28'; c++) yield return c;
            for (char c = '\ufb2a'; c <= '\ufb36'; c++) yield return c;
            for (char c = '\ufb38'; c <= '\ufb3c'; c++) yield return c;
            yield return '\ufb3e';
            yield return '\ufb40';
            yield return '\ufb41';
            yield return '\ufb43';
            yield return '\ufb44';
            for (char c = '\ufb46'; c <= '\ufbb1'; c++) yield return c;
            for (char c = '\ufbd3'; c <= '\ufd3d'; c++) yield return c;
            for (char c = '\ufd50'; c <= '\ufd8f'; c++) yield return c;
            for (char c = '\ufd92'; c <= '\ufdc7'; c++) yield return c;
            for (char c = '\ufdf0'; c <= '\ufdfb'; c++) yield return c;
            for (char c = '\ufe70'; c <= '\ufe74'; c++) yield return c;
            for (char c = '\ufe76'; c <= '\ufefc'; c++) yield return c;
            for (char c = '\uff66'; c <= '\uffbe'; c++) yield return c;
            for (char c = '\uffc2'; c <= '\uffc7'; c++) yield return c;
            for (char c = '\uffca'; c <= '\uffcf'; c++) yield return c;
            for (char c = '\uffd2'; c <= '\uffd7'; c++) yield return c;
            for (char c = '\uffda'; c <= '\uffdc'; c++) yield return c;
        }

        public static IEnumerable<char> GetLettersAndDigits()
        {
            foreach (char c in GetLetters()) yield return c;
            foreach (char c in GetDigits()) yield return c;
        }

        public static IEnumerable<char> GetUpperChars()
        {
            for (char c = 'A'; c <= '\u005a'; c++) yield return c;
            for (char c = '\u00c0'; c <= '\u00d6'; c++) yield return c;
            for (char c = '\u00d8'; c <= '\u00de'; c++) yield return c;
            yield return '\u0100';
            yield return '\u0102';
            yield return '\u0104';
            yield return '\u0106';
            yield return '\u0108';
            yield return '\u010a';
            yield return '\u010c';
            yield return '\u010e';
            yield return '\u0110';
            yield return '\u0112';
            yield return '\u0114';
            yield return '\u0116';
            yield return '\u0118';
            yield return '\u011a';
            yield return '\u011c';
            yield return '\u011e';
            yield return '\u0120';
            yield return '\u0122';
            yield return '\u0124';
            yield return '\u0126';
            yield return '\u0128';
            yield return '\u012a';
            yield return '\u012c';
            yield return '\u012e';
            yield return '\u0130';
            yield return '\u0132';
            yield return '\u0134';
            yield return '\u0136';
            yield return '\u0139';
            yield return '\u013b';
            yield return '\u013d';
            yield return '\u013f';
            yield return '\u0141';
            yield return '\u0143';
            yield return '\u0145';
            yield return '\u0147';
            yield return '\u014a';
            yield return '\u014c';
            yield return '\u014e';
            yield return '\u0150';
            yield return '\u0152';
            yield return '\u0154';
            yield return '\u0156';
            yield return '\u0158';
            yield return '\u015a';
            yield return '\u015c';
            yield return '\u015e';
            yield return '\u0160';
            yield return '\u0162';
            yield return '\u0164';
            yield return '\u0166';
            yield return '\u0168';
            yield return '\u016a';
            yield return '\u016c';
            yield return '\u016e';
            yield return '\u0170';
            yield return '\u0172';
            yield return '\u0174';
            yield return '\u0176';
            yield return '\u0178';
            yield return '\u0179';
            yield return '\u017b';
            yield return '\u017d';
            yield return '\u0181';
            yield return '\u0182';
            yield return '\u0184';
            yield return '\u0186';
            yield return '\u0187';
            for (char c = '\u0189'; c <= '\u018b'; c++) yield return c;
            for (char c = '\u018e'; c <= '\u0191'; c++) yield return c;
            yield return '\u0193';
            yield return '\u0194';
            for (char c = '\u0196'; c <= '\u0198'; c++) yield return c;
            yield return '\u019c';
            yield return '\u019d';
            yield return '\u019f';
            yield return '\u01a0';
            yield return '\u01a2';
            yield return '\u01a4';
            yield return '\u01a6';
            yield return '\u01a7';
            yield return '\u01a9';
            yield return '\u01ac';
            yield return '\u01ae';
            yield return '\u01af';
            for (char c = '\u01b1'; c <= '\u01b3'; c++) yield return c;
            yield return '\u01b5';
            yield return '\u01b7';
            yield return '\u01b8';
            yield return '\u01bc';
            yield return '\u01c4';
            yield return '\u01c7';
            yield return '\u01ca';
            yield return '\u01cd';
            yield return '\u01cf';
            yield return '\u01d1';
            yield return '\u01d3';
            yield return '\u01d5';
            yield return '\u01d7';
            yield return '\u01d9';
            yield return '\u01db';
            yield return '\u01de';
            yield return '\u01e0';
            yield return '\u01e2';
            yield return '\u01e4';
            yield return '\u01e6';
            yield return '\u01e8';
            yield return '\u01ea';
            yield return '\u01ec';
            yield return '\u01ee';
            yield return '\u01f1';
            yield return '\u01f4';
            for (char c = '\u01f6'; c <= '\u01f8'; c++) yield return c;
            yield return '\u01fa';
            yield return '\u01fc';
            yield return '\u01fe';
            yield return '\u0200';
            yield return '\u0202';
            yield return '\u0204';
            yield return '\u0206';
            yield return '\u0208';
            yield return '\u020a';
            yield return '\u020c';
            yield return '\u020e';
            yield return '\u0210';
            yield return '\u0212';
            yield return '\u0214';
            yield return '\u0216';
            yield return '\u0218';
            yield return '\u021a';
            yield return '\u021c';
            yield return '\u021e';
            yield return '\u0220';
            yield return '\u0222';
            yield return '\u0224';
            yield return '\u0226';
            yield return '\u0228';
            yield return '\u022a';
            yield return '\u022c';
            yield return '\u022e';
            yield return '\u0230';
            yield return '\u0232';
            yield return '\u023a';
            yield return '\u023b';
            yield return '\u023d';
            yield return '\u023e';
            yield return '\u0241';
            for (char c = '\u0243'; c <= '\u0246'; c++) yield return c;
            yield return '\u0248';
            yield return '\u024a';
            yield return '\u024c';
            yield return '\u024e';
            yield return '\u0370';
            yield return '\u0372';
            yield return '\u0376';
            yield return '\u037f';
            yield return '\u0386';
            for (char c = '\u0388'; c <= '\u038a'; c++) yield return c;
            yield return '\u038c';
            yield return '\u038e';
            yield return '\u038f';
            for (char c = '\u0391'; c <= '\u03a1'; c++) yield return c;
            for (char c = '\u03a3'; c <= '\u03ab'; c++) yield return c;
            yield return '\u03cf';
            for (char c = '\u03d2'; c <= '\u03d4'; c++) yield return c;
            yield return '\u03d8';
            yield return '\u03da';
            yield return '\u03dc';
            yield return '\u03de';
            yield return '\u03e0';
            yield return '\u03e2';
            yield return '\u03e4';
            yield return '\u03e6';
            yield return '\u03e8';
            yield return '\u03ea';
            yield return '\u03ec';
            yield return '\u03ee';
            yield return '\u03f4';
            yield return '\u03f7';
            yield return '\u03f9';
            yield return '\u03fa';
            for (char c = '\u03fd'; c <= '\u042f'; c++) yield return c;
            yield return '\u0460';
            yield return '\u0462';
            yield return '\u0464';
            yield return '\u0466';
            yield return '\u0468';
            yield return '\u046a';
            yield return '\u046c';
            yield return '\u046e';
            yield return '\u0470';
            yield return '\u0472';
            yield return '\u0474';
            yield return '\u0476';
            yield return '\u0478';
            yield return '\u047a';
            yield return '\u047c';
            yield return '\u047e';
            yield return '\u0480';
            yield return '\u048a';
            yield return '\u048c';
            yield return '\u048e';
            yield return '\u0490';
            yield return '\u0492';
            yield return '\u0494';
            yield return '\u0496';
            yield return '\u0498';
            yield return '\u049a';
            yield return '\u049c';
            yield return '\u049e';
            yield return '\u04a0';
            yield return '\u04a2';
            yield return '\u04a4';
            yield return '\u04a6';
            yield return '\u04a8';
            yield return '\u04aa';
            yield return '\u04ac';
            yield return '\u04ae';
            yield return '\u04b0';
            yield return '\u04b2';
            yield return '\u04b4';
            yield return '\u04b6';
            yield return '\u04b8';
            yield return '\u04ba';
            yield return '\u04bc';
            yield return '\u04be';
            yield return '\u04c0';
            yield return '\u04c1';
            yield return '\u04c3';
            yield return '\u04c5';
            yield return '\u04c7';
            yield return '\u04c9';
            yield return '\u04cb';
            yield return '\u04cd';
            yield return '\u04d0';
            yield return '\u04d2';
            yield return '\u04d4';
            yield return '\u04d6';
            yield return '\u04d8';
            yield return '\u04da';
            yield return '\u04dc';
            yield return '\u04de';
            yield return '\u04e0';
            yield return '\u04e2';
            yield return '\u04e4';
            yield return '\u04e6';
            yield return '\u04e8';
            yield return '\u04ea';
            yield return '\u04ec';
            yield return '\u04ee';
            yield return '\u04f0';
            yield return '\u04f2';
            yield return '\u04f4';
            yield return '\u04f6';
            yield return '\u04f8';
            yield return '\u04fa';
            yield return '\u04fc';
            yield return '\u04fe';
            yield return '\u0500';
            yield return '\u0502';
            yield return '\u0504';
            yield return '\u0506';
            yield return '\u0508';
            yield return '\u050a';
            yield return '\u050c';
            yield return '\u050e';
            yield return '\u0510';
            yield return '\u0512';
            yield return '\u0514';
            yield return '\u0516';
            yield return '\u0518';
            yield return '\u051a';
            yield return '\u051c';
            yield return '\u051e';
            yield return '\u0520';
            yield return '\u0522';
            yield return '\u0524';
            yield return '\u0526';
            yield return '\u0528';
            yield return '\u052a';
            yield return '\u052c';
            yield return '\u052e';
            for (char c = '\u0531'; c <= '\u0556'; c++) yield return c;
            for (char c = '\u10a0'; c <= '\u10c5'; c++) yield return c;
            yield return '\u10c7';
            yield return '\u10cd';
            for (char c = '\u13a0'; c <= '\u13f5'; c++) yield return c;
            for (char c = '\u1c90'; c <= '\u1cba'; c++) yield return c;
            for (char c = '\u1cbd'; c <= '\u1cbf'; c++) yield return c;
            yield return '\u1e00';
            yield return '\u1e02';
            yield return '\u1e04';
            yield return '\u1e06';
            yield return '\u1e08';
            yield return '\u1e0a';
            yield return '\u1e0c';
            yield return '\u1e0e';
            yield return '\u1e10';
            yield return '\u1e12';
            yield return '\u1e14';
            yield return '\u1e16';
            yield return '\u1e18';
            yield return '\u1e1a';
            yield return '\u1e1c';
            yield return '\u1e1e';
            yield return '\u1e20';
            yield return '\u1e22';
            yield return '\u1e24';
            yield return '\u1e26';
            yield return '\u1e28';
            yield return '\u1e2a';
            yield return '\u1e2c';
            yield return '\u1e2e';
            yield return '\u1e30';
            yield return '\u1e32';
            yield return '\u1e34';
            yield return '\u1e36';
            yield return '\u1e38';
            yield return '\u1e3a';
            yield return '\u1e3c';
            yield return '\u1e3e';
            yield return '\u1e40';
            yield return '\u1e42';
            yield return '\u1e44';
            yield return '\u1e46';
            yield return '\u1e48';
            yield return '\u1e4a';
            yield return '\u1e4c';
            yield return '\u1e4e';
            yield return '\u1e50';
            yield return '\u1e52';
            yield return '\u1e54';
            yield return '\u1e56';
            yield return '\u1e58';
            yield return '\u1e5a';
            yield return '\u1e5c';
            yield return '\u1e5e';
            yield return '\u1e60';
            yield return '\u1e62';
            yield return '\u1e64';
            yield return '\u1e66';
            yield return '\u1e68';
            yield return '\u1e6a';
            yield return '\u1e6c';
            yield return '\u1e6e';
            yield return '\u1e70';
            yield return '\u1e72';
            yield return '\u1e74';
            yield return '\u1e76';
            yield return '\u1e78';
            yield return '\u1e7a';
            yield return '\u1e7c';
            yield return '\u1e7e';
            yield return '\u1e80';
            yield return '\u1e82';
            yield return '\u1e84';
            yield return '\u1e86';
            yield return '\u1e88';
            yield return '\u1e8a';
            yield return '\u1e8c';
            yield return '\u1e8e';
            yield return '\u1e90';
            yield return '\u1e92';
            yield return '\u1e94';
            yield return '\u1e9e';
            yield return '\u1ea0';
            yield return '\u1ea2';
            yield return '\u1ea4';
            yield return '\u1ea6';
            yield return '\u1ea8';
            yield return '\u1eaa';
            yield return '\u1eac';
            yield return '\u1eae';
            yield return '\u1eb0';
            yield return '\u1eb2';
            yield return '\u1eb4';
            yield return '\u1eb6';
            yield return '\u1eb8';
            yield return '\u1eba';
            yield return '\u1ebc';
            yield return '\u1ebe';
            yield return '\u1ec0';
            yield return '\u1ec2';
            yield return '\u1ec4';
            yield return '\u1ec6';
            yield return '\u1ec8';
            yield return '\u1eca';
            yield return '\u1ecc';
            yield return '\u1ece';
            yield return '\u1ed0';
            yield return '\u1ed2';
            yield return '\u1ed4';
            yield return '\u1ed6';
            yield return '\u1ed8';
            yield return '\u1eda';
            yield return '\u1edc';
            yield return '\u1ede';
            yield return '\u1ee0';
            yield return '\u1ee2';
            yield return '\u1ee4';
            yield return '\u1ee6';
            yield return '\u1ee8';
            yield return '\u1eea';
            yield return '\u1eec';
            yield return '\u1eee';
            yield return '\u1ef0';
            yield return '\u1ef2';
            yield return '\u1ef4';
            yield return '\u1ef6';
            yield return '\u1ef8';
            yield return '\u1efa';
            yield return '\u1efc';
            yield return '\u1efe';
            for (char c = '\u1f08'; c <= '\u1f0f'; c++) yield return c;
            for (char c = '\u1f18'; c <= '\u1f1d'; c++) yield return c;
            for (char c = '\u1f28'; c <= '\u1f2f'; c++) yield return c;
            for (char c = '\u1f38'; c <= '\u1f3f'; c++) yield return c;
            for (char c = '\u1f48'; c <= '\u1f4d'; c++) yield return c;
            yield return '\u1f59';
            yield return '\u1f5b';
            yield return '\u1f5d';
            yield return '\u1f5f';
            for (char c = '\u1f68'; c <= '\u1f6f'; c++) yield return c;
            for (char c = '\u1fb8'; c <= '\u1fbb'; c++) yield return c;
            for (char c = '\u1fc8'; c <= '\u1fcb'; c++) yield return c;
            for (char c = '\u1fd8'; c <= '\u1fdb'; c++) yield return c;
            for (char c = '\u1fe8'; c <= '\u1fec'; c++) yield return c;
            for (char c = '\u1ff8'; c <= '\u1ffb'; c++) yield return c;
            yield return '\u2102';
            yield return '\u2107';
            for (char c = '\u210b'; c <= '\u210d'; c++) yield return c;
            for (char c = '\u2110'; c <= '\u2112'; c++) yield return c;
            yield return '\u2115';
            for (char c = '\u2119'; c <= '\u211d'; c++) yield return c;
            yield return '\u2124';
            yield return '\u2126';
            yield return '\u2128';
            for (char c = '\u212a'; c <= '\u212d'; c++) yield return c;
            for (char c = '\u2130'; c <= '\u2133'; c++) yield return c;
            yield return '\u213e';
            yield return '\u213f';
            yield return '\u2145';
            yield return '\u2183';
            for (char c = '\u2c00'; c <= '\u2c2f'; c++) yield return c;
            yield return '\u2c60';
            for (char c = '\u2c62'; c <= '\u2c64'; c++) yield return c;
            yield return '\u2c67';
            yield return '\u2c69';
            yield return '\u2c6b';
            for (char c = '\u2c6d'; c <= '\u2c70'; c++) yield return c;
            yield return '\u2c72';
            yield return '\u2c75';
            for (char c = '\u2c7e'; c <= '\u2c80'; c++) yield return c;
            yield return '\u2c82';
            yield return '\u2c84';
            yield return '\u2c86';
            yield return '\u2c88';
            yield return '\u2c8a';
            yield return '\u2c8c';
            yield return '\u2c8e';
            yield return '\u2c90';
            yield return '\u2c92';
            yield return '\u2c94';
            yield return '\u2c96';
            yield return '\u2c98';
            yield return '\u2c9a';
            yield return '\u2c9c';
            yield return '\u2c9e';
            yield return '\u2ca0';
            yield return '\u2ca2';
            yield return '\u2ca4';
            yield return '\u2ca6';
            yield return '\u2ca8';
            yield return '\u2caa';
            yield return '\u2cac';
            yield return '\u2cae';
            yield return '\u2cb0';
            yield return '\u2cb2';
            yield return '\u2cb4';
            yield return '\u2cb6';
            yield return '\u2cb8';
            yield return '\u2cba';
            yield return '\u2cbc';
            yield return '\u2cbe';
            yield return '\u2cc0';
            yield return '\u2cc2';
            yield return '\u2cc4';
            yield return '\u2cc6';
            yield return '\u2cc8';
            yield return '\u2cca';
            yield return '\u2ccc';
            yield return '\u2cce';
            yield return '\u2cd0';
            yield return '\u2cd2';
            yield return '\u2cd4';
            yield return '\u2cd6';
            yield return '\u2cd8';
            yield return '\u2cda';
            yield return '\u2cdc';
            yield return '\u2cde';
            yield return '\u2ce0';
            yield return '\u2ce2';
            yield return '\u2ceb';
            yield return '\u2ced';
            yield return '\u2cf2';
            yield return '\ua640';
            yield return '\ua642';
            yield return '\ua644';
            yield return '\ua646';
            yield return '\ua648';
            yield return '\ua64a';
            yield return '\ua64c';
            yield return '\ua64e';
            yield return '\ua650';
            yield return '\ua652';
            yield return '\ua654';
            yield return '\ua656';
            yield return '\ua658';
            yield return '\ua65a';
            yield return '\ua65c';
            yield return '\ua65e';
            yield return '\ua660';
            yield return '\ua662';
            yield return '\ua664';
            yield return '\ua666';
            yield return '\ua668';
            yield return '\ua66a';
            yield return '\ua66c';
            yield return '\ua680';
            yield return '\ua682';
            yield return '\ua684';
            yield return '\ua686';
            yield return '\ua688';
            yield return '\ua68a';
            yield return '\ua68c';
            yield return '\ua68e';
            yield return '\ua690';
            yield return '\ua692';
            yield return '\ua694';
            yield return '\ua696';
            yield return '\ua698';
            yield return '\ua69a';
            yield return '\ua722';
            yield return '\ua724';
            yield return '\ua726';
            yield return '\ua728';
            yield return '\ua72a';
            yield return '\ua72c';
            yield return '\ua72e';
            yield return '\ua732';
            yield return '\ua734';
            yield return '\ua736';
            yield return '\ua738';
            yield return '\ua73a';
            yield return '\ua73c';
            yield return '\ua73e';
            yield return '\ua740';
            yield return '\ua742';
            yield return '\ua744';
            yield return '\ua746';
            yield return '\ua748';
            yield return '\ua74a';
            yield return '\ua74c';
            yield return '\ua74e';
            yield return '\ua750';
            yield return '\ua752';
            yield return '\ua754';
            yield return '\ua756';
            yield return '\ua758';
            yield return '\ua75a';
            yield return '\ua75c';
            yield return '\ua75e';
            yield return '\ua760';
            yield return '\ua762';
            yield return '\ua764';
            yield return '\ua766';
            yield return '\ua768';
            yield return '\ua76a';
            yield return '\ua76c';
            yield return '\ua76e';
            yield return '\ua779';
            yield return '\ua77b';
            yield return '\ua77d';
            yield return '\ua77e';
            yield return '\ua780';
            yield return '\ua782';
            yield return '\ua784';
            yield return '\ua786';
            yield return '\ua78b';
            yield return '\ua78d';
            yield return '\ua790';
            yield return '\ua792';
            yield return '\ua796';
            yield return '\ua798';
            yield return '\ua79a';
            yield return '\ua79c';
            yield return '\ua79e';
            yield return '\ua7a0';
            yield return '\ua7a2';
            yield return '\ua7a4';
            yield return '\ua7a6';
            yield return '\ua7a8';
            for (char c = '\ua7aa'; c <= '\ua7ae'; c++) yield return c;
            for (char c = '\ua7b0'; c <= '\ua7b4'; c++) yield return c;
            yield return '\ua7b6';
            yield return '\ua7b8';
            yield return '\ua7ba';
            yield return '\ua7bc';
            yield return '\ua7be';
            yield return '\ua7c0';
            yield return '\ua7c2';
            for (char c = '\ua7c4'; c <= '\ua7c7'; c++) yield return c;
            yield return '\ua7c9';
            yield return '\ua7d0';
            yield return '\ua7d6';
            yield return '\ua7d8';
            yield return '\ua7f5';
            for (char c = '\uff21'; c <= '\uff3a'; c++) yield return c;
        }

        public static IEnumerable<char> GetLowerChars()
        {
            for (char c = 'a'; c <= '\u007a'; c++) yield return c;
            yield return '\u00b5';
            for (char c = '\u00df'; c <= '\u00f6'; c++) yield return c;
            for (char c = '\u00f8'; c <= '\u00ff'; c++) yield return c;
            yield return '\u0101';
            yield return '\u0103';
            yield return '\u0105';
            yield return '\u0107';
            yield return '\u0109';
            yield return '\u010b';
            yield return '\u010d';
            yield return '\u010f';
            yield return '\u0111';
            yield return '\u0113';
            yield return '\u0115';
            yield return '\u0117';
            yield return '\u0119';
            yield return '\u011b';
            yield return '\u011d';
            yield return '\u011f';
            yield return '\u0121';
            yield return '\u0123';
            yield return '\u0125';
            yield return '\u0127';
            yield return '\u0129';
            yield return '\u012b';
            yield return '\u012d';
            yield return '\u012f';
            yield return '\u0131';
            yield return '\u0133';
            yield return '\u0135';
            yield return '\u0137';
            yield return '\u0138';
            yield return '\u013a';
            yield return '\u013c';
            yield return '\u013e';
            yield return '\u0140';
            yield return '\u0142';
            yield return '\u0144';
            yield return '\u0146';
            yield return '\u0148';
            yield return '\u0149';
            yield return '\u014b';
            yield return '\u014d';
            yield return '\u014f';
            yield return '\u0151';
            yield return '\u0153';
            yield return '\u0155';
            yield return '\u0157';
            yield return '\u0159';
            yield return '\u015b';
            yield return '\u015d';
            yield return '\u015f';
            yield return '\u0161';
            yield return '\u0163';
            yield return '\u0165';
            yield return '\u0167';
            yield return '\u0169';
            yield return '\u016b';
            yield return '\u016d';
            yield return '\u016f';
            yield return '\u0171';
            yield return '\u0173';
            yield return '\u0175';
            yield return '\u0177';
            yield return '\u017a';
            yield return '\u017c';
            for (char c = '\u017e'; c <= '\u0180'; c++) yield return c;
            yield return '\u0183';
            yield return '\u0185';
            yield return '\u0188';
            yield return '\u018c';
            yield return '\u018d';
            yield return '\u0192';
            yield return '\u0195';
            for (char c = '\u0199'; c <= '\u019b'; c++) yield return c;
            yield return '\u019e';
            yield return '\u01a1';
            yield return '\u01a3';
            yield return '\u01a5';
            yield return '\u01a8';
            yield return '\u01aa';
            yield return '\u01ab';
            yield return '\u01ad';
            yield return '\u01b0';
            yield return '\u01b4';
            yield return '\u01b6';
            yield return '\u01b9';
            yield return '\u01ba';
            for (char c = '\u01bd'; c <= '\u01bf'; c++) yield return c;
            yield return '\u01c6';
            yield return '\u01c9';
            yield return '\u01cc';
            yield return '\u01ce';
            yield return '\u01d0';
            yield return '\u01d2';
            yield return '\u01d4';
            yield return '\u01d6';
            yield return '\u01d8';
            yield return '\u01da';
            yield return '\u01dc';
            yield return '\u01dd';
            yield return '\u01df';
            yield return '\u01e1';
            yield return '\u01e3';
            yield return '\u01e5';
            yield return '\u01e7';
            yield return '\u01e9';
            yield return '\u01eb';
            yield return '\u01ed';
            yield return '\u01ef';
            yield return '\u01f0';
            yield return '\u01f3';
            yield return '\u01f5';
            yield return '\u01f9';
            yield return '\u01fb';
            yield return '\u01fd';
            yield return '\u01ff';
            yield return '\u0201';
            yield return '\u0203';
            yield return '\u0205';
            yield return '\u0207';
            yield return '\u0209';
            yield return '\u020b';
            yield return '\u020d';
            yield return '\u020f';
            yield return '\u0211';
            yield return '\u0213';
            yield return '\u0215';
            yield return '\u0217';
            yield return '\u0219';
            yield return '\u021b';
            yield return '\u021d';
            yield return '\u021f';
            yield return '\u0221';
            yield return '\u0223';
            yield return '\u0225';
            yield return '\u0227';
            yield return '\u0229';
            yield return '\u022b';
            yield return '\u022d';
            yield return '\u022f';
            yield return '\u0231';
            for (char c = '\u0233'; c <= '\u0239'; c++) yield return c;
            yield return '\u023c';
            yield return '\u023f';
            yield return '\u0240';
            yield return '\u0242';
            yield return '\u0247';
            yield return '\u0249';
            yield return '\u024b';
            yield return '\u024d';
            for (char c = '\u024f'; c <= '\u0293'; c++) yield return c;
            for (char c = '\u0295'; c <= '\u02af'; c++) yield return c;
            yield return '\u0371';
            yield return '\u0373';
            yield return '\u0377';
            for (char c = '\u037b'; c <= '\u037d'; c++) yield return c;
            yield return '\u0390';
            for (char c = '\u03ac'; c <= '\u03ce'; c++) yield return c;
            yield return '\u03d0';
            yield return '\u03d1';
            for (char c = '\u03d5'; c <= '\u03d7'; c++) yield return c;
            yield return '\u03d9';
            yield return '\u03db';
            yield return '\u03dd';
            yield return '\u03df';
            yield return '\u03e1';
            yield return '\u03e3';
            yield return '\u03e5';
            yield return '\u03e7';
            yield return '\u03e9';
            yield return '\u03eb';
            yield return '\u03ed';
            for (char c = '\u03ef'; c <= '\u03f3'; c++) yield return c;
            yield return '\u03f5';
            yield return '\u03f8';
            yield return '\u03fb';
            yield return '\u03fc';
            for (char c = '\u0430'; c <= '\u045f'; c++) yield return c;
            yield return '\u0461';
            yield return '\u0463';
            yield return '\u0465';
            yield return '\u0467';
            yield return '\u0469';
            yield return '\u046b';
            yield return '\u046d';
            yield return '\u046f';
            yield return '\u0471';
            yield return '\u0473';
            yield return '\u0475';
            yield return '\u0477';
            yield return '\u0479';
            yield return '\u047b';
            yield return '\u047d';
            yield return '\u047f';
            yield return '\u0481';
            yield return '\u048b';
            yield return '\u048d';
            yield return '\u048f';
            yield return '\u0491';
            yield return '\u0493';
            yield return '\u0495';
            yield return '\u0497';
            yield return '\u0499';
            yield return '\u049b';
            yield return '\u049d';
            yield return '\u049f';
            yield return '\u04a1';
            yield return '\u04a3';
            yield return '\u04a5';
            yield return '\u04a7';
            yield return '\u04a9';
            yield return '\u04ab';
            yield return '\u04ad';
            yield return '\u04af';
            yield return '\u04b1';
            yield return '\u04b3';
            yield return '\u04b5';
            yield return '\u04b7';
            yield return '\u04b9';
            yield return '\u04bb';
            yield return '\u04bd';
            yield return '\u04bf';
            yield return '\u04c2';
            yield return '\u04c4';
            yield return '\u04c6';
            yield return '\u04c8';
            yield return '\u04ca';
            yield return '\u04cc';
            yield return '\u04ce';
            yield return '\u04cf';
            yield return '\u04d1';
            yield return '\u04d3';
            yield return '\u04d5';
            yield return '\u04d7';
            yield return '\u04d9';
            yield return '\u04db';
            yield return '\u04dd';
            yield return '\u04df';
            yield return '\u04e1';
            yield return '\u04e3';
            yield return '\u04e5';
            yield return '\u04e7';
            yield return '\u04e9';
            yield return '\u04eb';
            yield return '\u04ed';
            yield return '\u04ef';
            yield return '\u04f1';
            yield return '\u04f3';
            yield return '\u04f5';
            yield return '\u04f7';
            yield return '\u04f9';
            yield return '\u04fb';
            yield return '\u04fd';
            yield return '\u04ff';
            yield return '\u0501';
            yield return '\u0503';
            yield return '\u0505';
            yield return '\u0507';
            yield return '\u0509';
            yield return '\u050b';
            yield return '\u050d';
            yield return '\u050f';
            yield return '\u0511';
            yield return '\u0513';
            yield return '\u0515';
            yield return '\u0517';
            yield return '\u0519';
            yield return '\u051b';
            yield return '\u051d';
            yield return '\u051f';
            yield return '\u0521';
            yield return '\u0523';
            yield return '\u0525';
            yield return '\u0527';
            yield return '\u0529';
            yield return '\u052b';
            yield return '\u052d';
            yield return '\u052f';
            for (char c = '\u0560'; c <= '\u0588'; c++) yield return c;
            for (char c = '\u10d0'; c <= '\u10fa'; c++) yield return c;
            for (char c = '\u10fd'; c <= '\u10ff'; c++) yield return c;
            for (char c = '\u13f8'; c <= '\u13fd'; c++) yield return c;
            for (char c = '\u1c80'; c <= '\u1c88'; c++) yield return c;
            for (char c = '\u1d00'; c <= '\u1d2b'; c++) yield return c;
            for (char c = '\u1d6b'; c <= '\u1d77'; c++) yield return c;
            for (char c = '\u1d79'; c <= '\u1d9a'; c++) yield return c;
            yield return '\u1e01';
            yield return '\u1e03';
            yield return '\u1e05';
            yield return '\u1e07';
            yield return '\u1e09';
            yield return '\u1e0b';
            yield return '\u1e0d';
            yield return '\u1e0f';
            yield return '\u1e11';
            yield return '\u1e13';
            yield return '\u1e15';
            yield return '\u1e17';
            yield return '\u1e19';
            yield return '\u1e1b';
            yield return '\u1e1d';
            yield return '\u1e1f';
            yield return '\u1e21';
            yield return '\u1e23';
            yield return '\u1e25';
            yield return '\u1e27';
            yield return '\u1e29';
            yield return '\u1e2b';
            yield return '\u1e2d';
            yield return '\u1e2f';
            yield return '\u1e31';
            yield return '\u1e33';
            yield return '\u1e35';
            yield return '\u1e37';
            yield return '\u1e39';
            yield return '\u1e3b';
            yield return '\u1e3d';
            yield return '\u1e3f';
            yield return '\u1e41';
            yield return '\u1e43';
            yield return '\u1e45';
            yield return '\u1e47';
            yield return '\u1e49';
            yield return '\u1e4b';
            yield return '\u1e4d';
            yield return '\u1e4f';
            yield return '\u1e51';
            yield return '\u1e53';
            yield return '\u1e55';
            yield return '\u1e57';
            yield return '\u1e59';
            yield return '\u1e5b';
            yield return '\u1e5d';
            yield return '\u1e5f';
            yield return '\u1e61';
            yield return '\u1e63';
            yield return '\u1e65';
            yield return '\u1e67';
            yield return '\u1e69';
            yield return '\u1e6b';
            yield return '\u1e6d';
            yield return '\u1e6f';
            yield return '\u1e71';
            yield return '\u1e73';
            yield return '\u1e75';
            yield return '\u1e77';
            yield return '\u1e79';
            yield return '\u1e7b';
            yield return '\u1e7d';
            yield return '\u1e7f';
            yield return '\u1e81';
            yield return '\u1e83';
            yield return '\u1e85';
            yield return '\u1e87';
            yield return '\u1e89';
            yield return '\u1e8b';
            yield return '\u1e8d';
            yield return '\u1e8f';
            yield return '\u1e91';
            yield return '\u1e93';
            for (char c = '\u1e95'; c <= '\u1e9d'; c++) yield return c;
            yield return '\u1e9f';
            yield return '\u1ea1';
            yield return '\u1ea3';
            yield return '\u1ea5';
            yield return '\u1ea7';
            yield return '\u1ea9';
            yield return '\u1eab';
            yield return '\u1ead';
            yield return '\u1eaf';
            yield return '\u1eb1';
            yield return '\u1eb3';
            yield return '\u1eb5';
            yield return '\u1eb7';
            yield return '\u1eb9';
            yield return '\u1ebb';
            yield return '\u1ebd';
            yield return '\u1ebf';
            yield return '\u1ec1';
            yield return '\u1ec3';
            yield return '\u1ec5';
            yield return '\u1ec7';
            yield return '\u1ec9';
            yield return '\u1ecb';
            yield return '\u1ecd';
            yield return '\u1ecf';
            yield return '\u1ed1';
            yield return '\u1ed3';
            yield return '\u1ed5';
            yield return '\u1ed7';
            yield return '\u1ed9';
            yield return '\u1edb';
            yield return '\u1edd';
            yield return '\u1edf';
            yield return '\u1ee1';
            yield return '\u1ee3';
            yield return '\u1ee5';
            yield return '\u1ee7';
            yield return '\u1ee9';
            yield return '\u1eeb';
            yield return '\u1eed';
            yield return '\u1eef';
            yield return '\u1ef1';
            yield return '\u1ef3';
            yield return '\u1ef5';
            yield return '\u1ef7';
            yield return '\u1ef9';
            yield return '\u1efb';
            yield return '\u1efd';
            for (char c = '\u1eff'; c <= '\u1f07'; c++) yield return c;
            for (char c = '\u1f10'; c <= '\u1f15'; c++) yield return c;
            for (char c = '\u1f20'; c <= '\u1f27'; c++) yield return c;
            for (char c = '\u1f30'; c <= '\u1f37'; c++) yield return c;
            for (char c = '\u1f40'; c <= '\u1f45'; c++) yield return c;
            for (char c = '\u1f50'; c <= '\u1f57'; c++) yield return c;
            for (char c = '\u1f60'; c <= '\u1f67'; c++) yield return c;
            for (char c = '\u1f70'; c <= '\u1f7d'; c++) yield return c;
            for (char c = '\u1f80'; c <= '\u1f87'; c++) yield return c;
            for (char c = '\u1f90'; c <= '\u1f97'; c++) yield return c;
            for (char c = '\u1fa0'; c <= '\u1fa7'; c++) yield return c;
            for (char c = '\u1fb0'; c <= '\u1fb4'; c++) yield return c;
            yield return '\u1fb6';
            yield return '\u1fb7';
            yield return '\u1fbe';
            for (char c = '\u1fc2'; c <= '\u1fc4'; c++) yield return c;
            yield return '\u1fc6';
            yield return '\u1fc7';
            for (char c = '\u1fd0'; c <= '\u1fd3'; c++) yield return c;
            yield return '\u1fd6';
            yield return '\u1fd7';
            for (char c = '\u1fe0'; c <= '\u1fe7'; c++) yield return c;
            for (char c = '\u1ff2'; c <= '\u1ff4'; c++) yield return c;
            yield return '\u1ff6';
            yield return '\u1ff7';
            yield return '\u210a';
            yield return '\u210e';
            yield return '\u210f';
            yield return '\u2113';
            yield return '\u212f';
            yield return '\u2134';
            yield return '\u2139';
            yield return '\u213c';
            yield return '\u213d';
            for (char c = '\u2146'; c <= '\u2149'; c++) yield return c;
            yield return '\u214e';
            yield return '\u2184';
            for (char c = '\u2c30'; c <= '\u2c5f'; c++) yield return c;
            yield return '\u2c61';
            yield return '\u2c65';
            yield return '\u2c66';
            yield return '\u2c68';
            yield return '\u2c6a';
            yield return '\u2c6c';
            yield return '\u2c71';
            yield return '\u2c73';
            yield return '\u2c74';
            for (char c = '\u2c76'; c <= '\u2c7b'; c++) yield return c;
            yield return '\u2c81';
            yield return '\u2c83';
            yield return '\u2c85';
            yield return '\u2c87';
            yield return '\u2c89';
            yield return '\u2c8b';
            yield return '\u2c8d';
            yield return '\u2c8f';
            yield return '\u2c91';
            yield return '\u2c93';
            yield return '\u2c95';
            yield return '\u2c97';
            yield return '\u2c99';
            yield return '\u2c9b';
            yield return '\u2c9d';
            yield return '\u2c9f';
            yield return '\u2ca1';
            yield return '\u2ca3';
            yield return '\u2ca5';
            yield return '\u2ca7';
            yield return '\u2ca9';
            yield return '\u2cab';
            yield return '\u2cad';
            yield return '\u2caf';
            yield return '\u2cb1';
            yield return '\u2cb3';
            yield return '\u2cb5';
            yield return '\u2cb7';
            yield return '\u2cb9';
            yield return '\u2cbb';
            yield return '\u2cbd';
            yield return '\u2cbf';
            yield return '\u2cc1';
            yield return '\u2cc3';
            yield return '\u2cc5';
            yield return '\u2cc7';
            yield return '\u2cc9';
            yield return '\u2ccb';
            yield return '\u2ccd';
            yield return '\u2ccf';
            yield return '\u2cd1';
            yield return '\u2cd3';
            yield return '\u2cd5';
            yield return '\u2cd7';
            yield return '\u2cd9';
            yield return '\u2cdb';
            yield return '\u2cdd';
            yield return '\u2cdf';
            yield return '\u2ce1';
            yield return '\u2ce3';
            yield return '\u2ce4';
            yield return '\u2cec';
            yield return '\u2cee';
            yield return '\u2cf3';
            for (char c = '\u2d00'; c <= '\u2d25'; c++) yield return c;
            yield return '\u2d27';
            yield return '\u2d2d';
            yield return '\ua641';
            yield return '\ua643';
            yield return '\ua645';
            yield return '\ua647';
            yield return '\ua649';
            yield return '\ua64b';
            yield return '\ua64d';
            yield return '\ua64f';
            yield return '\ua651';
            yield return '\ua653';
            yield return '\ua655';
            yield return '\ua657';
            yield return '\ua659';
            yield return '\ua65b';
            yield return '\ua65d';
            yield return '\ua65f';
            yield return '\ua661';
            yield return '\ua663';
            yield return '\ua665';
            yield return '\ua667';
            yield return '\ua669';
            yield return '\ua66b';
            yield return '\ua66d';
            yield return '\ua681';
            yield return '\ua683';
            yield return '\ua685';
            yield return '\ua687';
            yield return '\ua689';
            yield return '\ua68b';
            yield return '\ua68d';
            yield return '\ua68f';
            yield return '\ua691';
            yield return '\ua693';
            yield return '\ua695';
            yield return '\ua697';
            yield return '\ua699';
            yield return '\ua69b';
            yield return '\ua723';
            yield return '\ua725';
            yield return '\ua727';
            yield return '\ua729';
            yield return '\ua72b';
            yield return '\ua72d';
            for (char c = '\ua72f'; c <= '\ua731'; c++) yield return c;
            yield return '\ua733';
            yield return '\ua735';
            yield return '\ua737';
            yield return '\ua739';
            yield return '\ua73b';
            yield return '\ua73d';
            yield return '\ua73f';
            yield return '\ua741';
            yield return '\ua743';
            yield return '\ua745';
            yield return '\ua747';
            yield return '\ua749';
            yield return '\ua74b';
            yield return '\ua74d';
            yield return '\ua74f';
            yield return '\ua751';
            yield return '\ua753';
            yield return '\ua755';
            yield return '\ua757';
            yield return '\ua759';
            yield return '\ua75b';
            yield return '\ua75d';
            yield return '\ua75f';
            yield return '\ua761';
            yield return '\ua763';
            yield return '\ua765';
            yield return '\ua767';
            yield return '\ua769';
            yield return '\ua76b';
            yield return '\ua76d';
            yield return '\ua76f';
            for (char c = '\ua771'; c <= '\ua778'; c++) yield return c;
            yield return '\ua77a';
            yield return '\ua77c';
            yield return '\ua77f';
            yield return '\ua781';
            yield return '\ua783';
            yield return '\ua785';
            yield return '\ua787';
            yield return '\ua78c';
            yield return '\ua78e';
            yield return '\ua791';
            for (char c = '\ua793'; c <= '\ua795'; c++) yield return c;
            yield return '\ua797';
            yield return '\ua799';
            yield return '\ua79b';
            yield return '\ua79d';
            yield return '\ua79f';
            yield return '\ua7a1';
            yield return '\ua7a3';
            yield return '\ua7a5';
            yield return '\ua7a7';
            yield return '\ua7a9';
            yield return '\ua7af';
            yield return '\ua7b5';
            yield return '\ua7b7';
            yield return '\ua7b9';
            yield return '\ua7bb';
            yield return '\ua7bd';
            yield return '\ua7bf';
            yield return '\ua7c1';
            yield return '\ua7c3';
            yield return '\ua7c8';
            yield return '\ua7ca';
            yield return '\ua7d1';
            yield return '\ua7d3';
            yield return '\ua7d5';
            yield return '\ua7d7';
            yield return '\ua7d9';
            yield return '\ua7f6';
            yield return '\ua7fa';
            for (char c = '\uab30'; c <= '\uab5a'; c++) yield return c;
            for (char c = '\uab60'; c <= '\uab68'; c++) yield return c;
            for (char c = '\uab70'; c <= '\uabbf'; c++) yield return c;
            for (char c = '\ufb00'; c <= '\ufb06'; c++) yield return c;
            for (char c = '\ufb13'; c <= '\ufb17'; c++) yield return c;
            for (char c = '\uff41'; c <= '\uff5a'; c++) yield return c;
        }

        public static IEnumerable<char> GetNumbers()
        {
            foreach (char c in GetDigits()) yield return c;
            yield return '\u00b2';
            yield return '\u00b3';
            yield return '\u00b9';
            for (char c = '\u00bc'; c <= '\u00be'; c++) yield return c;
            for (char c = '\u09f4'; c <= '\u09f9'; c++) yield return c;
            for (char c = '\u0b72'; c <= '\u0b77'; c++) yield return c;
            for (char c = '\u0bf0'; c <= '\u0bf2'; c++) yield return c;
            for (char c = '\u0c78'; c <= '\u0c7e'; c++) yield return c;
            for (char c = '\u0d58'; c <= '\u0d5e'; c++) yield return c;
            for (char c = '\u0d70'; c <= '\u0d78'; c++) yield return c;
            for (char c = '\u0f2a'; c <= '\u0f33'; c++) yield return c;
            for (char c = '\u1369'; c <= '\u137c'; c++) yield return c;
            for (char c = '\u16ee'; c <= '\u16f0'; c++) yield return c;
            for (char c = '\u17f0'; c <= '\u17f9'; c++) yield return c;
            yield return '\u19da';
            yield return '\u2070';
            for (char c = '\u2074'; c <= '\u2079'; c++) yield return c;
            for (char c = '\u2080'; c <= '\u2089'; c++) yield return c;
            for (char c = '\u2150'; c <= '\u2182'; c++) yield return c;
            for (char c = '\u2185'; c <= '\u2189'; c++) yield return c;
            for (char c = '\u2460'; c <= '\u249b'; c++) yield return c;
            for (char c = '\u24ea'; c <= '\u24ff'; c++) yield return c;
            for (char c = '\u2776'; c <= '\u2793'; c++) yield return c;
            yield return '\u2cfd';
            yield return '\u3007';
            for (char c = '\u3021'; c <= '\u3029'; c++) yield return c;
            for (char c = '\u3038'; c <= '\u303a'; c++) yield return c;
            for (char c = '\u3192'; c <= '\u3195'; c++) yield return c;
            for (char c = '\u3220'; c <= '\u3229'; c++) yield return c;
            for (char c = '\u3248'; c <= '\u324f'; c++) yield return c;
            for (char c = '\u3251'; c <= '\u325f'; c++) yield return c;
            for (char c = '\u3280'; c <= '\u3289'; c++) yield return c;
            for (char c = '\u32b1'; c <= '\u32bf'; c++) yield return c;
            for (char c = '\ua6e6'; c <= '\ua6ef'; c++) yield return c;
            for (char c = '\ua830'; c <= '\ua835'; c++) yield return c;
        }

        public static IEnumerable<char> GetDigits()
        {
            for (char c = '0'; c <= '9'; c++) yield return c;
            for (char c = '\u0660'; c <= '\u0669'; c++) yield return c;
            for (char c = '\u06f0'; c <= '\u06f9'; c++) yield return c;
            for (char c = '\u07c0'; c <= '\u07c9'; c++) yield return c;
            for (char c = '\u0966'; c <= '\u096f'; c++) yield return c;
            for (char c = '\u09e6'; c <= '\u09ef'; c++) yield return c;
            for (char c = '\u0a66'; c <= '\u0a6f'; c++) yield return c;
            for (char c = '\u0ae6'; c <= '\u0aef'; c++) yield return c;
            for (char c = '\u0b66'; c <= '\u0b6f'; c++) yield return c;
            for (char c = '\u0be6'; c <= '\u0bef'; c++) yield return c;
            for (char c = '\u0c66'; c <= '\u0c6f'; c++) yield return c;
            for (char c = '\u0ce6'; c <= '\u0cef'; c++) yield return c;
            for (char c = '\u0d66'; c <= '\u0d6f'; c++) yield return c;
            for (char c = '\u0de6'; c <= '\u0def'; c++) yield return c;
            for (char c = '\u0e50'; c <= '\u0e59'; c++) yield return c;
            for (char c = '\u0ed0'; c <= '\u0ed9'; c++) yield return c;
            for (char c = '\u0f20'; c <= '\u0f29'; c++) yield return c;
            for (char c = '\u1040'; c <= '\u1049'; c++) yield return c;
            for (char c = '\u1090'; c <= '\u1099'; c++) yield return c;
            for (char c = '\u17e0'; c <= '\u17e9'; c++) yield return c;
            for (char c = '\u1810'; c <= '\u1819'; c++) yield return c;
            for (char c = '\u1946'; c <= '\u194f'; c++) yield return c;
            for (char c = '\u19d0'; c <= '\u19d9'; c++) yield return c;
            for (char c = '\u1a80'; c <= '\u1a89'; c++) yield return c;
            for (char c = '\u1a90'; c <= '\u1a99'; c++) yield return c;
            for (char c = '\u1b50'; c <= '\u1b59'; c++) yield return c;
            for (char c = '\u1bb0'; c <= '\u1bb9'; c++) yield return c;
            for (char c = '\u1c40'; c <= '\u1c49'; c++) yield return c;
            for (char c = '\u1c50'; c <= '\u1c59'; c++) yield return c;
            for (char c = '\ua620'; c <= '\ua629'; c++) yield return c;
            for (char c = '\ua8d0'; c <= '\ua8d9'; c++) yield return c;
            for (char c = '\ua900'; c <= '\ua909'; c++) yield return c;
            for (char c = '\ua9d0'; c <= '\ua9d9'; c++) yield return c;
            for (char c = '\ua9f0'; c <= '\ua9f9'; c++) yield return c;
            for (char c = '\uaa50'; c <= '\uaa59'; c++) yield return c;
            for (char c = '\uabf0'; c <= '\uabf9'; c++) yield return c;
            for (char c = '\uff10'; c <= '\uff19'; c++) yield return c;
            
        }

        public static IEnumerable<char> GetAsciiPunctuation()
        {
            for (char c = '!'; c <= '#'; c++) yield return c;
            for (char c = '%'; c <= '*'; c++) yield return c;
            for (char c = ','; c <= '/'; c++) yield return c;
            yield return ':';
            yield return ';';
            yield return '?';
            yield return '&';
            for (char c = '['; c <= ']'; c++) yield return c;
            yield return '_';
            yield return '{';
            yield return '}';
        }
        
        public static IEnumerable<char> GetPunctuationChars()
        {
            foreach (char c in GetAsciiPunctuation()) yield return c;
            yield return '\u00a1';
            yield return '\u00a7';
            yield return '\u00ab';
            yield return '\u00b6';
            yield return '\u00b7';
            yield return '\u00bb';
            yield return '\u00bf';
            yield return '\u037e';
            yield return '\u0387';
            for (char c = '\u055a'; c <= '\u055f'; c++) yield return c;
            yield return '\u0589';
            yield return '\u058a';
            yield return '\u05be';
            yield return '\u05c0';
            yield return '\u05c3';
            yield return '\u05c6';
            yield return '\u05f3';
            yield return '\u05f4';
            yield return '\u0609';
            yield return '\u060a';
            yield return '\u060c';
            yield return '\u060d';
            yield return '\u061b';
            for (char c = '\u061d'; c <= '\u061f'; c++) yield return c;
            for (char c = '\u066a'; c <= '\u066d'; c++) yield return c;
            yield return '\u06d4';
            for (char c = '\u0700'; c <= '\u070d'; c++) yield return c;
            for (char c = '\u07f7'; c <= '\u07f9'; c++) yield return c;
            for (char c = '\u0830'; c <= '\u083e'; c++) yield return c;
            yield return '\u085e';
            yield return '\u0964';
            yield return '\u0965';
            yield return '\u0970';
            yield return '\u09fd';
            yield return '\u0a76';
            yield return '\u0af0';
            yield return '\u0c77';
            yield return '\u0c84';
            yield return '\u0df4';
            yield return '\u0e4f';
            yield return '\u0e5a';
            yield return '\u0e5b';
            for (char c = '\u0f04'; c <= '\u0f12'; c++) yield return c;
            yield return '\u0f14';
            for (char c = '\u0f3a'; c <= '\u0f3d'; c++) yield return c;
            yield return '\u0f85';
            for (char c = '\u0fd0'; c <= '\u0fd4'; c++) yield return c;
            yield return '\u0fd9';
            yield return '\u0fda';
            for (char c = '\u104a'; c <= '\u104f'; c++) yield return c;
            yield return '\u10fb';
            for (char c = '\u1360'; c <= '\u1368'; c++) yield return c;
            yield return '\u1400';
            yield return '\u166e';
            yield return '\u169b';
            yield return '\u169c';
            for (char c = '\u16eb'; c <= '\u16ed'; c++) yield return c;
            yield return '\u1735';
            yield return '\u1736';
            for (char c = '\u17d4'; c <= '\u17d6'; c++) yield return c;
            for (char c = '\u17d8'; c <= '\u17da'; c++) yield return c;
            for (char c = '\u1800'; c <= '\u180a'; c++) yield return c;
            yield return '\u1944';
            yield return '\u1945';
            yield return '\u1a1e';
            yield return '\u1a1f';
            for (char c = '\u1aa0'; c <= '\u1aa6'; c++) yield return c;
            for (char c = '\u1aa8'; c <= '\u1aad'; c++) yield return c;
            for (char c = '\u1b5a'; c <= '\u1b60'; c++) yield return c;
            yield return '\u1b7d';
            yield return '\u1b7e';
            for (char c = '\u1bfc'; c <= '\u1bff'; c++) yield return c;
            for (char c = '\u1c3b'; c <= '\u1c3f'; c++) yield return c;
            yield return '\u1c7e';
            yield return '\u1c7f';
            for (char c = '\u1cc0'; c <= '\u1cc7'; c++) yield return c;
            yield return '\u1cd3';
            for (char c = '\u2010'; c <= '\u2027'; c++) yield return c;
            for (char c = '\u2030'; c <= '\u2043'; c++) yield return c;
            for (char c = '\u2045'; c <= '\u2051'; c++) yield return c;
            for (char c = '\u2053'; c <= '\u205e'; c++) yield return c;
            yield return '\u207d';
            yield return '\u207e';
            yield return '\u208d';
            yield return '\u208e';
            for (char c = '\u2308'; c <= '\u230b'; c++) yield return c;
            yield return '\u2329';
            yield return '\u232a';
            for (char c = '\u2768'; c <= '\u2775'; c++) yield return c;
            yield return '\u27c5';
            yield return '\u27c6';
            for (char c = '\u27e6'; c <= '\u27ef'; c++) yield return c;
            for (char c = '\u2983'; c <= '\u2998'; c++) yield return c;
            for (char c = '\u29d8'; c <= '\u29db'; c++) yield return c;
            yield return '\u29fc';
            yield return '\u29fd';
            for (char c = '\u2cf9'; c <= '\u2cfc'; c++) yield return c;
            yield return '\u2cfe';
            yield return '\u2cff';
            yield return '\u2d70';
            for (char c = '\u2e00'; c <= '\u2e2e'; c++) yield return c;
            for (char c = '\u2e30'; c <= '\u2e4f'; c++) yield return c;
            for (char c = '\u2e52'; c <= '\u2e5d'; c++) yield return c;
            for (char c = '\u3001'; c <= '\u3003'; c++) yield return c;
            for (char c = '\u3008'; c <= '\u3011'; c++) yield return c;
            for (char c = '\u3014'; c <= '\u301f'; c++) yield return c;
            yield return '\u3030';
            yield return '\u303d';
            yield return '\u30a0';
            yield return '\u30fb';
            yield return '\ua4fe';
            yield return '\ua4ff';
            for (char c = '\ua60d'; c <= '\ua60f'; c++) yield return c;
            yield return '\ua673';
            yield return '\ua67e';
            for (char c = '\ua6f2'; c <= '\ua6f7'; c++) yield return c;
            for (char c = '\ua874'; c <= '\ua877'; c++) yield return c;
            yield return '\ua8ce';
            yield return '\ua8cf';
            for (char c = '\ua8f8'; c <= '\ua8fa'; c++) yield return c;
            yield return '\ua8fc';
            yield return '\ua92e';
            yield return '\ua92f';
            yield return '\ua95f';
            for (char c = '\ua9c1'; c <= '\ua9cd'; c++) yield return c;
            yield return '\ua9de';
            yield return '\ua9df';
            for (char c = '\uaa5c'; c <= '\uaa5f'; c++) yield return c;
            yield return '\uaade';
            yield return '\uaadf';
            yield return '\uaaf0';
            yield return '\uaaf1';
            yield return '\uabeb';
            yield return '\ufd3e';
            yield return '\ufd3f';
            for (char c = '\ufe10'; c <= '\ufe19'; c++) yield return c;
            for (char c = '\ufe30'; c <= '\ufe52'; c++) yield return c;
            for (char c = '\ufe54'; c <= '\ufe61'; c++) yield return c;
            yield return '\ufe63';
            yield return '\ufe68';
            yield return '\ufe6a';
            yield return '\ufe6b';
            for (char c = '\uff01'; c <= '\uff03'; c++) yield return c;
            for (char c = '\uff05'; c <= '\uff0a'; c++) yield return c;
            for (char c = '\uff0c'; c <= '\uff0f'; c++) yield return c;
            yield return '\uff1a';
            yield return '\uff1b';
            yield return '\uff1f';
            yield return '\uff20';
            for (char c = '\uff3b'; c <= '\uff3d'; c++) yield return c;
            yield return '\uff3f';
            yield return '\uff5b';
            yield return '\uff5d';
            for (char c = '\uff5f'; c <= '\uff65'; c++) yield return c;
        }

        public static IEnumerable<char> GetAsciiSymbols()
        {
            yield return '$';
            yield return '+';
            for (char c = '<'; c <= '>'; c++) yield return c;
            yield return '^';
            yield return '`';
        }

        public static IEnumerable<char> GetSymbols()
        {
            foreach (char c in GetAsciiSymbols()) yield return c;
            yield return '\u007c';
            yield return '\u007e';
            for (char c = '\u00a2'; c <= '\u00a6'; c++) yield return c;
            yield return '\u00a8';
            yield return '\u00a9';
            yield return '\u00ac';
            for (char c = '\u00ae'; c <= '\u00b1'; c++) yield return c;
            yield return '\u00b4';
            yield return '\u00b8';
            yield return '\u00d7';
            yield return '\u00f7';
            for (char c = '\u02c2'; c <= '\u02c5'; c++) yield return c;
            for (char c = '\u02d2'; c <= '\u02df'; c++) yield return c;
            for (char c = '\u02e5'; c <= '\u02eb'; c++) yield return c;
            yield return '\u02ed';
            for (char c = '\u02ef'; c <= '\u02ff'; c++) yield return c;
            yield return '\u0375';
            yield return '\u0384';
            yield return '\u0385';
            yield return '\u03f6';
            yield return '\u0482';
            for (char c = '\u058d'; c <= '\u058f'; c++) yield return c;
            for (char c = '\u0606'; c <= '\u0608'; c++) yield return c;
            yield return '\u060b';
            yield return '\u060e';
            yield return '\u060f';
            yield return '\u06de';
            yield return '\u06e9';
            yield return '\u06fd';
            yield return '\u06fe';
            yield return '\u07f6';
            yield return '\u07fe';
            yield return '\u07ff';
            yield return '\u0888';
            yield return '\u09f2';
            yield return '\u09f3';
            yield return '\u09fa';
            yield return '\u09fb';
            yield return '\u0af1';
            yield return '\u0b70';
            for (char c = '\u0bf3'; c <= '\u0bfa'; c++) yield return c;
            yield return '\u0c7f';
            yield return '\u0d4f';
            yield return '\u0d79';
            yield return '\u0e3f';
            for (char c = '\u0f01'; c <= '\u0f03'; c++) yield return c;
            yield return '\u0f13';
            for (char c = '\u0f15'; c <= '\u0f17'; c++) yield return c;
            for (char c = '\u0f1a'; c <= '\u0f1f'; c++) yield return c;
            yield return '\u0f34';
            yield return '\u0f36';
            yield return '\u0f38';
            for (char c = '\u0fbe'; c <= '\u0fc5'; c++) yield return c;
            for (char c = '\u0fc7'; c <= '\u0fcc'; c++) yield return c;
            yield return '\u0fce';
            yield return '\u0fcf';
            for (char c = '\u0fd5'; c <= '\u0fd8'; c++) yield return c;
            yield return '\u109e';
            yield return '\u109f';
            for (char c = '\u1390'; c <= '\u1399'; c++) yield return c;
            yield return '\u166d';
            yield return '\u17db';
            yield return '\u1940';
            for (char c = '\u19de'; c <= '\u19ff'; c++) yield return c;
            for (char c = '\u1b61'; c <= '\u1b6a'; c++) yield return c;
            for (char c = '\u1b74'; c <= '\u1b7c'; c++) yield return c;
            yield return '\u1fbd';
            for (char c = '\u1fbf'; c <= '\u1fc1'; c++) yield return c;
            for (char c = '\u1fcd'; c <= '\u1fcf'; c++) yield return c;
            for (char c = '\u1fdd'; c <= '\u1fdf'; c++) yield return c;
            for (char c = '\u1fed'; c <= '\u1fef'; c++) yield return c;
            yield return '\u1ffd';
            yield return '\u1ffe';
            yield return '\u2044';
            yield return '\u2052';
            for (char c = '\u207a'; c <= '\u207c'; c++) yield return c;
            for (char c = '\u208a'; c <= '\u208c'; c++) yield return c;
            for (char c = '\u20a0'; c <= '\u20c0'; c++) yield return c;
            yield return '\u2100';
            yield return '\u2101';
            for (char c = '\u2103'; c <= '\u2106'; c++) yield return c;
            yield return '\u2108';
            yield return '\u2109';
            yield return '\u2114';
            for (char c = '\u2116'; c <= '\u2118'; c++) yield return c;
            for (char c = '\u211e'; c <= '\u2123'; c++) yield return c;
            yield return '\u2125';
            yield return '\u2127';
            yield return '\u2129';
            yield return '\u212e';
            yield return '\u213a';
            yield return '\u213b';
            for (char c = '\u2140'; c <= '\u2144'; c++) yield return c;
            for (char c = '\u214a'; c <= '\u214d'; c++) yield return c;
            yield return '\u214f';
            yield return '\u218a';
            yield return '\u218b';
            for (char c = '\u2190'; c <= '\u2307'; c++) yield return c;
            for (char c = '\u230c'; c <= '\u2328'; c++) yield return c;
            for (char c = '\u232b'; c <= '\u2426'; c++) yield return c;
            for (char c = '\u2440'; c <= '\u244a'; c++) yield return c;
            for (char c = '\u249c'; c <= '\u24e9'; c++) yield return c;
            for (char c = '\u2500'; c <= '\u2767'; c++) yield return c;
            for (char c = '\u2794'; c <= '\u27c4'; c++) yield return c;
            for (char c = '\u27c7'; c <= '\u27e5'; c++) yield return c;
            for (char c = '\u27f0'; c <= '\u2982'; c++) yield return c;
            for (char c = '\u2999'; c <= '\u29d7'; c++) yield return c;
            for (char c = '\u29dc'; c <= '\u29fb'; c++) yield return c;
            for (char c = '\u29fe'; c <= '\u2b73'; c++) yield return c;
            for (char c = '\u2b76'; c <= '\u2b95'; c++) yield return c;
            for (char c = '\u2b97'; c <= '\u2bff'; c++) yield return c;
            for (char c = '\u2ce5'; c <= '\u2cea'; c++) yield return c;
            yield return '\u2e50';
            yield return '\u2e51';
            for (char c = '\u2e80'; c <= '\u2e99'; c++) yield return c;
            for (char c = '\u2e9b'; c <= '\u2ef3'; c++) yield return c;
            for (char c = '\u2f00'; c <= '\u2fd5'; c++) yield return c;
            for (char c = '\u2ff0'; c <= '\u2ffb'; c++) yield return c;
            yield return '\u3004';
            yield return '\u3012';
            yield return '\u3013';
            yield return '\u3020';
            yield return '\u3036';
            yield return '\u3037';
            yield return '\u303e';
            yield return '\u303f';
            yield return '\u309b';
            yield return '\u309c';
            yield return '\u3190';
            yield return '\u3191';
            for (char c = '\u3196'; c <= '\u319f'; c++) yield return c;
            for (char c = '\u31c0'; c <= '\u31e3'; c++) yield return c;
            for (char c = '\u3200'; c <= '\u321e'; c++) yield return c;
            for (char c = '\u322a'; c <= '\u3247'; c++) yield return c;
            yield return '\u3250';
            for (char c = '\u3260'; c <= '\u327f'; c++) yield return c;
            for (char c = '\u328a'; c <= '\u32b0'; c++) yield return c;
            for (char c = '\u32c0'; c <= '\u33ff'; c++) yield return c;
            for (char c = '\u4dc0'; c <= '\u4dff'; c++) yield return c;
            for (char c = '\ua490'; c <= '\ua4c6'; c++) yield return c;
            for (char c = '\ua700'; c <= '\ua716'; c++) yield return c;
            yield return '\ua720';
            yield return '\ua721';
            yield return '\ua789';
            yield return '\ua78a';
            for (char c = '\ua828'; c <= '\ua82b'; c++) yield return c;
            for (char c = '\ua836'; c <= '\ua839'; c++) yield return c;
            for (char c = '\uaa77'; c <= '\uaa79'; c++) yield return c;
            yield return '\uab5b';
            yield return '\uab6a';
            yield return '\uab6b';
            yield return '\ufb29';
            for (char c = '\ufbb2'; c <= '\ufbc2'; c++) yield return c;
            for (char c = '\ufd40'; c <= '\ufd4f'; c++) yield return c;
            yield return '\ufdcf';
            for (char c = '\ufdfc'; c <= '\ufdff'; c++) yield return c;
            yield return '\ufe62';
            for (char c = '\ufe64'; c <= '\ufe66'; c++) yield return c;
            yield return '\ufe69';
            yield return '\uff04';
            yield return '\uff0b';
            for (char c = '\uff1c'; c <= '\uff1e'; c++) yield return c;
            yield return '\uff3e';
            yield return '\uff40';
            yield return '\uff5c';
            yield return '\uff5e';
            for (char c = '\uffe0'; c <= '\uffe6'; c++) yield return c;
            for (char c = '\uffe8'; c <= '\uffee'; c++) yield return c;
            yield return '\ufffc';
            yield return '\ufffd';
        }

        public static IEnumerable<char> GetAllChars()
        {
            for (char c = char.MinValue; c < char.MaxValue; c++)
                yield return c;
            yield return char.MaxValue;
        }
        private static IEnumerable<CharacterGroupType> Map(IEnumerable<string> source)
        {
            CharacterGroupType t;
            foreach (string s in source)
                if (Enum.TryParse<CharacterGroupType>(s, out t))
                    yield return t;
        }
        internal static IEnumerable<CharacterGroup> Map(IEnumerable<CharacterGroupType> source)
        {
            IEnumerable<CharacterGroup> mapped = source.Distinct().Select(k => ByType[k]);
            return mapped.Skip(1).Any() ? mapped.Where(m => !mapped.Any(a => a._includes.Contains(m))) : mapped;
        }
        
        public char[] GetCharacters(IEnumerable<string> include, IEnumerable<string> exclude, IEnumerable<char> explicitInclude, IEnumerable<char> explicitExclude)
        {
            IEnumerable<char> result;
            if (explicitInclude != null && (explicitInclude = explicitInclude.Distinct()).Any())
            {
                if (include != null && (include = include.Where(s => !string.IsNullOrEmpty(s))).Any())
                    result = explicitInclude.Concat(Map(Map(include)).SelectMany(g => g.GetValues()).Where(c => !explicitInclude.Contains(c)));
                else
                    result = explicitInclude;
            }
            else if (include != null && (include = include.Where(s => !string.IsNullOrEmpty(s))).Any())
            {
                if (!(result = Map(Map(include)).SelectMany(g => g.GetValues())).Any())
                    return Array.Empty<char>();
            }
            else
                result = GetAllChars();
            if (explicitExclude != null && (explicitExclude = explicitExclude.Distinct()).Any() && !(result = result.Where(c => !explicitExclude.Contains(c))).Any())
                    return Array.Empty<char>();
            if (exclude != null && (exclude = exclude.Where(s => !string.IsNullOrEmpty(s))).Any())
            {
                IEnumerable<CharacterGroup> excludeGroups = Map(Map(exclude));
                result = result.Where(c => !excludeGroups.Any(e => e.Test(c)));
            }
            return result.ToArray();
        }
        public static bool IsUriDataChar(char c)
        {
            switch (c)
            {
                case '-':
                case '.':
                case '_':
                case '~':
                    return true;
                default:
                    return char.IsAsciiLetterOrDigit(c);
            }
        }
        public static bool IsCsIdentifierChar(char c)
        {
            switch (c)
            {
                case '_':
                    return true;
                default:
                    return char.IsAsciiLetterOrDigit(c);
            }
        }
        static CharacterGroup()
        {
            Dictionary<CharacterGroupType, CharacterGroup> byName = new Dictionary<CharacterGroupType, CharacterGroup>();
            Dictionary<CharacterGroupType, Collection<CharacterGroup>> includes = new Dictionary<CharacterGroupType, Collection<CharacterGroup>>();
            Dictionary<CharacterGroupType, Collection<CharacterGroup>> includedBy = new Dictionary<CharacterGroupType, Collection<CharacterGroup>>();
            Collection<CharacterGroup> all = new Collection<CharacterGroup>();
            All = new ReadOnlyCollection<CharacterGroup>(all);
            ByType = new ReadOnlyDictionary<CharacterGroupType, CharacterGroup>(byName);
            all.Add(AddDelegated(CharacterGroupType.LettersAndDigits, byName, includes, includedBy, char.IsLetterOrDigit, GetLettersAndDigits));
            all.Add(AddDelegated(CharacterGroupType.AsciiChars, byName, includes, includedBy, char.IsAscii, GetAsciiChars));
            all.Add(AddDelegated(CharacterGroupType.UriDataChars, byName, includes, includedBy, IsUriDataChar, GetUriDataChars));
            all.Add(AddDelegated(CharacterGroupType.CsIdentifierChars, byName, includes, includedBy, IsCsIdentifierChar, GetCsIdentifierChars));
            all.Add(AddDelegated(CharacterGroupType.AsciiLettersAndDigits, byName, includes, includedBy, char.IsAsciiLetterOrDigit, GetAsciiLettersAndDigits));
            all.Add(AddDelegated(CharacterGroupType.Letters, byName, includes, includedBy, char.IsLetter, GetLetters));
            all.Add(AddDelegated(CharacterGroupType.AsciiLetters, byName, includes, includedBy, char.IsAsciiLetter, GetAsciiLetters));
            all.Add(AddDelegated(CharacterGroupType.UpperChars, byName, includes, includedBy, char.IsUpper, GetUpperChars));
            all.Add(AddDelegated(CharacterGroupType.LowerChars, byName, includes, includedBy, char.IsLower, GetLowerChars));
            all.Add(AddExplicit(CharacterGroupType.Consonants, "BCDFGJJKLMNPQRSTVWXYZbcdfgjjklmnpqrstvwxyz", byName, includes, includedBy));
            all.Add(AddDelegated(CharacterGroupType.AsciiLettersUpper, byName, includes, includedBy, char.IsAsciiLetterUpper, GetAsciiLettersUpper));
            all.Add(AddDelegated(CharacterGroupType.AsciiLettersLower, byName, includes, includedBy, char.IsAsciiLetterLower, GetAsciiLettersLower));
            all.Add(AddDelegated(CharacterGroupType.Numbers, byName, includes, includedBy, char.IsNumber, GetNumbers));
            all.Add(AddExplicit(CharacterGroupType.HardConsonants, "BCDGJKPQTXbcdgjkpqtx", byName, includes, includedBy));
            all.Add(AddExplicit(CharacterGroupType.SoftConsonants, "CFHLMNRSVWYZcfhlmnrsvwyz", byName, includes, includedBy));
            all.Add(AddExplicit(CharacterGroupType.Vowels, "AEIOUYaeiouy", byName, includes, includedBy));
            all.Add(AddDelegated(CharacterGroupType.AsciiHexDigits, byName, includes, includedBy, char.IsAsciiHexDigit, GetAsciiHexDigits));
            all.Add(AddDelegated(CharacterGroupType.Surrogates, byName, includes, includedBy, char.IsSurrogate, GetSurrogates));
            all.Add(AddDelegated(CharacterGroupType.Digits, byName, includes, includedBy, char.IsDigit, GetDigits));
            all.Add(AddExplicit(CharacterGroupType.ConsonantsUpper, "BCDFGJJKLMNPQRSTVWXYZ", byName, includes, includedBy));
            all.Add(AddExplicit(CharacterGroupType.ConsonantsLower, "bcdfgjjklmnpqrstvwxyz", byName, includes, includedBy));
            all.Add(AddDelegated(CharacterGroupType.AsciiHexDigitsUpper, byName, includes, includedBy, char.IsAsciiHexDigitUpper, GetAsciiHexDigitsUpper));
            all.Add(AddDelegated(CharacterGroupType.AsciiHexDigitsLower, byName, includes, includedBy, char.IsAsciiHexDigitLower, GetAsciiHexDigitsLower));
            all.Add(AddDelegated(CharacterGroupType.WhiteSpaceChars, byName, includes, includedBy, char.IsWhiteSpace, GetWhiteSpaceChars));
            all.Add(AddDelegated(CharacterGroupType.HighSurrogates, byName, includes, includedBy, char.IsHighSurrogate, GetHighSurrogates));
            all.Add(AddDelegated(CharacterGroupType.LowSurrogates, byName, includes, includedBy, char.IsLowSurrogate, GetLowSurrogates));
            all.Add(AddDelegated(CharacterGroupType.ControlChars, byName, includes, includedBy, char.IsControl, GetControlChars));
            all.Add(AddExplicit(CharacterGroupType.HardConsonantsUpper, "BCDGJKPQTX", byName, includes, includedBy));
            all.Add(AddExplicit(CharacterGroupType.HardConsonantsLower, "bcdgjkpqtx", byName, includes, includedBy));
            all.Add(AddExplicit(CharacterGroupType.SoftConsonantsUpper, "CFHLMNRSVWYZ", byName, includes, includedBy));
            all.Add(AddExplicit(CharacterGroupType.SoftConsonantsLower, "cfhlmnrsvwyz", byName, includes, includedBy));
            all.Add(AddExplicit(CharacterGroupType.VowelsUpper, "AEIOUY", byName, includes, includedBy));
            all.Add(AddExplicit(CharacterGroupType.VowelsLower, "aeiouy", byName, includes, includedBy));
            all.Add(AddDelegated(CharacterGroupType.Separators, byName, includes, includedBy, char.IsSeparator, GetSeparators));
            all.Add(AddDelegated(CharacterGroupType.PunctuationChars, byName, includes, includedBy, char.IsPunctuation, GetPunctuationChars));
            all.Add(AddDelegated(CharacterGroupType.Symbols, byName, includes, includedBy, char.IsSymbol, GetSymbols));
            all.Add(AddDelegated(CharacterGroupType.AsciiDigits, byName, includes, includedBy, char.IsAsciiDigit, GetAsciiDigits));
            all.Add(AddDelegated(CharacterGroupType.AsciiControlChars, byName, includes, includedBy, char.IsAsciiDigit, GetAsciiControlChars));
            all.Add(AddDelegated(CharacterGroupType.AsciiPunctuation, byName, includes, includedBy, char.IsAsciiDigit, GetAsciiPunctuation));
            all.Add(AddDelegated(CharacterGroupType.AsciiSymbols, byName, includes, includedBy, char.IsAsciiDigit, GetAsciiSymbols));
            
            SetIncludes(CharacterGroupType.LettersAndDigits, byName, includes, includedBy, CharacterGroupType.AsciiLettersUpper, CharacterGroupType.Digits, CharacterGroupType.UpperChars, CharacterGroupType.Vowels, CharacterGroupType.HardConsonants, CharacterGroupType.SoftConsonantsLower,
                CharacterGroupType.AsciiHexDigitsLower, CharacterGroupType.LowerChars, CharacterGroupType.AsciiLetters, CharacterGroupType.UriDataChars, CharacterGroupType.CsIdentifierChars, CharacterGroupType.AsciiLettersAndDigits, CharacterGroupType.SoftConsonantsUpper, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower,
                CharacterGroupType.VowelsLower, CharacterGroupType.AsciiDigits, CharacterGroupType.VowelsUpper, CharacterGroupType.ConsonantsUpper, CharacterGroupType.AsciiLettersLower, CharacterGroupType.AsciiHexDigitsUpper, CharacterGroupType.Letters, CharacterGroupType.HardConsonantsUpper,
                CharacterGroupType.Consonants, CharacterGroupType.AsciiHexDigits, CharacterGroupType.SoftConsonants);
            SetIncludes(CharacterGroupType.AsciiChars, byName, includes, includedBy, CharacterGroupType.AsciiLettersUpper, CharacterGroupType.Vowels, CharacterGroupType.HardConsonants, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.AsciiHexDigitsLower, CharacterGroupType.AsciiLetters,
                CharacterGroupType.UriDataChars, CharacterGroupType.CsIdentifierChars, CharacterGroupType.AsciiLettersAndDigits, CharacterGroupType.SoftConsonantsUpper, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.VowelsLower, CharacterGroupType.AsciiDigits, CharacterGroupType.VowelsUpper,
                CharacterGroupType.ConsonantsUpper, CharacterGroupType.AsciiLettersLower, CharacterGroupType.AsciiHexDigitsUpper, CharacterGroupType.HardConsonantsUpper, CharacterGroupType.Consonants, CharacterGroupType.AsciiHexDigits, CharacterGroupType.SoftConsonants,
                CharacterGroupType.AsciiControlChars, CharacterGroupType.AsciiPunctuation, CharacterGroupType.AsciiSymbols);
            SetIncludes(CharacterGroupType.UriDataChars, byName, includes, includedBy, CharacterGroupType.CsIdentifierChars, CharacterGroupType.AsciiLettersAndDigits, CharacterGroupType.AsciiLettersUpper, CharacterGroupType.Vowels, CharacterGroupType.HardConsonants, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.AsciiHexDigitsLower,
                CharacterGroupType.AsciiLetters, CharacterGroupType.SoftConsonantsUpper, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.VowelsLower, CharacterGroupType.AsciiDigits, CharacterGroupType.VowelsUpper, CharacterGroupType.ConsonantsUpper,
                CharacterGroupType.AsciiLettersLower, CharacterGroupType.AsciiHexDigitsUpper, CharacterGroupType.HardConsonantsUpper, CharacterGroupType.Consonants, CharacterGroupType.AsciiHexDigits, CharacterGroupType.SoftConsonants);
            SetIncludes(CharacterGroupType.CsIdentifierChars, byName, includes, includedBy, CharacterGroupType.AsciiLettersAndDigits, CharacterGroupType.AsciiLettersUpper, CharacterGroupType.Vowels, CharacterGroupType.HardConsonants, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.AsciiHexDigitsLower,
                CharacterGroupType.AsciiLetters, CharacterGroupType.SoftConsonantsUpper, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.VowelsLower, CharacterGroupType.AsciiDigits, CharacterGroupType.VowelsUpper, CharacterGroupType.ConsonantsUpper,
                CharacterGroupType.AsciiLettersLower, CharacterGroupType.AsciiHexDigitsUpper, CharacterGroupType.HardConsonantsUpper, CharacterGroupType.Consonants, CharacterGroupType.AsciiHexDigits, CharacterGroupType.SoftConsonants);
            SetIncludes(CharacterGroupType.AsciiLettersAndDigits, byName, includes, includedBy, CharacterGroupType.AsciiLettersUpper, CharacterGroupType.Vowels, CharacterGroupType.HardConsonants, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.AsciiHexDigitsLower,
                CharacterGroupType.AsciiLetters, CharacterGroupType.SoftConsonantsUpper, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.VowelsLower, CharacterGroupType.AsciiDigits, CharacterGroupType.VowelsUpper, CharacterGroupType.ConsonantsUpper,
                CharacterGroupType.AsciiLettersLower, CharacterGroupType.AsciiHexDigitsUpper, CharacterGroupType.HardConsonantsUpper, CharacterGroupType.Consonants, CharacterGroupType.AsciiHexDigits, CharacterGroupType.SoftConsonants);
            SetIncludes(CharacterGroupType.Letters, byName, includes, includedBy, CharacterGroupType.AsciiLettersUpper, CharacterGroupType.UpperChars, CharacterGroupType.Vowels, CharacterGroupType.HardConsonants, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.LowerChars,
                CharacterGroupType.AsciiLetters, CharacterGroupType.SoftConsonantsUpper, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.VowelsLower, CharacterGroupType.VowelsUpper, CharacterGroupType.ConsonantsUpper,
                CharacterGroupType.AsciiLettersLower, CharacterGroupType.HardConsonantsUpper, CharacterGroupType.Consonants, CharacterGroupType.SoftConsonants);
            SetIncludes(CharacterGroupType.AsciiLetters, byName, includes, includedBy, CharacterGroupType.AsciiLettersUpper, CharacterGroupType.Vowels, CharacterGroupType.HardConsonants, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.SoftConsonantsUpper,
                CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.VowelsLower, CharacterGroupType.VowelsUpper, CharacterGroupType.ConsonantsUpper, CharacterGroupType.AsciiLettersLower, CharacterGroupType.HardConsonantsUpper,
                CharacterGroupType.Consonants, CharacterGroupType.SoftConsonants);
            SetIncludes(CharacterGroupType.UpperChars, byName, includes, includedBy, CharacterGroupType.AsciiLettersUpper, CharacterGroupType.SoftConsonantsUpper, CharacterGroupType.VowelsUpper, CharacterGroupType.ConsonantsUpper, CharacterGroupType.HardConsonantsUpper);
            SetIncludes(CharacterGroupType.LowerChars, byName, includes, includedBy, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.VowelsLower, CharacterGroupType.AsciiLettersLower);
            SetIncludes(CharacterGroupType.Consonants, byName, includes, includedBy, CharacterGroupType.HardConsonants, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.ConsonantsUpper, CharacterGroupType.HardConsonantsUpper);
            SetIncludes(CharacterGroupType.AsciiLettersUpper, byName, includes, includedBy, CharacterGroupType.SoftConsonantsUpper, CharacterGroupType.VowelsUpper, CharacterGroupType.ConsonantsUpper, CharacterGroupType.HardConsonantsUpper);
            SetIncludes(CharacterGroupType.AsciiLettersLower, byName, includes, includedBy, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.ConsonantsLower, CharacterGroupType.HardConsonantsLower, CharacterGroupType.VowelsLower);
            SetIncludes(CharacterGroupType.Numbers, byName, includes, includedBy, CharacterGroupType.Digits, CharacterGroupType.AsciiDigits);
            SetIncludes(CharacterGroupType.SoftConsonants, byName, includes, includedBy, CharacterGroupType.SoftConsonantsLower, CharacterGroupType.SoftConsonantsUpper);
            SetIncludes(CharacterGroupType.AsciiHexDigits, byName, includes, includedBy, CharacterGroupType.AsciiHexDigitsLower, CharacterGroupType.AsciiDigits, CharacterGroupType.AsciiHexDigitsUpper);
            SetIncludes(CharacterGroupType.Vowels, byName, includes, includedBy, CharacterGroupType.VowelsLower, CharacterGroupType.VowelsUpper);
            SetIncludes(CharacterGroupType.HardConsonants, byName, includes, includedBy, CharacterGroupType.HardConsonantsLower, CharacterGroupType.HardConsonantsUpper);
            SetIncludes(CharacterGroupType.Surrogates, byName, includes, includedBy, CharacterGroupType.HighSurrogates, CharacterGroupType.LowSurrogates);
            SetIncludes(CharacterGroupType.Digits, byName, includes, includedBy, CharacterGroupType.AsciiDigits);
            SetIncludes(CharacterGroupType.ConsonantsUpper, byName, includes, includedBy, CharacterGroupType.HardConsonantsUpper);
            SetIncludes(CharacterGroupType.ConsonantsLower, byName, includes, includedBy, CharacterGroupType.HardConsonantsLower);
            SetIncludes(CharacterGroupType.AsciiHexDigitsUpper, byName, includes, includedBy, CharacterGroupType.AsciiDigits);
            SetIncludes(CharacterGroupType.AsciiHexDigitsLower, byName, includes, includedBy, CharacterGroupType.AsciiDigits);
            SetIncludes(CharacterGroupType.WhiteSpaceChars, byName, includes, includedBy, CharacterGroupType.Separators);
            SetIncludes(CharacterGroupType.ControlChars, byName, includes, includedBy, CharacterGroupType.AsciiControlChars);
            SetIncludes(CharacterGroupType.PunctuationChars, byName, includes, includedBy, CharacterGroupType.AsciiPunctuation);
            SetIncludes(CharacterGroupType.Symbols, byName, includes, includedBy, CharacterGroupType.AsciiSymbols);
        }
    }
    public sealed class ExplicitCharacterGroup : CharacterGroup
    {
        private readonly string _characters;
        internal ExplicitCharacterGroup(CharacterGroupType type, string characters, Collection<CharacterGroup> includes, Collection<CharacterGroup> includedBy) : base(type, includes, includedBy)
        {
            _characters = characters;
        }
        public override IEnumerable<char> GetValues() { return _characters; }
        public override bool Test(char c) { return _characters.Contains(c); }
    }
    public sealed class DelgateCharacterGroup : CharacterGroup
    {
        private readonly Func<IEnumerable<char>> _getValues;
        private readonly Predicate<char> _test;
        private IEnumerable<char> GetValuesDefault()
        {
            for (char c = char.MinValue; c < char.MaxValue; c++)
            {
                if (_test(c)) yield return c;
            }
            if (_test(char.MaxValue)) yield return char.MaxValue;
        }
        public DelgateCharacterGroup(CharacterGroupType type, Collection<CharacterGroup> includes, Collection<CharacterGroup> includedBy, Predicate<char> test, Func<IEnumerable<char>> getValues) : base(type, includes, includedBy)
        {
            _test = test;
            _getValues = getValues ?? GetValuesDefault;
        }
        public override IEnumerable<char> GetValues() { return _getValues(); }
        public override bool Test(char c) { return _test(c); }
    }   
}
'@ -ErrorAction Stop;
