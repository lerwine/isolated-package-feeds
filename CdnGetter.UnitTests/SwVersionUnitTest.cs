using System.Text;
using System.Text.Json.Nodes;

namespace CdnGetter.UnitTests;

public class SwVersionUnitTest
{
    /// <summary>
    /// Generates test data for <see cref="PreReleaseSegmentConstructor1Test(bool, string?, string, string)" />.
    /// </summary>
    public class PreReleaseSegmentConstructor1TestData : TheoryData<bool, string?, string, string>
    {
        public PreReleaseSegmentConstructor1TestData()
        {
            Add(false, null, string.Empty, "-");
            Add(true, null, string.Empty, ".");
            Add(false, string.Empty, string.Empty, "-");
            Add(true, string.Empty, string.Empty, ".");
            foreach (string s in new string[] { "r" })
            {
                Add(false, s, s, $"-{s}");
                Add(true, s, s, $".{s}");
            }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion.PreReleaseSegment(bool, string)" /> that will not throw an excePATCHaion.
    /// </summary>
    [Theory]
    [ClassData(typeof(PreReleaseSegmentConstructor1TestData))]
    public void PreReleaseSegmentConstructor1Test(bool altSeprator, string? value, string expectedValue, string expectedToString)
    {
        SwVersion.PreReleaseSegment target = new(altSeprator, value!);
        Assert.Equal(altSeprator, target.AltSeparator);
        Assert.Equal(expectedValue, target.Value);
        string actualToString = target.ToString();
        Assert.Equal(expectedToString, actualToString);
    }

    /// <summary>
    /// Generates test data for <see cref="PreReleaseSegmentConstructor2Test(bool, string)" />.
    /// </summary>
    public class PreReleaseSegmentConstructor2TestData : TheoryData<bool, string>
    {
        public PreReleaseSegmentConstructor2TestData()
        {
            foreach (string s in new string[] {
                "-",  ".", "+",
                " - ",  " . ", " + ",
                "r- ",  "r. ", "r+ ",
                " -v",  " .v", " +v" })
            {
                Add(false, s);
                Add(true, s);
            }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion.PreReleaseSegment(bool, string)" /> that will throw an excePATCHaion.
    /// </summary>
    [Theory]
    [ClassData(typeof(PreReleaseSegmentConstructor2TestData))]
    public void PreReleaseSegmentConstructor2Test(bool altSeprator, string value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(nameof(value), () => new SwVersion.PreReleaseSegment(altSeprator, value));
    }

    [Serializable]
    public class VersionValues
    {
        public string? Prefix { get; set; }
        public int Major { get; set; }
        public int? Minor { get; set; }
        public int? Patch { get; set; }
        public int? Revision { get; set; }
        public int[]? AdditionalNumerical { get; set; }
        public PreReleaseSegment[]? PreRelease { get; set; }
        public BuildSegment[]? Build { get; set; }
        public SwVersion.VersionStringFormat Format { get; set; }
        public VersionValues() { }
        public VersionValues(string? prefix, int major, int? minor, int? patch, int? revision, int[]? additionalNumerical, PreReleaseSegment[]? preRelease, BuildSegment[]? build,
            SwVersion.VersionStringFormat format)
        {
            Prefix = prefix;
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            AdditionalNumerical = additionalNumerical;
            PreRelease = preRelease;
            Build = build;
            Format = format;
        }
        [Serializable]
        public class PreReleaseSegment
        {
            public bool AltSeparator { get; }
            public string Value { get; }
            public PreReleaseSegment() { Value = string.Empty; }
            public PreReleaseSegment(bool altSeparator, string value)
            {
                AltSeparator = altSeparator;
                Value = value ?? "";
            }
        }
        [Serializable]
        public class BuildSegment
        {
            public SwVersion.BuildSeparator Separator { get; }
            public string Value { get; }
            public BuildSegment() { Value = string.Empty; }
            public BuildSegment(SwVersion.BuildSeparator separator, string value)
            {
                Separator = separator;
                Value = value ?? "";
            }
        }
        internal void AssertEquals(SwVersion target)
        {
            Assert.Equal(Prefix, target.Prefix);
            Assert.Equal(Major, target.Major);
            Assert.Equal(Minor, target.Minor);
            Assert.Equal(Patch, target.Patch);
            Assert.Equal(Revision, target.Revision);
            if (AdditionalNumerical is null)
                Assert.Null(target.AdditionalNumerical);
            else
            {
                Assert.NotNull(target.AdditionalNumerical);
                Assert.Equal(AdditionalNumerical, target.AdditionalNumerical);
            }
            if (PreRelease is null)
                Assert.Null(target.PreRelease);
            else
            {
                Assert.NotNull(target.PreRelease);
                Assert.Equal(PreRelease.Select(t => new SwVersion.PreReleaseSegment(t.AltSeparator, t.Value)), target.PreRelease);
            }
            if (Build is null)
                Assert.Null(target.Build);
            else
            {
                Assert.NotNull(target.Build);
                Assert.Equal(Build.Select(t => new SwVersion.BuildSegment(t.Separator, t.Value)), target.Build);
            }
            Assert.Equal(Format, target.Format);
        }
    }
    
    /// <summary>
    /// Generates test data for <see cref="ParsingConstructorTest(string?, VersionValues)" />
    /// </summary>
    public class ParsingConstructorTestData : TheoryData<string?, string>
    {
        public ParsingConstructorTestData()
        {
            Add("-q*[E]o-^&gU$^A-Ma<!XS\"58.21.35.80*]/j", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-q*[E]o-^&gU$^A-Ma<!XS\"")
                .AddProperty(nameof(SwVersion.Major), 58).AddProperty(nameof(SwVersion.Minor), 21).AddProperty(nameof(SwVersion.Patch), 35).AddProperty(nameof(SwVersion.Revision), 80)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "*]/j").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("/m$]{en>\"/-P(Bj#\\70.75.5OT+0$L", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "/m$]{en>\"/-P(Bj#\\")
                .AddProperty(nameof(SwVersion.Major), 70).AddProperty(nameof(SwVersion.Minor), 75).AddProperty(nameof(SwVersion.Patch), 5).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "OT").AddProperty(nameof(SwVersion.Build), "+0$L").ToJsonString());
            Add("-q-Cv%\"-~_&!:,'-27.97b", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-q-Cv%\"-~_&!:,'")
                .AddProperty(nameof(SwVersion.Major), -27).AddProperty(nameof(SwVersion.Minor), 97).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "b").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add(">LM']:U|fX~-Y\\rhDMV52.67b", new JsonObject().AddProperty(nameof(SwVersion.Prefix), ">LM']:U|fX~-Y\\rhDMV")
                .AddProperty(nameof(SwVersion.Major), 52).AddProperty(nameof(SwVersion.Minor), 67).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "b").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-O%JsT-c)j-DMH98.56I", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-O%JsT-c)j-DMH")
                .AddProperty(nameof(SwVersion.Major), 98).AddProperty(nameof(SwVersion.Minor), 56).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "I").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-}-itY'W'Hx-80.5-", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-}-itY'W'Hx")
                .AddProperty(nameof(SwVersion.Major), -80).AddProperty(nameof(SwVersion.Minor), 5).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("|n?N,=* K{]#J*Ns-23.49t+u", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "|n?N,=* K{]#J*Ns")
                .AddProperty(nameof(SwVersion.Major), -23).AddProperty(nameof(SwVersion.Minor), 49).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "t").AddProperty(nameof(SwVersion.Build), "+u").ToJsonString());
            Add("U&y-THWKh=]|Gj(y;H68+Y", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "U&y-THWKh=]|Gj(y;H")
                .AddProperty(nameof(SwVersion.Major), 68).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddProperty(nameof(SwVersion.Build), "+Y").ToJsonString());
            Add("-,-E\"w(-IfdUa12+", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-,-E\"w(-IfdUa")
                .AddProperty(nameof(SwVersion.Major), 12).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddProperty(nameof(SwVersion.Build), "+").ToJsonString());
            Add("?%\\psP-}nb[NW*38.19n+Q-7:$", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "?%\\psP-}nb[NW*")
                .AddProperty(nameof(SwVersion.Major), 38).AddProperty(nameof(SwVersion.Minor), 19).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "n").AddProperty(nameof(SwVersion.Build), "+Q-7:$").ToJsonString());
            Add("-YEkQ$VEKv!T&27.60-", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-YEkQ$VEKv!T&")
                .AddProperty(nameof(SwVersion.Major), 27).AddProperty(nameof(SwVersion.Minor), 60).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("->eH,<(b-=-J* qC\\a954woi+Kn", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "->eH,<(b-=-J* qC\\a")
                .AddProperty(nameof(SwVersion.Major), 95).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "4woi").AddProperty(nameof(SwVersion.Build), "+Kn").ToJsonString());
            Add("->h>-iJIh_b/-p'gUb-25.25.92.0.0.9.13.79d%\"h", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "->h>-iJIh_b/-p'gUb")
                .AddProperty(nameof(SwVersion.Major), -25).AddProperty(nameof(SwVersion.Minor), 25).AddProperty(nameof(SwVersion.Patch), 92).AddProperty(nameof(SwVersion.Revision), 0)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical), 0, 9, 13, 79).AddProperty(nameof(SwVersion.PreRelease), "d%\"h").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-Jt-PICXVU-JF`o22.90-+Ok", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-Jt-PICXVU-JF`o")
                .AddProperty(nameof(SwVersion.Major), 22).AddProperty(nameof(SwVersion.Minor), 90).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddProperty(nameof(SwVersion.Build), "+Ok").ToJsonString());
            Add("}<hm&iODMV#c:57.8.44]S", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "}<hm&iODMV#c:")
                .AddProperty(nameof(SwVersion.Major), 57).AddProperty(nameof(SwVersion.Minor), 8).AddProperty(nameof(SwVersion.Patch), 44).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "]S").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("->k!\\>,-Ki[Y&yG(64.65-", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "->k!\\>,-Ki[Y&yG(")
                .AddProperty(nameof(SwVersion.Major), 64).AddProperty(nameof(SwVersion.Minor), 65).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add(",^XN-|Mg/:xx]]/M-71.42{+O,", new JsonObject().AddProperty(nameof(SwVersion.Prefix), ",^XN-|Mg/:xx]]/M")
                .AddProperty(nameof(SwVersion.Major), -71).AddProperty(nameof(SwVersion.Minor), 42).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "{").AddProperty(nameof(SwVersion.Build), "+O,").ToJsonString());
            Add("-$g#k-P-(KM|/eV-80.90}", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-$g#k-P-(KM|/eV")
                .AddProperty(nameof(SwVersion.Major), -80).AddProperty(nameof(SwVersion.Minor), 90).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "}").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("^Zpa*-\"kCY,HZzr~_$-19.21-r5ULG+[", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "^Zpa*-\"kCY,HZzr~_$")
                .AddProperty(nameof(SwVersion.Major), -19).AddProperty(nameof(SwVersion.Minor), 21).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-r5ULG").AddProperty(nameof(SwVersion.Build), "+[").ToJsonString());
            Add("JJA|B_<;[~U|Ia\"-55.91.18.27edTEpn%", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "JJA|B_<;[~U|Ia\"")
                .AddProperty(nameof(SwVersion.Major), -55).AddProperty(nameof(SwVersion.Minor), 91).AddProperty(nameof(SwVersion.Patch), 18).AddProperty(nameof(SwVersion.Revision), 27)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "edTEpn%").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-U{-jq-m;z~h-20", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-U{-jq-m;z~h")
                .AddProperty(nameof(SwVersion.Major), -20).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-CDTO'XcDs&qz_-17.734+ej.JYZ", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-CDTO'XcDs&qz_")
                .AddProperty(nameof(SwVersion.Major), -17).AddProperty(nameof(SwVersion.Minor), 73).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "4").AddProperty(nameof(SwVersion.Build), "+ej.JYZ").ToJsonString());
            Add("v(Td-pPH$>SiE_\\32.75.76.779Ri+", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "v(Td-pPH$>SiE_\\")
                .AddProperty(nameof(SwVersion.Major), 32).AddProperty(nameof(SwVersion.Minor), 75).AddProperty(nameof(SwVersion.Patch), 76).AddProperty(nameof(SwVersion.Revision), 77)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "9Ri").AddProperty(nameof(SwVersion.Build), "+").ToJsonString());
            Add("-|=xatnj-Z|u@-t-40.42-FnW|^7", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-|=xatnj-Z|u@-t")
                .AddProperty(nameof(SwVersion.Major), -40).AddProperty(nameof(SwVersion.Minor), 42).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-FnW|^7").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("#obPg,-=/Bxfa10.51.27<q,", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "#obPg,-=/Bxfa")
                .AddProperty(nameof(SwVersion.Major), 10).AddProperty(nameof(SwVersion.Minor), 51).AddProperty(nameof(SwVersion.Patch), 27).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "<q,").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("Ci_=CQT-kBjdO-10+hqH2J", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "Ci_=CQT-kBjdO")
                .AddProperty(nameof(SwVersion.Major), -10).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddProperty(nameof(SwVersion.Build), "+hqH2J").ToJsonString());
            Add("p(aiaa-FmD~t[!-'65.16.", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "p(aiaa-FmD~t[!-'")
                .AddProperty(nameof(SwVersion.Major), 65).AddProperty(nameof(SwVersion.Minor), 16).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), ".").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-w'tm-*N_Q-):o]na-53.35?", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-w'tm-*N_Q-):o]na")
                .AddProperty(nameof(SwVersion.Major), -53).AddProperty(nameof(SwVersion.Minor), 35).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "?").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-[VfAj(N*&&-UYd47.30.47.12Qyd+h[I", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-[VfAj(N*&&-UYd")
                .AddProperty(nameof(SwVersion.Major), 47).AddProperty(nameof(SwVersion.Minor), 30).AddProperty(nameof(SwVersion.Patch), 47).AddProperty(nameof(SwVersion.Revision), 12)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "Qyd").AddProperty(nameof(SwVersion.Build), "+h[I").ToJsonString());
            Add("-j-[-AL-86.61.88-+ x", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-j-[-AL")
                .AddProperty(nameof(SwVersion.Major), -86).AddProperty(nameof(SwVersion.Minor), 61).AddProperty(nameof(SwVersion.Patch), 88).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddProperty(nameof(SwVersion.Build), "+ x").ToJsonString());
            Add("Z-{bB-AI\\]j19.10.32-", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "Z-{bB-AI\\]j")
                .AddProperty(nameof(SwVersion.Major), 19).AddProperty(nameof(SwVersion.Minor), 10).AddProperty(nameof(SwVersion.Patch), 32).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("\\~x_gL-U~/X-unThB96.23.75D,", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "\\~x_gL-U~/X-unThB")
                .AddProperty(nameof(SwVersion.Major), 96).AddProperty(nameof(SwVersion.Minor), 23).AddProperty(nameof(SwVersion.Patch), 75).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "D,").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-B)[bIy-,([$wI43.39#+E6w;[9", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-B)[bIy-,([$wI")
                .AddProperty(nameof(SwVersion.Major), 43).AddProperty(nameof(SwVersion.Minor), 39).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "#").AddProperty(nameof(SwVersion.Build), "+E6w;[9").ToJsonString());
            Add("-u-?EImMnpD@g`72.17.94.63.51.83Zp", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-u-?EImMnpD@g`")
                .AddProperty(nameof(SwVersion.Major), 72).AddProperty(nameof(SwVersion.Minor), 17).AddProperty(nameof(SwVersion.Patch), 94).AddProperty(nameof(SwVersion.Revision), 63)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical), 51, 83).AddProperty(nameof(SwVersion.PreRelease), "Zp").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-`'e>{q!`S-17.52.70.43i\"T", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-`'e>{q!`S")
                .AddProperty(nameof(SwVersion.Major), -17).AddProperty(nameof(SwVersion.Minor), 52).AddProperty(nameof(SwVersion.Patch), 70).AddProperty(nameof(SwVersion.Revision), 43)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "i\"T").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-YOq}mcM-A-Z([`94.88.89.9-u", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-YOq}mcM-A-Z([`")
                .AddProperty(nameof(SwVersion.Major), 94).AddProperty(nameof(SwVersion.Minor), 88).AddProperty(nameof(SwVersion.Patch), 89).AddProperty(nameof(SwVersion.Revision), 9)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-u").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("OJBvb-~F-xs`(X86-vcYho5+", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "OJBvb-~F-xs`(X")
                .AddProperty(nameof(SwVersion.Major), 86).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-vcYho5").AddProperty(nameof(SwVersion.Build), "+").ToJsonString());
            Add("-<y-\\RwH`D{PF_g-50-]=x", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-<y-\\RwH`D{PF_g")
                .AddProperty(nameof(SwVersion.Major), -50).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-]=x").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-F\"M-Y%dO$Ti-SBd@$-90-}5+L", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-F\"M-Y%dO$Ti-SBd@$")
                .AddProperty(nameof(SwVersion.Major), -90).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-}5").AddProperty(nameof(SwVersion.Build), "+L").ToJsonString());
            Add("*d?;>?k-Fgx!-_35.24%+V{1", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "*d?;>?k-Fgx!-_")
                .AddProperty(nameof(SwVersion.Major), 35).AddProperty(nameof(SwVersion.Minor), 24).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "%").AddProperty(nameof(SwVersion.Build), "+V{1").ToJsonString());
            Add("-oS\"fU-=$EGfR-$zY11.60.49.22g>,+h", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-oS\"fU-=$EGfR-$zY")
                .AddProperty(nameof(SwVersion.Major), 11).AddProperty(nameof(SwVersion.Minor), 60).AddProperty(nameof(SwVersion.Patch), 49).AddProperty(nameof(SwVersion.Revision), 22)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "g>,").AddProperty(nameof(SwVersion.Build), "+h").ToJsonString());
            Add("dJ*?B\\p-Je:-g;-91.80.61-Txv+|", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "dJ*?B\\p-Je:-g;")
                .AddProperty(nameof(SwVersion.Major), -91).AddProperty(nameof(SwVersion.Minor), 80).AddProperty(nameof(SwVersion.Patch), 61).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-Txv").AddProperty(nameof(SwVersion.Build), "+|").ToJsonString());
            Add("VqPORM-CXa-Fu>H30.82-Wpl2d+@3xb", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "VqPORM-CXa-Fu>H")
                .AddProperty(nameof(SwVersion.Major), 30).AddProperty(nameof(SwVersion.Minor), 82).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-Wpl2d").AddProperty(nameof(SwVersion.Build), "+@3xb").ToJsonString());
            Add("b(H$ou TlO-^;bw`D;-20^i@0+D", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "b(H$ou TlO-^;bw`D;")
                .AddProperty(nameof(SwVersion.Major), -20).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "^i@0").AddProperty(nameof(SwVersion.Build), "+D").ToJsonString());
            Add("-N?ggy-dGdpY-i56-$\"EkA7", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-N?ggy-dGdpY-i")
                .AddProperty(nameof(SwVersion.Major), 56).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-$\"EkA7").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("yY*(@/-e][G}Gs-Vo]`\\X86.96b", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "yY*(@/-e][G}Gs-Vo]`\\X")
                .AddProperty(nameof(SwVersion.Major), 86).AddProperty(nameof(SwVersion.Minor), 96).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "b").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("M*ePYtlm%[#:SUY`91", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "M*ePYtlm%[#:SUY`")
                .AddProperty(nameof(SwVersion.Major), 91).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-#}i#R-ao-85.22.83.62Wx/sr", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-#}i#R-ao")
                .AddProperty(nameof(SwVersion.Major), -85).AddProperty(nameof(SwVersion.Minor), 22).AddProperty(nameof(SwVersion.Patch), 83).AddProperty(nameof(SwVersion.Revision), 62)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "Wx/sr").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("<%-i)-q))R:a-13-sl%j", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "<%-i)-q))R:a")
                .AddProperty(nameof(SwVersion.Major), -13).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-sl%j").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("or>pE=~}C%i73.89u+/\"ZO#", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "or>pE=~}C%i")
                .AddProperty(nameof(SwVersion.Major), 73).AddProperty(nameof(SwVersion.Minor), 89).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "u").AddProperty(nameof(SwVersion.Build), "+/\"ZO#").ToJsonString());
            Add("'xDb vS-FH<j'p-%#gv-99.496a+u>/cY:", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "'xDb vS-FH<j'p-%#gv")
                .AddProperty(nameof(SwVersion.Major), -99).AddProperty(nameof(SwVersion.Minor), 49).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "6a").AddProperty(nameof(SwVersion.Build), "+u>/cY:").ToJsonString());
            Add("L'X%R' ~|/U-58.1)jZEL", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "L'X%R' ~|/U")
                .AddProperty(nameof(SwVersion.Major), -58).AddProperty(nameof(SwVersion.Minor), 1).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), ")jZEL").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("?,-;ot%ty-$27.83.44 .", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "?,-;ot%ty-$")
                .AddProperty(nameof(SwVersion.Major), 27).AddProperty(nameof(SwVersion.Minor), 83).AddProperty(nameof(SwVersion.Patch), 44).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), " .").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("Dps-?XOcz-&^z>_>>-69+-iz", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "Dps-?XOcz-&^z>_>>")
                .AddProperty(nameof(SwVersion.Major), -69).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddProperty(nameof(SwVersion.Build), "+-iz").ToJsonString());
            Add(" QzFqeaB-98-`Mvg;", new JsonObject().AddProperty(nameof(SwVersion.Prefix), " QzFqeaB")
                .AddProperty(nameof(SwVersion.Major), -98).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-`Mvg;").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-UTTMN-'Sb-7.47.59-+EC ", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-UTTMN-'Sb")
                .AddProperty(nameof(SwVersion.Major), -7).AddProperty(nameof(SwVersion.Minor), 47).AddProperty(nameof(SwVersion.Patch), 59).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddProperty(nameof(SwVersion.Build), "+EC ").ToJsonString());
            Add("@eN;G&J,[rZ-55.9.57 >", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "@eN;G&J,[rZ")
                .AddProperty(nameof(SwVersion.Major), -55).AddProperty(nameof(SwVersion.Minor), 9).AddProperty(nameof(SwVersion.Patch), 57).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), " >").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-XtT(-g76.58.25.28oG1+&<j", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-XtT(-g")
                .AddProperty(nameof(SwVersion.Major), 76).AddProperty(nameof(SwVersion.Minor), 58).AddProperty(nameof(SwVersion.Patch), 25).AddProperty(nameof(SwVersion.Revision), 28)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "oG1").AddProperty(nameof(SwVersion.Build), "+&<j").ToJsonString());
            Add("-&a,ejE-mK-30.19.48;e", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-&a,ejE-mK")
                .AddProperty(nameof(SwVersion.Major), -30).AddProperty(nameof(SwVersion.Minor), 19).AddProperty(nameof(SwVersion.Patch), 48).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), ";e").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-*#/uy{-WDPj \"-2;%Dk+yz!", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-*#/uy{-WDPj \"")
                .AddProperty(nameof(SwVersion.Major), -2).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), ";%Dk").AddProperty(nameof(SwVersion.Build), "+yz!").ToJsonString());
            Add("kV(U-|C;X-o?xM(Lp64.98U+V>z}", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "kV(U-|C;X-o?xM(Lp")
                .AddProperty(nameof(SwVersion.Major), 64).AddProperty(nameof(SwVersion.Minor), 98).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "U").AddProperty(nameof(SwVersion.Build), "+V>z}").ToJsonString());
            Add("-wK$_h=JzOLln(zvg*55.33.41.57.42-7U[", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-wK$_h=JzOLln(zvg*")
                .AddProperty(nameof(SwVersion.Major), 55).AddProperty(nameof(SwVersion.Minor), 33).AddProperty(nameof(SwVersion.Patch), 41).AddProperty(nameof(SwVersion.Revision), 57)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical), 42).AddProperty(nameof(SwVersion.PreRelease), "-7U[").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("',_?Z(hNq#r-_HcK_t-81.94u", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "',_?Z(hNq#r-_HcK_t")
                .AddProperty(nameof(SwVersion.Major), -81).AddProperty(nameof(SwVersion.Minor), 94).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "u").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-<re[Id> y,->g{XM78.71.46-+IL|pU/", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-<re[Id> y,->g{XM")
                .AddProperty(nameof(SwVersion.Major), 78).AddProperty(nameof(SwVersion.Minor), 71).AddProperty(nameof(SwVersion.Patch), 46).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddProperty(nameof(SwVersion.Build), "+IL|pU/").ToJsonString());
            Add("Dk~-JFy?:u:h|-68-$JkS", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "Dk~-JFy?:u:h|")
                .AddProperty(nameof(SwVersion.Major), -68).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-$JkS").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("]U!B-MYx%pR-|;i59.19.10eZ", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "]U!B-MYx%pR-|;i")
                .AddProperty(nameof(SwVersion.Major), 59).AddProperty(nameof(SwVersion.Minor), 19).AddProperty(nameof(SwVersion.Patch), 10).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "eZ").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-vqh[-jREa$h*-l-6.50.34KKs\\,V+h", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-vqh[-jREa$h*-l")
                .AddProperty(nameof(SwVersion.Major), -6).AddProperty(nameof(SwVersion.Minor), 50).AddProperty(nameof(SwVersion.Patch), 34).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "KKs\\,V").AddProperty(nameof(SwVersion.Build), "+h").ToJsonString());
            Add("}hg(O^C-@#xUG_21.44.994 ", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "}hg(O^C-@#xUG_")
                .AddProperty(nameof(SwVersion.Major), 21).AddProperty(nameof(SwVersion.Minor), 44).AddProperty(nameof(SwVersion.Patch), 99).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "4 ").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("\";Gm;^- a,=68-3", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "\";Gm;^- a,=")
                .AddProperty(nameof(SwVersion.Major), 68).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-3").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("?Si|R(To-xqgK\\_s-96.86Kn+v2\\p9Q`", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "?Si|R(To-xqgK\\_s")
                .AddProperty(nameof(SwVersion.Major), -96).AddProperty(nameof(SwVersion.Minor), 86).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "Kn").AddProperty(nameof(SwVersion.Build), "+v2\\p9Q`").ToJsonString());
            Add("^HV-J}V=MHupQu-15-ttc+!q", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "^HV-J}V=MHupQu")
                .AddProperty(nameof(SwVersion.Major), -15).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-ttc").AddProperty(nameof(SwVersion.Build), "+!q").ToJsonString());
            Add("-@uJBF-sQ-60.42-f[UvCh+`lis{", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-@uJBF-sQ")
                .AddProperty(nameof(SwVersion.Major), -60).AddProperty(nameof(SwVersion.Minor), 42).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-f[UvCh").AddProperty(nameof(SwVersion.Build), "+`lis{").ToJsonString());
            Add("-?[GI-S-e!w[-45.90.56*P", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-?[GI-S-e!w[")
                .AddProperty(nameof(SwVersion.Major), -45).AddProperty(nameof(SwVersion.Minor), 90).AddProperty(nameof(SwVersion.Patch), 56).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "*P").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-Mh-#/zxV;$_69+%%~Qs", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-Mh-#/zxV;$_")
                .AddProperty(nameof(SwVersion.Major), 69).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddProperty(nameof(SwVersion.Build), "+%%~Qs").ToJsonString());
            Add("YJmH/{e(LOp|b~-|J32G/i#+;TP*.", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "YJmH/{e(LOp|b~-|J")
                .AddProperty(nameof(SwVersion.Major), 32).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "G/i#").AddProperty(nameof(SwVersion.Build), "+;TP*.").ToJsonString());
            Add(">>h-u-30-#{", new JsonObject().AddProperty(nameof(SwVersion.Prefix), ">>h-u")
                .AddProperty(nameof(SwVersion.Major), -30).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-#{").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("[p-USz?-<K92.88.34-+p1Y?muO", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "[p-USz?-<K")
                .AddProperty(nameof(SwVersion.Major), 92).AddProperty(nameof(SwVersion.Minor), 88).AddProperty(nameof(SwVersion.Patch), 34).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddProperty(nameof(SwVersion.Build), "+p1Y?muO").ToJsonString());
            Add(">E)zQihlHX[hv]}(-88.47j+", new JsonObject().AddProperty(nameof(SwVersion.Prefix), ">E)zQihlHX[hv]}(")
                .AddProperty(nameof(SwVersion.Major), -88).AddProperty(nameof(SwVersion.Minor), 47).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "j").AddProperty(nameof(SwVersion.Build), "+").ToJsonString());
            Add("]Jv%uvIZmt<-] b$70.43.59.0-XNT!.+-\"a8", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "]Jv%uvIZmt<-] b$")
                .AddProperty(nameof(SwVersion.Major), 70).AddProperty(nameof(SwVersion.Minor), 43).AddProperty(nameof(SwVersion.Patch), 59).AddProperty(nameof(SwVersion.Revision), 0)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-XNT!.").AddProperty(nameof(SwVersion.Build), "+-\"a8").ToJsonString());
            Add("\"wHh-F$Qb_XtVgy-96.32.61z+>", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "\"wHh-F$Qb_XtVgy")
                .AddProperty(nameof(SwVersion.Major), -96).AddProperty(nameof(SwVersion.Minor), 32).AddProperty(nameof(SwVersion.Patch), 61).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "z").AddProperty(nameof(SwVersion.Build), "+>").ToJsonString());
            Add("Pm`WUvu-J#*#_,2+", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "Pm`WUvu-J#*#_,")
                .AddProperty(nameof(SwVersion.Major), 2).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddProperty(nameof(SwVersion.Build), "+").ToJsonString());
            Add("-S[[-d(kiko-82-U^{v+", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-S[[-d(kiko")
                .AddProperty(nameof(SwVersion.Major), -82).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-U^{v").AddProperty(nameof(SwVersion.Build), "+").ToJsonString());
            Add("cfmHC#y|yQ??nr-81.67.16.68-{_wk&Q", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "cfmHC#y|yQ??nr")
                .AddProperty(nameof(SwVersion.Major), -81).AddProperty(nameof(SwVersion.Minor), 67).AddProperty(nameof(SwVersion.Patch), 16).AddProperty(nameof(SwVersion.Revision), 68)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-{_wk&Q").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("{jGS-m28", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "{jGS-m")
                .AddProperty(nameof(SwVersion.Major), 28).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-{TK_%}[-66IwC$KT", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-{TK_%}[")
                .AddProperty(nameof(SwVersion.Major), -66).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "IwC$KT").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("UA-x-o57.41.92.52>g5", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "UA-x-o")
                .AddProperty(nameof(SwVersion.Major), 57).AddProperty(nameof(SwVersion.Minor), 41).AddProperty(nameof(SwVersion.Patch), 92).AddProperty(nameof(SwVersion.Revision), 52)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), ">g5").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-yr{rZFP-*-2.28-", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-yr{rZFP-*")
                .AddProperty(nameof(SwVersion.Major), -2).AddProperty(nameof(SwVersion.Minor), 28).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-?/k-V%db>yC-o>jk-89.33#_/)f@P+0 co2", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-?/k-V%db>yC-o>jk")
                .AddProperty(nameof(SwVersion.Major), -89).AddProperty(nameof(SwVersion.Minor), 33).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "#_/)f@P").AddProperty(nameof(SwVersion.Build), "+0 co2").ToJsonString());
            Add("F,u}rk-lkPK46R", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "F,u}rk-lkPK")
                .AddProperty(nameof(SwVersion.Major), 46).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "R").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("r!j_YEX]v}o-[S~19.71.969:", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "r!j_YEX]v}o-[S~")
                .AddProperty(nameof(SwVersion.Major), 19).AddProperty(nameof(SwVersion.Minor), 71).AddProperty(nameof(SwVersion.Patch), 96).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "9:").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-qd#,Sdj50-rF", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-qd#,Sdj")
                .AddProperty(nameof(SwVersion.Major), 50).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-rF").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-[HLsa-82.90.34J+ED%", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-[HLsa")
                .AddProperty(nameof(SwVersion.Major), -82).AddProperty(nameof(SwVersion.Minor), 90).AddProperty(nameof(SwVersion.Patch), 34).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "J").AddProperty(nameof(SwVersion.Build), "+ED%").ToJsonString());
            Add("-BcmyVJ-} LYTR69-<%PG2r", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-BcmyVJ-} LYTR")
                .AddProperty(nameof(SwVersion.Major), 69).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-<%PG2r").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("tn_GN-KVqJ-knRCEbe-70.75-Vf+g*(\\", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "tn_GN-KVqJ-knRCEbe")
                .AddProperty(nameof(SwVersion.Major), -70).AddProperty(nameof(SwVersion.Minor), 75).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "-Vf").AddProperty(nameof(SwVersion.Build), "+g*(\\").ToJsonString());
            Add("-#-GjI-p-86+o2", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-#-GjI-p")
                .AddProperty(nameof(SwVersion.Major), -86).AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddNullProperty(nameof(SwVersion.PreRelease)).AddProperty(nameof(SwVersion.Build), "+o2").ToJsonString());
            Add("Q^ls->E?MaNr&O=T71.62.68OR", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "Q^ls->E?MaNr&O=T")
                .AddProperty(nameof(SwVersion.Major), 71).AddProperty(nameof(SwVersion.Minor), 62).AddProperty(nameof(SwVersion.Patch), 68).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "OR").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("w]GMC>WO-uQ,53.69y+1I D<D", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "w]GMC>WO-uQ,")
                .AddProperty(nameof(SwVersion.Major), 53).AddProperty(nameof(SwVersion.Minor), 69).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "y").AddProperty(nameof(SwVersion.Build), "+1I D<D").ToJsonString());
            Add("GnKN-eY=< xdQZ-90.93.17}\"=Tcg=+sQEf2I;", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "GnKN-eY=< xdQZ")
                .AddProperty(nameof(SwVersion.Major), -90).AddProperty(nameof(SwVersion.Minor), 93).AddProperty(nameof(SwVersion.Patch), 17).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "}\"=Tcg=").AddProperty(nameof(SwVersion.Build), "+sQEf2I;").ToJsonString());
            Add("';KF?<-QTdKZ?-RTb$f:-74.45.41hl", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "';KF?<-QTdKZ?-RTb$f:")
                .AddProperty(nameof(SwVersion.Major), -74).AddProperty(nameof(SwVersion.Minor), 45).AddProperty(nameof(SwVersion.Patch), 41).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "hl").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-PWb,dvk-;xO-c-56.53$M0RH", new JsonObject().AddProperty(nameof(SwVersion.Prefix), "-PWb,dvk-;xO-c")
                .AddProperty(nameof(SwVersion.Major), -56).AddProperty(nameof(SwVersion.Minor), 53).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)).AddProperty(nameof(SwVersion.PreRelease), "$M0RH").AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string?)" />
    /// </summary>
    [Theory]
    [ClassData(typeof(ParsingConstructorTestData))]
    public void ParsingConstructorTest(string? versionString, string jsonData)
    {
        SwVersion target = new(versionString);
        JsonObject obj = (JsonObject)JsonNode.Parse(jsonData)!;
        Assert.Equal(obj[nameof(SwVersion.Major)]!.GetValue<int>(), target.Major);
    }

    [Fact]
    public void ComponentConstructor0Test()
    {
        SwVersion result = new();
        Assert.Null(result.Prefix);
        Assert.Equal(0, result.Major);
        Assert.Null(result.Minor);
        Assert.Null(result.Patch);
        Assert.Null(result.Revision);
        Assert.Null(result.AdditionalNumerical);
        Assert.Null(result.PreRelease);
        Assert.Null(result.Build);
    }
    
    private static readonly char[] _letterChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    private static readonly char[] _numericChars =  new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private static readonly char[] _alNumChars = _numericChars.Concat(_letterChars).ToArray();
    private static readonly char[] _allCharsSource = Enumerable.Range(33, 95).Select(i => (char)i).Concat(_alNumChars).Concat(_alNumChars).Concat(_alNumChars).Concat(_alNumChars).ToArray();
    private static readonly char[] _nonNumericChars = Enumerable.Range(33, 95).Select(i => (char)i).Where(c => !char.IsNumber(c)).ToArray();
    private static readonly char[] _nonNumericCharsSource = _allCharsSource.Where(c => !char.IsNumber(c)).ToArray();
    private static readonly char[] _buildSeparatorChars = new char[] { '+', '.', '-' };
    private static readonly char[] _preReleaseSeparatorChars = new char[] { '.', '-' };
    private static readonly char[] _validBuildChars = Enumerable.Range(33, 95).Select(i => (char)i).Where(c => !_buildSeparatorChars.Contains(c)).ToArray();
    private static readonly char[] _buildCharsSource = _allCharsSource.Where(c => !_buildSeparatorChars.Contains(c)).ToArray();
    private static readonly char[] _validPrefixChars = _nonNumericChars.Where(c => !_buildSeparatorChars.Contains(c)).ToArray();
    private static readonly char[] _prefixCharsSource = _nonNumericCharsSource.Where(c => !_buildSeparatorChars.Contains(c)).ToArray();
    
    private static readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;
}