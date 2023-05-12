namespace CdnGetter.UnitTests;

public partial class SwVersionUnitTest
{
    public class SemanticLikeRegexTestData : TheoryData<string, string?, string?, string?, string?, string?, string?, char?, string?, string?>
    {
        public SemanticLikeRegexTestData()
        {
            Add(string.Empty, /*pfx*/null, /*major*/null, /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/null, /*build*/null);
            Add("glide-rome-06-23-2021__patch0-07-07-2021", /*pfx*/"glide-rome-", /*major*/"06", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/"23-2021__patch0-07-07-2021",
                /*build*/null);
            Add("madrid-12-18-2018__patch4-05-29-2019_06-05-2019_1042", /*pfx*/"madrid-", /*major*/"12", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/"18-2018__patch4-05-29-2019_06-05-2019_1042",
                /*build*/null);
            Add("pip-1.3.1-py33-none-any.whl", /*pfx*/"pip-", /*major*/"1", /*minor*/"3", /*patch*/"1", /*rev*/null, /*xnum*/null, /*delim*/'-', /*pre*/"py33-none-any.whl", /*build*/null);
            Add("1!2.0", /*pfx*/null, /*major*/"1", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/"!2.0", /*build*/null);
            Add("v0.8", /*pfx*/"v", /*major*/"0", /*minor*/"8", /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/null, /*build*/null);
            Add("4.08c (1290)", /*pfx*/null, /*major*/"4", /*minor*/"08", /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/"c (1290)", /*build*/null);
            Add("1.2.3-164-g6f10c", /*pfx*/null, /*major*/"1", /*minor*/"2", /*patch*/"3", /*rev*/null, /*xnum*/null, /*delim*/'-', /*pre*/"164-g6f10c", /*build*/null);
            Add("00", /*pfx*/null, /*major*/"00", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/null, /*build*/null);
            Add("-00", /*pfx*/null, /*major*/"-00", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/null, /*build*/null);
            Add("-019", /*pfx*/null, /*major*/"-019", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/null, /*build*/null);
            Add("1", /*pfx*/null, /*major*/"1", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/null, /*build*/null);
            Add("0193", /*pfx*/null, /*major*/"0193", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/null, /*build*/null);
            Add("-0639.7379", /*pfx*/null, /*major*/"-0639", /*minor*/"7379", /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("0732.2302.-3", /*pfx*/null, /*major*/"0732", /*minor*/"2302", /*patch*/"-3", /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("35.528.-92.604", /*pfx*/null, /*major*/"35", /*minor*/"528", /*patch*/"-92", /*rev*/"604", /*xnum*/null,
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("-0770.4810.-457.-0.-77.8.594.7428.-8882", /*pfx*/null, /*major*/"-0770", /*minor*/"4810", /*patch*/"-457", /*rev*/"-0", /*xnum*/"-77.8.594.7428.-8882",
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("-3.-603.581.13.704.0.3210.-474.5700.3", /*pfx*/null, /*major*/"-3", /*minor*/"-603", /*patch*/"581", /*rev*/"13", /*xnum*/"704.0.3210.-474.5700.3",
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("5.02.0.4.-0915.557.5044.7.-0.686.-292", /*pfx*/null, /*major*/"5", /*minor*/"02", /*patch*/"0", /*rev*/"4", /*xnum*/"-0915.557.5044.7.-0.686.-292",
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("-2.-7.7065.409.81.-4.126.5.912.0831.-02.0", /*pfx*/null, /*major*/"-2", /*minor*/"-7", /*patch*/"7065", /*rev*/"409", /*xnum*/"81.-4.126.5.912.0831.-02.0",
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("31HAO", /*pfx*/null, /*major*/"31", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/"HAO",
                /*build*/null);
            Add("-59.9646-&6.MJJ", /*pfx*/null, /*major*/"-59", /*minor*/"9646", /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/"&6.MJJ",
                /*build*/null);
            Add("5.-5000.5162-", /*pfx*/null, /*major*/"5", /*minor*/"-5000", /*patch*/"5162", /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/string.Empty,
                /*build*/null);
            Add("-74.7242.5012.232.,RPO", /*pfx*/null, /*major*/"-74", /*minor*/"7242", /*patch*/"5012", /*rev*/"232", /*xnum*/null,
                /*delim*/'.', /*pre*/",RPO",
                /*build*/null);
            Add("28.106.-99.916.6.-2.74.6.65.?AJ7<", /*pfx*/null, /*major*/"28", /*minor*/"106", /*patch*/"-99", /*rev*/"916", /*xnum*/"6.-2.74.6.65",
                /*delim*/'.', /*pre*/"?AJ7<",
                /*build*/null);
            Add("8.8794.-6563.-020.-1721.621.8.09.282.-615._:_3", /*pfx*/null, /*major*/"8", /*minor*/"8794", /*patch*/"-6563", /*rev*/"-020", /*xnum*/"-1721.621.8.09.282.-615",
                /*delim*/'.', /*pre*/"_:_3",
                /*build*/null);
            Add("4760.-456.-598.3151.-715.1731.-593.7.531.7767.508W!I<N", /*pfx*/null, /*major*/"4760", /*minor*/"-456", /*patch*/"-598", /*rev*/"3151", /*xnum*/"-715.1731.-593.7.531.7767.508",
                /*delim*/null, /*pre*/"W!I<N",
                /*build*/null);
            Add("-850.4089.-94.-3.197.409.2.4.86.3970.0105.1.-*2MS1", /*pfx*/null, /*major*/"-850", /*minor*/"4089", /*patch*/"-94", /*rev*/"-3", /*xnum*/"197.409.2.4.86.3970.0105.1",
                /*delim*/'.', /*pre*/"-*2MS1",
                /*build*/null);
            Add("05+;9)5TN", /*pfx*/null, /*major*/"05", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null,
                /*build*/";9)5TN");
            Add("-34.2926+E,@DT,S", /*pfx*/null, /*major*/"-34", /*minor*/"2926", /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null,
                /*build*/"E,@DT,S");
            Add("8.29.709+,=M(\"", /*pfx*/null, /*major*/"8", /*minor*/"29", /*patch*/"709", /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null,
                /*build*/",=M(\"");
            Add("636.-5.51.2674+", /*pfx*/null, /*major*/"636", /*minor*/"-5", /*patch*/"51", /*rev*/"2674", /*xnum*/null,
                /*delim*/null, /*pre*/null,
                /*build*/string.Empty);
            Add("10.0.7558.436.4051.26.4.53.-85+", /*pfx*/null, /*major*/"10", /*minor*/"0", /*patch*/"7558", /*rev*/"436", /*xnum*/"4051.26.4.53.-85",
                /*delim*/null, /*pre*/null,
                /*build*/string.Empty);
            Add("69.81.793.-450.381.-8.-0.-1.-0209.9+T%Z6<Z?U", /*pfx*/null, /*major*/"69", /*minor*/"81", /*patch*/"793", /*rev*/"-450", /*xnum*/"381.-8.-0.-1.-0209.9",
                /*delim*/null, /*pre*/null,
                /*build*/"T%Z6<Z?U");
            Add("-0946.30.0.89.070.-2.-44.6017.-0.-25.-7+E^G", /*pfx*/null, /*major*/"-0946", /*minor*/"30", /*patch*/"0", /*rev*/"89", /*xnum*/"070.-2.-44.6017.-0.-25.-7",
                /*delim*/null, /*pre*/null,
                /*build*/"E^G");
            Add("1.490.-9.5.4.24.653.192.0.972.9734.28+I'2DU76", /*pfx*/null, /*major*/"1", /*minor*/"490", /*patch*/"-9", /*rev*/"5", /*xnum*/"4.24.653.192.0.972.9734.28",
                /*delim*/null, /*pre*/null,
                /*build*/"I'2DU76");
            Add("4-(V];+8=M", /*pfx*/null, /*major*/"4", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/"(V];",
                /*build*/"8=M");
            Add("-3.0-;/&G+!8MY]M0", /*pfx*/null, /*major*/"-3", /*minor*/"0", /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/";/&G",
                /*build*/"!8MY]M0");
            Add("-080.57.9-2+,IA", /*pfx*/null, /*major*/"-080", /*minor*/"57", /*patch*/"9", /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/"2",
                /*build*/",IA");
            Add("606.-45.9036.720-+", /*pfx*/null, /*major*/"606", /*minor*/"-45", /*patch*/"9036", /*rev*/"720", /*xnum*/null,
                /*delim*/'-', /*pre*/string.Empty,
                /*build*/string.Empty);
            Add("-07.245.88.-630.89.-1040.3.1.487-.+4WP,B", /*pfx*/null, /*major*/"-07", /*minor*/"245", /*patch*/"88", /*rev*/"-630", /*xnum*/"89.-1040.3.1.487",
                /*delim*/'-', /*pre*/".",
                /*build*/"4WP,B");
            Add("50.-1.802.7.94.06.88.8.-78.6.&@5+]SG-Y91", /*pfx*/null, /*major*/"50", /*minor*/"-1", /*patch*/"802", /*rev*/"7", /*xnum*/"94.06.88.8.-78.6",
                /*delim*/'.', /*pre*/"&@5",
                /*build*/"]SG-Y91");
            Add("07.0.-5526.9.2.-936.-0.-02.00.-417.4B+T6V", /*pfx*/null, /*major*/"07", /*minor*/"0", /*patch*/"-5526", /*rev*/"9", /*xnum*/"2.-936.-0.-02.00.-417.4",
                /*delim*/null, /*pre*/"B",
                /*build*/"T6V");
            Add("94.6381.-097.541.3212.700.502.5.20.531.20.96.G*+;$", /*pfx*/null, /*major*/"94", /*minor*/"6381", /*patch*/"-097", /*rev*/"541", /*xnum*/"3212.700.502.5.20.531.20.96",
                /*delim*/'.', /*pre*/"G*",
                /*build*/";$");
            Add("+'\";*[^G-%TGJ,JD(520", /*pfx*/"+'\";*[^G-%TGJ,JD(",
                /*major*/"520", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null, /*delim*/null, /*pre*/null, /*build*/null);
            Add("+N?;.QA=OOV))#!+\\#IOE-951.5", /*pfx*/"+N?;.QA=OOV))#!+\\#IOE-",
                /*major*/"951", /*minor*/"5", /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("*/M:EO)NKJ:E\"@W\"DP-7419.1.8", /*pfx*/"*/M:EO)NKJ:E\"@W\"DP-",
                /*major*/"7419", /*minor*/"1", /*patch*/"8", /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("+\\W\"#!F#C379.05.7.-70", /*pfx*/"+\\W\"#!F#C",
                /*major*/"379", /*minor*/"05", /*patch*/"7", /*rev*/"-70", /*xnum*/null,
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("[>G]F?D^-07.646.-22.-79.-8.40.1279.21.-2", /*pfx*/"[>G]F?D^-",
                /*major*/"07", /*minor*/"646", /*patch*/"-22", /*rev*/"-79", /*xnum*/"-8.40.1279.21.-2",
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("$%99.1.-3.-02.9.0378.1089.1.52.-01", /*pfx*/"$%",
                /*major*/"99", /*minor*/"1", /*patch*/"-3", /*rev*/"-02", /*xnum*/"9.0378.1089.1.52.-01",
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("FYMA/C,.@E[WEYCJ6.0.155.-080.-8371.749.2.7.-704.-1304.-11", /*pfx*/"FYMA/C,.@E[WEYCJ",
                /*major*/"6", /*minor*/"0", /*patch*/"155", /*rev*/"-080", /*xnum*/"-8371.749.2.7.-704.-1304.-11",
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("+('(G\"S_MF1509.36.230.77.5.0.69.-0.14.073.-2583.265", /*pfx*/"+('(G\"S_MF",
                /*major*/"1509", /*minor*/"36", /*patch*/"230", /*rev*/"77", /*xnum*/"5.0.69.-0.14.073.-2583.265",
                /*delim*/null, /*pre*/null, /*build*/null);
            Add("-\\@HQX_.-9906V=T", /*pfx*/"-\\@HQX_.-",
                /*major*/"9906", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/"V=T",
                /*build*/null);
            Add("OFL2129.72-A", /*pfx*/"OFL",
                /*major*/"2129", /*minor*/"72", /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/"A",
                /*build*/null);
            Add("L\"[+(@,E21.910.143.\\.C?B>HS", /*pfx*/"L\"[+(@,E",
                /*major*/"21", /*minor*/"910", /*patch*/"143", /*rev*/null, /*xnum*/null,
                /*delim*/'.', /*pre*/"\\.C?B>HS",
                /*build*/null);
            Add("WP_\\?!@#-J=MU#N^13.0.-6001.13._J.?", /*pfx*/"WP_\\?!@#-J=MU#N^",
                /*major*/"13", /*minor*/"0", /*patch*/"-6001", /*rev*/"13", /*xnum*/null,
                /*delim*/'.', /*pre*/"_J.?",
                /*build*/null);
            Add("T?H;\\XN]/V-<KHL-1.-4.5352.-144.512.-8682.3.-06.-686.Q-\"", /*pfx*/"T?H;\\XN]/V-<KHL-",
                /*major*/"1", /*minor*/"-4", /*patch*/"5352", /*rev*/"-144", /*xnum*/"512.-8682.3.-06.-686",
                /*delim*/'.', /*pre*/"Q-\"",
                /*build*/null);
            Add(">?Q!%S0.8.3.-502.071.30.-8018.00.515.56-", /*pfx*/">?Q!%S",
                /*major*/"0", /*minor*/"8", /*patch*/"3", /*rev*/"-502", /*xnum*/"071.30.-8018.00.515.56",
                /*delim*/'-', /*pre*/string.Empty,
                /*build*/null);
            Add("B'CU<L^V/(-1.0.3782.-60.086.-16.-22.643.-5763.-944.-1-=\",L,", /*pfx*/"B'CU<L^V/(-",
                /*major*/"1", /*minor*/"0", /*patch*/"3782", /*rev*/"-60", /*xnum*/"086.-16.-22.643.-5763.-944.-1",
                /*delim*/'-', /*pre*/"=\",L,",
                /*build*/null);
            Add("NW^RNS;@S>?DHK9.-692.-861.977.0928.3.7008.500.3.0.8124.351.N]9Z;5?", /*pfx*/"NW^RNS;@S>?DHK",
                /*major*/"9", /*minor*/"-692", /*patch*/"-861", /*rev*/"977", /*xnum*/"0928.3.7008.500.3.0.8124.351",
                /*delim*/'.', /*pre*/"N]9Z;5?",
                /*build*/null);
            Add("Z#;\"?U<\\3+:,\"", /*pfx*/"Z#;\"?U<\\",
                /*major*/"3", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null,
                /*build*/":,\"");
            Add(".:&]E%67.0+", /*pfx*/".:&]E%",
                /*major*/"67", /*minor*/"0", /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null,
                /*build*/string.Empty);
            Add("+[L\"<'D\"#-&2520.-11.703+=+H", /*pfx*/"+[L\"<'D\"#-&",
                /*major*/"2520", /*minor*/"-11", /*patch*/"703", /*rev*/null, /*xnum*/null,
                /*delim*/null, /*pre*/null,
                /*build*/"=+H");
            Add("HJ<?E;BWC/IA8.-7.0723.-8+I%$:D*", /*pfx*/"HJ<?E;BWC/IA",
                /*major*/"8", /*minor*/"-7", /*patch*/"0723", /*rev*/"-8", /*xnum*/null,
                /*delim*/null, /*pre*/null,
                /*build*/"I%$:D*");
            Add("YZ<C'N$)F>Z,W'-5.2.400.298.139.-608.79.9.-5+C", /*pfx*/"YZ<C'N$)F>Z,W'-",
                /*major*/"5", /*minor*/"2", /*patch*/"400", /*rev*/"298", /*xnum*/"139.-608.79.9.-5",
                /*delim*/null, /*pre*/null,
                /*build*/"C");
            Add("%,/-<\\1.9608.9100.5224.-6974.-29.205.45.67.58+/#", /*pfx*/"%,/-<\\",
                /*major*/"1", /*minor*/"9608", /*patch*/"9100", /*rev*/"5224", /*xnum*/"-6974.-29.205.45.67.58",
                /*delim*/null, /*pre*/null,
                /*build*/"/#");
            Add(".(P-^OXPBFA.P8283.-2.97.995.-2950.-618.451.-8.28.46.1+J", /*pfx*/".(P-^OXPBFA.P",
                /*major*/"8283", /*minor*/"-2", /*patch*/"97", /*rev*/"995", /*xnum*/"-2950.-618.451.-8.28.46.1",
                /*delim*/null, /*pre*/null,
                /*build*/"J");
            Add("#[<##EHXHO>N)G-\"I],-248.9876.2.-107.150.102.613.0.835.00.0.3745+FU\\C;:PU", /*pfx*/"#[<##EHXHO>N)G-\"I],-",
                /*major*/"248", /*minor*/"9876", /*patch*/"2", /*rev*/"-107", /*xnum*/"150.102.613.0.835.00.0.3745",
                /*delim*/null, /*pre*/null,
                /*build*/"FU\\C;:PU");
            Add("S>(R_;?T%!+L\\N7.X[<+EYU)?", /*pfx*/"S>(R_;?T%!+L\\N",
                /*major*/"7", /*minor*/null, /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/'.', /*pre*/"X[<",
                /*build*/"EYU)?");
            Add("@DHX?=Y15.6-^7+Y#8?", /*pfx*/"@DHX?=Y",
                /*major*/"15", /*minor*/"6", /*patch*/null, /*rev*/null, /*xnum*/null,
                /*delim*/'-', /*pre*/"^7",
                /*build*/"Y#8?");
            Add("'VI#-_.Z?.%=*(-8767.46.6.%3/1[KW+*&EKMS", /*pfx*/"'VI#-_.Z?.%=*(-",
                /*major*/"8767", /*minor*/"46", /*patch*/"6", /*rev*/null, /*xnum*/null,
                /*delim*/'.', /*pre*/"%3/1[KW",
                /*build*/"*&EKMS");
            Add("'G\"$S>BV[239.0532.-091.0.@G+R$=", /*pfx*/"'G\"$S>BV[",
                /*major*/"239", /*minor*/"0532", /*patch*/"-091", /*rev*/"0", /*xnum*/null,
                /*delim*/'.', /*pre*/"@G",
                /*build*/"R$=");
            Add("N7.5037.206.00.6.73.-2.-38.67.=+HKR>1", /*pfx*/"N",
                /*major*/"7", /*minor*/"5037", /*patch*/"206", /*rev*/"00", /*xnum*/"6.73.-2.-38.67",
                /*delim*/'.', /*pre*/"=",
                /*build*/"HKR>1");
            Add("%S03.20.-08.78.66.-221.088.9008.0341.268-G2.5UT:+1", /*pfx*/"%S",
                /*major*/"03", /*minor*/"20", /*patch*/"-08", /*rev*/"78", /*xnum*/"66.-221.088.9008.0341.268",
                /*delim*/'-', /*pre*/"G2.5UT:",
                /*build*/"1");
            Add("<MK!@CJY#\"!5759.711.915.9169.5.-27.6862.0.7933.7.2317-52Q@6YC+O\"^^IF'", /*pfx*/"<MK!@CJY#\"!",
                /*major*/"5759", /*minor*/"711", /*patch*/"915", /*rev*/"9169", /*xnum*/"5.-27.6862.0.7933.7.2317",
                /*delim*/'-', /*pre*/"52Q@6YC",
                /*build*/"O\"^^IF'");
            Add(",P!RFKDT213.-6.84.-5.8917.82.-02.140.-32.2424.4.-67.$N+>*@\"]L", /*pfx*/",P!RFKDT",
                /*major*/"213", /*minor*/"-6", /*patch*/"84", /*rev*/"-5", /*xnum*/"8917.82.-02.140.-32.2424.4.-67",
                /*delim*/'.', /*pre*/"$N",
                /*build*/">*@\"]L");
        }
    }
}