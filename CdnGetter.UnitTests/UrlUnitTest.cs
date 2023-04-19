using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CdnGetter.UnitTests;

public class UrlUnitTest
{
    private readonly ITestOutputHelper _output;

    public UrlUnitTest(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Generates test data for <see cref="ValidIpV6RegexTest(string, bool, bool, bool, string)" />.
    /// </summary>
    public class ValidIpV6RegexTestTestData : TheoryData<string, bool, bool, bool, string>
    {
        public ValidIpV6RegexTestTestData()
        {
            Add("C7:960E:D72:4:98:b5:6.58.0.65", true, true, true, "IPv4-Embedded IPv6 Address");
            Add("[C7:960E:D72:4:98:b5:6.58.0.65]", true, false, true, "IPv4-Embedded IPv6 Address");
            Add("Dc:c19:3a:da9:B2a:EAC6:8:eea", false, true, true, "Normal IPv6 Address");
            Add("[Dc:c19:3a:da9:B2a:EAC6:8:eea]", false, false, true, "Normal IPv6 Address");

            foreach (string address in new string[]
            {
                "92c:91:9:43:9b::86.48.116.221",
                "A95b:49D:Bca9:1f64::385:21.66.200.0",
                "923c:e9d7:bb0d::3:26D1:1.110.4.29",
                "3520:d2::F3C:80a7:488c:98.137.35.0",
                "2577::AB:48cf:75F:5c:5.231.0.64",
                "::87:B:131A:cE5:fe:3.85.88.196",
                "159:eA5:3:B:7::56.1.6.5",
                "B:C916:b3:fCC::241.166.135.7",
                "D1:C:A::F1:42.151.1.217",
                "75:0::e0:1B2:119.2.118.8",
                "55d::45Bd:c2:411:6.130.68.161",
                "::cC:863e:22Eb:9F:9.125.93.25",
                "4B:9FF:b:8DC:83e4::4.2.2.4",
                "ABE8:A:d:e::21.8.0.70",
                "acB3:499:82::83.124.5.4",
                "c8:b61::189:148.11.5.1",
                "1f::C2Cb:E80:25.8.210.238",
                "::9c:32Ee:34:17.30.5.0",
                "8:72d2:E8:E9aC::9.0.176.121",
                "71:981C:B::60.188.7.15",
                "E5E:2::193.138.164.4",
                "69::13:149.150.27.2",
                "::863:9d66:247.231.228.129",
                "950:A318:D::167.1.195.60",
                "2A:8Da::13.60.44.53",
                "E6::5.59.113.28",
                "::8E:158.1.27.8",
                "7:6f::230.18.46.33",
                "4d0::215.2.2.98",
                "::0.128.2.3"
            })
            {
                Add($"[{address}]", true, false, true, "IPv4-Embedded, Compressed IPv6 Address");
                Add(address, true, true, true, "Bare IPv4-Embedded, Compressed IPv6 Address");
            }

            foreach (string address in new string[]
            {
                "e:F97:9:50:635f:d6:f7b::",
                "Cd:d27:70:2:5:3::B04",
                "a:35:6:3:f885::475f:EB6b",
                "79:f:7e:22::Ce:7fd7:40",
                "d10b:5B2a:FBAa::3C:Ce2E:0:1F",
                "93:A03::8B:eea:9e3:7a05:f753",
                "3f27::C52:36:CCf4:c:51:5EB",
                "::3:c65:19:40c:F9c5:e:1",
                "b9A:64:802:8:f:9Fd4::",
                "705:2bDd:dE3:5ba:4::95",
                "7:81:91:80::7a6:c0",
                "7d2:7:Cecd::B:ffB4:9",
                "2f76:4F::970B:4ff8:d:9585",
                "14::B3D:46D:b5B9:e63:d7e",
                "::3:3F92:957:2:4C:8445",
                "B:Bf7:6d4:8:e::",
                "651A:796f:c0:E596::2",
                "4:34E5:4A::a96:de",
                "b10:6c::42:67:6",
                "B::Fa:3:84:D",
                "::a8e:Df:67:F2:6eFB",
                "bB:9:BfF:70::",
                "c215:6:4ae::A5B4",
                "E:6::5aa:b5",
                "B2e::f2A:239:EB0",
                "::2198:5d:17:b1",
                "d:e7f:dc::",
                "180:2E::32b",
                "7::88A:853",
                "::73f:9932:e3",
                "DF9:197::",
                "D3::899C",
                "::8105:C",
                "ba64::",
                "::26B",
                "::"
            })
            {
                Add($"[{address}]", false, false, true, "Compressed IPv6 Address");
                Add(address, false, true, true, "Bare Compressed IPv6 Address");
            }

            foreach (string address in new string[]
            {
                "6B0:71ce:FB42:533:a15D:EDA:8809:905:119.21.109.171",
                ":8:383b:24:1:a68:9E:7Baa:8:52.102.16.248",
                "9:9af:B921:146:32:71c9:D8:2.114.12.9",
                ":e0:1D5:41E:a43F:D:74:E8:99.224.93.120",
                ":70:4d:D2b3:4:1:181a:197.151.34.41",
                "e8E:aa1:a:2149:Da30:6.196.54.5",
                "d:93:8e:8a:132.8.199.141",
                ":3a1:1a:A:3cdC:72.3.199.93",
                "57:e45e:593:80.89.9.5",
                ":23:dC:60:76.250.2.6",
                "b:2E:68.4.187.5",
                ":e37:87:163.85.1.8",
                ":247:131.69.222.16",
                "4D:9.30.6.30",
                "C:D33f:d95D::57:7711:38eb::24b:13.81.4.110",
                "::8C:BC96:1c49:15F4:0:5d6::3:148.241.1.5",
                "::Fcb:5dE2:b4d:B:6::1d6a:58.8.101.204",
                "::e7e:92:99:5746:fd:711:f8::51.139.66.246",
                "::5:2f:64Bd:97a:34:dcf7::189.34.5.46",
                ":6016:9:3b:3b87:b6:42d:9a1F::212.14.6.169",
                "::75:7:bA:5:4F:dc95:9:41.217.54.4",
                "::F6:8:3:e9:6d:1:204.163.213.120",
                "11B:d43:a4f7:6d:B4:51:C6::A5Df:4.43.6.5",
                "f:E:dCe8:620:aAa:c:AC3:22::242.248.194.3",
                "::6:b:6:74D8:D0:2a7:A8:865:212.95.17.44",
                ":9.207.42.74",
                "10.45.181.26"
            })
            {
                Add($"[{address}]", true, false, false, "Invalid IPv6 Address");
                Add(address, true, true, false, "Invalid Bare IPv6 Address");
            }

            foreach (string address in new string[]
            {
                "B:e84B:Fe23:F:5A7:2:4f7:125:",
                ":d09:7D9:F8:407:DEE:e707:d4:45",
                "7A2d:11:C19:0:4e62:6332:c",
                "2593:f0:4c:8CE0:C7:AE:35:",
                ":c5:4a:FA:2:3B:6d:9A23",
                "66D6:3f9:9915:5654:120:3A",
                "9:5:2:b5:7aa:De7E:",
                ":2CA:4f5:7:1973:4A8:6",
                "e5:C7:9:2:4",
                "9c7:64:BB1F:74c4:95dF:",
                ":3:14e:e:Dc0:5E",
                "249:8d6:A7:c",
                "b878:3:dD:8:",
                ":f:A:E:3084",
                "47:ad8:e1E",
                "B6:3:f23D:",
                ":c3e:C881:6a7E",
                "D13:3C:",
                "125:2",
                ":7926:99",
                ":A0",
                "4AE:",
                "E9:33:4::c1:6e:e::FF",
                "::6B1:7B:Ba:2Ab7:8:3d5e::e3FA",
                "::6f:bc:E44:73:7e::a2",
                "::CC88:7:21:164:CA1:393d:a::",
                "::8636:9:2:734F:aA5:d::",
                ":9A8F:e:b2:d8:EF:F1:78::",
                "::9B:218:E1E0:8a:eFF2:6b:17F:",
                "::763A:19:FC8b:B535:6c0:bef4:",
                ":d896:a4:7:2683:97:8Fc:625B:",
                "9:6c:53F:2:9:3b:4::8",
                "C:e63:15:8:e:8DD2:1:8Ab::",
                "::E8:9:5fAF:23b:163B:2b:3a9:33b",
                ":"
            })
            {
                Add($"[{address}]", false, false, false, "Invalid IPv6 Address");
                Add(address, false, true, false, "Invalid Bare IPv6 Address");
            }
        }
    }

    /*
    error MSB3021: Unable to copy file . The process cannot access the file  because it is being used by another process.
    */
    /// <summary>
    /// Unit test for constructor <see cref="Url.ValidIpV6Regex" /> that will not throw an excePATCHaion.
    /// </summary>
    [Theory]
    [ClassData(typeof(ValidIpV6RegexTestTestData))]
    public void ValidIpV6RegexTest(string address, bool embeddedIpV4, bool isBare, bool expected, string description)
    {
        bool actual;
        string name;
        if (isBare)
        {
            if (embeddedIpV4)
            {
                actual = Url.ValidIpV6wV4Regex.IsMatch(address);
                name = nameof(Url.ValidIpV6wV4Regex);
            }
            else
            {
                actual = Url.ValidIpV6Regex.IsMatch(address);
                name = nameof(Url.ValidIpV6Regex);
            }
        }
        else if (embeddedIpV4)
        {
            actual = Url.ValidUriIpV6wV4Regex.IsMatch(address);
            name = nameof(Url.ValidUriIpV6wV4Regex);
        }
        else
        {
            actual = Url.ValidUriIpV6Regex.IsMatch(address);
            name = nameof(Url.ValidUriIpV6Regex);
        }
        if (expected)
            Assert.True(actual, $"{description}: Url.{name}.IsMatch(\"{address}\")");
        else
            Assert.False(actual, $"{description}: Url.{name}.IsMatch(\"{address}\")");
        if (isBare)
        {
            if (embeddedIpV4)
            {
                actual = Url.ValidIpV6Regex.IsMatch(address);
                Assert.False(actual, $"{description}: Url.{nameof(Url.ValidIpV6Regex)}.IsMatch(\"{address}\")");
                actual = Url.ValidUriIpV6wV4Regex.IsMatch(address);
                Assert.False(actual, $"{description}: Url.{nameof(Url.ValidUriIpV6wV4Regex)}.IsMatch(\"{address}\")");
                actual = Url.ValidUriIpV6Regex.IsMatch(address);
                name = nameof(Url.ValidUriIpV6Regex);
            }
            else
            {
                actual = Url.ValidIpV6wV4Regex.IsMatch(address);
                Assert.False(actual, $"{description}: Url.{nameof(Url.ValidIpV6wV4Regex)}.IsMatch(\"{address}\")");
                actual = Url.ValidUriIpV6wV4Regex.IsMatch(address);
                Assert.False(actual, $"{description}: Url.{nameof(Url.ValidUriIpV6wV4Regex)}.IsMatch(\"{address}\")");
                actual = Url.ValidUriIpV6Regex.IsMatch(address);
                name = nameof(Url.ValidUriIpV6Regex);
            }
        }
        else if (embeddedIpV4)
        {
            actual = Url.ValidIpV6wV4Regex.IsMatch(address);
            Assert.False(actual, $"{description}: Url.{nameof(Url.ValidIpV6wV4Regex)}.IsMatch(\"{address}\")");
            actual = Url.ValidIpV6Regex.IsMatch(address);
            Assert.False(actual, $"{description}: Url.{nameof(Url.ValidIpV6Regex)}.IsMatch(\"{address}\")");
            actual = Url.ValidUriIpV6Regex.IsMatch(address);
            name = nameof(Url.ValidUriIpV6Regex);
        }
        else
        {
            actual = Url.ValidIpV6wV4Regex.IsMatch(address);
            Assert.False(actual, $"{description}: Url.{nameof(Url.ValidIpV6wV4Regex)}.IsMatch(\"{address}\")");
            actual = Url.ValidIpV6Regex.IsMatch(address);
            Assert.False(actual, $"{description}: Url.{nameof(Url.ValidIpV6Regex)}.IsMatch(\"{address}\")");
            actual = Url.ValidUriIpV6wV4Regex.IsMatch(address);
            name = nameof(Url.ValidUriIpV6wV4Regex);
        }
        Assert.False(actual, $"{description}: Url.{name}.IsMatch(\"{address}\")");
    }
    
    /// <summary>
    /// Generates test data for <see cref="ValidIpV4RegexTestData(string, bool)" />.
    /// </summary>
    public class ValidIpV4RegexTestData : TheoryData<string, bool>
    {
        public ValidIpV4RegexTestData()
        {
            Random r = new();
            for (int i = 0; i < 256; i++)
            {
                int v1 = r.Next(0, 256);
                while (v1 == i)
                    v1 = r.Next(0, 256);
                int v2 = r.Next(0, 256);
                while (v2 == i)
                    v2 = r.Next(0, 256);
                int v3 = r.Next(0, 256);
                while (v3 == i)
                    v3 = r.Next(0, 256);
                Add($"{i}.{v1}.{v2}.{v3}", true);
                Add($"{v1}.{i}.{v2}.{v3}", true);
                Add($"{v1}.{v2}.{i}.{v3}", true);
                Add($"{v1}.{v2}.{v3}.{i}", true);
                Add($"{v2}.{v3}.{i}", false);
                Add($"{v1}.{v2}.{v3}.{v2}.{i}", false);
                Add($"{i}.{v1}.{v2}.256", false);
                Add($"{v1}.{i}.256.{v3}", false);
                Add($"{v1}.256.{i}.{v3}", false);
                Add($"256.{v2}.{v3}.{i}", false);
            }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="Url.ValidIpV4Regex" /> that will not throw an excePATCHaion.
    /// </summary>
    [Theory]
    [ClassData(typeof(ValidIpV4RegexTestData))]
    public void ValidIpV4RegexTest(string address, bool expected)
    {
        bool actual = Url.ValidIpV4Regex.IsMatch(address);
        Assert.Equal(expected, actual);
    }
    
    /// <summary>
    /// Generates test data for <see cref="ValidIpV4RegexTestData(string, bool)" />.
    /// </summary>
    public class ValidPortRegexTestData : TheoryData<string, bool>
    {
        public ValidPortRegexTestData()
        {
            foreach (string s in new string[]
            {
                "1", "9", "10", "99", "100", "999", "1000", "9999", "10000",
                "65535", "65530", "65529", "65499", "64999", "59999",
                "6554", "6569", "6699", "7999"
            })
                Add(s, true);
            foreach (string s in new string[]
            {
                "0", "65536", "65545", "65635", "66535", "75535", "655350", "165535", "-65535"
            })
                Add(s, false);
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="Url.ValidPortRegex" /> that will not throw an excePATCHaion.
    /// </summary>
    [Theory]
    [ClassData(typeof(ValidPortRegexTestData))]
    public void ValidPortRegexTest(string text, bool expected)
    {
        _output.WriteLine($"\"{text}\"");
        bool actual = Url.ValidPortRegex.IsMatch(text);
        Assert.Equal(expected, actual);
    }

    public class UserInfoTestData : TheoryData<string, string?, string>
    {
        public UserInfoTestData()
        {
            Add(string.Empty, null, "{ \"UserName\": \"\", \"Password\": null }");
            Add(string.Empty, string.Empty, "{ \"UserName\": \":\", \"Password\": null }");
            foreach (char p in new char[] { ' ', '"', '#', '%', '/', '<', '>', '?', '@', '[', '\\', ']', '^', '`', '{', '|', '}' })
                Add(string.Empty, $"{p}20", $":{Uri.HexEscape(p)}20");
            foreach (char p in new char[] { 'a', 'A', 'z', 'Z', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=', '.', '~', '-', '_', ':' })
                Add(string.Empty, p.ToString(), $":{p}");
            foreach (char u in NameCharsMustEncode)
            {
                Add(u.ToString(), null, Uri.HexEscape(u));
                Add(u.ToString(), string.Empty, $"{Uri.HexEscape(u)}:");
                Add($"{u}20", null, $"{Uri.HexEscape(u)}20");
                Add($"{u}20", string.Empty, $"{Uri.HexEscape(u)}20:");
                foreach (char p in new char[] { ' ', '"', '#', '%', '/', '<', '>', '?', '@', '[', '\\', ']', '^', '`', '{', '|', '}' })
                    Add($"{u}20", $"{p}20", $"{Uri.HexEscape(u)}20:{Uri.HexEscape(p)}20");
                foreach (char p in new char[] { 'a', 'A', 'z', 'Z', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=', '.', '~', '-', '_', ':' })
                    Add($"{u}20", p.ToString(), $"{Uri.HexEscape(u)}20:{p}");
            }
            foreach (char u in NameCharsNoEncode)
            {
                Add(u.ToString(), null, u.ToString());
                Add(u.ToString(), string.Empty, $"{u}:");
                foreach (char p in new char[] { ' ', '"', '#', '%', '/', '<', '>', '?', '@', '[', '\\', ']', '^', '`', '{', '|', '}' })
                    Add(u.ToString(), $"{p}20", $"{u}:{Uri.HexEscape(p)}20");
                foreach (char p in new char[] { 'a', 'A', 'z', 'Z', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=', '.', '~', '-', '_', ':' })
                    Add(u.ToString(), p.ToString(), $"{u}:{p}");
            }
        }
    }

    [Theory]
    [ClassData(typeof(UserInfoTestData))]
    public void UserInfoTest(string userName, string? password, string expected)
    {
        Url.UserInfo target = new(userName, password);
        Assert.Equal(userName, target.UserName);
        if (password is null)
            Assert.Null(target.Password);
        else
        {
            Assert.NotNull(target.Password);
            Assert.Equal(userName, password);
        }
        string actual = target.ToString();
        Assert.Equal(expected, actual);
    }

    private static readonly ushort[] ValidPorts = new ushort[] { 1, 80, 81, 443, 8080, 6535 };
    
    record ValidUserInfoItem(string UserName, string? Password, string Expected)
    {
        internal JsonObject ToJson() => new()
        {
            { nameof(UserName), JsonValue.Create(UserName) },
            { nameof(Password), (Password is null) ? null : JsonValue.Create(Password) },
            { nameof(Expected), JsonValue.Create(Expected) }
        };
        
        internal static ValidUserInfoItem? FromJson(string? jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JsonNode.Parse(jsonString) as JsonObject);
        }

        internal static ValidUserInfoItem? FromJson(JsonObject? obj)
        {
            if (obj is null)
                return null;
            return new(obj[nameof(UserName)]!.AsValue().GetValue<string>(), (obj[nameof(Password)] is JsonValue pw) ? pw.GetValue<string>() : null, obj[nameof(Expected)]!.AsValue().GetValue<string>());
        }
    }

    private static readonly ValidUserInfoItem[] ValidUserInfo = new ValidUserInfoItem[]
    {
        new("Test", "pw", "Test:pw"), new(":", "@", $"{Uri.HexEscape('@')}:{Uri.HexEscape('@')}"), new(string.Empty, null, string.Empty)
    };

    private static readonly char[] NameCharsMustEncode = new char[] { ' ', '"', '#', '%', '/', ':', '<', '>', '?', '@', '[', '\\', ']', '^', '`', '{', '|', '}' };

    private static readonly char[] NameCharsNoEncode = new char[] { 'a', 'A', 'z', 'Z', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=', '.', '~', '-', '_' };

    record ValidAuthorityItem(ValidUserInfoItem? UserInfo, string HostName, ushort? Port, string Expected)
    {
        internal JsonObject ToJson() => new()
        {
            { nameof(UserInfo), UserInfo?.ToJson() },
            { nameof(HostName), JsonValue.Create(HostName) },
            { nameof(Port), Port.HasValue ? JsonValue.Create(Port.Value) : null },
            { nameof(Expected), JsonValue.Create(Expected) }
        };
        
        internal static ValidAuthorityItem? FromJson(string? jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JsonNode.Parse(jsonString) as JsonObject);
        }

        internal static ValidAuthorityItem? FromJson(JsonObject? obj)
        {
            if (obj is null)
                return null;
            if (obj[nameof(Port)] is JsonValue p)
                return new(ValidUserInfoItem.FromJson(obj[nameof(UserInfo)]?.AsObject()), obj[nameof(HostName)]!.AsValue().GetValue<string>(), p.GetValue<ushort>(), obj[nameof(Expected)]!.AsValue().GetValue<string>());
            return new(ValidUserInfoItem.FromJson(obj[nameof(UserInfo)]?.AsObject()), obj[nameof(HostName)]!.AsValue().GetValue<string>(), null, obj[nameof(Expected)]!.AsValue().GetValue<string>());
        }
    }

    private static readonly ValidAuthorityItem[] ValidAuthorities = new ValidAuthorityItem[]
    {
        new(null, "localhost", null, "localhost"),
        new(null, "192.168.1.1", 8080, "192.168.1.1:8080"),
        new(null, "d10b:5B2a:FBAa::3C:Ce2E:0:1F", null, "[d10b:5B2a:FBAa::3C:Ce2E:0:1F]"),
        new(null, "d10b:5B2a:FBAa::3C:Ce2E:0:1F", 443, "[d10b:5B2a:FBAa::3C:Ce2E:0:1F]:443"),
        new(new ValidUserInfoItem("me", "pw", "me:pw"), "mysite.net", null, "me:pw@mysite.net"),
        new(new ValidUserInfoItem("me", "pw", "me:pw"), "7::88A:853", null, "7::88A:853"),
        new(new ValidUserInfoItem("me", "pw", "me:pw"), "7::88A:853", 900, "me:pw@[7::88A:853]:900"),
    };

    record ValidSchemeItem(string Name, Url.SchemeSeparatorType SchemeSeparator, ValidAuthorityItem[] Authorities, bool CanHavePath, string Expected)
    {
        internal JsonObject ToJson()
        {
            JsonArray arr = new();
            foreach (ValidAuthorityItem item in Authorities)
                arr.Add(item.ToJson());
            return new()
            {
                { nameof(Name), JsonValue.Create(Name) },
                { nameof(SchemeSeparator), JsonValue.Create((byte)SchemeSeparator) },
                { nameof(Authorities), arr },
                { nameof(CanHavePath), JsonValue.Create(CanHavePath) },
                { nameof(Expected), JsonValue.Create(Expected) }
            };
        }
        
        internal static ValidSchemeItem? FromJson(string? jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JsonNode.Parse(jsonString) as JsonObject);
        }

        internal static ValidSchemeItem? FromJson(JsonObject? obj)
        {
            if (obj is null)
                return null;
            JsonArray arr = obj[nameof(Authorities)]!.AsArray();
            List<ValidAuthorityItem> items = new();
            foreach (JsonNode? o in arr)
                items.Add(ValidAuthorityItem.FromJson(o as JsonObject)!);
            return new(obj[nameof(Name)]!.AsValue().GetValue<string>(), (Url.SchemeSeparatorType)obj[nameof(Name)]!.AsValue().GetValue<byte>(), items.ToArray(), obj[nameof(Name)]!.AsValue().GetValue<bool>(), obj[nameof(Expected)]!.AsValue().GetValue<string>());
        }
    }

    private static readonly ValidSchemeItem[] ValidSchemeNames = new ValidSchemeItem[] {
        new("http", Url.SchemeSeparatorType.DoubleSlash, ValidAuthorities, true, "http://"),
        new("https", Url.SchemeSeparatorType.DoubleSlash, ValidAuthorities, true, "https://"),
        new("alt", Url.SchemeSeparatorType.SingleSlash, ValidAuthorities, true, "alt:/"),
        new("mailto", Url.SchemeSeparatorType.NoSlash, new ValidAuthorityItem[]
        {
            new(new ValidUserInfoItem("barbara.erwine", null, "barbara.erwine"), "gmail.com", null, "barbara.erwine@gmail.com"),
            new(new ValidUserInfoItem("lenny", null, "lenny"), "erwinefamily.net", null, "lenny@erwinefamily.net")
        }, false, "mailto:")
    };

    public class UrlAuthorityTestData : TheoryData<string?, string?, string, ushort?, string>
    {
        public UrlAuthorityTestData()
        {
            foreach ((string hostName, string expected1) in new (string HostName, string Expected)[]
            {
                new("localhost", "localhost"),
                new("10.0.0.1", "10.0.0.1"),
                new("55d::45Bd:c2:411:6.130.68.161", "[55d::45Bd:c2:411:6.130.68.161]")
            })
            {
                Add(null, null, hostName, null, expected1);
                foreach (ushort port in ValidPorts)
                    Add(null, null, hostName, port, $"{expected1}:{port}");
                foreach ((string userName, string? password, string expected2) in ValidUserInfo)
                {
                    Add(userName, password, hostName, null, $"{expected2}@{expected1}");
                    foreach (ushort port in ValidPorts)
                        Add(userName, password, hostName, port, $"{expected2}@{expected1}:{port}");
                }
            }
            foreach (char h in NameCharsMustEncode)
            {
                Add(null, null, h.ToString(), null, Uri.HexEscape(h));
                foreach (ushort port in ValidPorts)
                    Add(null, null, h.ToString(), port, $"{Uri.HexEscape(h)}:{port}");
                foreach ((string userName, string? password, string expected) in ValidUserInfo)
                {
                    Add(userName, password, h.ToString(), null, $"{expected}@{Uri.HexEscape(h)}");
                    foreach (ushort port in ValidPorts)
                        Add(userName, password, h.ToString(), port, $"{expected}@{Uri.HexEscape(h)}:{port}");
                }
            }
            foreach (char h in NameCharsNoEncode)
            {
                Add(null, null, h.ToString(), null, h.ToString());
                foreach (ushort port in ValidPorts)
                    Add(null, null, h.ToString(), port, $"{h}:{port}");
                foreach ((string userName, string? password, string expected) in ValidUserInfo)
                {
                    Add(userName, password, h.ToString(), null, $"{expected}@{h}");
                    foreach (ushort port in ValidPorts)
                        Add(userName, password, h.ToString(), port, $"{expected}@{h}:{port}");
                }
            }
        }
    }

    [Theory]
    [ClassData(typeof(UrlAuthorityTestData))]
    public void UrlAuthorityTest(string? userName, string? password, string hostName, ushort? port, string expected)
    {
        Url.UrlAuthority target;
        if (userName is null)
        {
            target = new Url.UrlAuthority(hostName, port);
            Assert.Null(target.UserInfo);
        }
        else
        {
            target = new Url.UrlAuthority(new Url.UserInfo(userName, password), hostName, port);
            Url.UserInfo userInfo = target.UserInfo!;
            Assert.NotNull(userInfo);
            Assert.Equal(userName, userInfo.UserName);
            if (password is null)
                Assert.Null(userInfo.Password);
            else
            {
                Assert.NotNull(userInfo.Password);
                Assert.Equal(password, userInfo.Password);
            }
        }
        Assert.Equal(hostName, target.HostName);
        if (port.HasValue)
        {
            Assert.NotNull(target.Port);
            Assert.Equal(port, target.Port);
        }
        else
            Assert.Null(target.Port);
        string actual = target.ToString();
        Assert.Equal(expected, actual);
    }

    public class UriQueryElementTestData : TheoryData<string, string?, string>
    {
        public UriQueryElementTestData()
        {
            foreach (char k in new char[] { ' ', '"', '#', '%', '&', '<', '=', '>', '?', '[', '\\', ']', '^', '`', '{', '|', '}' })
            {
                Add(k.ToString(), null, Uri.HexEscape(k));
                foreach (char v in new char[] { ' ', '"', '#', '%', '&','<', '>', '?', '[', '\\', ']', '^', '`', '{', '|', '}' })
                    Add(k.ToString(), v.ToString(), $"{Uri.HexEscape(k)}={Uri.HexEscape(v)}");
                foreach (string value in new string[] { "MyVal", "a", "A", "z", "Z", "^", "!", "$", "'", "(", ")", "*", "+", ",", ":", ";", "@", "=", "/", ".", "~", "-", string.Empty })
                    Add(k.ToString(), value, $"{Uri.HexEscape(k)}={value}");
            }

            foreach (string key in new string[] { "Test", "a", "A", "z", "Z", "!", "$", "'", "(", ")", "*", "+", ",", ":", ";", "@", "/", ".", "~", "-", string.Empty })
            {
                Add(key, null, key);
                foreach (char v in new char[] { ' ', '"', '#', '%', '&','<', '>', '?', '[', '\\', ']', '^', '`', '{', '|', '}' })
                    Add(key, v.ToString(), $"{key}={Uri.HexEscape(v)}");
                foreach (string value in new string[] { "MyVal", "a", "A", "z", "Z", "^", "!", "$", "'", "(", ")", "*", "+", ",", ":", ";", "@", "=", "/", ".", "~", "-", string.Empty })
                    Add(key, value, $"{key}={value}");
            }
        }
    }

    [Theory]
    [ClassData(typeof(UriQueryElementTestData))]
    public void UriQueryElementTest(string key, string? value, string expected)
    {
        Url.UriQueryElement target = new(key, value);
        Assert.Equal(target.Key, key);
        string actual;
        if (value is null)
            Assert.Null(target.Value);
        else
        {
            actual = target.Value!;
            Assert.NotNull(actual);
            Assert.Equal(value, actual);
        }
        actual = target.ToString();
        Assert.Equal(expected, actual);
    }

    record QueryTestDataItem(string Key, string? Value, string Expected)
    {
        internal JsonObject ToJson() => new()
        {
            { nameof(Key), JsonValue.Create(Key) },
            { nameof(Value), (Value is null) ? null : JsonValue.Create(Value) },
            { nameof(Expected), JsonValue.Create(Expected) }
        };
        
        internal static QueryTestDataItem? FromJson(string? jsonString)
        {
            return string.IsNullOrEmpty(jsonString) ? null : FromJson(JsonNode.Parse(jsonString) as JsonObject);
        }

        internal static QueryTestDataItem? FromJson(JsonObject? obj)
        {
            if (obj is null)
                return null;
            return new(obj[nameof(Key)]!.AsValue().GetValue<string>(), (obj[nameof(Value)] is JsonValue pw) ? pw.GetValue<string>() : null, obj[nameof(Expected)]!.AsValue().GetValue<string>());
        }
    }

    private static readonly QueryTestDataItem[] ValidQuerySubComponents = new QueryTestDataItem[]
    {
        new("id", "12", "id=12"),
        new(string.Empty, string.Empty, "="),
        new("%20", "[nil]", "%2520=%5Bnil%5D")
    };

    record PathTestDataItem(string Path, bool IsRooted, string[] Expected);
    
    private static readonly PathTestDataItem[] ValidPathComponents = new PathTestDataItem[]
    {
        new("", false, Array.Empty<string>()),
        new("/", true, Array.Empty<string>()),
        new("\\", true, Array.Empty<string>()),
        new("\\/", true, Array.Empty<string>()),
        new("Test", false, new string[] { "Test" }),
        new("Test/", false, new string[] { "Test" }),
        new("Test\\Data", false, new string[] { "Test", "Data" }),
        new(" ", false, new string[] { "%20" }),
        new("/Test", true, new string[] { "Test" }),
        new("/Test\\Data", true, new string[] { "Test", "Data" })
    };

    record FragmentTestDataItem(string Fragment, string Expected);

    private static readonly FragmentTestDataItem[] ValidFragmentComponents = new FragmentTestDataItem[]
    {
        new("/page", "/page"),
        new("!$&'()*+,:;@=/?.~-AazZ", "!$&'()*+,:;@=/?.~-AazZ"),
        new("#frag", "%22frag"),
        new("\"#%<>[\\]^`{|}", "%20%22%23%25%3C%3E%5B%5C%5D%5E%60%7B%7C%7D"),
        new(string.Empty, string.Empty)
    };

    /*
    
        */

    public class Constructor6TestData : TheoryData<string, byte, string, string?, string?, string?, string>
    {
        public Constructor6TestData()
        {
            foreach (ValidSchemeItem scheme in ValidSchemeNames.Where(s => s.CanHavePath))
            {
                foreach (ValidAuthorityItem authority in scheme.Authorities)
                {
                    foreach (FragmentTestDataItem fragment in ValidFragmentComponents)
                    {
                        Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), null, null, fragment.Fragment, $"{scheme.Expected}{authority.Expected}#{fragment.Expected}");
                        string expected = string.Join("&", ValidQuerySubComponents.Select(q => q.Expected));
                        JsonArray arr = new();
                        foreach (JsonObject obj in ValidQuerySubComponents.Select(q => q.ToJson()))
                            arr.Add(obj);
                        string qj = arr.ToJsonString();
                        Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), null, qj, fragment.Fragment, $"{scheme.Expected}{authority.Expected}?{expected}#{fragment.Expected}");
                        expected = ValidQuerySubComponents[0].Expected;
                        arr = new()
                        {
                            ValidQuerySubComponents[0].ToJson()
                        };
                        qj = arr.ToJsonString();
                        Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), null, qj, fragment.Fragment, $"{scheme.Expected}{authority.Expected}?{expected}#{fragment.Expected}");
                        foreach (PathTestDataItem path in ValidPathComponents)
                        {
                            if (path.IsRooted)
                                Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), path.Path, null, fragment.Fragment, $"{scheme.Expected}{authority.Expected}{path.Expected}#{fragment.Expected}");
                            else
                                Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), path.Path, null, fragment.Fragment, $"{scheme.Expected}{authority.Expected}/{path.Expected}#{fragment.Expected}");
                            expected = string.Join("&", ValidQuerySubComponents.Select(q => q.Expected));
                            arr = new();
                            foreach (JsonObject obj in ValidQuerySubComponents.Select(q => q.ToJson()))
                                arr.Add(obj);
                            qj = arr.ToJsonString();
                            if (path.IsRooted)
                                Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), path.Path, qj, fragment.Fragment, $"{scheme.Expected}{authority.Expected}{path.Expected}?{expected}#{fragment.Expected}");
                            else
                                Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), path.Path, qj, fragment.Fragment, $"{scheme.Expected}{authority.Expected}/{path.Expected}?{expected}#{fragment.Expected}");
                            expected = ValidQuerySubComponents[0].Expected;
                            arr = new()
                            {
                                ValidQuerySubComponents[0].ToJson()
                            };
                            qj = arr.ToJsonString();
                            if (path.IsRooted)
                                Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), path.Path, qj, fragment.Fragment, $"{scheme.Expected}{authority.Expected}{path.Expected}?{expected}#{fragment.Expected}");
                            else
                                Add(scheme.Name, (byte)scheme.SchemeSeparator, authority.ToJson().ToJsonString(), path.Path, qj, fragment.Fragment, $"{scheme.Expected}{authority.Expected}/{path.Expected}?{expected}#{fragment.Expected}");
                        }
                    }
                }
            }
        }
    }

    [Theory]
    [ClassData(typeof(Constructor6TestData))]
    public void Constructor6Test(string scheme, byte schemeSeparator, string authority, string? path, string? query, string fragment, string expected)
    {
        Url.SchemeSeparatorType expectedSeparator = (Url.SchemeSeparatorType)schemeSeparator;
        ValidAuthorityItem a = ValidAuthorityItem.FromJson(authority)!;
        Url.UrlAuthority ua = new(a.HostName, a.Port);
        if (a.UserInfo is not null)
            ua.UserInfo = new(a.UserInfo.UserName, a.UserInfo.Password);
        Url target;
        if (!string.IsNullOrEmpty(query) && JsonNode.Parse(query) is JsonArray qa)
            target = new Url(scheme, expectedSeparator, ua, path, qa.Select(n => QueryTestDataItem.FromJson(n as JsonObject)).Select(d => new Url.UriQueryElement(d.Key, d.Value)), fragment);
        else
            target = new Url(scheme, expectedSeparator, ua, path, null, fragment);
        Assert.Equal(scheme, target.Scheme);
        Assert.Equal(expectedSeparator, target.SchemeSeparator);
    }

    public class Constructor5aTestData : TheoryData<string, byte, string, string?, string?, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor5aTestData))]
    public void Constructor5aTest(string scheme, byte schemeSeparator, string authority, string? path, string query, string expected)
    {

    }

    public class Constructor4TestData : TheoryData<string, byte, string, string?, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor4TestData))]
    public void Constructor4Test(string scheme, byte schemeSeparator, string authority, string path, string expected)
    {

    }

    public class Constructor3TestData : TheoryData<string, byte, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor3TestData))]
    public void Constructor3Test(string scheme, byte schemeSeparator, string authority, string expected)
    {

    }

    public class Constructor5bTestData : TheoryData<string, byte, string, string?, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor5bTestData))]
    public void Constructor5bTest(string scheme, byte schemeSeparator, string path, string? query, string fragment, string expected)
    {

    }

    public class Constructor4bTestData : TheoryData<string, byte, string, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor4bTestData))]
    public void Constructor4bTest(string scheme, byte schemeSeparator, string path, string expected)
    {

    }

    public class Constructor3bTestData : TheoryData<string, byte, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor3bTestData))]
    public void Constructor3bTest(string scheme, byte schemeSeparator, string path, string expected)
    {

    }

    public class Constructor4cTestData : TheoryData<string?, string?, string?, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor4cTestData))]
    public void Constructor4cTest(string? authority, string? path, string? query, string fragment, string expected)
    {

    }

    public class Constructor3cTestData : TheoryData<string?, string?, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor3cTestData))]
    public void Constructor3cTest(string? authority, string? path, string query, string expected)
    {

    }

    public class Constructor2aTestData : TheoryData<string?, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor2aTestData))]
    public void Constructor2aTest(string? authority, string path, string expected)
    {

    }

    public class Constructor1aTestData : TheoryData<string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor1aTestData))]
    public void Constructor1aTest(string authority, string expected)
    {

    }

    public class Constructor3dTestData : TheoryData<string?, string?, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor3dTestData))]
    public void Constructor3dTest(string? path, string? query, string fragment, string expected)
    {

    }

    public class Constructor2bTestData : TheoryData<string?, string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor2bTestData))]
    public void Constructor2bTest(string? path, string query, string expected)
    {

    }

    public class Constructor1bTestData : TheoryData<string, string>
    {

    }

    [Theory]
    [ClassData(typeof(Constructor1bTestData))]
    public void Constructor1bTest(string path, string expected)
    {

    }

    [Fact]
    public void Constructor0Test()
    {

    }
}