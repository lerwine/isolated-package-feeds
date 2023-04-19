using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CdnGetter.UnitTests;

public class UrlUnitTest
{
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
}