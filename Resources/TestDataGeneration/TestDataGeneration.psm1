Add-Type -AssemblyName 'System.Text.Json' -ErrorAction Stop;

if ($null -eq $Script:Random) { New-Variable -Name 'Random' -Option ReadOnly -Scope 'Script' -Value ([Random]::new()) }
if ($null -eq $Script:Vowels) { New-Variable -Name 'Vowels' -Option Constant -Scope 'Script' -Value 'AEIOUYaeiouy' }
if ($null -eq $Script:HardConsonants) { New-Variable -Name 'HardConsonants' -Option Constant -Scope 'Script' -Value 'BCDGJKPQTXbcdgjkpqtx' }
if ($null -eq $Script:SoftConsonants) { New-Variable -Name 'SoftConsonants' -Option Constant -Scope 'Script' -Value 'CFHLMNRSVWYZcfhlmnrsvwyz' }
if ($null -eq $Script:Consonants) { New-Variable -Name 'Consonants' -Option Constant -Scope 'Script' -Value 'CFHLMNRSVWYZcfhlmnrsvwyz' }

Function Get-RandomCharacterSource {
    [CmdletBinding()]
    Param(
        [ValidateSet('AsciiChars', 'AsciiDigits', 'AsciiHexDigits', 'AsciiHexDigitsLower', 'AsciiHexDigitsUpper', 'AsciiLetters',
            'AsciiLettersLower', 'AsciiLettersOrDigits', 'AsciiLettersUpper', 'ControlChars', 'Digits', 'Letters', 'LettersOrDigits', 'LowerCase', 'Numbers',
            'PunctuationChars', 'SeparatorChars', 'SymbolChars', 'UpperCase', 'WhiteSpace', 'Vowels', 'Consonants', 'HardConsonants', 'SoftConsonants')]
        [string[]]$Include,

        [char[]]$ExplicitInclude,

        [ValidateSet('AsciiChars', 'AsciiDigits', 'AsciiHexDigits', 'AsciiHexDigitsLower', 'AsciiHexDigitsUpper', 'AsciiLetters',
            'AsciiLettersLower', 'AsciiLettersOrDigits', 'AsciiLettersUpper', 'ControlChars', 'Digits', 'Letters', 'LettersOrDigits', 'LowerCase', 'Numbers',
            'PunctuationChars', 'SeparatorChars', 'SymbolChars', 'UpperCase', 'WhiteSpace', 'Vowels', 'Consonants', 'HardConsonants', 'SoftConsonants')]
        [string[]]$Exclude,

        [char[]]$ExplicitExclude,

        [ValidateRange(0, ([int]([char]::MaxValue)) - 1)]
        [int]$MinResultCount = 2,

        [switch]$AsString
    )
    [System.Collections.ObjectModel.Collection[char]]$Results = [System.Linq.Enumerable]::Range(([int][char]::MinValue), ([int][char]::MaxValue)) | ForEach-Object { [char]$_ };
    if ($PSBoundParameters.ContainsKey('Include')) {
        $Available = $Results;
        $Results = [System.Collections.ObjectModel.Collection[char]]::new();
        $Include = $Include | Select-Object -Unique;
        if ($Include -icontains 'AsciiLetters' -or $Include -icontains 'AsciiLettersOrDigits' -or $Include -icontains 'Letters' -or $Include -icontains 'LettersOrDigits') {
            $Include = @($Include | Where-Object { $_ -ne 'HardConsonants' -and $_ -ne 'SoftConsonants' -and $_ -ne 'Consonants' -and $_ -ne 'Vowels' });
        } else {
            if ($Include -icontains 'Consonants') {
                $Include = @($Include | Where-Object { $_ -ne 'HardConsonants' -and $_ -ne 'SoftConsonants' });
            } else {
                if ($Include -icontains 'HardConsonants' -and $Include -icontains 'SoftConsonants') {
                    $Include = @($Include | Where-Object { $_ -ne 'HardConsonants' -and $_ -ne 'SoftConsonants' }) + @('Consonants');
                }
            }
        }
        if ($Include -icontains 'LettersOrDigits') {
            $Include = @($Include | Where-Object { $_ -ne 'Letters' -and $_ -ne 'Digits' -and $_ -ne 'AsciiLettersOrDigits' -and $_ -ne 'AsciiLetters' -and $_ -ne 'AsciiDigits' });
        } else {
            if ($Include -icontains 'AsciiLettersOrDigits') { $Include = @($Include | Where-Object { $_ -ne 'AsciiLetters' -and $_ -ne 'AsciiDigits' }) }
        }
        foreach ($i in $Include) {
            if ($Available.Count -eq 0) { break }
            switch ($i) {
                'AsciiChars' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAscii($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiDigits' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAsciiDigit($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiHexDigits' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAsciiHexDigit($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiHexDigitsLower' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAsciiHexDigitLower($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiHexDigitsUpper' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAsciiHexDigitUpper($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiLetters' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAsciiLetter($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiLettersLower' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAsciiLetterLower($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiLettersOrDigits' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAsciiLetterOrDigit($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiLettersUpper' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsAsciiLetterUpper($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'ControlChars' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsControl($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Digits' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsDigit($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Letters' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsLetter($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'LettersOrDigits' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsLetterOrDigit($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'LowerCase' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsLower($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Numbers' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsNumber($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'PunctuationChars' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsPunctuation($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'SeparatorChars' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsSeparator($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'SymbolChars' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsSymbol($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'UpperCase' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsUpper($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'WhiteSpace' {
                    for ($i = 0; $i -lt $Available.Count; $i++) {
                        $c = $Available[$i];
                        if ([char]::IsWhiteSpace($c)) {
                            $Results.Add($c);
                            $Available.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Vowels' {
                    $Script:Vowels | ForEach-Object {
                        if (-not $Results.Contains($_)) {
                            $Results.Add($_);
                            $Available.Remove($_) | Out-Null;
                        }
                    }
                    break;
                }
                'Consonants' {
                    $Script:Consonants | ForEach-Object {
                        if (-not $Results.Contains($_)) {
                            $Results.Add($_);
                            $Available.Remove($_) | Out-Null;
                        }
                    }
                    break;
                }
                'HardConsonants' {
                    $Script:HardConsonants | ForEach-Object {
                        if (-not $Results.Contains($_)) {
                            $Results.Add($_);
                            $Available.Remove($_) | Out-Null;
                        }
                    }
                    break;
                }
                'SoftConsonants' {
                    $Script:SoftConsonants | ForEach-Object {
                        if (-not $Results.Contains($_)) {
                            $Results.Add($_);
                            $Available.Remove($_) | Out-Null;
                        }
                    }
                    break;
                }
            }
        }
    }
    if ($PSBoundParameters.Contains('ExplicitInclude') -and $Available.Count -gt 0) {
        foreach ($c in $ExplicitInclude) {
            if (-not $Results.Contains($c)) { $Results.Add($c) }
        }
    }
    
    $BreakOn = 0;
    if ($MinResultCount -gt 1) { $BreakOn = $MinResultCount - 1 }

    if ($PSBoundParameters.ContainsKey('Exclude') -and $Results.Count -gt $BreakOn) {
        $Exclude = $Exclude | Select-Object -Unique;
        if ($Exclude -icontains 'AsciiLetters' -or $Exclude -icontains 'AsciiLettersOrDigits' -or $Exclude -icontains 'Letters' -or $Exclude -icontains 'LettersOrDigits') {
            $Exclude = @($Exclude | Where-Object { $_ -ne 'HardConsonants' -and $_ -ne 'SoftConsonants' -and $_ -ne 'Consonants' -and $_ -ne 'Vowels' });
        } else {
            if ($Exclude -icontains 'Consonants') {
                $Exclude = @($Exclude | Where-Object { $_ -ne 'HardConsonants' -and $_ -ne 'SoftConsonants' });
            } else {
                if ($Exclude -icontains 'HardConsonants' -and $Exclude -icontains 'SoftConsonants') {
                    $Exclude = @($Exclude | Where-Object { $_ -ne 'HardConsonants' -and $_ -ne 'SoftConsonants' }) + @('Consonants');
                }
            }
        }
        if ($Exclude -icontains 'LettersOrDigits') {
            $Exclude = @($Exclude | Where-Object { $_ -ne 'Letters' -and $_ -ne 'Digits' -and $_ -ne 'AsciiLettersOrDigits' -and $_ -ne 'AsciiLetters' -and $_ -ne 'AsciiDigits' });
        } else {
            if ($Exclude -icontains 'AsciiLettersOrDigits') { $Exclude = @($Exclude | Where-Object { $_ -ne 'AsciiLetters' -and $_ -ne 'AsciiDigits' }) }
        }
        foreach ($e in $Exclude) {
            if ($Results.Count -le $BreakOn) { break }
            switch ($e) {
                'AsciiChars' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAscii($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiDigits' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAsciiDigit($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiHexDigits' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAsciiHexDigit($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiHexDigitsLower' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAsciiHexDigitLower($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiHexDigitsUpper' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAsciiHexDigitUpper($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiLetters' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAsciiLetter($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiLettersLower' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAsciiLetterLower($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiLettersOrDigits' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAsciiLetterOrDigit($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'AsciiLettersUpper' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsAsciiLetterUpper($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'ControlChars' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsControl($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Digits' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsDigit($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Letters' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsLetter($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'LettersOrDigits' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsLetterOrDigit($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'LowerCase' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsLower($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Numbers' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsNumber($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'PunctuationChars' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsPunctuation($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'SeparatorChars' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsSeparator($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'SymbolChars' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsSymbol($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'UpperCase' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsUpper($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'WhiteSpace' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ([char]::IsWhiteSpace($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Vowels' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ($Script:Vowels.Contains($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'Consonants' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ($Script:Consonants.Contains($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'HardConsonants' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ($Script:HardConsonants.Contains($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
                'SoftConsonants' {
                    for ($i = 0; $i -lt $Results.Count; $i++) {
                        if ($Script:SoftConsonants.Contains($Results[$i])) {
                            $Results.RemoveAt($i);
                            $i--;
                        }
                    }
                    break;
                }
            }
        }
    }

    if ($PSBoundParameters.Contains('ExplicitInclude') -and $Results.Count -gt $BreakOn) {
        foreach ($c in $ExplicitInclude) { $Results.Remove($c) | Out-Null }
    }

    if ($Results -lt $MinResultCount) {
        if ($MinResultCount -eq 1) {
            Write-Error -Message "The exclusions did not allow for at least 1 character." -Category InvalidResult -ErrorId 'TooFewResults';
        } else {
            Write-Error -Message "The exclusions did not allow for at least $MinResultCount characters." -Category InvalidResult -ErrorId 'TooFewResults';
        }
    } else {
        if ($AsString.IsPresent) {
            (-join $Results) | Write-Output;
        } else {
            $Results | Write-Output;
        }
    }
}

Function Convert-RangePatternToTuple {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [string]$Pattern
    )
    Begin {
        if ($null -eq $Script:__Convert_RangePatternToTuple) {
            New-Variable -Name '__Convert_RangePatternToTuple' -Scope 'Script' -Option ReadOnly -Value ([System.Text.RegularExpressions.Regex]::new(
                '^(?<s>\d+)(-(?<e>\d+))?$',
                [System.Text.RegularExpressions.RegexOptions]::Compiled
            )) -Force;
        }
    }

    Process {
        $m = $Script:__Convert_RangePatternToTuple.Match($Pattern);
        if ($m.Success) {
            $Start = 0;
            if ([int]::TryParse($m.Groups['s'].Value, [ref]$Start)) {
                $g = $m.Groups['e'].Value;
                if ($g.Success) {
                    $End = 0;
                    if ([int]::TryParse($g.Value, [ref]$End)) {
                        if ($End -lt $Start) {
                            Write-Error -Message "Range start cannot be greater than the range end" -Category InvalidArgument;
                            Write-Output -InputObject ([ValueTuple]::Create($End, $End)) -NoEnumerate;
                        } else {
                            Write-Output -InputObject ([ValueTuple]::Create($Start, $End)) -NoEnumerate;
                        }
                    } else {
                        Write-Error -Message "Could not parse $($g.Value) as an integer." -Category InvalidArgument;
                        Write-Output -InputObject ([ValueTuple]::Create($Start, $Start)) -NoEnumerate;
                    }
                } else {
                    Write-Output -InputObject ([ValueTuple]::Create($Start, $Start)) -NoEnumerate;
                }
            } else {
                Write-Error -Message "Could not parse $($m.Groups['s'].Value) as an integer." -Category InvalidArgument;
                Write-Output -InputObject ([ValueTuple]::Create(0, 0)) -NoEnumerate;
            }
        }
    }
}

Function Convert-RangeValuesToTuple {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [int[]]$Values
    )

    Begin { $AllValues = @() }

    Process { $AllValues += $Values }

    End {
        switch ($AllValues.Count) {
            0 {
                Write-Error -Message 'Cannot convert empty array to range Tuple' -Category InvalidArgument;
                return [System.ValueTuple]::Create(0, 0);
                break;
            }
            1 {
                return [System.ValueTuple]::Create($AllValues[0], $AllValues[0]);
            }
            2 {
                if ($AllValues[1] -lt $AllValues[0]) {
                    Write-Error -Message 'First range value cannot be greater than the second' -Category InvalidArgument;
                    if ($AllValues[1] -lt 0 -and [Math]::Abs($AllValues[0]) -lt [Math]::Abs($AllValues[1])) { return [System.ValueTuple]::Create($AllValues[0], $AllValues[0]) }
                    [System.ValueTuple]::Create($AllValues[1], $AllValues[1]);
                }
                return [System.ValueTuple]::Create($AllValues[0], $AllValues[1]);
            }
            default {
                Write-Error -Message "Cannot convert array of $_ values to range Tuple" -Category InvalidArgument;
                if ($AllValues[1] -lt $AllValues[0]) {
                    if ($AllValues[1] -lt 0 -and [Math]::Abs($AllValues[0]) -lt [Math]::Abs($AllValues[1])) { return [System.ValueTuple]::Create($AllValues[0], $AllValues[0]) }
                    [System.ValueTuple]::Create($AllValues[1], $AllValues[1]);
                }
                return [System.ValueTuple]::Create($AllValues[0], $AllValues[1]);
            }
        }
    }
}

<#
.SYNOPSIS
    Generates a random number.
#>
Function Get-RandomNumber {

    [CmdletBinding(DefaultParameterSetName = "FixedRepeat")]
    Param(
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [Alias('Item1')]
        [int]$MinValue = [Int]::MinValue,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [Alias('Item2')]
        [int]$MaxValue = [int]::MaxValue,

        [Parameter(Mandatory = $true, ParameterSetName = "Range")]
        [System.ValueTuple[int, int]]$Range,

        [Parameter(ParameterSetName = "FixedRepeat")]
        [ValidateRange(1)]
        [int]$Repeat = 1,

        [Parameter(ParameterSetName = "RandomRepeat")]
        [ValidateRange(0)]
        [int]$MinRepeat = 1,

        [Parameter(Mandatory = $true, ParameterSetName = "RandomRepeat")]
        [ValidateRange(1)]
        [int]$MaxRepeat,

        [System.IO.TextWriter]$Writer
    )
    if ($PSBoundParameters.ContainsKey('Range')) {
        $MinValue = $Range.Item1;
        $MaxValue = $Range.Item2;
    }
    if ($MinValue -gt $MaxValue) {
        Write-Error -Message "MinValue cannot be greater than MaxValue" -Category InvalidArgument -ErrorId 'InvalidValueRange';
    }
    if ($PSCmdlet.ParameterSetName -eq 'RandomRepeat') {
        if ($MinRepeat -gt $MaxRepeat) {
            Write-Error -Message "MinRepeat cannot be greater than MaxRepeat" -Category InvalidArgument -ErrorId 'InvalidRepeatRange';
            $Repeat = 0;
        } else {
            if ($MinRepeat -eq $MaxRepeat) {
                $Repeat = $MinRepeat;
            } else {
                if ($MaxRepeat -eq [int]::MaxValue) {
                    $Repeat = $Script:Random.Next($MinRepeat - 1, [int]::MaxValue) + 1;
                } else {
                    $Repeat = $Script:Random.Next($MinRepeat, $MaxRepeat + 1);
                }
            }
        }
    }
    if ($Repeat -gt 0) {
        if ($PSBoundParameters.ContainsKey('Writer')) {
            if ($MinValue -eq $MaxValue) {
                $Writer.Write($MinValue);
                for ($i = 1; $i -lt $Repeat; $i++) {
                    $Writer.Write(', ');
                    $Writer.Write($MinValue);
                }
            } else {
                if ($MaxValue -eq [int]::MaxValue) {
                    if ($MinValue -eq [int]::MinValue) {
                        if ($Script:Random.Next(0, 2) -eq 1) {
                            $Writer.Write($Script:Random.Next(-1, $MaxValue) + 1);
                        } else {
                            $Writer.Write($Script:Random.Next($MinValue, 0));
                        }
                        for ($i = 1; $i -lt $Repeat; $i++) {
                            $Writer.Write(', ');
                            if ($Script:Random.Next(0, 2) -eq 1) {
                                $Writer.Write($Script:Random.Next(-1, $MaxValue) + 1);
                            } else {
                                $Writer.Write($Script:Random.Next($MinValue, 0));
                            }
                        }
                    } else {
                        $Writer.Write($Script:Random.Next($MinValue - 1, [int]::MaxValue) + 1);
                        for ($i = 1; $i -lt $Repeat; $i++) {
                            $Writer.Write(', ');
                            $Writer.Write($Script:Random.Next($MinValue - 1, [int]::MaxValue) + 1);
                        }
                    }
                } else {
                    $Writer.Write($Script:Random.Next($MinValue, $MaxValue + 1));
                    for ($i = 1; $i -lt $Repeat; $i++) {
                        $Writer.Write(', ');
                        $Writer.Write($Script:Random.Next($MinValue, $MaxValue + 1));
                    }
                }
            }
        } else {
            if ($MinValue -eq $MaxValue) {
                for ($i = 0; $i -lt $Repeat; $i++) { $MinValue | Write-Output }
            } else {
                if ($MaxValue -eq [int]::MaxValue) {
                    if ($MinValue -eq [int]::MinValue) {
                        for ($i = 0; $i -lt $Repeat; $i++) {
                            if ($Script:Random.Next(0, 2) -eq 1) {
                                ($Script:Random.Next(-1, $MaxValue) + 1) | Write-Output;
                            } else {
                                $Script:Random.Next($MinValue, 0) | Write-Output;
                            }
                        }
                    } else {
                        for ($i = 0; $i -lt $Repeat; $i++) {
                            ($Script:Random.Next($MinValue - 1, [int]::MaxValue) + 1) | Write-Output;
                        }
                    }
                } else {
                    for ($i = 0; $i -lt $Repeat; $i++) { $Script:Random.Next($MinValue, $MaxValue + 1) | Write-Output }
                }
            }
        }
    }
}

Function Select-Random {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [object[]]$InputObject,

        [ValidateRange(1, 1024)]
        [int]$Count = 1
    )

    Begin { $AllObjects = @() }

    Process { $AllObjects += $InputObject }

    End {
        $Total = $AllObjects.Count;
        if ($Total -le $Count) {
            $AllObjects | Write-Output;
        } else {
            $Indexes = @();
            for ($n = 0; $n -lt $Count; $n++) {
                $i = $Script:Random.Next(0, $Total);
                while ($Indexes -contains $i) { $i = $Script:Random.Next(0, $Total) }
                $Indexes += $i;
                $AllObjects[$i] | Write-Output;
            }
        }
    }
}

Function Get-RandomString {
    [CmdletBinding(DefaultParameterSetName = "FixedLength")]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidateLength(2, [int]::MaxValue)]
        [string]$Source,

        [Parameter(Position = 1, ParameterSetName = "FixedLength")]
        [ValidateRange(1)]
        [int]$Length = 1,

        [Parameter(ParameterSetName = "RandomLength")]
        [ValidateRange(0)]
        [int]$MinLength = 1,

        [Parameter(Mandatory = $true, ParameterSetName = "RandomLength")]
        [ValidateRange(1)]
        [int]$MaxLength = 8,

        [ValidateRange(1)]
        [int]$Repeat = 1,

        [ValidateRange(0)]
        [int]$MinRepeat = 1,

        [ValidateRange(1)]
        [int]$MaxRepeat = 1,

        [string[]]$JoinWith,

        [System.IO.TextWriter]$Writer
    )
    $SourceLen = $Source.Length;
    if ((-not $PSBoundParameters.ContainsKey('Repeat')) -and ($Repeat = $MaxRepeat) -ne $MinRepeat) {
        if ($MinRepeat -gt $MaxRepeat) {
            Write-Error -Message "MinRepeat cannot be greater than the MaxRepeat" -Category InvalidArgument -ErrorId 'InvalidLengthRange' -TargetObject $MinRepeat -CategoryTargetName 'MinRepeat';
            $Repeat = 0;
        } else {
            $Repeat = Get-RandomNumber -MinValue $MinRepeat -MaxValue $MaxRepeat;
        }
    }
    if ($Repeat -gt 1) {
        if ($PSBoundParameters.ContainsKey('Length') -or ($Length = $MaxLength) -eq $MinLength) {
            if ($PSBoundParameters.ContainsKey('JoinWith')) {
                if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer = [System.IO.StringWriter]::new() }
                $Jwc = $JoinWith.Count;
                if ($Jwc -eq 1) {
                    # Repeat: >1; Length: >0; Jwc = 1
                    for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                    $c = $JoinWith[0];
                    for ($n = 1; $n -lt $Repeat; $n++) {
                        $Writer.Write($c);
                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                    }
                } else {
                    # Repeat: >1; Length: >0; Jwc = >1
                    for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                    for ($n = 1; $n -lt $Repeat; $n++) {
                        $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                    }
                }
                if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer.ToString() | Write-Output }
            } else {
                if ($PSBoundParameters.ContainsKey('Writer')) {
                    # Repeat: >1; Length: >0; Jwc = 0
                    for ($n = 0; $n -lt $Repeat; $n++) {
                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                    }
                } else {
                    if ($Length -eq 1) {
                        # Repeat: >1; Length: 1; Jwc = 0
                        for ($i = 1; $i -lt $Repeat; $i++) { $Source.Substring($Script:Random.Next(0, $SourceLen), 1) | Write-Output }
                    } else {
                        # Repeat: >1; Length: >1; Jwc = 0
                        for ($n = 0; $n -lt $Repeat; $n++) {
                            $Writer = [System.IO.StringWriter]::new();
                            for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                            $Writer.ToString() | Write-Output;
                        }
                    }
                }
            }
        } else {
            if ($MinLength -gt $MaxLength) {
                Write-Error -Message "MinLength cannot be greater than the MaxLength" -Category InvalidArgument -ErrorId 'InvalidLengthRange' -TargetObject $MinLength -CategoryTargetName 'MinLength';
            } else {
                if ($PSBoundParameters.ContainsKey('JoinWith')) {
                    if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer = [System.IO.StringWriter]::new() }
                    $Jwc = $JoinWith.Count;
                    $Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength;
                    for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                    if ($Jwc -eq 1) {
                        # Repeat: >1; MinLength:MaxLength; Jwc: 1
                        $c = $JoinWith[0];
                        for ($n = 1; $n -lt $Repeat; $n++) {
                            $Writer.Write($c);
                            $Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength;
                            for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                        }
                    } else {
                        # Repeat: >1; MinLength:MaxLength; Jwc: >1
                        for ($n = 1; $n -lt $Repeat; $n++) {
                            $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                            $Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength;
                            for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                        }
                    }
                    if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer.ToString() | Write-Output }
                } else {
                    if ($PSBoundParameters.ContainsKey('Writer')) {
                        # Repeat: >1; MinLength:MaxLength; Jwc: 0
                        for ($n = 0; $n -lt $Repeat; $n++) {
                            $Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength;
                            for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                        }
                    } else {
                        # Repeat: >1; MinLength:MaxLength; Jwc: 0
                        $Writer = [System.IO.StringWriter]::new();
                        for ($n = 0; $n -lt $Repeat; $n++) {
                            $Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength;
                            for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                        }
                        $Writer.ToString() | Write-Output;
                    }
                }
            }
        }
    } else {
        if ($Repeat -eq 1) {
            if ($PSBoundParameters.ContainsKey('Length') -or ($Length = $MaxLength) -eq $MinLength) {
                if ($Length -gt 1) {
                    # Repeat: 1; Length: >1
                    if ($PSBoundParameters.ContainsKey('Writer')) {
                        for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                    } else {
                        $Writer = [System.IO.StringWriter]::new();
                        for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                        $Writer.ToString() | Write-Output;
                    }
                } else {
                    # Repeat: 1; Length: 1
                    if ($PSBoundParameters.ContainsKey('Writer')) {
                        $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]);
                    } else {
                        $Source.Substring($Script:Random.Next(0, $SourceLen), 1) | Write-Output;
                    }
                }
            } else {
                if ($MinLength -gt $MaxLength) {
                    Write-Error -Message "MinLength cannot be greater than the MaxLength" -Category InvalidArgument -ErrorId 'InvalidLengthRange' -TargetObject $MinLength -CategoryTargetName 'MinLength';
                } else {
                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 1) {
                        # Repeat: 1; Length: >1
                        if ($PSBoundParameters.ContainsKey('Writer')) {
                            for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                        } else {
                            $Writer = [System.IO.StringWriter]::new();
                            for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]) }
                            $Writer.ToString() | Write-Output;
                        }
                    } else {
                        if ($Length -eq 1) {
                            # Repeat: 1; Length: 1
                            if ($PSBoundParameters.ContainsKey('Writer')) {
                                $Writer.Write($Source[$Script:Random.Next(0, $SourceLen)]);
                            } else {
                                $Source.Substring($Script:Random.Next(0, $SourceLen), 1) | Write-Output;
                            }
                        } else {
                            # Repeat: 1; Length: 0
                            if (-not $PSBoundParameters.ContainsKey('Writer')) { '' | Write-Output }
                        }
                    }
                }
            }
        }
    }
}

Function Get-RandomCsIdentifer {
    [CmdletBinding(DefaultParameterSetName = "FixedLength")]
    Param(
        [Parameter(Position = 1, ParameterSetName = "FixedLength")]
        [ValidateRange(1)]
        [int]$Length = 1,

        [Parameter(ParameterSetName = "RandomLength")]
        [ValidateRange(1)]
        [int]$MinLength = 1,

        [Parameter(Mandatory = $true, ParameterSetName = "RandomLength")]
        [ValidateRange(1)]
        [int]$MaxLength = 8,

        [ValidateRange(1)]
        [int]$SegmentCount = 1,

        [ValidateRange(1)]
        [int]$MinSegmentCount = 1,

        [ValidateRange(1)]
        [int]$MaxSegmentCount = 1,

        [ValidateSet('UcFirst', 'LcFirst', 'AllUpper', 'AllLower')]
        [string]$Style = 'UcFirst',

        [string[]]$JoinWith = @('.'),

        [switch]$NoNumbers,

        [System.IO.TextWriter]$Writer
    )
    if ((-not $PSBoundParameters.ContainsKey('SegmentCount')) -and ($SegmentCount = $MaxSegmentCount) -ne $MinSegmentCount) {
        if ($MinSegmentCount -gt $MaxSegmentCount) {
            Write-Error -Message "MinSegmentCount cannot be greater than the MaxSegmentCount" -Category InvalidArgument -ErrorId 'InvalidSegmentCountRange' -TargetObject $MinSegmentCount -CategoryTargetName 'MinSegmentCount';
            $SegmentCount = 0;
        } else {
            $SegmentCount = Get-RandomNumber -MinValue $MinSegmentCount -MaxValue $MaxSegmentCount;
        }
    }
    if ($SegmentCount -gt 1) {
        if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer = [System.IO.StringWriter]::new() }
        $Jwc = $JoinWith.Length;
        if ($PSBoundParameters.ContainsKey('Length') -or ($Length = $MaxLength) -eq $MinLength) {
            # SegmentCount: >1; Length: >0
            if ($Jwc -eq 1) {
                $c = $JoinWith[0];
                switch ($Style) {
                    'AllUpper' {
                        if ($NoNumbers.IsPresent) {
                            Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                            }
                        } else {
                            $Count = $Script:UcAlpha.Length;
                            $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Count)]);
                            Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Count)]);
                                Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                        }
                        break;
                    }
                    'AllLower' {
                        $Count = $Script:LcAlpha.Length;
                        if ($NoNumbers.IsPresent) {
                            Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                            }
                        } else {
                            $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                            Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                                Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                        }
                        break;
                    }
                    'LcFirst' {
                        $Count = $Script:LcAlpha.Length;
                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                        if ($NoNumbers.IsPresent) {
                            Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                                Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            }
                        } else {
                            Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                                Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                        }
                        break;
                    }
                    default {
                        $Count = $Script:UcAlpha.Length;
                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $l)]);
                        if ($NoNumbers.IsPresent) {
                            Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $l)]);
                                Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            }
                        } else {
                            Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $l)]);
                                Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                        }
                        break;
                    }
                }
            } else {
                switch ($Style) {
                    'AllUpper' {
                        if ($NoNumbers.IsPresent) {
                            Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                            }
                        } else {
                            $Count = $Script:UcAlpha.Length;
                            $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Count)]);
                            Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Count)]);
                                Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                        }
                        break;
                    }
                    'AllLower' {
                        $Count = $Script:LcAlpha.Length;
                        if ($NoNumbers.IsPresent) {
                            Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                            }
                        } else {
                            $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                            Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                                Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                        }
                        break;
                    }
                    'LcFirst' {
                        $Count = $Script:LcAlpha.Length;
                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                        if ($NoNumbers.IsPresent) {
                            Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                                Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            }
                        } else {
                            Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                                Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                        }
                        break;
                    }
                    default {
                        $Count = $Script:UcAlpha.Length;
                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $l)]);
                        if ($NoNumbers.IsPresent) {
                            Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $l)]);
                                Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            }
                        } else {
                            Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            for ($n = 1; $n -lt $SegmentCount; $n++) {
                                $Writer.Write($c);
                                $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $l)]);
                                Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                        }
                        break;
                    }
                }
            }
        } else {
            if ($MinLength -gt $MaxLength) {
                Write-Error -Message "MinLength cannot be greater than the MaxLength" -Category InvalidArgument -ErrorId 'InvalidSegmentCountRange' -TargetObject $MinLength -CategoryTargetName 'MinLength';
            } else {
                if ($Jwc -eq 1) {
                    $c = $JoinWith[0];
                    # SegmentCount: >1; MinLength:MaxLength; JWD: 1
                    switch ($Style) {
                        'AllUpper' {
                            if ($NoNumbers.IsPresent) {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($c);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                                    }
                                }
                            } else {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($c);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        'AllLower' {
                            if ($NoNumbers.IsPresent) {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($c);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                                    }
                                }
                            } else {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($c);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        'LcFirst' {
                            if ($NoNumbers.IsPresent) {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($c);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            } else {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($c);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        default {
                            if ($NoNumbers.IsPresent) {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($c);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            } else {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($c);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                } else {
                    # SegmentCount: >1; MinLength:MaxLength; JWD: >1
                    switch ($Style) {
                        'AllUpper' {
                            if ($NoNumbers.IsPresent) {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                                    }
                                }
                            } else {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        'AllLower' {
                            if ($NoNumbers.IsPresent) {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                                    }
                                }
                            } else {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        'LcFirst' {
                            if ($NoNumbers.IsPresent) {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            } else {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        default {
                            if ($NoNumbers.IsPresent) {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            } else {
                                if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                    $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($Length -gt 1) {
                                        Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $SegmentCount; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    if (($Length = Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) -gt 0) {
                                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        if ($Length -gt 1) {
                                            Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer.ToString() | Write-Output }
    } else {
        if ($SegmentCount -eq 1) {
            if ($PSBoundParameters.ContainsKey('Length') -or ($Length = $MaxLength) -eq $MinLength) {
                # SegmentCount: 1; Length: >0
                if ($Length -gt 1) {
                    if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer = [System.IO.StringWriter]::new() }
                    switch ($Style) {
                        'AllUpper' {
                            if ($NoNumbers.IsPresent) {
                                Get-RandomString -Source $Script:UcAlpha -Length $Length -Writer $Writer;
                            } else {
                                $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                                Get-RandomString -Source $Script:UcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                            break;
                        }
                        'AllLower' {
                            if ($NoNumbers.IsPresent) {
                                Get-RandomString -Source $Script:LcAlpha -Length $Length -Writer $Writer;
                            } else {
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                Get-RandomString -Source $Script:LcAlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                            break;
                        }
                        'LcFirst' {
                            $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                            if ($NoNumbers.IsPresent) {
                                $Count = $Script:AlphaChars.Length;
                                for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Script:AlphaChars[$Script:Random.Next(0, $Count)]) }
                            } else {
                                Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                            break;
                        }
                        default {
                            $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                            if ($NoNumbers.IsPresent) {
                                Get-RandomString -Source $Script:AlphaChars -Length ($Length - 1) -Writer $Writer;
                            } else {
                                Get-RandomString -Source $Script:AlphaNum -Length ($Length - 1) -Writer $Writer;
                            }
                            break;
                        }
                    }
                    if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer.ToString() | Write-Output }
                } else {
                    if ($PSBoundParameters.ContainsKey('Writer')) {
                        switch ($Style) {
                            'AllLower' {
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                break;
                            }
                            'LcFirst' {
                                $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                break;
                            }
                            default {
                                $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                                break;
                            }
                        }
                    } else {
                        switch ($Style) {
                            'AllLower' {
                                $Script:LcAlpha.Substring($Script:Random.Next(0, $Script:LcAlpha.Length), 1) | Write-Output;
                                break;
                            }
                            'LcFirst' {
                                $Script:LcAlpha.Substring($Script:Random.Next(0, $Script:LcAlpha.Length), 1) | Write-Output;
                                break;
                            }
                            default {
                                $Script:UcAlpha.Substring($Script:Random.Next(0, $Script:UcAlpha.Length), 1) | Write-Output;
                                break;
                            }
                        }
                    }
                }
            } else {
                if ($MinLength -gt $MaxLength) {
                    Write-Error -Message "MinLength cannot be greater than the MaxLength" -Category InvalidArgument -ErrorId 'InvalidSegmentCountRange' -TargetObject $MinLength -CategoryTargetName 'MinLength';
                } else {
                    switch (Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength) {
                        0 {
                            # SegmentCount: 1; Length: 0
                            if (-not $PSBoundParameters.ContainsKey('Writer')) { '' | Write-Output }
                            break;
                        }
                        1 {
                            # SegmentCount: 1; Length: 1
                            if ($PSBoundParameters.ContainsKey('Writer')) {
                                switch ($Style) {
                                    'AllLower' {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        break;
                                    }
                                    'LcFirst' {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                        break;
                                    }
                                    default {
                                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                                        break;
                                    }
                                }
                            } else {
                                switch ($Style) {
                                    'AllLower' {
                                        $Script:LcAlpha.Substring($Script:Random.Next(0, $Script:LcAlpha.Length), 1) | Write-Output;
                                        break;
                                    }
                                    'LcFirst' {
                                        $Script:LcAlpha.Substring($Script:Random.Next(0, $Script:LcAlpha.Length), 1) | Write-Output;
                                        break;
                                    }
                                    default {
                                        $Script:UcAlpha.Substring($Script:Random.Next(0, $Script:UcAlpha.Length), 1) | Write-Output;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                        default {
                            # SegmentCount: 1; Length: >1
                            if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer = [System.IO.StringWriter]::new() }
                            switch ($Style) {
                                'AllUpper' {
                                    $Count = $Script:UcAlpha.Length;
                                    if ($NoNumbers.IsPresent) {
                                        for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Count)]) }
                                    } else {
                                        $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Count)]);
                                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Script:UcAlphaNum[$Script:Random.Next(0, $Count)]) }
                                    }
                                    break;
                                }
                                'AllLower' {
                                    $Count = $Script:LcAlpha.Length;
                                    if ($NoNumbers.IsPresent) {
                                        for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]) }
                                    } else {
                                        $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Count)]);
                                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Script:LcAlphaNum[$Script:Random.Next(0, $Count)]) }
                                    }
                                    break;
                                }
                                'LcFirst' {
                                    $Writer.Write($Script:LcAlpha[$Script:Random.Next(0, $Script:LcAlpha.Length)]);
                                    if ($NoNumbers.IsPresent) {
                                        $Count = $Script:AlphaChars.Length;
                                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Script:AlphaChars[$Script:Random.Next(0, $Count)]) }
                                    } else {
                                        $Count = $Script:AlphaNum.Length;
                                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Script:AlphaNum[$Script:Random.Next(0, $Count)]) }
                                    }
                                    break;
                                }
                                default {
                                    $Writer.Write($Script:UcAlpha[$Script:Random.Next(0, $Script:UcAlpha.Length)]);
                                    if ($NoNumbers.IsPresent) {
                                        $Count = $Script:AlphaChars.Length;
                                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Script:AlphaChars[$Script:Random.Next(0, $Count)]) }
                                    } else {
                                        $Count = $Script:AlphaNum.Length;
                                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write($Script:AlphaNum[$Script:Random.Next(0, $Count)]) }
                                    }
                                    break;
                                }
                            }
                            if (-not $PSBoundParameters.ContainsKey('Writer')) { $Writer.ToString() | Write-Output }
                            break;
                        }
                    }
                }
            }
        } else {
            if ($MinLength -gt $MaxLength) {
                Write-Error -Message "MinLength cannot be greater than the MaxLength" -Category InvalidArgument -ErrorId 'InvalidSegmentCountRange' -TargetObject $MinLength -CategoryTargetName 'MinLength';
            }
        }
    }
}

