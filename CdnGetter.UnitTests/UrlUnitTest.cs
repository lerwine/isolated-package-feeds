using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CdnGetter.UnitTests;

public class UrlUnitTest
{
    /// <summary>
    /// Generates test data for <see cref="ValidIpV6RegexTest(string, bool, string)" />.
    /// </summary>
    public class ValidIpV6RegexTestTestData : TheoryData<string, bool, string>
    {
        public ValidIpV6RegexTestTestData()
        {
            Add("::", true, "Compressed IPv6 Adress");
            Add("::b6:d63e:bf:a:73:4e:ee14", true, "Compressed IPv6 Adress");
            Add("::964:12:876d:fd3:46:683:c3", true, "Compressed IPv6 Adress");
            Add("::73.9.79.14", true, "IPv4-mapped IPv6 addresses and IPv4-translated addresses");
            Add("::ec3", true, "Compressed IPv6 Adress");
            Add("::1b:3:24.196.75.82", true, "IPv4-mapped IPv6 addresses and IPv4-translated addresses");
            Add("::7:167.95.6.0", true, "IPv4-mapped IPv6 addresses and IPv4-translated addresses");
            Add("50::", true, "Compressed IPv6 Adress");
            Add("8::66:2e6d:81:802e:dc4:88", true, "Compressed IPv6 Adress");
            Add("6::c2:9023:89:9db:e3:1bcb", true, "Compressed IPv6 Adress");
            Add("ff83::4a9:d20:4838:6ea:5", true, "Compressed IPv6 Adress");
            Add("ef::855e:d31:64:e", true, "Compressed IPv6 Adress");
            Add("9::e:b656:c660", true, "Compressed IPv6 Adress");
            Add("e::58:ef5", true, "Compressed IPv6 Adress");
            Add("6df6::9", true, "Compressed IPv6 Adress");
            Add("e46::2b", true, "Compressed IPv6 Adress");
            Add("ab36:63::301:7342:7bc:fb49:fe4", true, "Compressed IPv6 Adress");
            Add("d:324d::ee", true, "Compressed IPv6 Adress");
            Add("21:f66e:f5::4a7:ae1:4e14:34", true, "Compressed IPv6 Adress");
            Add("7107:9ac:a::abed", true, "Compressed IPv6 Adress");
            Add("d:a4:d:6::6d9:b8:a11", true, "Compressed IPv6 Adress");
            Add("4be:b:9ae:f32f::9b4", true, "Compressed IPv6 Adress");
            Add("94e:bd:4:d:a::249c:ac9", true, "Compressed IPv6 Adress");
            Add("ceaf:4:5625:88bb:2aa::500", true, "Compressed IPv6 Adress");
            Add("640:5:4c2d:d:9190:825::8", true, "Compressed IPv6 Adress");
            Add("9d:fb:8e85:661b:bf4c:8f2:6::", true, "Compressed IPv6 Adress");
            Add("31c:2:e259:bc1:0:94cf:b:3ddf", true, "Normal IPv6 Adress");
            Add("0:2:e259:bc1:c70:94cf:b:3ddf", true, "Normal IPv6 Adress");
            Add("0:2:e259:bc1:c70:94cf:b:0", true, "Normal IPv6 Adress");
            Add("8:7:4:712a::18.13.33.76", true, "IPv4-Embedded IPv6 Address");
            Add("b9:b::45.30.145.227", true, "IPv4-Embedded IPv6 Address");
            Add("2c::e1:a0f%1", true, "Link-local IPv6 addresses with zone index");
            Add("9b::7a:a3%eth0", true, "Link-local IPv6 addresses with zone index");
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
    public void ValidIpV6RegexTest(string address, bool expected, string description)
    {
        bool actual = Url.ValidIpV6Regex.IsMatch(address);
        if (expected)
            Assert.True(actual, description);
        else
            Assert.False(actual, description);
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