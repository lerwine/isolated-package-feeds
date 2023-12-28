
if ($null -eq $Script:Random) { New-Variable -Name 'Random' -Value ([Random]::new()) -Option ReadOnly -Force }
Function Get-RandomCharacterSource {
    [CmdletBinding()]
    Param(
        [ValidateSet('LettersAndDigits', 'AsciiChars', 'UriDataChars', 'CsIdentifierChars', 'AsciiLettersAndDigits', 'Letters', 'AsciiLetters', 'UpperChars', 'LowerChars', 'Consonants', 'AsciiLettersUpper', 'AsciiLettersLower', 'Numbers', 'HardConsonants', 'SoftConsonants', 'Vowels', 'AsciiHexDigits', 'Surrogates', 'Digits',
            'ConsonantsUpper', 'ConsonantsLower', 'AsciiHexDigitsUpper', 'AsciiHexDigitsLower', 'WhiteSpaceChars', 'HighSurrogates', 'LowSurrogates', 'ControlChars', 'HardConsonantsUpper', 'HardConsonantsLower', 'SoftConsonantsUpper', 'SoftConsonantsLower', 'VowelsUpper', 'VowelsLower',
            'Separators', 'PunctuationChars', 'Symbols', 'AsciiDigits', 'AsciiControlChars', 'AsciiPunctuation', 'AsciiSymbols')]
        [string[]]$Include,

        [char[]]$ExplicitInclude,

        [ValidateSet('LettersAndDigits', 'AsciiChars', 'UriDataChars', 'CsIdentifierChars', 'AsciiLettersAndDigits', 'Letters', 'AsciiLetters', 'UpperChars', 'LowerChars', 'Consonants', 'AsciiLettersUpper', 'AsciiLettersLower', 'Numbers', 'HardConsonants', 'SoftConsonants', 'Vowels', 'AsciiHexDigits', 'Surrogates', 'Digits',
            'ConsonantsUpper', 'ConsonantsLower', 'AsciiHexDigitsUpper', 'AsciiHexDigitsLower', 'WhiteSpaceChars', 'HighSurrogates', 'LowSurrogates', 'ControlChars', 'HardConsonantsUpper', 'HardConsonantsLower', 'SoftConsonantsUpper', 'SoftConsonantsLower', 'VowelsUpper', 'VowelsLower',
            'Separators', 'PunctuationChars', 'Symbols', 'AsciiDigits', 'AsciiControlChars', 'AsciiPunctuation', 'AsciiSymbols')]
        [string[]]$Exclude,

        [char[]]$ExplicitExclude,

        [ValidateRange(0, ([int]([char]::MaxValue)) - 1)]
        [int]$MinResultCount = 2,

        [switch]$AsString
    )
    [RandomCharacterSource]::new([TestDataGeneration.CharacterGroup]::GetCharacters($Include, $Exclude, $ExplicitInclude, $ExplicitExclude)) | Write-Output;
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

    Begin {
        if ($PSCmdlet.ParameterSetName -eq 'RandomRepeat') { $Repeat = [TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, $MinRepeat, $MaxRepeat) }
    }

    Process {
        if ($Repeat -gt 0) {
            if ($PSBoundParameters.ContainsKey('Writer')) {
                [TestDataGeneration.CharacterGroup]::WriteRandomNumbers($Writer, $Script:Random, $Repeat, $MinValue, $MaxValue);
            } else {
                [TestDataGeneration.CharacterGroup]::GetRandomNumbers($Repeat, $Script:Random, $MinValue, $MaxValue) | Write-Output;
            }
        }
    }
}


class RandomCharacterSource
{
    [char[]]$Values;
    
    [char] GetNext() {
        switch ($this.Values.Length) {
            0 { return [char]0; }
            1 { return $this.Values[0]; }
        }
        return $this.Values[(Get-RandomNumber -MinValue 0 -MaxValue $this.Values.Length)];
    }
    
    [string] GetNext([int]$Length) {
        if ($Length -lt 1) { return '' };
        switch ($this.Values.Length) {
            0 { return [string]::new([char]0, $Length); }
            1 { return [string]::new($this.Values[0], $Length); }
        }
        $arr = New-Object -TypeName 'System.Char[]' -ArgumentList $Length;
        for ($i = 0; $i -lt $Length; $i++) { $arr[$i] = $this.Values[(Get-RandomNumber -MinValue 0 -MaxValue $this.Values.Length)] }
        return [string]::new($arr);
    }

    [string] GetNext([int]$MinLength, [int]$MaxLength) { return GetNext((Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength)) }
    