Function Get-RandomIpAddress {
    [CmdletBinding()]
    Param(
        [ValidateRange(1, 32768)]
        [int]$Count = 1,

        [System.IO.TextWriter]$Writer
    )
    if ($PSBoundParameters.ContainsKey('Writer')) {
        Get-RandomNumber -MinValue 0 -MaxValue 256 -Writer $Writer;
        for ($i = 0; $i -lt 3; $i++) {
            $Writer.Write('.');
            Get-RandomNumber -MinValue 0 -MaxValue 256 -Writer $Writer;
        }
        for ($i = 1; $i -lt $Count; $i++) {
            $Writer.Write(";");
            Get-RandomNumber -MinValue 0 -MaxValue 256 -Writer $Writer;
            for ($i = 0; $i -lt 3; $i++) {
                $Writer.Write('.');
                Get-RandomNumber -MinValue 0 -MaxValue 256 -Writer $Writer;
            }
        }
    } else {
        if ($Count -gt 1) {
            $Value = "$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256)";
            $Value | Write-Output;
            $Emitted = @($Value);
            for ($i = 1; $i -lt $Count; $i++) {
                $Value = "$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256)";
                while ($Emitted -contains $Value) {
                    $Value = "$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256)";
                }
                $Value | Write-Output;
                $Emitted += $Value;
            }
        } else {
            "$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256).$(Get-RandomNumber -MinValue 0 -MaxValue 256)" | Write-Output;
        }
    }
}

