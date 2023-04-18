using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CdnGetter.UnitTests;

public class UrlUnitTest
{
    /// <summary>
    /// Generates test data for <see cref="ValidIpV6RegexTest(string, bool, bool, string)" />.
    /// </summary>
    public class ValidIpV6RegexTestTestData : TheoryData<string, bool, bool, string>
    {
        public ValidIpV6RegexTestTestData()
        {
            Add("C7:960E:D72:4:98:b5:6.58.0.65", true, true, "IPv4-Embedded IPv6 Address");
            Add("Dc:c19:3a:da9:B2a:EAC6:8:eea", false, true, "Normal IPv6 Address");
            Add("e:F97:9:50:635f:d6:f7b::", false, true, "Compressed IPv6 Address");
            Add("Cd:d27:70:2:5:3::B04", false, true, "Compressed IPv6 Address");
            Add("92c:91:9:43:9b::86.48.116.221", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("a:35:6:3:f885::475f:EB6b", false, true, "Compressed IPv6 Address");
            Add("A95b:49D:Bca9:1f64::385:21.66.200.0", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("79:f:7e:22::Ce:7fd7:40", false, true, "Compressed IPv6 Address");
            Add("923c:e9d7:bb0d::3:26D1:1.110.4.29", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("d10b:5B2a:FBAa::3C:Ce2E:0:1F", false, true, "Compressed IPv6 Address");
            Add("3520:d2::F3C:80a7:488c:98.137.35.0", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("93:A03::8B:eea:9e3:7a05:f753", false, true, "Compressed IPv6 Address");
            Add("2577::AB:48cf:75F:5c:5.231.0.64", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("3f27::C52:36:CCf4:c:51:5EB", false, true, "Compressed IPv6 Address");
            Add("::87:B:131A:cE5:fe:3.85.88.196", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("::3:c65:19:40c:F9c5:e:1", false, true, "Compressed IPv6 Address");
            Add("b9A:64:802:8:f:9Fd4::", false, true, "Compressed IPv6 Address");
            Add("159:eA5:3:B:7::56.1.6.5", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("705:2bDd:dE3:5ba:4::95", false, true, "Compressed IPv6 Address");
            Add("B:C916:b3:fCC::241.166.135.7", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("7:81:91:80::7a6:c0", false, true, "Compressed IPv6 Address");
            Add("D1:C:A::F1:42.151.1.217", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("7d2:7:Cecd::B:ffB4:9", false, true, "Compressed IPv6 Address");
            Add("75:0::e0:1B2:119.2.118.8", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("2f76:4F::970B:4ff8:d:9585", false, true, "Compressed IPv6 Address");
            Add("55d::45Bd:c2:411:6.130.68.161", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("14::B3D:46D:b5B9:e63:d7e", false, true, "Compressed IPv6 Address");
            Add("::cC:863e:22Eb:9F:9.125.93.25", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("::3:3F92:957:2:4C:8445", false, true, "Compressed IPv6 Address");
            Add("4B:9FF:b:8DC:83e4::4.2.2.4", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("B:Bf7:6d4:8:e::", false, true, "Compressed IPv6 Address");
            Add("ABE8:A:d:e::21.8.0.70", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("651A:796f:c0:E596::2", false, true, "Compressed IPv6 Address");
            Add("acB3:499:82::83.124.5.4", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("4:34E5:4A::a96:de", false, true, "Compressed IPv6 Address");
            Add("c8:b61::189:148.11.5.1", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("b10:6c::42:67:6", false, true, "Compressed IPv6 Address");
            Add("1f::C2Cb:E80:25.8.210.238", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("B::Fa:3:84:D", false, true, "Compressed IPv6 Address");
            Add("::9c:32Ee:34:17.30.5.0", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("::a8e:Df:67:F2:6eFB", false, true, "Compressed IPv6 Address");
            Add("8:72d2:E8:E9aC::9.0.176.121", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("bB:9:BfF:70::", false, true, "Compressed IPv6 Address");
            Add("71:981C:B::60.188.7.15", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("c215:6:4ae::A5B4", false, true, "Compressed IPv6 Address");
            Add("E5E:2::193.138.164.4", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("E:6::5aa:b5", false, true, "Compressed IPv6 Address");
            Add("69::13:149.150.27.2", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("B2e::f2A:239:EB0", false, true, "Compressed IPv6 Address");
            Add("::863:9d66:247.231.228.129", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("::2198:5d:17:b1", false, true, "Compressed IPv6 Address");
            Add("950:A318:D::167.1.195.60", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("d:e7f:dc::", false, true, "Compressed IPv6 Address");
            Add("2A:8Da::13.60.44.53", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("180:2E::32b", false, true, "Compressed IPv6 Address");
            Add("E6::5.59.113.28", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("7::88A:853", false, true, "Compressed IPv6 Address");
            Add("::8E:158.1.27.8", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("::73f:9932:e3", false, true, "Compressed IPv6 Address");
            Add("7:6f::230.18.46.33", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("DF9:197::", false, true, "Compressed IPv6 Address");
            Add("4d0::215.2.2.98", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("D3::899C", false, true, "Compressed IPv6 Address");
            Add("::0.128.2.3", true, true, "IPv4-Embedded, Compressed IPv6 Address");
            Add("::8105:C", false, true, "Compressed IPv6 Address");
            Add("ba64::", false, true, "Compressed IPv6 Address");
            Add("::26B", false, true, "Compressed IPv6 Address");
            Add("::", false, true, "Compressed IPv6 Address");
            Add("B:e84B:Fe23:F:5A7:2:4f7:125:", false, false, "Invalid IPv6 Address");
            Add("6B0:71ce:FB42:533:a15D:EDA:8809:905:119.21.109.171", true, false, "Invalid IPv6 Address");
            Add(":d09:7D9:F8:407:DEE:e707:d4:45", false, false, "Invalid IPv6 Address");
            Add(":8:383b:24:1:a68:9E:7Baa:8:52.102.16.248", true, false, "Invalid IPv6 Address");
            Add("7A2d:11:C19:0:4e62:6332:c", false, false, "Invalid IPv6 Address");
            Add("9:9af:B921:146:32:71c9:D8:2.114.12.9", true, false, "Invalid IPv6 Address");
            Add("2593:f0:4c:8CE0:C7:AE:35:", false, false, "Invalid IPv6 Address");
            Add(":c5:4a:FA:2:3B:6d:9A23", false, false, "Invalid IPv6 Address");
            Add(":e0:1D5:41E:a43F:D:74:E8:99.224.93.120", true, false, "Invalid IPv6 Address");
            Add("66D6:3f9:9915:5654:120:3A", false, false, "Invalid IPv6 Address");
            Add("9:5:2:b5:7aa:De7E:", false, false, "Invalid IPv6 Address");
            Add(":2CA:4f5:7:1973:4A8:6", false, false, "Invalid IPv6 Address");
            Add(":70:4d:D2b3:4:1:181a:197.151.34.41", true, false, "Invalid IPv6 Address");
            Add("e5:C7:9:2:4", false, false, "Invalid IPv6 Address");
            Add("e8E:aa1:a:2149:Da30:6.196.54.5", true, false, "Invalid IPv6 Address");
            Add("9c7:64:BB1F:74c4:95dF:", false, false, "Invalid IPv6 Address");
            Add(":3:14e:e:Dc0:5E", false, false, "Invalid IPv6 Address");
            Add("249:8d6:A7:c", false, false, "Invalid IPv6 Address");
            Add("d:93:8e:8a:132.8.199.141", true, false, "Invalid IPv6 Address");
            Add("b878:3:dD:8:", false, false, "Invalid IPv6 Address");
            Add(":f:A:E:3084", false, false, "Invalid IPv6 Address");
            Add(":3a1:1a:A:3cdC:72.3.199.93", true, false, "Invalid IPv6 Address");
            Add("47:ad8:e1E", false, false, "Invalid IPv6 Address");
            Add("57:e45e:593:80.89.9.5", true, false, "Invalid IPv6 Address");
            Add("B6:3:f23D:", false, false, "Invalid IPv6 Address");
            Add(":c3e:C881:6a7E", false, false, "Invalid IPv6 Address");
            Add(":23:dC:60:76.250.2.6", true, false, "Invalid IPv6 Address");
            Add("D13:3C:", false, false, "Invalid IPv6 Address");
            Add("b:2E:68.4.187.5", true, false, "Invalid IPv6 Address");
            Add("125:2", false, false, "Invalid IPv6 Address");
            Add(":7926:99", false, false, "Invalid IPv6 Address");
            Add(":e37:87:163.85.1.8", true, false, "Invalid IPv6 Address");
            Add(":A0", false, false, "Invalid IPv6 Address");
            Add(":247:131.69.222.16", true, false, "Invalid IPv6 Address");
            Add("4AE:", false, false, "Invalid IPv6 Address");
            Add("4D:9.30.6.30", true, false, "Invalid IPv6 Address");
            Add("E9:33:4::c1:6e:e::FF", false, false, "Invalid IPv6 Address");
            Add("C:D33f:d95D::57:7711:38eb::24b:13.81.4.110", true, false, "Invalid IPv6 Address");
            Add("::6B1:7B:Ba:2Ab7:8:3d5e::e3FA", false, false, "Invalid IPv6 Address");
            Add("::8C:BC96:1c49:15F4:0:5d6::3:148.241.1.5", true, false, "Invalid IPv6 Address");
            Add("::6f:bc:E44:73:7e::a2", false, false, "Invalid IPv6 Address");
            Add("::Fcb:5dE2:b4d:B:6::1d6a:58.8.101.204", true, false, "Invalid IPv6 Address");
            Add("::CC88:7:21:164:CA1:393d:a::", false, false, "Invalid IPv6 Address");
            Add("::e7e:92:99:5746:fd:711:f8::51.139.66.246", true, false, "Invalid IPv6 Address");
            Add("::8636:9:2:734F:aA5:d::", false, false, "Invalid IPv6 Address");
            Add("::5:2f:64Bd:97a:34:dcf7::189.34.5.46", true, false, "Invalid IPv6 Address");
            Add(":9A8F:e:b2:d8:EF:F1:78::", false, false, "Invalid IPv6 Address");
            Add(":6016:9:3b:3b87:b6:42d:9a1F::212.14.6.169", true, false, "Invalid IPv6 Address");
            Add("::9B:218:E1E0:8a:eFF2:6b:17F:", false, false, "Invalid IPv6 Address");
            Add("::75:7:bA:5:4F:dc95:9:41.217.54.4", true, false, "Invalid IPv6 Address");
            Add("::763A:19:FC8b:B535:6c0:bef4:", false, false, "Invalid IPv6 Address");
            Add("::F6:8:3:e9:6d:1:204.163.213.120", true, false, "Invalid IPv6 Address");
            Add(":d896:a4:7:2683:97:8Fc:625B:", false, false, "Invalid IPv6 Address");
            Add("9:6c:53F:2:9:3b:4::8", false, false, "Invalid IPv6 Address");
            Add("11B:d43:a4f7:6d:B4:51:C6::A5Df:4.43.6.5", true, false, "Invalid IPv6 Address");
            Add("C:e63:15:8:e:8DD2:1:8Ab::", false, false, "Invalid IPv6 Address");
            Add("f:E:dCe8:620:aAa:c:AC3:22::242.248.194.3", true, false, "Invalid IPv6 Address");
            Add("::E8:9:5fAF:23b:163B:2b:3a9:33b", false, false, "Invalid IPv6 Address");
            Add("::6:b:6:74D8:D0:2a7:A8:865:212.95.17.44", true, false, "Invalid IPv6 Address");
            Add(":", false, false, "Invalid IPv6 Address");
            Add(":9.207.42.74", true, false, "Invalid IPv6 Address");
            Add("10.45.181.26", true, false, "Invalid IPv6 Address");
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
    public void ValidIpV6RegexTest(string address, bool embeddedIpV4, bool expected, string description)
    {
        bool actual = (embeddedIpV4 ? Url.ValidIpV6wV4Regex : Url.ValidIpV6Regex).IsMatch(address);
        if (expected)
            Assert.True(actual, $"{description}: {address}");
        else
            Assert.False(actual, $"{description}: {address}");
        actual = (embeddedIpV4 ? Url.ValidIpV6Regex : Url.ValidIpV6wV4Regex).IsMatch(address);
        Assert.False(actual, $"{description}: {address}");
    }
    
    /// <summary>
    /// Generates test data for <see cref="ValidIpV4RegexTestData(string, bool)" />.
    /// </summary>
    public class ValidIpV4RegexTestData : TheoryData<string, bool>
    {
        public ValidIpV4RegexTestData()
        {
            Random r = new Random();
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
    [ClassData(typeof(ValidIpV6RegexTestTestData))]
    public void ValidIpV4RegexTest(string address, bool expected)
    {
        bool actual = Url.ValidIpV4Regex.IsMatch(address);
        Assert.Equal(expected, actual);
    }
}