    [void] WriteNext([System.IO.TextWriter]$Writer) {
        switch ($this.Values.Length) {
            0 {
                $Writer.Write(([char]0));
                break;
            }
            1 {
                $Writer.Write($this.Values[0]);
                break;
            }
            default {
                $Writer.Write($this.Values[(Get-RandomNumber -MinValue 0 -MaxValue $this.Values.Length)]);
                break;
            }
        }
    }
    
    [void] WriteNext([System.IO.TextWriter]$Writer, [int]$Length) {
        if ($Length -lt 1) { return };
        switch ($this.Values.Length) {
            0 {
                [char]$c = 0;
                for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($c) }
                break;
            }
            1 {
                $c = $this.Values[0];
                for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($c) }
                break;
            }
            default {
                for ($i = 0; $i -lt $Length; $i++) { $Writer.Write($this.Values[(Get-RandomNumber -MinValue 0 -MaxValue $this.Values.Length)]) }
                break;
            }
        }
    }

    [void] WriteNext([System.IO.TextWriter]$Writer, [int]$MinLength, [int]$MaxLength) { WriteNext($Writer, (Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength)) }

    RandomCharacterSource([char[]]$Values) { $this.Values = $Values }
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
                while ($Indexes -contains $i) { $i = [TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, $Total) }
                $Indexes += $i;
                $AllObjects[$i] | Write-Output;
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
        $Writer.Write([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255));
        for ($i = 0; $i -lt 3; $i++) {
            $Writer.Write('.');
            $Writer.Write([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255));
        }
        for ($i = 1; $i -lt $Count; $i++) {
            $Writer.Write(";");
            $Writer.Write([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255));
            for ($i = 0; $i -lt 3; $i++) {
                $Writer.Write('.');
                $Writer.Write([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255));
            }
        }
    } else {
        if ($Count -gt 1) {
            $Value = "$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255))";
            $Value | Write-Output;
            $Emitted = @($Value);
            for ($i = 1; $i -lt $Count; $i++) {
                $Value = "$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255))";
                while ($Emitted -contains $Value) {
                    $Value = "$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255))";
                }
                $Value | Write-Output;
                $Emitted += $Value;
            }
        } else {
            "$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255)).$([TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, 0, 255))" | Write-Output;
        }
    }
}

Function Get-RandomString {
    [CmdletBinding(DefaultParameterSetName = "FixedRepeat")]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ $_.Length -gt 0 })]
        [RandomCharacterSource]$Source,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [Alias('Item1')]
        [int]$MinLength = 1,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [Alias('Item2')]
        [int]$MaxLength = 8,

        [Parameter(ParameterSetName = "FixedRepeat")]
        [ValidateRange(1)]
        [int]$Repeat = 1,

        [Parameter(ParameterSetName = "RandomRepeat")]
        [ValidateRange(0)]
        [int]$MinRepeat = 1,

        [Parameter(Mandatory = $true, ParameterSetName = "RandomRepeat")]
        [ValidateRange(1)]
        [int]$MaxRepeat,

        [string]$JoinWith,

        [System.IO.TextWriter]$Writer
    )
    
    Begin {
        if ($PSCmdlet.ParameterSetName -eq 'RandomRepeat') { $Repeat = [TestDataGeneration.CharacterGroup]::GetRandomNumber($Script:Random, $MinRepeat, $MaxRepeat) }
    }

    Process {
        if ($Repeat -gt 0) {
            if ($PSBoundParameters.ContainsKey('Writer')) {
                if ($PSBoundParameters.ContainsKey('JoinWith')) {
                    $Source.WriteNext($MinLength, $MaxLength);
                    for ($i = 1; $i -lt $Repeat; $i++) {
                        $Writer.Write($JoinWith);
                        $Source.WriteNext($MinLength, $MaxLength);
                    }
                } else {
                    for ($i = 0; $i -lt $Repeat; $i++) { $Source.WriteNext($MinLength, $MaxLength) }
                }
            } else {
                if ($PSBoundParameters.ContainsKey('JoinWith') -and $Repeat -gt 1) {
                    $Writer = [System.IO.StringWriter]::new();
                    $Source.WriteNext($MinLength, $MaxLength);
                    for ($i = 1; $i -lt $Repeat; $i++) {
                        $Writer.Write($JoinWith);
                        $Source.WriteNext($MinLength, $MaxLength);
                    }
                    $Writer.ToString() | Write-Output;
                } else {
                    for ($i = 0; $i -lt $Repeat; $i++) { $Source.WriteNext($MinLength, $MaxLength) }
                }
            }
        }
    }
}