Function Get-RandomUrl {
    [CmdletBinding(DefaultParameterSetName = 'Relative')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'AnyHttp')]
        [switch]$AnyHttp,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'AnyAbsolute')]
        [switch]$AnyAbsolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'http')]
        [switch]$Http,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'https')]
        [switch]$Https,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'file')]
        [switch]$File,
        
        [Parameter(ParameterSetName = 'Relative')]
        [switch]$Relative,
        
        [Parameter(ParameterSetName = 'AnyHttp')]
        [Parameter(ParameterSetName = 'http')]
        [Parameter(ParameterSetName = 'https')]
        [ValidateSet("None", "SingleChar", "DoubleChar", "MultiChar", "Random", "Any")]
        [string]$UserType = "Any",

        [Parameter(ParameterSetName = 'AnyHttp')]
        [Parameter(ParameterSetName = 'http')]
        [Parameter(ParameterSetName = 'https')]
        [ValidateSet("None", "SingleChar", "DoubleChar", "MultiChar", "Empty", "Any", "Random")]
        [string]$PwType = "Any",

        [Parameter(ParameterSetName = 'AnyHttp')]
        [Parameter(ParameterSetName = 'http')]
        [Parameter(ParameterSetName = 'https')]
        [Parameter(ParameterSetName = 'file')]
        [ValidateSet("Any", "RandomFqdn", "RandomIpAddress", "RandomHostName", "Random1Character", "Random2Character", "RandomMultiCharHostName")]
        [string]$HostType = "Any",

        [Parameter(Mandatory = $true, ParameterSetName = 'LocalFile')]
        [switch]$LocalFile,

        [Parameter(ParameterSetName = 'AnyHttp')]
        [Parameter(ParameterSetName = 'http')]
        [Parameter(ParameterSetName = 'https')]
        [ValidateSet("Any", "Random", "Standard", "Explicit", "RandomNonStandard", "OppositeProtocol")]
        [string]$PortType = "Any",

        [ValidateRange(0)]
        [int[]]$PathSegmentLength = (1, 4),
        
        [ValidateRange(0)]
        [int[]]$PathSegmentCount = (0, 3),
        
        [ValidateRange(0)]
        [int[]]$ExtensionLength = (1, 4),
        
        [ValidateRange(0)]
        [int[]]$ExtensionCount = (0, 3),
        
        [ValidateSet("Random", 'Always', 'Never')]
        [string]$TrailingSlash = "Random",

        [Parameter(ParameterSetName = 'Relative')]
        [ValidateSet("Random", 'Always', 'Never')]
        [string]$LeadingSlash = "Random",
        
        [ValidatePattern('^(\d+(-\d+)?(=(\d+(-\d+)?)?\??)?|=(\d+(-\d+)?)?\??)$')]
        [string]$QueryItemPattern = '0-8=0-8',

        [Parameter(ParameterSetName = 'Relative')]
        [Parameter(ParameterSetName = 'Rooted')]
        [Parameter(ParameterSetName = 'AnyHttp')]
        [Parameter(ParameterSetName = 'http')]
        [Parameter(ParameterSetName = 'https')]
        [ValidateRange(0)]
        [int[]]$QueryCount = (0, 3),
        
        [Parameter(ParameterSetName = 'Relative')]
        [Parameter(ParameterSetName = 'Rooted')]
        [Parameter(ParameterSetName = 'AnyHttp')]
        [Parameter(ParameterSetName = 'http')]
        [Parameter(ParameterSetName = 'https')]
        [ValidateRange(0)]
        [int[]]$FragmentLength = (1, 4),
        
        [ValidateRange(1, 32768)]
        [int]$Count = 1
    )

    for ($c = 0; $c -lt $Count; $c++) {
        $Writer = [System.IO.StringWriter]::new();
        $IsHttp = $false;
        $NotFile = $true;
        $IsAbsolute = $false;
        if ($AnyHttp.IsPresent) {
            $IsHttp = $IsAbsolute = $true;
            if ($Script:Random.Next(0, 2) -eq 1) {$Writer.Write('http://') } else { $Writer.Write('https://') }
            $Writer.Write("$Scheme`://");
        } else {
            if ($Http.IsPresent) {
                $IsHttp = $IsAbsolute = $true;
                $Writer.Write('http://');
            } else {
                if ($Https.IsPresent) {
                    $IsHttp = $IsAbsolute = $true;
                    $Writer.Write('https://');
                } else {
                    if ($File.IsPresent -or $LocalFile.IsPresent) {
                        $NotFile = $false;
                        $IsAbsolute = $true;
                        $Writer.Write('file://');
                    } else {
                        if ($AnyAbsolute.IsPresent) {
                            switch ($Script:Random.Next(0, 3)) {
                                0 {
                                    $IsHttp = $IsAbsolute = $true;
                                    $Writer.Write('http://');
                                    break;
                                }
                                1 {
                                    $IsHttp = $IsAbsolute = $true;
                                    $Writer.Write('https://');
                                    break;
                                }
                                default {
                                    $NotFile = $false;
                                    $IsAbsolute = $true;
                                    $Writer.Write('file://');
                                    break;
                                }
                            }
                        } else {
                            if (-not ($Relative.IsPresent -or $LeadingSlash.IsPresent)) {
                                switch ($Script:Random.Next(0, 4)) {
                                    0 {
                                        $IsHttp = $IsAbsolute = $true;
                                        $Writer.Write('http://');
                                        break;
                                    }
                                    1 {
                                        $IsHttp = $IsAbsolute = $true;
                                        $Writer.Write('https://');
                                        break;
                                    }
                                    2 {
                                        $NotFile = $false;
                                        $IsAbsolute = $true;
                                        $Writer.Write('file://');
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if ($IsHttp) {
            switch ($UserType) {
                "SingleChar" {
                    Get-RandomCsIdentifer -Length 1 -Style AllLower -Writer $Writer;
                    switch ($PwType) {
                        "SingleChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -Length 1 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "DoubleChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -Length 2 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "MultiChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -MinLength 3 -MaxLength 8 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "Any" {
                            $Length = Get-RandomNumber -MinValue -7 -MaxValue 7;
                            if ($Length -gt -1) {
                                $Writer.Write(':');
                                if ($Length -gt 0) {
                                    Get-RandomString -Source $Script:AlphaNum -Length $Length -Writer $Writer;
                                }
                            }
                            break;
                        }
                        "Empty" {
                            $Writer.Write(':');
                            break;
                        }
                    }
                    $Writer.Write('@');
                }
                "DoubleChar" {
                    Get-RandomCsIdentifer -Length 2 -Style AllLower -Writer $Writer;
                    switch ($PwType) {
                        "SingleChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -Length 1 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "DoubleChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -Length 2 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "MultiChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -MinLength 3 -MaxLength 8 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "Any" {
                            $Length = Get-RandomNumber -MinValue -7 -MaxValue 7;
                            if ($Length -gt -1) {
                                $Writer.Write(':');
                                if ($Length -gt 0) {
                                    Get-RandomString -Source $Script:AlphaNum -Length $Length -Writer $Writer;
                                }
                            }
                            break;
                        }
                        "Empty" {
                            $Writer.Write(':');
                            break;
                        }
                    }
                    $Writer.Write('@');
                    break;
                }
                "MultiChar" {
                    Get-RandomCsIdentifer -MinLength 3 -MaxLength 8 -Style AllLower -Writer $Writer;
                    switch ($PwType) {
                        "SingleChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -Length 1 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "DoubleChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -Length 2 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "MultiChar" {
                            $Writer.Write(':');
                            Get-RandomCsIdentifer -MinLength 3 -MaxLength 8 -Style AllLower -Writer $Writer;
                            break;
                        }
                        "Any" {
                            $Length = Get-RandomNumber -MinValue -7 -MaxValue 7;
                            if ($Length -gt -1) {
                                $Writer.Write(':');
                                if ($Length -gt 0) {
                                    Get-RandomString -Source $Script:AlphaNum -Length $Length -Writer $Writer;
                                }
                            }
                            break;
                        }
                        "Empty" {
                            $Writer.Write(':');
                            break;
                        }
                    }
                    $Writer.Write('@');
                    break;
                }
                "Any" {
                    $Length = Get-RandomNumber -MinValue -7 -MaxValue 7;
                    if ($Length -gt 0) {
                        Get-RandomCsIdentifer -Length $Length -Style AllLower -Writer $Writer;
                        switch ($PwType) {
                            "SingleChar" {
                                $Writer.Write(':');
                                Get-RandomCsIdentifer -Length 1 -Style AllLower -Writer $Writer;
                                break;
                            }
                            "DoubleChar" {
                                $Writer.Write(':');
                                Get-RandomCsIdentifer -Length 2 -Style AllLower -Writer $Writer;
                                break;
                            }
                            "MultiChar" {
                                $Writer.Write(':');
                                Get-RandomCsIdentifer -MinLength 3 -MaxLength 8 -Style AllLower -Writer $Writer;
                                break;
                            }
                            "Any" {
                                $Length = Get-RandomNumber -MinValue -7 -MaxValue 7;
                                if ($Length -gt -1) {
                                    $Writer.Write(':');
                                    if ($Length -gt 0) {
                                        Get-RandomString -Source $Script:AlphaNum -Length $Length -Writer $Writer;
                                    }
                                }
                                break;
                            }
                            "Empty" {
                                $Writer.Write(':');
                                break;
                            }
                        }
                        $Writer.Write('@');
                    }
                    break;
                }
            }
            switch ($HostType) {
                "RandomFqdn" {
                    Get-RandomCsIdentifer -MaxLength 8 -MinSegmentCount 2 -MaxSegmentCount 5 -Style AllLower -JoinWith '.' -Writer $Writer;
                    break;
                }
                "RandomIpAddress" {
                    Get-RandomIpAddress -Writer $Writer;
                    break;
                }
                "RandomHostName" {
                    Get-RandomCsIdentifer -MaxLength 8 -Style AllLower -Writer $Writer;
                    break;
                }
                "Random1Character" {
                    Get-RandomCsIdentifer -Length 1 -Style AllLower -Writer $Writer;
                    break;
                }
                "Random2Character" {
                    Get-RandomCsIdentifer -Length 2 -Style AllLower -Writer $Writer;
                    break;
                }
                "RandomMultiCharHostName" {
                    Get-RandomCsIdentifer -MinLength 3 -MaxLength 8 -Style AllLower -Writer $Writer;
                    break;
                }
                default {
                    $Length = Get-RandomNumber -MinValue 0 -MaxValue 5;
                    if ($Length -eq 0) {
                        Get-RandomIpAddress -Writer $Writer;
                    } else {
                        Get-RandomCsIdentifer -MaxLength 8 -MaxSegmentCount 5 -Style AllLower -JoinWith '.' -Writer $Writer;
                    }
                    break;
                }
            }
        } else {
            if ($File.IsPresent) {
                switch ($HostType) {
                    "RandomFqdn" {
                        Get-RandomCsIdentifer -MaxLength 8 -MinSegmentCount 2 -MaxSegmentCount 5 -Style AllLower -JoinWith '.' -Writer $Writer;
                        break;
                    }
                    "RandomIpAddress" {
                        Get-RandomIpAddress -Writer $Writer;
                        break;
                    }
                    "RandomHostName" {
                        Get-RandomCsIdentifer -MaxLength 8 -Style AllLower -Writer $Writer;
                        break;
                    }
                    "Random1Character" {
                        Get-RandomCsIdentifer -Length 1 -Style AllLower -Writer $Writer;
                        break;
                    }
                    "Random2Character" {
                        Get-RandomCsIdentifer -Length 2 -Style AllLower -Writer $Writer;
                        break;
                    }
                    "RandomMultiCharHostName" {
                        Get-RandomCsIdentifer -MinLength 3 -MaxLength 8 -Style AllLower -Writer $Writer;
                        break;
                    }
                    default {
                        $Length = Get-RandomNumber -MinValue -5 -MaxValue 5;
                        if ($Length -eq 0) {
                            Get-RandomIpAddress -Writer $Writer;
                        } else {
                            if ($Length -gt 0) {
                                Get-RandomCsIdentifer -MaxLength 8 -MaxSegmentCount 5 -Style AllLower -JoinWith '.' -Writer $Writer;
                            }
                        }
                        break;
                    }
                }
            }
        }
        if ($Http) {
            switch ($PortType) {
                "Any" {
                    $Value = $Script:Random.Next(8080, 8104);
                    switch ($Value) {
                        8090 {
                            $Writer.Write(":");
                            $Writer.Write(80);
                            break;
                        }
                        8091 {
                            $Writer.Write(":");
                            $Writer.Write(443);
                            break;
                        }
                        default {
                            if ($Value -lt 8090) {
                                $Writer.Write(":");
                                $Writer.Write($Value);
                            }
                            break;
                        }
                    }
                    break;
                }
                "RandomNonStandard" {
                    $Writer.Write(":");
                    Get-RandomNumber -MinValue 8080 -MaxValue 8090 -Writer $Writer;
                    break;
                }
                "Explicit" {
                    $Writer.Write(":");
                    if ($Https.IsPresent) { $Writer.Write(443) } else { $Writer.Write(80) }
                    break;
                }
                "OppositeProtocol" {
                    $Writer.Write(":");
                    if ($Https.IsPresent) { $Writer.Write(80) } else { $Writer.Write(443) }
                    break;
                }
            }
        }
        
        $Length = (Convert-RangeValuesToTuple -Values $PathSegmentCount) | Get-RandomNumber;
        if ($Length -gt 0) {
            if ($IsAbsolute) {
                $Writer.Write('/');
            } else {
                switch ($LeadingSlash) {
                    'Always' {
                        $Writer.Write('/');
                        break;
                    }
                    'Random' {
                        if ($Script:Random.Next(0, 2) -eq 1) {
                            $Writer.Write('/');
                        }
                        break;
                    }
                }
            }
            $Range = Convert-RangeValuesToTuple -Values $PathSegmentLength;
            Get-RandomString -Source $Script:AlphaNum -MinLength $Range.Item1 -MaxLength $Range.Item2 -Repeat $Length -Writer $Writer -JoinWith '/';
            $Length = (Convert-RangeValuesToTuple -Values $ExtensionCount) | Get-RandomNumber;
            if ($Length -gt 0) {
                $Range = Convert-RangeValuesToTuple -Values $ExtensionLength;
                if ($Range.Item2 -eq 0) {
                    for ($i = 0; $i -lt $Length; $i++) { $Writer.Write('.') }
                } else {
                    $Writer.Write('.');
                    Get-RandomString -Source $Script:AlphaNum -MinLength $Range.Item1 -MaxLength $Range.Item2 -Repeat $Length -Writer $Writer -JoinWith '.';
                }
            }
        } else {
            switch ($TrailingSlash) {
                'Always' {
                    $Writer.Write('/');
                    break;
                }
                'Random' {
                    if (((-not $IsAbsolute) -and $LeadingSlash -eq 'Always') -or $Script:Random.Next(0, 2) -eq 1) {
                        $Writer.Write('/');
                    }
                    break;
                }
                default {
                    if ((-not $IsAbsolute) -and $LeadingSlash) { $Writer.Write('/') }
                    break;
                }
            }
        }
        if ($IsHttp) {
            $Length = (Convert-RangeValuesToTuple -Values $QueryCount) | Get-RandomNumber;
            if ($Length -gt 0) {
                $Writer.Write('?');
                $Kvp = $QueryItemPattern.Split('=');
                if ($Kvp.Length -eq 1) {
                    $ValueRange = Convert-RangePatternToTuple -Pattern $Kvp[0];
                    if ($ValueRange.Item2 -eq 0) {
                        for ($i = 1; $i -lt $Length; $i++) { $Writer.Write('&'); }
                    }
                } else {
                    $RequireValue = $true;
                    $KeyRange = [System.ValueTuple]::Create(0, 0);
                    $ValueRange = [System.ValueTuple]::Create(0, 0);
                    if ($Kvp[0].Length -gt 0) {
                        $KeyRange = Convert-RangePatternToTuple -Pattern $Kvp[0];
                    }
                    if ($Kvp[1].Length -gt 0) {
                        if ($Kvp[1] -eq '?') {
                            $RequireValue = $false;
                        } else {
                            if ($Kvp[1].EndsWith('?')) {
                                $RequireValue = $false;
                                $ValueRange = Convert-RangePatternToTuple -Pattern $Kvp[1].Substring(0, $Kvp[1].Length - 1);
                            } else {
                                $ValueRange = Convert-RangePatternToTuple -Pattern $Kvp[1];
                            }
                        }
                    }
                    if ($KeyRange.Item2 -eq 0) {
                        if ($ValueRange.Item2 -eq 0) {
                            $Writer.Write('=');
                            for ($i = 1; $i -lt $Length; $i++) { $Writer.Write('&=') }
                        } else {
                            if ($ValueRange.Item1 -eq $ValueRange.Item2) {
                                $len = $ValueRange.Item1;
                                if ($RequireValue -or $Script:Random.Next(0, 3) -eq 1) {
                                    Get-RandomString -Source $Script:AlphaNum -Length $len -Writer $Writer;
                                }
                                for ($i = 1; $i -lt $Length; $i++) {
                                    $Writer.Write('&=');
                                    if ($RequireValue -or $Script:Random.Next(0, 3) -eq 1) {
                                        Get-RandomString -Source $Script:AlphaNum -Length $len -Writer $Writer;
                                    }
                                }
                            } else {
                                $Writer.Write('=');
                                if ($RequireValue -or $Script:Random.Next(0, 3) -eq 1) {
                                    $len = $ValueRange | Get-RandomNumber;
                                    if ($len -gt 0) {
                                        Get-RandomString -Source $Script:AlphaNum -Length $len -Writer $Writer;
                                    }
                                }
                                for ($i = 1; $i -lt $Length; $i++) {
                                    $Writer.Write('&=');
                                    if ($RequireValue -or $Script:Random.Next(0, 3) -eq 1) {
                                        $len = $ValueRange | Get-RandomNumber;
                                        if ($len -gt 0) {
                                            Get-RandomString -Source $Script:AlphaNum -Length $len -Writer $Writer;
                                        }
                                    }
                                }
                            }
                        }
                    } else {
                        if ($KeyRange.Item1 -eq $KeyRange.Item2) {
                            $k = $KeyRange.Item1;
                        } else {
                            $k = $KeyRange | Get-RandomNumber;
                            if ($k -gt 0) {
                                Get-RandomString -Source $Script:AlphaNum -Length $k -Writer $Writer;
                            }
                            $Writer.Write('=');
                            if ($ValueRange.Item2 -eq 0) {
                                for ($i = 1; $i -lt $Length; $i++) {
                                    $Writer.Write('&');
                                    $k = $KeyRange | Get-RandomNumber;
                                    if ($k -gt 0) {
                                        Get-RandomString -Source $Script:AlphaNum -Length $k -Writer $Writer;
                                    }
                                    $Writer.Write('=');
                                }
                            } else {
                                if ($ValueRange.Item1 -eq $ValueRange.Item2) {
                                    $len = $ValueRange.Item1;
                                    if ($RequireValue -or $Script:Random.Next(0, 3) -eq 1) {
                                        Get-RandomString -Source $Script:AlphaNum -Length $len -Writer $Writer;
                                    }
                                    for ($i = 1; $i -lt $Length; $i++) {
                                        $Writer.Write('&');
                                        $k = $KeyRange | Get-RandomNumber;
                                        if ($k -gt 0) {
                                            Get-RandomString -Source $Script:AlphaNum -Length $k -Writer $Writer;
                                        }
                                        $Writer.Write('=');
                                        if ($RequireValue -or $Script:Random.Next(0, 3) -eq 1) {
                                            Get-RandomString -Source $Script:AlphaNum -Length $len -Writer $Writer;
                                        }
                                    }
                                } else {
                                    if ($RequireValue -or $Script:Random.Next(0, 3) -eq 1) {
                                        $len = $ValueRange | Get-RandomNumber;
                                        if ($len -gt 0) {
                                            Get-RandomString -Source $Script:AlphaNum -Length $len -Writer $Writer;
                                        }
                                    }
                                    for ($i = 1; $i -lt $Length; $i++) {
                                        $Writer.Write('&');
                                        $k = $KeyRange | Get-RandomNumber;
                                        if ($k -gt 0) {
                                            Get-RandomString -Source $Script:AlphaNum -Length $k -Writer $Writer;
                                        }
                                        $Writer.Write('=');
                                        if ($RequireValue -or $Script:Random.Next(0, 3) -eq 1) {
                                            $len = $ValueRange | Get-RandomNumber;
                                            if ($len -gt 0) {
                                                Get-RandomString -Source $Script:AlphaNum -Length $len -Writer $Writer;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            $Range = Convert-RangeValuesToTuple -Values $FragmentLength;
            if ($Range.Item2 -gt 0) {
                if ($Range.Item1 -eq $Range.Item2) {
                    $len = $Range.Item1;
                    $Writer.Write('#');
                    if ($len -gt 1) {
                        Get-RandomString -Source $Script:AlphaNum -Length ($len - 1) -Writer $Writer;
                    }
                } else {
                    $len = $Range | Get-RandomNumber;
                    if ($len -gt 0) {
                        $Writer.Write('#');
                        if ($len -gt 1) {
                            Get-RandomString -Source $Script:AlphaNum -Length ($len - 1) -Writer $Writer;
                        }
                    }
                }
            }
        }
        $Writer.ToString() | Write-Output;
    }
}
