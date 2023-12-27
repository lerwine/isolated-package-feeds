if ($null -ne (Get-Module -Name 'TestDataGeneration')) { Remove-Module -Name 'TestDataGeneration' }
Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath 'TestDataGeneration.psd1') -ErrorAction Stop;

<#

#>

$CharacterGroups = @{
    "LettersOrDigits" = @{
        Test = { [char]::IsLetterOrDigit($args[0]); };
        Includes = @("AsciiLettersUpper", "Digits", "UpperChars", "Vowels", "HardConsonants", "SoftConsonantsLower", "AsciiHexDigitsLower", "LowerChars",
            "AsciiLetters", "AsciiLettersOrDigits", "SoftConsonantsUpper", "ConsonantsLower", "HardConsonantsLower", "VowelsLower", "AsciiDigits", "VowelsUpper",
            "ConsonantsUpper", "AsciiLettersLower", "AsciiHexDigitsUpper", "Letters", "HardConsonantsUpper", "Consonants", "AsciiHexDigits", "SoftConsonants");
    }
    "AsciiChars" = @{
        GetValues = { return @([System.Linq.Enumerable]::Range(0x0000, 0x0080) | ForEach-Object { [char]$_ }) }
        Test = { [char]::IsAscii($args[0]); };
        Includes = @("AsciiLettersUpper", "Vowels", "HardConsonants", "SoftConsonantsLower", "AsciiHexDigitsLower", "AsciiLetters", "AsciiLettersOrDigits",
            "SoftConsonantsUpper", "ConsonantsLower", "HardConsonantsLower", "VowelsLower", "AsciiDigits", "VowelsUpper", "ConsonantsUpper", "AsciiLettersLower",
            "AsciiHexDigitsUpper", "HardConsonantsUpper", "Consonants", "AsciiHexDigits", "SoftConsonants");
    }
    "AsciiLettersOrDigits" = @{
        GetValues = { return @(([System.Linq.Enumerable]::Range(0x0030, 0x0039) + [System.Linq.Enumerable]::Range(0x0041, 0x005a) +
            [System.Linq.Enumerable]::Range(0x0061, 0x007a)) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsAsciiLetterOrDigit($args[0]); };
        Includes = @("AsciiLettersUpper", "Vowels", "HardConsonants", "SoftConsonantsLower", "AsciiHexDigitsLower", "AsciiLetters", "SoftConsonantsUpper",
            "ConsonantsLower", "HardConsonantsLower", "VowelsLower", "AsciiDigits", "VowelsUpper", "ConsonantsUpper", "AsciiLettersLower", "AsciiHexDigitsUpper",
            "HardConsonantsUpper", "Consonants", "AsciiHexDigits", "SoftConsonants");
    }
    "Letters" = @{
        Test = { [char]::IsLetter($args[0]); };
        Includes = @("AsciiLettersUpper", "UpperChars", "Vowels", "HardConsonants", "SoftConsonantsLower", "LowerChars", "AsciiLetters", "SoftConsonantsUpper",
            "ConsonantsLower", "HardConsonantsLower", "VowelsLower", "VowelsUpper", "ConsonantsUpper", "AsciiLettersLower", "HardConsonantsUpper", "Consonants",
            "SoftConsonants");       
    }
    "AsciiLetters" = @{
        GetValues = { return @(([System.Linq.Enumerable]::Range(0x0041, 0X005a) + [System.Linq.Enumerable]::Range(0x0061, 0X007a)) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsAsciiLetter($args[0]); };
        Includes = @("AsciiLettersUpper", "Vowels", "HardConsonants", "SoftConsonantsLower", "SoftConsonantsUpper", "ConsonantsLower", "HardConsonantsLower",
            "VowelsLower", "VowelsUpper", "ConsonantsUpper", "AsciiLettersLower", "HardConsonantsUpper", "Consonants", "SoftConsonants");
    }
    "UpperChars" = @{
        Test = { [char]::IsUpper($args[0]); };
        Includes = @("AsciiLettersUpper", "SoftConsonantsUpper", "VowelsUpper", "ConsonantsUpper", "HardConsonantsUpper");
    }
    "LowerChars" = @{
        Test = { [char]::IsLower($args[0]); };
        Includes = @("SoftConsonantsLower", "ConsonantsLower", "HardConsonantsLower", "VowelsLower", "AsciiLettersLower");
    }
    "Consonants" = @{
        GetValues = { return "BCDFGJJKLMNPQRSTVWXYZbcdfgjjklmnpqrstvwxyz" };
        Test = { "BCDFGJJKLMNPQRSTVWXYZbcdfgjjklmnpqrstvwxyz".Contains($args[0]); };
        Includes = @("HardConsonants", "ConsonantsLower", "HardConsonantsLower", "ConsonantsUpper", "HardConsonantsUpper");
    }
    "AsciiLettersUpper" = @{
        GetValues = { return @([System.Linq.Enumerable]::Range(0x0041, 0x005a) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsAsciiLetterUpper($args[0]); };
        Includes = @("SoftConsonantsUpper", "VowelsUpper", "ConsonantsUpper", "HardConsonantsUpper");
    }
    "AsciiLettersLower" = @{
        GetValues = { return [System.Linq.Enumerable]::Range(0x0061, 0X007a) | ForEach-Object { [char]$_ } };
        Test = { [char]::IsAsciiLetterLower($args[0]); };
        Includes = @("SoftConsonantsLower", "ConsonantsLower", "HardConsonantsLower", "VowelsLower");
    }
    "Numbers" = @{
        Test = { [char]::IsNumber($args[0]); };
        Includes = @("Digits", "AsciiDigits");
    }
    "SoftConsonants" = @{
        GetValues = { return "CFHLMNRSVWYZcfhlmnrsvwyz" };
        Test = { "CFHLMNRSVWYZcfhlmnrsvwyz".Contains($args[0]); };
        Includes = @("SoftConsonantsLower", "SoftConsonantsUpper");
    }
    "AsciiHexDigits" = @{
        GetValues = { return @(([System.Linq.Enumerable]::Range(0x0030, 0x0039) + [System.Linq.Enumerable]::Range(0x0041, 0x0046) +
            [System.Linq.Enumerable]::Range(0x0061, 0x0066)) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsAsciiHexDigit($args[0]); };
        Includes = @("AsciiHexDigitsLower", "AsciiDigits", "AsciiHexDigitsUpper");
    }
    "HighSurrogates" = @{
        GetValues = { return @([System.Linq.Enumerable]::Range(0xd800, 0xdbff) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsHighSurrogate($args[0]); };
        Includes = @();
    }
    "Vowels" = @{
        GetValues = { return "AEIOUYaeiouy"; }
        Test = { "AEIOUYaeiouy".Contains($args[0]); };
        Includes = @("VowelsLower", "VowelsUpper");
    }
    "HardConsonants" = @{
        GetValues = { return "BCDGJKPQTXbcdgjkpqtx"; }
        Test = { "BCDGJKPQTXbcdgjkpqtx".Contains($args[0]); };
        Includes = @("HardConsonantsLower", "HardConsonantsUpper");
    }
    "Surrogates" = @{
        GetValues = { return @([System.Linq.Enumerable]::Range(0xd800, 0xdfff) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsSurrogate($args[0]); };
        Includes = @("HighSurrogates", "LowSurrogates");
    }
    "Digits" = @{
        Test = { [char]::IsDigit($args[0]); };
        Includes = @("AsciiDigits");
    }
    "ConsonantsLower" = @{
        GetValues = { return "bcdfgjjklmnpqrstvwxyz" };
        Test = { "bcdfgjjklmnpqrstvwxyz".Contains($args[0]); };
        Includes = @("HardConsonantsLower");
    }
    "ConsonantsUpper" = @{
        GetValues = { return "BCDFGJJKLMNPQRSTVWXYZ" };
        Test = { "BCDFGJJKLMNPQRSTVWXYZ".Contains($args[0]); };
        Includes = @("HardConsonantsUpper");
    }
    "AsciiHexDigitsUpper" = @{
        GetValues = { return @(([System.Linq.Enumerable]::Range(0x0030, 0x0039) + [System.Linq.Enumerable]::Range(0x0041, 0x0046)) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsAsciiHexDigitUpper($args[0]); };
        Includes = @("AsciiDigits");
    }
    "AsciiHexDigitsLower" = @{
        GetValues = { return @(([System.Linq.Enumerable]::Range(0x0030, 0x0039) + [System.Linq.Enumerable]::Range(0x0061, 0x0066)) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsAsciiHexDigitLower($args[0]); };
        Includes = @("AsciiDigits");
    }
    "LowSurrogates" = @{
        GetValues = { return @([System.Linq.Enumerable]::Range(0xdc00, 0xdfff) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsLowSurrogate($args[0]); };
        Includes = @();
    }
    "ControlChars" = @{
        GetValues = { return @(([System.Linq.Enumerable]::Range(0x0000, 0x001f) + [System.Linq.Enumerable]::Range(0x007f, 0x009f)) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsControl($args[0]); };
        Includes = @();
    }
    "SoftConsonantsLower" = @{
        GetValues = { return "cfhlmnrsvwyz" }
        Test = { "cfhlmnrsvwyz".Contains($args[0]); };
        Includes = @();
    }
    "Separators" = @{
        GetValues = { return @(((0x0020, 0x00a0, 0x1680) + [System.Linq.Enumerable]::Range(0x2000, 0x200a) +
            (0x2028, 0x2029, 0x202f, 0x205f, 0x3000)) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsSeparator($args[0]); };
        Includes = @();
    }
    "PunctuationChars" = @{
        Test = { [char]::IsPunctuation($args[0]); };
        Includes = @();
    }
    "Symbols" = @{
        Test = { [char]::IsSymbol($args[0]); };
        Includes = @();
    }
    "SoftConsonantsUpper" = @{
        GetValues = { return "CFHLMNRSVWYZ" };
        Test = { "CFHLMNRSVWYZ".Contains($args[0]); };
        Includes = @();
    }
    "HardConsonantsLower" = @{
        GetValues = { return "bcdgjkpqtx" };
        Test = { "bcdgjkpqtx".Contains($args[0]); };
        Includes = @();
    }
    "VowelsLower" = @{
        GetValues = { return "aeiouy" };
        Test = { "aeiouy".Contains($args[0]); };
        Includes = @();
    }
    "AsciiDigits" = @{
        GetValues = { return [System.Linq.Enumerable]::Range(0x0030, 0x0039) | ForEach-Object { [char]$_ } };
        Test = { [char]::IsAsciiDigit($args[0]); };
        Includes = @();
    }
    "VowelsUpper" = @{
        GetValues = { return "AEIOUY"; };
        Test = { "AEIOUY".Contains($args[0]); };
        Includes = @();
    }
    "WhiteSpaceChars" = @{
        GetValues = { return @(([System.Linq.Enumerable]::Range(0x0009, 0x000d) + (0x0020, 0x0085, 0x00a0, 0x1680) +
            [System.Linq.Enumerable]::Range(0x2000, 0x200a) + (0x2028, 0x2029, 0x202f, 0x205f, 0x3000)) | ForEach-Object { [char]$_ }) };
        Test = { [char]::IsWhiteSpace($args[0]); };
        Includes = @("Separators");
    }
    "HardConsonantsUpper" = @{
        GetValues = { return "BCDGJKPQTX" };
        Test = { "BCDGJKPQTX".Contains($args[0]); };
        Includes = @();
    }
};

<#
$GroupDefinitions = @{
    VowelsUpper = { "AEIOUY".Contains($args[0]) };
    VowelsLower = { "aeiouy".Contains($args[0]) };
    Vowels = { "AEIOUYaeiouy".Contains($args[0]) };
    HardConsonantsUpper = { "BCDGJKPQTX".Contains($args[0]) };
    HardConsonantsLower = { "bcdgjkpqtx".Contains($args[0]) };
    HardConsonants = { "BCDGJKPQTXbcdgjkpqtx".Contains($args[0]) };
    SoftConsonantsUpper = { "CFHLMNRSVWYZ".Contains($args[0]) };
    SoftConsonantsLower = { "cfhlmnrsvwyz".Contains($args[0]) };
    SoftConsonants = { "CFHLMNRSVWYZcfhlmnrsvwyz".Contains($args[0]) };
    ConsonantsUpper =  { "BCDFGJJKLMNPQRSTVWXYZ".Contains($args[0]) };
    ConsonantsLower = { "bcdfgjjklmnpqrstvwxyz".Contains($args[0]) };
    Consonants = { "BCDFGJJKLMNPQRSTVWXYZbcdfgjjklmnpqrstvwxyz".Contains($args[0]) };
    AsciiChars = { [char]::IsAscii($args[0]) }
    AsciiDigits = { [char]::IsAsciiDigit($args[0]) }
    AsciiHexDigits = { [char]::IsAsciiHexDigit($args[0]) }
    AsciiHexDigitsLower = { [char]::IsAsciiHexDigitLower($args[0]) }
    AsciiHexDigitsUpper = { [char]::IsAsciiHexDigitUpper($args[0]) }
    AsciiLetters = { [char]::IsAsciiLetter($args[0]) }
    AsciiLettersLower = { [char]::IsAsciiLetterLower($args[0]) }
    AsciiLettersOrDigits = { [char]::IsAsciiLetterOrDigit($args[0]) }
    AsciiLettersUpper = { [char]::IsAsciiLetterUpper($args[0]) }
    ControlChars = { [char]::IsControl($args[0]) }
    Digits = { [char]::IsDigit($args[0]) }
    HighSurrogates = { [char]::IsHighSurrogate($args[0]) }
    Letters = { [char]::IsLetter($args[0]) }
    LettersOrDigits = { [char]::IsLetterOrDigit($args[0]) }
    LowerChars = { [char]::IsLower($args[0]) }
    LowSurrogates = { [char]::IsLowSurrogate($args[0]) }
    Numbers = { [char]::IsNumber($args[0]) }
    PunctuationChars = { [char]::IsPunctuation($args[0]) }
    Separators = { [char]::IsSeparator($args[0]) }
    Surrogates = { [char]::IsSurrogate($args[0]) }
    Symbols = { [char]::IsSymbol($args[0]) }
    UpperChars = { [char]::IsUpper($args[0]) }
    WhiteSpaceChars = { [char]::IsWhiteSpace($args[0]) }
};

$GroupDefinitions['WhiteSpaceChars'].GetType()
$ContainerDefinitions = @{};

$AllCharacters = [System.Linq.Enumerable]::Range(0, 65536);
foreach ($XKey in $GroupDefinitions.Keys) {
    $sb = $GroupDefinitions[$XKey];
    $XChars = @($AllCharacters | Where-Object {$sb.Invoke($_)});
    $ContainerDefinitions[$XKey] = @($GroupDefinitions.Keys | Where-Object {
        "$XKey/$_" | Write-Host -ForegroundColor Cyan;
        if ($_ -eq $XKey) { return $false }
        $sb = $GroupDefinitions[$_];
        $YChars = @($AllCharacters | Where-Object {$sb.Invoke($_)});
        return @($YChars | Where-Object { $XChars -ccontains $_ }).Count -eq $YChars.Count;
    };)
}
foreach ($XKey in $GroupDefinitions.Keys) {
    @"
    "$XKey" = @{
        Test = { $($GroupDefinitions[$XKey].ToString().Trim()); };
        Includes = @($(($ContainerDefinitions[$XKey] | % { "`"$_`"" }) -join ', '));
    }
"@
}
#>