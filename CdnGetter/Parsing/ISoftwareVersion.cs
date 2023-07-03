namespace CdnGetter.Parsing;

public interface ISoftwareVersion
{
    IToken? Prefix { get; }

    INumericalToken Major { get; }

    INumericalToken? Minor { get; }

    INumericalToken? Patch { get; }

    IEnumerable<INumericalToken> Micro { get; }

    IEnumerable<IToken> PreRelease { get; }

    IEnumerable<IToken> Build { get; }
}
    
    // public static readonly ValueConverter<SwVersion, string> Converter = new(
    //     v => v.ToString(),
    //     s => new(s)
    // );

    // /// <summary>
    // /// Matches a version string similar to the SemVer format.
    // /// </summary>
    // /// <see href="https://semver.org/" />
    // public static readonly Regex SemanticLikeRegex = new(@$"^(?<{REGEX_GROUP_pfx}>[+-]*[^\d+-]+([+-]+[^\d+-]+)*-*)?(?<{REGEX_GROUP_major}>-?\d+)(\.(?<{REGEX_GROUP_minor}>-?\d+)(\.(?<{REGEX_GROUP_patch}>-?\d+)(\.(?<{REGEX_GROUP_rev}>-?\d+)(\.(?<{REGEX_GROUP_xnum}>-?\d+(\.-?\d+)*))?)?)?)?((?<{REGEX_GROUP_DELIM}>\.)(?<{REGEX_GROUP_PRE}>([^\d+][^+]*)?)|(?<{REGEX_GROUP_DELIM}>-)(?<{REGEX_GROUP_PRE}>[^+]*)|(?<{REGEX_GROUP_PRE}>[^\d.+-][^+]*))?(\+(?<{REGEX_GROUP_BUILD}>.*))?$", RegexOptions.Compiled);

//     /// <summary>
//     /// Matches a version string similar to the PEP 440 format.
//     /// </summary>
//     /// <see href="https://peps.python.org/pep-0440/" />
//     public static readonly Regex Pep440Regex = new(@$"^
// (?<{REGEX_GROUP_pfx}>\D+)?
// ((?<{REGEX_GROUP_epoch}>\d+)!)?
// (?<{REGEX_GROUP_major}>\d+)(\.(?<{REGEX_GROUP_minor}>\d+)(\.(?<{REGEX_GROUP_patch}>\d+)(\.(?<{REGEX_GROUP_rev}>\d+)(\.(?<{REGEX_GROUP_xnum}>\d+(\.\d+)*))?)?)?)?
// (
//     (?<{REGEX_GROUP_DELIM}>[-_\.])?
//     (?<{REGEX_GROUP_PRE}>
//         (?<{REGEX_GROUP_modname}>[a-z]+)
//         [-_\.]?(?<{REGEX_GROUP_modnum}>\d+)?
//     )
//     |
//     (?<{REGEX_GROUP_DELIM}>-)(?<{REGEX_GROUP_PRE}>\d+) # post
// )?
// (
//     [+.-](?<{REGEX_GROUP_BUILD}>.*)
//     |
//     (?<{REGEX_GROUP_BUILD}>[^\d+_.-].*)
// )?
// $", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

//     /// <summary>
//     /// Matches a date-based version string
//     /// </summary>
//     public static readonly Regex DatedVersionRegex = new(@$"^
// (?<{REGEX_GROUP_pfx}>\D+)?
// (
//     (?<{REGEX_GROUP_minor}>1[012]|0?\d)-(?<{REGEX_GROUP_rel}>3[01]|[012]?\d)-(?<{REGEX_GROUP_major}>\d{{4}})
//     |
//     (?<{REGEX_GROUP_major}>\d{{2}}|\d{{4}})-(?<{REGEX_GROUP_minor}>1[012]|0?\d)-(?<{REGEX_GROUP_rel}>3[01]|[012]?\d)
// )
// (
//     (?<{REGEX_GROUP_DELIM}>(\.|_+)([^\d._]\D*)?|[^\d._]\D*)
//     (
//         (?<{REGEX_GROUP_patch}>\d+)
//         [_+-]*
//     )?
//     (
//         (?<{REGEX_GROUP_xnum}>(1[012]|0?\d)-(3[01]|[012]?\d))-(?<{REGEX_GROUP_rev}>\d{{4}})
//         |
//         (?<{REGEX_GROUP_rev}>\d{{2}}|\d{{4}})-(?<{REGEX_GROUP_xnum}>(1[012]|0?\d)-(3[01]|[012]?\d))
//     )
//     (
//         [_-]+
//         (
//             (?<{REGEX_GROUP_endday}>(1[012]|0?\d)-(3[01]|[012]?\d))-(?<{REGEX_GROUP_endyr}>\d{{4}})
//             |
//             (?<{REGEX_GROUP_endyr}>\d{{2}}|\d{{4}})-(?<{REGEX_GROUP_endday}>(1[012]|0?\d)-(3[01]|[012]?\d))
//         )
//     )
// )?
// (
//     [_+-](?<{REGEX_GROUP_BUILD}>.*)
//     |
//     (?<{REGEX_GROUP_BUILD}>\D.*)
// )?
// $", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);