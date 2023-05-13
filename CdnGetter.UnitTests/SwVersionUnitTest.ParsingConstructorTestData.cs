using System.Text.Json.Nodes;

namespace CdnGetter.UnitTests;

public partial class SwVersionUnitTest
{
    /// <summary>
    /// Generates test data for <see cref="ParsingConstructorTest(string?, string)" />
    /// </summary>
    public class ParsingConstructorTestData : TheoryData<string?, string>
    {
        public ParsingConstructorTestData()
        {
            Add("-86", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -86).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("<S,(-1", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "<S,(")
                .AddProperty(nameof(SwVersion.Major_obs), -1).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("25+2%'", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 25).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "2%'")).ToJsonString());
            Add("F\"R_^?4+BP/96KI", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "F\"R_^?")
                .AddProperty(nameof(SwVersion.Major_obs), 4).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "BP/96KI")).ToJsonString());
            Add("-6679+;^^R3]4", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -6679).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ";^^R3]4")).ToJsonString());
            Add("-;CL-42+0,", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-;CL")
                .AddProperty(nameof(SwVersion.Major_obs), -42).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "0,")).ToJsonString());
            Add("97-2\"N])P1", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 97).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "2\"N])P1"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("D&,NC,@Z-8776-70(VDH<", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "D&,NC,@Z")
                .AddProperty(nameof(SwVersion.Major_obs), -8776).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "70(VDH<"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-19-+*6YP", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -19).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "*6YP")).ToJsonString());
            Add("K2380-DU$))+YMKM)65", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "K")
                .AddProperty(nameof(SwVersion.Major_obs), 2380).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "DU$))"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "YMKM)65")).ToJsonString());
            Add("-804-)=\"I6CH+]", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -804).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ")=\"I6CH"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "]")).ToJsonString());
            Add("H%[-712-9+G_6NF", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "H%[")
                .AddProperty(nameof(SwVersion.Major_obs), -712).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "9"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "G_6NF")).ToJsonString());
            Add("4-E$B$0H.Q;$GF", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 4).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "E$B$0H"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "Q;$GF"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-WGZ^=,?<-513-]&][\".\"/XW@V=", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-WGZ^=,?<")
                .AddProperty(nameof(SwVersion.Major_obs), -513).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "]&][\""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\"/XW@V="))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("52->5FN-2%+ZFYN1R\\-T#", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 52).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ">5FN"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "2%"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "ZFYN1R\\"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "-").AddProperty(nameof(SwVersion.BuildSegment.Value), "T#")).ToJsonString());
            Add("('W543KF(R']G-+&>O=#+==FGJ", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "('W")
                .AddProperty(nameof(SwVersion.Major_obs), 543).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "KF(R']G"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "&>O=#"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "==FGJ")).ToJsonString());
            Add("9-A-A++TO", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 9).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "A"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "A"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "TO")).ToJsonString());
            Add("YQK5336-Z&-9SHXV*+.ZBZ1", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "YQK")
                .AddProperty(nameof(SwVersion.Major_obs), 5336).AddNullProperty(nameof(SwVersion.Minor_obs)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "Z&"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "9SHXV*"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "ZBZ1")).ToJsonString());
            Add("-213.5474", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -213).AddProperty(nameof(SwVersion.Minor_obs), 5474).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("D&#[Y4.1185", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "D&#[Y")
                .AddProperty(nameof(SwVersion.Major_obs), 4).AddProperty(nameof(SwVersion.Minor_obs), 1185).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("688.2+6/E", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 688).AddProperty(nameof(SwVersion.Minor_obs), 2).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "6/E")).ToJsonString());
            Add("\\YBO/[[-824.79+(7B@I1O/", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "\\YBO/[[")
                .AddProperty(nameof(SwVersion.Major_obs), -824).AddProperty(nameof(SwVersion.Minor_obs), 79).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "(7B@I1O/")).ToJsonString());
            Add("17.41+(V", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 17).AddProperty(nameof(SwVersion.Minor_obs), 41).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "(V")).ToJsonString());
            Add("-$O]K39.6228+A$3", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-$O]K")
                .AddProperty(nameof(SwVersion.Major_obs), 39).AddProperty(nameof(SwVersion.Minor_obs), 6228).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "A$3")).ToJsonString());
            Add("-90.276-GF", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -90).AddProperty(nameof(SwVersion.Minor_obs), 276).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "GF"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("E=GF3.167[I", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "E=GF")
                .AddProperty(nameof(SwVersion.Major_obs), 3).AddProperty(nameof(SwVersion.Minor_obs), 167).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "[I"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("1.77U+@*0H*", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 1).AddProperty(nameof(SwVersion.Minor_obs), 77).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "U"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "@*0H*")).ToJsonString());
            Add("GJS\\L1790.6750%!0+", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "GJS\\L")
                .AddProperty(nameof(SwVersion.Major_obs), 1790).AddProperty(nameof(SwVersion.Minor_obs), 6750).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "%!0"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("53.0-+S/9", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 53).AddProperty(nameof(SwVersion.Minor_obs), 0).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "S/9")).ToJsonString());
            Add("%-2347.9-N+PMPQ14>U", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "%")
                .AddProperty(nameof(SwVersion.Major_obs), -2347).AddProperty(nameof(SwVersion.Minor_obs), 9).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "N"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "PMPQ14>U")).ToJsonString());
            Add("-3737.8--1?8J(R", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -3737).AddProperty(nameof(SwVersion.Minor_obs), 8).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "1?8J(R"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("BGCB\"GF4014.141Y\\>*.O$", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "BGCB\"GF")
                .AddProperty(nameof(SwVersion.Major_obs), 4014).AddProperty(nameof(SwVersion.Minor_obs), 141).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "Y\\>*"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "O$"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("825.63--D@SXU=++NM2E<C", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 825).AddProperty(nameof(SwVersion.Minor_obs), 63).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "D@SXU="))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "NM2E<C")).ToJsonString());
            Add("U-992.3744L(W!.<HN+'.HE?ZA", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "U")
                .AddProperty(nameof(SwVersion.Major_obs), -992).AddProperty(nameof(SwVersion.Minor_obs), 3744).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "L(W!"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "<HN"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "'"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "HE?ZA")).ToJsonString());
            Add("-7.12-EH6Z8A]\"-PQ/4]]+YC/.V\"XBN)B=", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -7).AddProperty(nameof(SwVersion.Minor_obs), 12).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "EH6Z8A]\""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "PQ/4]]"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "YC/"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "V\"XBN)B=")).ToJsonString());
            Add("LW-3939.9589-#-43+UJ*,'X3.X=L:VPV", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "LW")
                .AddProperty(nameof(SwVersion.Major_obs), -3939).AddProperty(nameof(SwVersion.Minor_obs), 9589).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "#"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "43"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "UJ*,'X3"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "X=L:VPV")).ToJsonString());
            Add("7471.4.5", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 7471).AddProperty(nameof(SwVersion.Minor_obs), 4).AddProperty(nameof(SwVersion.Patch), 5).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("*'K?P$)E-423.88.9067", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "*'K?P$)E")
                .AddProperty(nameof(SwVersion.Major_obs), -423).AddProperty(nameof(SwVersion.Minor_obs), 88).AddProperty(nameof(SwVersion.Patch), 9067).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-9.5331.2419+*_=", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -9).AddProperty(nameof(SwVersion.Minor_obs), 5331).AddProperty(nameof(SwVersion.Patch), 2419).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "*_=")).ToJsonString());
            Add(":DDVN6.50.580+50OC)>\\", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), ":DDVN")
                .AddProperty(nameof(SwVersion.Major_obs), 6).AddProperty(nameof(SwVersion.Minor_obs), 50).AddProperty(nameof(SwVersion.Patch), 580).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "50OC)>\\")).ToJsonString());
            Add("22.3734.1+N", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 22).AddProperty(nameof(SwVersion.Minor_obs), 3734).AddProperty(nameof(SwVersion.Patch), 1).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "N")).ToJsonString());
            Add("G=944.7.87+#==N#N#F", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "G=")
                .AddProperty(nameof(SwVersion.Major_obs), 944).AddProperty(nameof(SwVersion.Minor_obs), 7).AddProperty(nameof(SwVersion.Patch), 87).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "#==N#N#F")).ToJsonString());
            Add("3682.241.7860-I4?]", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 3682).AddProperty(nameof(SwVersion.Minor_obs), 241).AddProperty(nameof(SwVersion.Patch), 7860).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "I4?]"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("K>(W)]JN5776.9.571II#I]1", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "K>(W)]JN")
                .AddProperty(nameof(SwVersion.Major_obs), 5776).AddProperty(nameof(SwVersion.Minor_obs), 9).AddProperty(nameof(SwVersion.Patch), 571).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "II#I]1"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-3812.3166.9672-+*", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -3812).AddProperty(nameof(SwVersion.Minor_obs), 3166).AddProperty(nameof(SwVersion.Patch), 9672).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "*")).ToJsonString());
            Add("?WBY[N659.5.7446-!FD%[^M+", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "?WBY[N")
                .AddProperty(nameof(SwVersion.Major_obs), 659).AddProperty(nameof(SwVersion.Minor_obs), 5).AddProperty(nameof(SwVersion.Patch), 7446).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "!FD%[^M"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("55.78.3VJ1D)S+;KXL", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 55).AddProperty(nameof(SwVersion.Minor_obs), 78).AddProperty(nameof(SwVersion.Patch), 3).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "VJ1D)S"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ";KXL")).ToJsonString());
            Add("OE!K&9165.556.418=FQ/+5", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "OE!K&")
                .AddProperty(nameof(SwVersion.Major_obs), 9165).AddProperty(nameof(SwVersion.Minor_obs), 556).AddProperty(nameof(SwVersion.Patch), 418).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "=FQ/"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "5")).ToJsonString());
            Add("984.76.99-4.M5RN", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 984).AddProperty(nameof(SwVersion.Minor_obs), 76).AddProperty(nameof(SwVersion.Patch), 99).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "4"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "M5RN"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-UR#%*DV\\9.352.7-\\R4-", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-UR#%*DV\\")
                .AddProperty(nameof(SwVersion.Major_obs), 9).AddProperty(nameof(SwVersion.Minor_obs), 352).AddProperty(nameof(SwVersion.Patch), 7).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\\R4"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("6025.539.25/G\"E[-4+'\\:VE\"/+FBVG:9*D", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 6025).AddProperty(nameof(SwVersion.Minor_obs), 539).AddProperty(nameof(SwVersion.Patch), 25).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "/G\"E["),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "4"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "'\\:VE\"/"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "FBVG:9*D")).ToJsonString());
            Add("$IH(>M(8750.691.1278-8G!_AO]-NNP+'A$S.", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "$IH(>M(")
                .AddProperty(nameof(SwVersion.Major_obs), 8750).AddProperty(nameof(SwVersion.Minor_obs), 691).AddProperty(nameof(SwVersion.Patch), 1278).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "8G!_AO]"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "NNP"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "'A$S"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("-7960.3.0:9Q:]-IX7>^D++5Y166Z", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -7960).AddProperty(nameof(SwVersion.Minor_obs), 3).AddProperty(nameof(SwVersion.Patch), 0).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ":9Q:]"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "IX7>^D"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "5Y166Z")).ToJsonString());
            Add(")>@BP86.7.7499IPM=).]$++M27!(", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), ")>@BP")
                .AddProperty(nameof(SwVersion.Major_obs), 86).AddProperty(nameof(SwVersion.Minor_obs), 7).AddProperty(nameof(SwVersion.Patch), 7499).AddNullProperty(nameof(SwVersion.Revision))
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "IPM=)"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "]$"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "M27!(")).ToJsonString());
            Add("129.766.0.68", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 129).AddProperty(nameof(SwVersion.Minor_obs), 766).AddProperty(nameof(SwVersion.Patch), 0).AddProperty(nameof(SwVersion.Revision), 68)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("^LW_847.9.1020.526", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "^LW_")
                .AddProperty(nameof(SwVersion.Major_obs), 847).AddProperty(nameof(SwVersion.Minor_obs), 9).AddProperty(nameof(SwVersion.Patch), 1020).AddProperty(nameof(SwVersion.Revision), 526)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-6414.4.82.246+:D31%", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -6414).AddProperty(nameof(SwVersion.Minor_obs), 4).AddProperty(nameof(SwVersion.Patch), 82).AddProperty(nameof(SwVersion.Revision), 246)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ":D31%")).ToJsonString());
            Add("@Y[K:E^79.7610.644.362+%[IYB", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "@Y[K:E^")
                .AddProperty(nameof(SwVersion.Major_obs), 79).AddProperty(nameof(SwVersion.Minor_obs), 7610).AddProperty(nameof(SwVersion.Patch), 644).AddProperty(nameof(SwVersion.Revision), 362)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "%[IYB")).ToJsonString());
            Add("5147.725.29.3530+V'", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 5147).AddProperty(nameof(SwVersion.Minor_obs), 725).AddProperty(nameof(SwVersion.Patch), 29).AddProperty(nameof(SwVersion.Revision), 3530)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "V'")).ToJsonString());
            Add("&GB^MY_52.68.466.8+2T<:", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "&GB^MY_")
                .AddProperty(nameof(SwVersion.Major_obs), 52).AddProperty(nameof(SwVersion.Minor_obs), 68).AddProperty(nameof(SwVersion.Patch), 466).AddProperty(nameof(SwVersion.Revision), 8)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "2T<:")).ToJsonString());
            Add("3263.3.1264.1069<", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 3263).AddProperty(nameof(SwVersion.Minor_obs), 3).AddProperty(nameof(SwVersion.Patch), 1264).AddProperty(nameof(SwVersion.Revision), 1069)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "<"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("_CT&R[[-52.826.3.174-()T", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "_CT&R[[")
                .AddProperty(nameof(SwVersion.Major_obs), -52).AddProperty(nameof(SwVersion.Minor_obs), 826).AddProperty(nameof(SwVersion.Patch), 3).AddProperty(nameof(SwVersion.Revision), 174)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "()T"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-3.4.1192.4-2@\"D+,#,C", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -3).AddProperty(nameof(SwVersion.Minor_obs), 4).AddProperty(nameof(SwVersion.Patch), 1192).AddProperty(nameof(SwVersion.Revision), 4)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "2@\"D"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ",#,C")).ToJsonString());
            Add(",I/'X759.16.1451.5-<HAN4+DZJ<AC", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), ",I/'X")
                .AddProperty(nameof(SwVersion.Major_obs), 759).AddProperty(nameof(SwVersion.Minor_obs), 16).AddProperty(nameof(SwVersion.Patch), 1451).AddProperty(nameof(SwVersion.Revision), 5)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "<HAN4"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "DZJ<AC")).ToJsonString());
            Add("543.1564.4.534-K+1;\\A4", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 543).AddProperty(nameof(SwVersion.Minor_obs), 1564).AddProperty(nameof(SwVersion.Patch), 4).AddProperty(nameof(SwVersion.Revision), 534)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "K"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "1;\\A4")).ToJsonString());
            Add("^7812.365.0.7607-U+\"", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "^")
                .AddProperty(nameof(SwVersion.Major_obs), 7812).AddProperty(nameof(SwVersion.Minor_obs), 365).AddProperty(nameof(SwVersion.Patch), 0).AddProperty(nameof(SwVersion.Revision), 7607)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "U"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "\"")).ToJsonString());
            Add("-3718.538.1.558-TW-X", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -3718).AddProperty(nameof(SwVersion.Minor_obs), 538).AddProperty(nameof(SwVersion.Patch), 1).AddProperty(nameof(SwVersion.Revision), 558)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "TW"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "X"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add(",]?&0.5.9.1)=^:H79-FODMB", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), ",]?&")
                .AddProperty(nameof(SwVersion.Major_obs), 0).AddProperty(nameof(SwVersion.Minor_obs), 5).AddProperty(nameof(SwVersion.Patch), 9).AddProperty(nameof(SwVersion.Revision), 1)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ")=^:H79"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "FODMB"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-53.88.773.35-A)0.H+*%+", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -53).AddProperty(nameof(SwVersion.Minor_obs), 88).AddProperty(nameof(SwVersion.Patch), 773).AddProperty(nameof(SwVersion.Revision), 35)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "A)0"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "H"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "*%"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("DP-9777.8757.8102.6779!%^H[;/-S8%,]3++QQH#U", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "DP")
                .AddProperty(nameof(SwVersion.Major_obs), -9777).AddProperty(nameof(SwVersion.Minor_obs), 8757).AddProperty(nameof(SwVersion.Patch), 8102).AddProperty(nameof(SwVersion.Revision), 6779)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "!%^H[;/"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "S8%,]3"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "QQH#U")).ToJsonString());
            Add("-5762.4.2.4\"$4/Q-\"S7%!+_5U>Y=[+0\\6", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -5762).AddProperty(nameof(SwVersion.Minor_obs), 4).AddProperty(nameof(SwVersion.Patch), 2).AddProperty(nameof(SwVersion.Revision), 4)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\"$4/Q"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\"S7%!"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "_5U>Y=["),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "0\\6")).ToJsonString());
            Add("B_)AG(1.3753.8.4216-.!'JC^\\[+/^+A?ZMS", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "B_)AG(")
                .AddProperty(nameof(SwVersion.Major_obs), 1).AddProperty(nameof(SwVersion.Minor_obs), 3753).AddProperty(nameof(SwVersion.Patch), 8).AddProperty(nameof(SwVersion.Revision), 4216)
                .AddNullProperty(nameof(SwVersion.AdditionalNumerical_obs))
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "!'JC^\\["))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "/^"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "A?ZMS")).ToJsonString());
            Add("-9.91.140.939.89", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -9).AddProperty(nameof(SwVersion.Minor_obs), 91).AddProperty(nameof(SwVersion.Patch), 140).AddProperty(nameof(SwVersion.Revision), 939)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 89)
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("PO=*V*?T8.44.889.411.847", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "PO=*V*?T")
                .AddProperty(nameof(SwVersion.Major_obs), 8).AddProperty(nameof(SwVersion.Minor_obs), 44).AddProperty(nameof(SwVersion.Patch), 889).AddProperty(nameof(SwVersion.Revision), 411)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 847)
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-577.82.91.93.5861+\"#SA", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -577).AddProperty(nameof(SwVersion.Minor_obs), 82).AddProperty(nameof(SwVersion.Patch), 91).AddProperty(nameof(SwVersion.Revision), 93)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 5861)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "\"#SA")).ToJsonString());
            Add("-=-9.8351.677.9.234+", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-=")
                .AddProperty(nameof(SwVersion.Major_obs), -9).AddProperty(nameof(SwVersion.Minor_obs), 8351).AddProperty(nameof(SwVersion.Patch), 677).AddProperty(nameof(SwVersion.Revision), 9)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 234)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("1.544.986.71.3+<K", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 1).AddProperty(nameof(SwVersion.Minor_obs), 544).AddProperty(nameof(SwVersion.Patch), 986).AddProperty(nameof(SwVersion.Revision), 71)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 3)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "<K")).ToJsonString());
            Add("Y(?HL:LZ2.8.87.3692.1+P39@", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "Y(?HL:LZ")
                .AddProperty(nameof(SwVersion.Major_obs), 2).AddProperty(nameof(SwVersion.Minor_obs), 8).AddProperty(nameof(SwVersion.Patch), 87).AddProperty(nameof(SwVersion.Revision), 3692)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 1)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "P39@")).ToJsonString());
            Add("-8405.1.9737.8.77-!", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -8405).AddProperty(nameof(SwVersion.Minor_obs), 1).AddProperty(nameof(SwVersion.Patch), 9737).AddProperty(nameof(SwVersion.Revision), 8)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 77)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "!"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("=RO-1.454.685.321.1545C]R(VZ40", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "=RO")
                .AddProperty(nameof(SwVersion.Major_obs), -1).AddProperty(nameof(SwVersion.Minor_obs), 454).AddProperty(nameof(SwVersion.Patch), 685).AddProperty(nameof(SwVersion.Revision), 321)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 1545)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "C]R(VZ40"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-81.3.67.6.14-+N)@", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -81).AddProperty(nameof(SwVersion.Minor_obs), 3).AddProperty(nameof(SwVersion.Patch), 67).AddProperty(nameof(SwVersion.Revision), 6)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 14)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "N)@")).ToJsonString());
            Add("#S\\T@;ZH71.7656.751.9.3Q(=+!=XZ!", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "#S\\T@;ZH")
                .AddProperty(nameof(SwVersion.Major_obs), 71).AddProperty(nameof(SwVersion.Minor_obs), 7656).AddProperty(nameof(SwVersion.Patch), 751).AddProperty(nameof(SwVersion.Revision), 9)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 3)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "Q(="))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "!=XZ!")).ToJsonString());
            Add("-579.295.8257.7570.8-+*!", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -579).AddProperty(nameof(SwVersion.Minor_obs), 295).AddProperty(nameof(SwVersion.Patch), 8257).AddProperty(nameof(SwVersion.Revision), 7570)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 8)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "*!")).ToJsonString());
            Add("R&FZ\\-8.794.1.2128.11-311+!/&", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "R&FZ\\")
                .AddProperty(nameof(SwVersion.Major_obs), -8).AddProperty(nameof(SwVersion.Minor_obs), 794).AddProperty(nameof(SwVersion.Patch), 1).AddProperty(nameof(SwVersion.Revision), 2128)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 11)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "311"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "!/&")).ToJsonString());
            Add("754.14.4.64.359-'UN*R,-Z,)LG", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 754).AddProperty(nameof(SwVersion.Minor_obs), 14).AddProperty(nameof(SwVersion.Patch), 4).AddProperty(nameof(SwVersion.Revision), 64)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 359)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "'UN*R,"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "Z,)LG"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-_XSD421.565.50.4.89-C@-", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-_XSD")
                .AddProperty(nameof(SwVersion.Major_obs), 421).AddProperty(nameof(SwVersion.Minor_obs), 565).AddProperty(nameof(SwVersion.Patch), 50).AddProperty(nameof(SwVersion.Revision), 4)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 89)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "C@"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-70.96.135.4.11-=C!DL$-X?J^;(]'+);+!", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -70).AddProperty(nameof(SwVersion.Minor_obs), 96).AddProperty(nameof(SwVersion.Patch), 135).AddProperty(nameof(SwVersion.Revision), 4)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 11)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "=C!DL$"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "X?J^;(]'"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ");"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "!")).ToJsonString());
            Add("AK=-1.55.87.1924.7505-3ZV*\\<F-KOP917F1+;.6%", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "AK=")
                .AddProperty(nameof(SwVersion.Major_obs), -1).AddProperty(nameof(SwVersion.Minor_obs), 55).AddProperty(nameof(SwVersion.Patch), 87).AddProperty(nameof(SwVersion.Revision), 1924)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 7505)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "3ZV*\\<F"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "KOP917F1"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ";"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "6%")).ToJsonString());
            Add("-1.3648.7337.540.5--8$GU5B:+P%PKW4R?+#M", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -1).AddProperty(nameof(SwVersion.Minor_obs), 3648).AddProperty(nameof(SwVersion.Patch), 7337).AddProperty(nameof(SwVersion.Revision), 540)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 5)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "8$GU5B:"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "P%PKW4R?"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "#M")).ToJsonString());
            Add("J&DWF2.8595.0.753.7!-(1??+_&(A<.ES:>DD,", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "J&DWF")
                .AddProperty(nameof(SwVersion.Major_obs), 2).AddProperty(nameof(SwVersion.Minor_obs), 8595).AddProperty(nameof(SwVersion.Patch), 0).AddProperty(nameof(SwVersion.Revision), 753)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 7)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "!"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "(1??"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "_&(A<"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "ES:>DD,")).ToJsonString());
            Add("2.1.808.2.405.42", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 2).AddProperty(nameof(SwVersion.Minor_obs), 1).AddProperty(nameof(SwVersion.Patch), 808).AddProperty(nameof(SwVersion.Revision), 2)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 405, 42)
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add(";/FT2.9265.0.86.1.8338", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), ";/FT")
                .AddProperty(nameof(SwVersion.Major_obs), 2).AddProperty(nameof(SwVersion.Minor_obs), 9265).AddProperty(nameof(SwVersion.Patch), 0).AddProperty(nameof(SwVersion.Revision), 86)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 1, 8338)
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("9.115.0.8360.8414.99+8B", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 9).AddProperty(nameof(SwVersion.Minor_obs), 115).AddProperty(nameof(SwVersion.Patch), 0).AddProperty(nameof(SwVersion.Revision), 8360)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 8414, 99)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "8B")).ToJsonString());
            Add("%S]1391.7320.48.7.2801.526+ICH@VF\\", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "%S]")
                .AddProperty(nameof(SwVersion.Major_obs), 1391).AddProperty(nameof(SwVersion.Minor_obs), 7320).AddProperty(nameof(SwVersion.Patch), 48).AddProperty(nameof(SwVersion.Revision), 7)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 2801, 526)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "ICH@VF\\")).ToJsonString());
            Add("133.4.424.360.87.399+91G", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 133).AddProperty(nameof(SwVersion.Minor_obs), 4).AddProperty(nameof(SwVersion.Patch), 424).AddProperty(nameof(SwVersion.Revision), 360)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 87, 399)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "91G")).ToJsonString());
            Add(">&J,WY8564.1783.4124.7.56.9634+?>O@F;CX", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), ">&J,WY")
                .AddProperty(nameof(SwVersion.Major_obs), 8564).AddProperty(nameof(SwVersion.Minor_obs), 1783).AddProperty(nameof(SwVersion.Patch), 4124).AddProperty(nameof(SwVersion.Revision), 7)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 56, 9634)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "?>O@F;CX")).ToJsonString());
            Add("8.1.2471.716.4.99AE$I=\\<5", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 8).AddProperty(nameof(SwVersion.Minor_obs), 1).AddProperty(nameof(SwVersion.Patch), 2471).AddProperty(nameof(SwVersion.Revision), 716)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 4, 99)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "AE$I=\\<5"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("A$&;Y?C7.7121.9.687.26.26-D", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "A$&;Y?C")
                .AddProperty(nameof(SwVersion.Major_obs), 7).AddProperty(nameof(SwVersion.Minor_obs), 7121).AddProperty(nameof(SwVersion.Patch), 9).AddProperty(nameof(SwVersion.Revision), 687)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 26, 26)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "D"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("21.4.69.16.964.6K6QL$]'5+^65", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 21).AddProperty(nameof(SwVersion.Minor_obs), 4).AddProperty(nameof(SwVersion.Patch), 69).AddProperty(nameof(SwVersion.Revision), 16)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 964, 6)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "K6QL$]'5"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "^65")).ToJsonString());
            Add(",U(23.75.2.251.20.7512-NBXE=5+\"C*5?V", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), ",U(")
                .AddProperty(nameof(SwVersion.Major_obs), 23).AddProperty(nameof(SwVersion.Minor_obs), 75).AddProperty(nameof(SwVersion.Patch), 2).AddProperty(nameof(SwVersion.Revision), 251)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 20, 7512)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "NBXE=5"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "\"C*5?V")).ToJsonString());
            Add("8.1.79.164.711.21R(&]+", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 8).AddProperty(nameof(SwVersion.Minor_obs), 1).AddProperty(nameof(SwVersion.Patch), 79).AddProperty(nameof(SwVersion.Revision), 164)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 711, 21)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "R(&]"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("-C?*PV>19.16.6495.3380.3.86-&C;8D&+$Z", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-C?*PV>")
                .AddProperty(nameof(SwVersion.Major_obs), 19).AddProperty(nameof(SwVersion.Minor_obs), 16).AddProperty(nameof(SwVersion.Patch), 6495).AddProperty(nameof(SwVersion.Revision), 3380)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 3, 86)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "&C;8D&"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "$Z")).ToJsonString());
            Add("42.101.2353.91.1784.33-SD<]Y#Q-1", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 42).AddProperty(nameof(SwVersion.Minor_obs), 101).AddProperty(nameof(SwVersion.Patch), 2353).AddProperty(nameof(SwVersion.Revision), 91)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 1784, 33)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "SD<]Y#Q"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "1"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("S-94.8.4699.119.83.45-545=7P-_O2$DD4<", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "S")
                .AddProperty(nameof(SwVersion.Major_obs), -94).AddProperty(nameof(SwVersion.Minor_obs), 8).AddProperty(nameof(SwVersion.Patch), 4699).AddProperty(nameof(SwVersion.Revision), 119)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 83, 45)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "545=7P"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "_O2$DD4<"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("575.592.99.7762.8510.240-.XA+DU_)^.RV", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 575).AddProperty(nameof(SwVersion.Minor_obs), 592).AddProperty(nameof(SwVersion.Patch), 99).AddProperty(nameof(SwVersion.Revision), 7762)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 8510, 240)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "XA"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "DU_)^"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "RV")).ToJsonString());
            Add("XS-71.9.4252.3.2016.0-*?,'6-\\9+R+TMW1!NZ", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "XS")
                .AddProperty(nameof(SwVersion.Major_obs), -71).AddProperty(nameof(SwVersion.Minor_obs), 9).AddProperty(nameof(SwVersion.Patch), 4252).AddProperty(nameof(SwVersion.Revision), 3)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 2016, 0)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "*?,'6"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\\9"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "R"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "TMW1!NZ")).ToJsonString());
            Add("3442.5.779.15.8230.891E9IB-W>5)E(+.Y", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 3442).AddProperty(nameof(SwVersion.Minor_obs), 5).AddProperty(nameof(SwVersion.Patch), 779).AddProperty(nameof(SwVersion.Revision), 15)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 8230, 891)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "E9IB"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "W>5)E("))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "Y")).ToJsonString());
            Add("-G\"(^?OU#-1827.888.7167.1.3.7-JI$6UD4)-X0&FV+/,]S-7:", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-G\"(^?OU#")
                .AddProperty(nameof(SwVersion.Major_obs), -1827).AddProperty(nameof(SwVersion.Minor_obs), 888).AddProperty(nameof(SwVersion.Patch), 7167).AddProperty(nameof(SwVersion.Revision), 1)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 3, 7)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "JI$6UD4)"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "X0&FV"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "/,]S"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "-").AddProperty(nameof(SwVersion.BuildSegment.Value), "7:")).ToJsonString());
            Add("33.3.408.79.3.2790.21", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 33).AddProperty(nameof(SwVersion.Minor_obs), 3).AddProperty(nameof(SwVersion.Patch), 408).AddProperty(nameof(SwVersion.Revision), 79)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 3, 2790, 21)
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("U-2644.325.403.5.6.63.1155", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "U")
                .AddProperty(nameof(SwVersion.Major_obs), -2644).AddProperty(nameof(SwVersion.Minor_obs), 325).AddProperty(nameof(SwVersion.Patch), 403).AddProperty(nameof(SwVersion.Revision), 5)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 6, 63, 1155)
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("6929.475.46.223.4.7.73+13_>X[GR", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 6929).AddProperty(nameof(SwVersion.Minor_obs), 475).AddProperty(nameof(SwVersion.Patch), 46).AddProperty(nameof(SwVersion.Revision), 223)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 4, 7, 73)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "13_>X[GR")).ToJsonString());
            Add("X:GG0.9.9666.8670.297.20.77+Y*", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "X:GG")
                .AddProperty(nameof(SwVersion.Major_obs), 0).AddProperty(nameof(SwVersion.Minor_obs), 9).AddProperty(nameof(SwVersion.Patch), 9666).AddProperty(nameof(SwVersion.Revision), 8670)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 297, 20, 77)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "Y*")).ToJsonString());
            Add("-184.316.6.10.64.369.41+?=U", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -184).AddProperty(nameof(SwVersion.Minor_obs), 316).AddProperty(nameof(SwVersion.Patch), 6).AddProperty(nameof(SwVersion.Revision), 10)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 64, 369, 41)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "?=U")).ToJsonString());
            Add(":>;,@G8.4156.846.3243.8.753.354+/O$[:03", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), ":>;,@G")
                .AddProperty(nameof(SwVersion.Major_obs), 8).AddProperty(nameof(SwVersion.Minor_obs), 4156).AddProperty(nameof(SwVersion.Patch), 846).AddProperty(nameof(SwVersion.Revision), 3243)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 8, 753, 354)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "/O$[:03")).ToJsonString());
            Add("405.84.699.94.1188.7.681-$S", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 405).AddProperty(nameof(SwVersion.Minor_obs), 84).AddProperty(nameof(SwVersion.Patch), 699).AddProperty(nameof(SwVersion.Revision), 94)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 1188, 7, 681)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "$S"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("\"'>IW&@71.12.58.114.2315.913.100-", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "\"'>IW&@")
                .AddProperty(nameof(SwVersion.Major_obs), 71).AddProperty(nameof(SwVersion.Minor_obs), 12).AddProperty(nameof(SwVersion.Patch), 58).AddProperty(nameof(SwVersion.Revision), 114)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 2315, 913, 100)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("1.812.17.96.47.9.87X=;\\+\\^W#BB", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 1).AddProperty(nameof(SwVersion.Minor_obs), 812).AddProperty(nameof(SwVersion.Patch), 17).AddProperty(nameof(SwVersion.Revision), 96)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 47, 9, 87)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "X=;\\"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "\\^W#BB")).ToJsonString());
            Add("/R92.742.21.8051.3915.2.182-+,E\\M/&W", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "/R")
                .AddProperty(nameof(SwVersion.Major_obs), 92).AddProperty(nameof(SwVersion.Minor_obs), 742).AddProperty(nameof(SwVersion.Patch), 21).AddProperty(nameof(SwVersion.Revision), 8051)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 3915, 2, 182)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ",E\\M/&W")).ToJsonString());
            Add("6034.9.25.29.63.3875.7-AT+S1\"", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 6034).AddProperty(nameof(SwVersion.Minor_obs), 9).AddProperty(nameof(SwVersion.Patch), 25).AddProperty(nameof(SwVersion.Revision), 29)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 63, 3875, 7)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "AT"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "S1\"")).ToJsonString());
            Add("IL?TX;=-75.8257.63.6968.35.8.2949-AG4S+!#>", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "IL?TX;=")
                .AddProperty(nameof(SwVersion.Major_obs), -75).AddProperty(nameof(SwVersion.Minor_obs), 8257).AddProperty(nameof(SwVersion.Patch), 63).AddProperty(nameof(SwVersion.Revision), 6968)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 35, 8, 2949)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "AG4S"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "!#>")).ToJsonString());
            Add("1119.3311.72.550.841.25.2-\\&&?8-3\"", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 1119).AddProperty(nameof(SwVersion.Minor_obs), 3311).AddProperty(nameof(SwVersion.Patch), 72).AddProperty(nameof(SwVersion.Revision), 550)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 841, 25, 2)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\\&&?8"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "3\""))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("T-786.82.25.0.3.6.1102-A-1SG", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "T")
                .AddProperty(nameof(SwVersion.Major_obs), -786).AddProperty(nameof(SwVersion.Minor_obs), 82).AddProperty(nameof(SwVersion.Patch), 25).AddProperty(nameof(SwVersion.Revision), 0)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 3, 6, 1102)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "A"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "1SG"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("4260.5222.66.63.99.3200.816-1^XO*KPZ.%+-##K\"8_H", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 4260).AddProperty(nameof(SwVersion.Minor_obs), 5222).AddProperty(nameof(SwVersion.Patch), 66).AddProperty(nameof(SwVersion.Revision), 63)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 99, 3200, 816)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "1^XO*KPZ"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "%"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "-").AddProperty(nameof(SwVersion.BuildSegment.Value), "##K\"8_H")).ToJsonString());
            Add("\"!\"*@^<S4804.8735.1.1769.2.0.118/:\"MX<FG-I#VG10+O3X+/V5HD9(", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "\"!\"*@^<S")
                .AddProperty(nameof(SwVersion.Major_obs), 4804).AddProperty(nameof(SwVersion.Minor_obs), 8735).AddProperty(nameof(SwVersion.Patch), 1).AddProperty(nameof(SwVersion.Revision), 1769)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 2, 0, 118)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "/:\"MX<FG"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "I#VG10"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "O3X"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "/V5HD9(")).ToJsonString());
            Add("6509.6.83.474.55.46.44-\\#<-]4+Y[.Y&", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 6509).AddProperty(nameof(SwVersion.Minor_obs), 6).AddProperty(nameof(SwVersion.Patch), 83).AddProperty(nameof(SwVersion.Revision), 474)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 55, 46, 44)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\\#<"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "]4"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "Y["),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), ".").AddProperty(nameof(SwVersion.BuildSegment.Value), "Y&")).ToJsonString());
            Add("!^IF'-21.4773.798.24.6.404.2946W*0.NP,ZBU)[+@;$+KU", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "!^IF'")
                .AddProperty(nameof(SwVersion.Major_obs), -21).AddProperty(nameof(SwVersion.Minor_obs), 4773).AddProperty(nameof(SwVersion.Patch), 798).AddProperty(nameof(SwVersion.Revision), 24)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 6, 404, 2946)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "W*0"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "NP,ZBU)["))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "@;$"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "KU")).ToJsonString());
            Add("-66.5311.1.1.2113.0.66.247", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -66).AddProperty(nameof(SwVersion.Minor_obs), 5311).AddProperty(nameof(SwVersion.Patch), 1).AddProperty(nameof(SwVersion.Revision), 1)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 2113, 0, 66, 247)
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("-$-763.42.93.1487.4.42.93.542", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-$")
                .AddProperty(nameof(SwVersion.Major_obs), -763).AddProperty(nameof(SwVersion.Minor_obs), 42).AddProperty(nameof(SwVersion.Patch), 93).AddProperty(nameof(SwVersion.Revision), 1487)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 4, 42, 93, 542)
                .AddNullProperty(nameof(SwVersion.PreRelease)).AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("31.686.3.912.92.72.9628.788+76", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 31).AddProperty(nameof(SwVersion.Minor_obs), 686).AddProperty(nameof(SwVersion.Patch), 3).AddProperty(nameof(SwVersion.Revision), 912)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 92, 72, 9628, 788)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "76")).ToJsonString());
            Add("))ZYML-9.148.8.4261.42.630.15.875+", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "))ZYML")
                .AddProperty(nameof(SwVersion.Major_obs), -9).AddProperty(nameof(SwVersion.Minor_obs), 148).AddProperty(nameof(SwVersion.Patch), 8).AddProperty(nameof(SwVersion.Revision), 4261)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 42, 630, 15, 875)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("-9.0.9.0.8.5734.8573.469+LU6*Q@", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -9).AddProperty(nameof(SwVersion.Minor_obs), 0).AddProperty(nameof(SwVersion.Patch), 9).AddProperty(nameof(SwVersion.Revision), 0)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 8, 5734, 8573, 469)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "LU6*Q@")).ToJsonString());
            Add("C=_C6.688.9.88.1553.5.53.2184+G95Y2", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "C=_C")
                .AddProperty(nameof(SwVersion.Major_obs), 6).AddProperty(nameof(SwVersion.Minor_obs), 688).AddProperty(nameof(SwVersion.Patch), 9).AddProperty(nameof(SwVersion.Revision), 88)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 1553, 5, 53, 2184)
                .AddNullProperty(nameof(SwVersion.PreRelease))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "G95Y2")).ToJsonString());
            Add("5720.81.8.7562.360.10.3738.8199-7<4", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 5720).AddProperty(nameof(SwVersion.Minor_obs), 81).AddProperty(nameof(SwVersion.Patch), 8).AddProperty(nameof(SwVersion.Revision), 7562)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 360, 10, 3738, 8199)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "7<4"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("[*XN%7.8243.9303.785.2563.1.4.2353F", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "[*XN%")
                .AddProperty(nameof(SwVersion.Major_obs), 7).AddProperty(nameof(SwVersion.Minor_obs), 8243).AddProperty(nameof(SwVersion.Patch), 9303).AddProperty(nameof(SwVersion.Revision), 785)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 2563, 1, 4, 2353)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "F"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("361.718.587.808.0.3.495.371-5+=\\7G5", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 361).AddProperty(nameof(SwVersion.Minor_obs), 718).AddProperty(nameof(SwVersion.Patch), 587).AddProperty(nameof(SwVersion.Revision), 808)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 0, 3, 495, 371)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "5"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "=\\7G5")).ToJsonString());
            Add("#L)TM824.40.30.9.7.5723.30.8E/[+@", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "#L)TM")
                .AddProperty(nameof(SwVersion.Major_obs), 824).AddProperty(nameof(SwVersion.Minor_obs), 40).AddProperty(nameof(SwVersion.Patch), 30).AddProperty(nameof(SwVersion.Revision), 9)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 7, 5723, 30, 8)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "E/["))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "@")).ToJsonString());
            Add("-56.364.5.96.2.60.87.96EY*\\M+", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), -56).AddProperty(nameof(SwVersion.Minor_obs), 364).AddProperty(nameof(SwVersion.Patch), 5).AddProperty(nameof(SwVersion.Revision), 96)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 2, 60, 87, 96)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "EY*\\M"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("-R:T@-8.6547.45.57.821.35.6601.609]S=/4+9J(M86/", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "-R:T@")
                .AddProperty(nameof(SwVersion.Major_obs), -8).AddProperty(nameof(SwVersion.Minor_obs), 6547).AddProperty(nameof(SwVersion.Patch), 45).AddProperty(nameof(SwVersion.Revision), 57)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 821, 35, 6601, 609)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "]S=/4"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "9J(M86/")).ToJsonString());
            Add("1046.3.3.87.269.9976.841.4731\"-_N", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 1046).AddProperty(nameof(SwVersion.Minor_obs), 3).AddProperty(nameof(SwVersion.Patch), 3).AddProperty(nameof(SwVersion.Revision), 87)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 269, 9976, 841, 4731)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "_N"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("%]]9571.8.2.6961.8.8695.4.88--F3/*", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "%]]")
                .AddProperty(nameof(SwVersion.Major_obs), 9571).AddProperty(nameof(SwVersion.Minor_obs), 8).AddProperty(nameof(SwVersion.Patch), 2).AddProperty(nameof(SwVersion.Revision), 6961)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 8, 8695, 4, 88)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "F3/*"))
                .AddNullProperty(nameof(SwVersion.Build)).ToJsonString());
            Add("570.6.82.0.9945.3.830.61&P]-\\+]\"EP\\\"ZP+(8'L2#0", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 570).AddProperty(nameof(SwVersion.Minor_obs), 6).AddProperty(nameof(SwVersion.Patch), 82).AddProperty(nameof(SwVersion.Revision), 0)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 9945, 3, 830, 61)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "&P]"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "\\"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "]\"EP\\\"ZP"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "(8'L2#0")).ToJsonString());
            Add("VG=H^5291.8.649.4.42.5.418.8-2.++>?@/)", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "VG=H^")
                .AddProperty(nameof(SwVersion.Major_obs), 5291).AddProperty(nameof(SwVersion.Minor_obs), 8).AddProperty(nameof(SwVersion.Patch), 649).AddProperty(nameof(SwVersion.Revision), 4)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 42, 5, 418, 8)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "2"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ""),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), ">?@/)")).ToJsonString());
            Add("0.83.8.7390.1049.99.457.550/Y/5:Y]K.+L1_O_+", new JsonObject().AddNullProperty(nameof(SwVersion.Prefix_obs))
                .AddProperty(nameof(SwVersion.Major_obs), 0).AddProperty(nameof(SwVersion.Minor_obs), 83).AddProperty(nameof(SwVersion.Patch), 8).AddProperty(nameof(SwVersion.Revision), 7390)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 1049, 99, 457, 550)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "/Y/5:Y]K"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), ""))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "L1_O_"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "")).ToJsonString());
            Add("C5981.75.6448.6.1.39.49.522(&4U-*^;SR+9S['+!", new JsonObject().AddProperty(nameof(SwVersion.Prefix_obs), "C")
                .AddProperty(nameof(SwVersion.Major_obs), 5981).AddProperty(nameof(SwVersion.Minor_obs), 75).AddProperty(nameof(SwVersion.Patch), 6448).AddProperty(nameof(SwVersion.Revision), 6)
                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical_obs), 1, 39, 49, 522)
                .AddObjectArrayProperty(nameof(SwVersion.PreRelease),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), true).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "(&4U"),
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), false).AddProperty(nameof(SwVersion.PreReleaseSegment.Value), "*^;SR"))
                .AddObjectArrayProperty(nameof(SwVersion.Build),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "9S['"),
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "+").AddProperty(nameof(SwVersion.BuildSegment.Value), "!")).ToJsonString());

        }
    }
}