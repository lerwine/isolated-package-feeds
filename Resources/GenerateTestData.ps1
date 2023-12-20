Add-Type -AssemblyName 'System.Text.Json';

if ($null -eq $Script:Random) { New-Variable -Name 'Random' -Option ReadOnly -Scope 'Script' -Value ([Random]::new()) }
if ($null -eq $Script:NumberChars) { New-Variable -Name 'NumberChars' -Option Constant -Scope 'Script' -Value '01234567890' }
if ($null -eq $Script:UcHardConsonants) { New-Variable -Name 'UcAlpha' -Option Constant -Scope 'Script' -Value 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' }
if ($null -eq $Script:UcVowels) { New-Variable -Name 'UcVowels' -Option Constant -Scope 'Script' -Value 'AEIOUY' }
if ($null -eq $Script:UcHardConsonants) { New-Variable -Name 'UcHardConsonants' -Option Constant -Scope 'Script' -Value 'BCDGJKPQTX' }
if ($null -eq $Script:UcSoftConsonants) { New-Variable -Name 'UcSoftConsonants' -Option Constant -Scope 'Script' -Value 'CFHLMNRSVWYZ' }
if ($null -eq $Script:LcAlpha) { New-Variable -Name 'LcAlpha' -Option Constant -Scope 'Script' -Value 'abcdefghijklmnopqrstuvwxyz' }
if ($null -eq $Script:LcVowels) { New-Variable -Name 'LcVowels' -Option Constant -Scope 'Script' -Value 'aeiouy' }
if ($null -eq $Script:LcHardConsonants) { New-Variable -Name 'LcHardConsonants' -Option Constant -Scope 'Script' -Value 'bcdgjkpqtx' }
if ($null -eq $Script:LcSoftConsonants) { New-Variable -Name 'LcSoftConsonants' -Option Constant -Scope 'Script' -Value 'cfhlmnrsvwyz' }
if ($null -eq $Script:AlphaChars) { New-Variable -Name 'AlphaChars' -Option Constant -Scope 'Script' -Value ($Script:UcAlpha + $Script:LcAlpha) }
if ($null -eq $Script:Vowels) { New-Variable -Name 'Vowels' -Option Constant -Scope 'Script' -Value ($Script:UcVowels + $Script:LcVowels) }
if ($null -eq $Script:HardConsonants) { New-Variable -Name 'HardConsonants' -Option Constant -Scope 'Script' -Value ($Script:UcHardConsonants + $Script:LcHardConsonants) }
if ($null -eq $Script:SoftConsonants) { New-Variable -Name 'SoftConsonants' -Option Constant -Scope 'Script' -Value ($Script:UcSoftConsonants + $Script:LcSoftConsonants) }
if ($null -eq $Script:AlphaNum) { New-Variable -Name 'AlphaNum' -Option Constant -Scope 'Script' -Value ($Script:AlphaChars + $Script:NumberChars) }
if ($null -eq $Script:UcAlphaNum) { New-Variable -Name 'UcAlphaNum' -Option Constant -Scope 'Script' -Value ($Script:UcAlpha + $Script:NumberChars) }
if ($null -eq $Script:LcAlphaNum) { New-Variable -Name 'LcAlphaNum' -Option Constant -Scope 'Script' -Value ($Script:LcAlpha + $Script:NumberChars) }

<#
.SYNOPSIS
    Generates a random number.
#>
Function Get-RandomNumber {

    [CmdletBinding(DefaultParameterSetName = "FixedRepeat")]
    Param(
        [int]$MinValue = [Int]::MinValue,

        [int]$MaxValue = [int]::MaxValue,

        [Parameter(ParameterSetName = "FixedRepeat")]
        [ValidateRange(1, [int]::MaxValue)]
        [int]$Repeat = 1,

        [Parameter(ParameterSetName = "RandomRepeat")]
        [ValidateRange(0, [int]::MaxValue)]
        [int]$MinRepeat = 1,

        [Parameter(Mandatory = $true, ParameterSetName = "RandomRepeat")]
        [ValidateRange(1, [int]::MaxValue)]
        [int]$MaxRepeat
    )
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
        if ($MinValue -eq $MaxValue) {
            for ($i = 0; $i -lt $Repeat; $i++) { $MinValue | Write-Output }
        } else {
            if ($MaxRepeat -eq [int]::MaxValue) {
                if ($MinRepeat -eq [int]::MinValue) {
                    for ($i = 0; $i -lt $Repeat; $i++) {
                        if ($Script:Random.Next(0, 2) -eq 1) {
                            ($Script:Random.Next(-1, $MaxValue) + 1) | Write-Output;
                        } else {
                            $Script:Random.Next($MinValue, 0) | Write-Output;
                        }
                    }
                } else {
                    for ($i = 0; $i -lt $Repeat; $i++) {
                        ($Script:Random.Next($MinRepeat - 1, [int]::MaxValue) + 1) | Write-Output;
                    }
                }
            } else {
                for ($i = 0; $i -lt $Repeat; $i++) { $Script:Random.Next($MinRepeat, $MaxRepeat + 1) | Write-Output }
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
        [int]$Count
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
        [string]$Source,

        [Parameter(Position = 1, ParameterSetName = "FixedLength")]
        [ValidateRange(1, [int]::MaxValue)]
        [int]$Length = 1,

        [Parameter(ParameterSetName = "RandomLength")]
        [ValidateRange(0, [int]::MaxValue)]
        [int]$MinLength = 1,

        [Parameter(Mandatory = $true, ParameterSetName = "RandomLength")]
        [ValidateRange(1, [int]::MaxValue)]
        [int]$MaxLength = 8,

        [ValidateRange(1, [int]::MaxValue)]
        [int]$Repeat = 1,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$MinRepeat = 1,

        [ValidateRange(1, [int]::MaxValue)]
        [int]$MaxRepeat = 1,

        [string[]]$JoinWith
    )

    if (-not $PSBoundParameters.ContainsKey('Repeat')) {
        if ($PSBoundParameters.ContainsKey('MaxRepeat')) {
            if ($MinRepeat -gt $MaxRepeat) {
                Write-Error -Message "MinRepeat cannot be greater than MaxRepeat" -Category InvalidArgument -ErrorId 'InvalidRepeatRange';
                $Repeat = 0;
            } else {
                if ($MinRepeat -eq $MaxRepeat) {
                    $Repeat = $MinRepeat;
                } else {
                    if (($Repeat = $Script:Random.Next($MinRepeat, $MaxRepeat)) -eq 0 -and $PSCmdlet.ParameterSetName -eq 'RandomLength' -and $MinLength -gt $MaxLength) {
                        Write-Error -Message "MinLength cannot be greater than MaxLength" -Category InvalidArgument -ErrorId 'InvalidLengthRange';
                    }
                }
            }
        }
    }
    $SourceLen = $Soure.Length;
    if ($Repeat -eq 1) {
        if ($PSBoundParameters.ContainsKey('Length')) {
            if ($Length -eq 1) {
                $Source.Substring($Random.Next(0, $SourceLen), 1) | Write-Output;
            } else {
                $Writer = [System.IO.StringWriter]::new();
                for ($n = 0; $n -lt $Length; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                $Writer.ToString() | Write-Output;
            }
        } else {
            if ($MinLength -gt $MaxLength) {
                Write-Error -Message "MinLength cannot be greater than MaxLength" -Category InvalidArgument -ErrorId 'InvalidLengthRange';
            } else {
                if ($MinLength -eq $MaxLength) {
                    if ($MaxLength -eq 1) {
                        $Source.Substring($Random.Next(0, $SourceLen), 1) | Write-Output;
                    } else {
                        $Writer = [System.IO.StringWriter]::new();
                        for ($n = 0; $n -lt $MaxLength; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                        $Writer.ToString() | Write-Output;
                    }
                } else {
                    switch ((Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength -Repeat $Repeat)) {
                        0 { '' | Write-Output; break; }
                        1 { $Source.Substring($Random.Next(0, $SourceLen), 1) | Write-Output; break; }
                        default {
                            $Writer = [System.IO.StringWriter]::new();
                            for ($n = 0; $n -lt $_; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                            $Writer.ToString() | Write-Output;
                            break;
                        }
                    }
                }
            }
        }
    } else {
        if ($Repeat -gt 1) {
            if ($PSBoundParameters.ContainsKey('JoinWith')) {
                $Writer = [System.IO.StringWriter]::new();
                $Cwl = $JoinWith.Length;
                if ($Cwl -eq 1) {
                    $c = $JoinWith[0];
                    if ($PSBoundParameters.ContainsKey('Length')) {
                        if ($Length -eq 1) {
                            $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                            for ($i = 1; $i -lt $Repeat; $i++) {
                                $Writer.Write($c);
                                $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                            }
                        } else {
                            for ($n = 0; $n -lt $Length; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                            for ($i = 1; $i -lt $Repeat; $i++) {
                                $Writer.Write($c);
                                $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                            }
                        }
                    } else {
                        if ($MinLength -gt $MaxLength) {
                            Write-Error -Message "MinLength cannot be greater than MaxLength" -Category InvalidArgument -ErrorId 'InvalidLengthRange';
                        } else {
                            if ($MinLength -eq $MaxLength) {
                                if ($MaxLength -eq 1) {
                                    $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                                    for ($i = 1; $i -lt $Repeat; $i++) {
                                        $Writer.Write($c);
                                        $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                                    }
                                } else {
                                    for ($n = 0; $n -lt $MaxLength; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                    for ($i = 1; $i -lt $Repeat; $i++) {
                                        $Writer.Write($c);
                                        for ($n = 0; $n -lt $MaxLength; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                    }
                                }
                            } else {
                                switch ((Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength -Repeat $Repeat)) {
                                    0 { break; }
                                    1 { $Writer.Write($Source[$Random.Next(0, $SourceLen)]); break; }
                                    default {
                                        for ($n = 0; $n -lt $_; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                        break;
                                    }
                                }
                                for ($i = 1; $i -lt $Repeat; $i++) {
                                    $Writer.Write($c);
                                    switch ((Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength -Repeat $Repeat)) {
                                        0 { break; }
                                        1 { $Writer.Write($Source[$Random.Next(0, $SourceLen)]); break; }
                                        default {
                                            for ($n = 0; $n -lt $_; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if ($PSBoundParameters.ContainsKey('Length')) {
                        if ($Length -eq 1) {
                            $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                            for ($i = 1; $i -lt $Repeat; $i++) {
                                $Writer.Write($JoinWith[$Random.Next(0, $Cwl)]);
                                $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                            }
                        } else {
                            for ($n = 0; $n -lt $Length; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                            for ($i = 1; $i -lt $Repeat; $i++) {
                                $Writer.Write($JoinWith[$Random.Next(0, $Cwl)]);
                                $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                            }
                        }
                    } else {
                        if ($MinLength -gt $MaxLength) {
                            Write-Error -Message "MinLength cannot be greater than MaxLength" -Category InvalidArgument -ErrorId 'InvalidLengthRange';
                        } else {
                            if ($MinLength -eq $MaxLength) {
                                if ($MaxLength -eq 1) {
                                    $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                                    for ($i = 1; $i -lt $Repeat; $i++) {
                                        $Writer.Write($JoinWith[$Random.Next(0, $Cwl)]);
                                        $Writer.Write($Source[$Random.Next(0, $SourceLen)]);
                                    }
                                } else {
                                    for ($n = 0; $n -lt $MaxLength; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                    for ($i = 1; $i -lt $Repeat; $i++) {
                                        $Writer.Write($JoinWith[$Random.Next(0, $Cwl)]);
                                        for ($n = 0; $n -lt $MaxLength; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                    }
                                }
                            } else {
                                switch ((Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength -Repeat $Repeat)) {
                                    0 { break; }
                                    1 { $Writer.Write($Source[$Random.Next(0, $SourceLen)]); break; }
                                    default {
                                        for ($n = 0; $n -lt $_; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                        break;
                                    }
                                }
                                for ($i = 1; $i -lt $Repeat; $i++) {
                                    $Writer.Write($JoinWith[$Random.Next(0, $Cwl)]);
                                    switch ((Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength -Repeat $Repeat)) {
                                        0 { break; }
                                        1 { $Writer.Write($Source[$Random.Next(0, $SourceLen)]); break; }
                                        default {
                                            for ($n = 0; $n -lt $_; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                $Writer.ToString() | Write-Output;
            } else {
                if ($PSBoundParameters.ContainsKey('Length')) {
                    if ($Length -eq 1) {
                        for ($i = 0; $i -lt $Repeat; $i++) { $Source.Substring($Random.Next(0, $SourceLen), 1) | Write-Output }
                    } else {
                        for ($i = 0; $i -lt $Repeat; $i++) {
                            $Writer = [System.IO.StringWriter]::new();
                            for ($n = 0; $n -lt $Length; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                            $Writer.ToString() | Write-Output;
                        }
                    }
                } else {
                    if ($PSCmdlet.ParameterSetName -eq 'RandomLength' -and $MinLength -gt $MaxLength) {
                        Write-Error -Message "MinLength cannot be greater than MaxLength" -Category InvalidArgument -ErrorId 'InvalidLengthRange';
                    } else {
                        if ($MinLength -eq $MaxLength) {
                            if ($MaxLength -eq 1) {
                                for ($i = 0; $i -lt $Repeat; $i++) { $Source.Substring($Random.Next(0, $SourceLen), 1) | Write-Output }
                            } else {
                                for ($i = 0; $i -lt $Repeat; $i++) {
                                    $Writer = [System.IO.StringWriter]::new();
                                    for ($n = 0; $n -lt $MaxLength; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                    $Writer.ToString() | Write-Output;
                                }
                            }
                        } else {
                            (Get-RandomNumber -MinValue $MinLength -MaxValue $MaxLength -Repeat $Repeat) | ForEach-Object {
                                switch ($_) {
                                    0 { '' | Write-Output; break; }
                                    1 { $Source.Substring($Random.Next(0, $SourceLen), 1) | Write-Output; break; }
                                    default {
                                        $Writer = [System.IO.StringWriter]::new();
                                        for ($n = 0; $n -lt $_; $n++) { $Writer.Write($Source[$Random.Next(0, $SourceLen)]) }
                                        $Writer.ToString() | Write-Output;
                                        break;
                                    }
                                }
                            }
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
        [ValidateRange(1, [int]::MaxValue)]
        [int]$Length = 1,

        [Parameter(ParameterSetName = "RandomLength")]
        [ValidateRange(1, [int]::MaxValue)]
        [int]$MinLength = 1,

        [Parameter(Mandatory = $true, ParameterSetName = "RandomLength")]
        [ValidateRange(1, [int]::MaxValue)]
        [int]$MaxLength = 8,

        [ValidateRange(1, [int]::MaxValue)]
        [int]$Segments = 1,

        [ValidateRange(1, [int]::MaxValue)]
        [int]$MinSegments = 1,

        [ValidateRange(1, [int]::MaxValue)]
        [int]$MaxSegments = 1,

        [ValidateSet('UcFirst', 'LcFirst', 'AllUpper', 'AllLower')]
        [string]$Style = 'UcFirst',

        [string[]]$JoinWith = @('.'),

        [switch]$NoNumbers
    )

    if (-not $PSBoundParameters.ContainsKey('Segments')) {
        if ($PSBoundParameters.ContainsKey('MaxSegments')) {
            if ($MinSegments -gt $MaxSegments) {
                Write-Error -Message "MinSegments cannot be greater than MaxSegments" -Category InvalidArgument -ErrorId 'InvalidSegmentRange';
                $Segments = 0;
            } else {
                if ($MinSegments -eq $MaxSegments) {
                    $Segments = $MinSegments;
                } else {
                    if (($Segments = $Script:Random.Next($MinSegments, $MaxSegments)) -eq 0 -and $PSCmdlet.ParameterSetName -eq 'RandomLength' -and $MinLength -gt $MaxLength) {
                        Write-Error -Message "MinLength cannot be greater than MaxLength" -Category InvalidArgument -ErrorId 'InvalidLengthRange';
                    }
                }
            }
        }
    }
    if ($Segments -gt 0) {
        if ($PSBoundParameters.ContainsKey('Length')) {
            switch ($Style) {
                'LcFirst' {
                    if ($Length -eq 1) {
                        (Get-RandomString -Source $Script:LcAlpha -Length 1 -Repeat $Segments -JoinWith $JoinWith) | Write-Output;
                    } else {
                        $Writer = [System.IO.StringWriter]::new();
                        $Writer.Write((Get-RandomString -Source $Script:LcAlpha -Length 1));
                        if ($NoNumbers.IsPresent) {
                            $Writer.Write((Get-RandomString -Source $Script:AlphaChars -Length $Length - 1));
                            if ($Segments -gt 1) {
                                $Jwc = $JoinWith.Count;
                                if ($Jwc -gt 1) {
                                    for ($i = 1; $i -lt $Segments; $i++) {
                                        $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                        $Writer.Write((Get-RandomString -Source $Script:LcAlpha -Length 1));
                                        $Writer.Write((Get-RandomString -Source $Script:AlphaChars -Length $Length - 1));
                                    }
                                } else {
                                    $c = $JoinWith[0];
                                    for ($i = 1; $i -lt $Segments; $i++) {
                                        $Writer.Write($c);
                                        $Writer.Write((Get-RandomString -Source $Script:LcAlpha -Length 1));
                                        $Writer.Write((Get-RandomString -Source $Script:AlphaChars -Length $Length - 1));
                                    }
                                }
                            }
                        } else {
                            $Writer.Write((Get-RandomString -Source $Script:AlphaNum -Length $Length - 1));
                            if ($Segments -gt 1) {
                                $Jwc = $JoinWith.Count;
                                if ($Jwc -gt 1) {
                                    for ($i = 1; $i -lt $Segments; $i++) {
                                        $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                        $Writer.Write((Get-RandomString -Source $Script:LcAlpha -Length 1));
                                        $Writer.Write((Get-RandomString -Source $Script:AlphaNum -Length $Length - 1));
                                    }
                                } else {
                                    $c = $JoinWith[0];
                                    for ($i = 1; $i -lt $Segments; $i++) {
                                        $Writer.Write($c);
                                        $Writer.Write((Get-RandomString -Source $Script:LcAlpha -Length 1));
                                        $Writer.Write((Get-RandomString -Source $Script:AlphaNum -Length $Length - 1));
                                    }
                                }
                            }
                        }
                        $Writer.ToString() | Write-Output;
                    }
                    break;
                }
                'AllUpper' {
                    if ($NoNumbers.IsPresent -or $Length -eq 1) {
                        (Get-RandomString -Source $Script:UcAlpha -Length $Length -Repeat $Segments -JoinWith $JoinWith) | Write-Output;
                    } else {
                        $Writer = [System.IO.StringWriter]::new();
                        $Writer.Write((Get-RandomString -Source $Script:UcAlpha -Length 1));
                        $Writer.Write((Get-RandomString -Source $Script:UcAlphaNum -Length $Length - 1));
                        if ($Segments -gt 1) {
                            $Jwc = $JoinWith.Count;
                            if ($Jwc -gt 1) {
                                for ($i = 1; $i -lt $Segments; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    $Writer.Write((Get-RandomString -Source $Script:UcAlpha -Length 1));
                                    $Writer.Write((Get-RandomString -Source $Script:UcAlphaNum -Length $Length - 1));
                                }
                            } else {
                                $c = $JoinWith[0];
                                for ($i = 1; $i -lt $Segments; $i++) {
                                    $Writer.Write($c);
                                    $Writer.Write((Get-RandomString -Source $Script:UcAlpha -Length 1));
                                    $Writer.Write((Get-RandomString -Source $Script:UcAlphaNum -Length $Length - 1));
                                }
                            }
                        }
                        $Writer.ToString() | Write-Output;
                    }
                    break;
                }
                'AllLower' {
                    if ($NoNumbers.IsPresent -or $Length -eq 1) {
                        (Get-RandomString -Source $Script:LcAlpha -Length $Length -Repeat $Segments -JoinWith $JoinWith) | Write-Output;
                    } else {
                        $Writer = [System.IO.StringWriter]::new();
                        $Writer.Write((Get-RandomString -Source $Script:LcAlpha -Length 1));
                        $Writer.Write((Get-RandomString -Source $Script:LcAlphaNum -Length $Length - 1));
                        if ($Segments -gt 1) {
                            $Jwc = $JoinWith.Count;
                            if ($Jwc -gt 1) {
                                for ($i = 1; $i -lt $Segments; $i++) {
                                    $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                    $Writer.Write((Get-RandomString -Source $Script:LcAlpha -Length 1));
                                    $Writer.Write((Get-RandomString -Source $Script:LcAlphaNum -Length $Length - 1));
                                }
                            } else {
                                $c = $JoinWith[0];
                                for ($i = 1; $i -lt $Segments; $i++) {
                                    $Writer.Write($c);
                                    $Writer.Write((Get-RandomString -Source $Script:LcAlpha -Length 1));
                                    $Writer.Write((Get-RandomString -Source $Script:LcAlphaNum -Length $Length - 1));
                                }
                            }
                        }
                        $Writer.ToString() | Write-Output;
                    }
                    break;
                }
                default {
                    if ($Length -eq 1) {
                        (Get-RandomString -Source $Script:UcAlpha -Length 1 -Repeat $Segments -JoinWith $JoinWith) | Write-Output;
                    } else {
                        $Writer = [System.IO.StringWriter]::new();
                        $Writer.Write((Get-RandomString -Source $Script:UcAlpha -Length 1));
                        if ($NoNumbers.IsPresent) {
                            $Writer.Write((Get-RandomString -Source $Script:AlphaChars -Length $Length - 1));
                            if ($Segments -gt 1) {
                                $Jwc = $JoinWith.Count;
                                if ($Jwc -gt 1) {
                                    for ($i = 1; $i -lt $Segments; $i++) {
                                        $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                        $Writer.Write((Get-RandomString -Source $Script:UcAlpha -Length 1));
                                        $Writer.Write((Get-RandomString -Source $Script:AlphaChars -Length $Length - 1));
                                    }
                                } else {
                                    $c = $JoinWith[0];
                                    for ($i = 1; $i -lt $Segments; $i++) {
                                        $Writer.Write($c);
                                        $Writer.Write((Get-RandomString -Source $Script:UcAlpha -Length 1));
                                        $Writer.Write((Get-RandomString -Source $Script:AlphaChars -Length $Length - 1));
                                    }
                                }
                            }
                        } else {
                            $Writer.Write((Get-RandomString -Source $Script:AlphaNum -Length $Length - 1));
                            if ($Segments -gt 1) {
                                $Jwc = $JoinWith.Count;
                                if ($Jwc -gt 1) {
                                    for ($i = 1; $i -lt $Segments; $i++) {
                                        $Writer.Write($JoinWith[$Script:Random.Next(0, $Jwc)]);
                                        $Writer.Write((Get-RandomString -Source $Script:UcAlpha -Length 1));
                                        $Writer.Write((Get-RandomString -Source $Script:AlphaNum -Length $Length - 1));
                                    }
                                } else {
                                    $c = $JoinWith[0];
                                    for ($i = 1; $i -lt $Segments; $i++) {
                                        $Writer.Write($c);
                                        $Writer.Write((Get-RandomString -Source $Script:UcAlpha -Length 1));
                                        $Writer.Write((Get-RandomString -Source $Script:AlphaNum -Length $Length - 1));
                                    }
                                }
                            }
                        }
                        $Writer.ToString() | Write-Output;
                    }
                    break;
                }
            }
        } else {

        }
    }
}

"$(('http', 'https') | Select-Random)://$(Get-RandomCsIdentifer -MinSegments 1 -MaxSegments 3 -Style AllLower -MaxLength 6)"

<#
class PreReleaseSegment {
    [bool]$AltSeparator;
    [string]$Value;
    PreReleaseSegment([bool]$AltSeparator) {
        $this.AltSeparator = $AltSeparator;
        if ($AltSeparator) {
            $this.Value = Get-RandomString -MinLength 1 -MaxLength 8 -Omit '+.-';
        } else {
            $this.Value = Get-RandomString -MinLength 0 -MaxLength 8 -Omit '+.-';
        }
    }
}

class BuildSegment {
    [string]$Separator;
    [string]$Value;
    BuildSegment([bool]$AltSeparator) {
        if ($AltSeparator) {
            if ($Script:Random.Next(0, 2) -eq 1) {
                $this.Separator = '-';
            } else {
                $this.Separator = '.';
            }
        } else {
            $this.Separator = '+';
        }
        $this.Value = Get-RandomString -MinLength 0 -MaxLength 8 -Omit '+.-';
    }
}

class SwVersion {
    [string]$Prefix;
    [int]$Major;
    [Nullable[int]]$Minor;
    [Nullable[int]]$Patch;
    [Nullable[int]]$Revision;
    [System.Collections.ObjectModel.Collection[int]]$AdditionalNumerical;
    [System.Collections.ObjectModel.Collection[PreReleaseSegment]]$PreRelease;
    [System.Collections.ObjectModel.Collection[BuildSegment]]$Build;

    SwVersion([bool]$HasPrefix, [int]$NumberCount, [bool]$NoPreSeparator, [int]$PreReleaseCount, [int]$BuildCount) {
        $this.Major = Get-RandomNumber -MinLength 1 -MaxLength 4;
        if ($Script:Random.Next(0, 2) -eq 1) { $this.Major *= -1 }
        if ($NumberCount -gt 1) {
            $this.Minor = Get-RandomNumber -MinLength 1 -MaxLength 4;
            if ($NumberCount -gt 2) {
                $this.Patch = Get-RandomNumber -MinLength 1 -MaxLength 4;
                if ($NumberCount -gt 3) {
                    $this.Revision = Get-RandomNumber -MinLength 1 -MaxLength 4;
                    if ($NumberCount -gt 4) {
                        $this.AdditionalNumerical = [System.Collections.ObjectModel.Collection[int]]::new();
                        for ($i = 4; $i -lt $NumberCount; $i++) {
                            $this.AdditionalNumerical.Add((Get-RandomNumber -MinLength 1 -MaxLength 4));
                        }
                    }
                }
            }
        }
        if ($PreReleaseCount -gt 0) {
            $this.PreRelease = [System.Collections.ObjectModel.Collection[PreReleaseSegment]]::new();
            $this.PreRelease.Add([PreReleaseSegment]::new($NoPreSeparator));
            for ($i = 1; $i -lt $PreReleaseCount; $i++) {
                $this.PreRelease.Add([PreReleaseSegment]::new(($Script:Random.Next(0, 2) -eq 1)));
            }
        }
        if ($BuildCount -gt 0) {
            $this.Build = [System.Collections.ObjectModel.Collection[BuildSegment]]::new();
            $this.Build.Add([BuildSegment]::new($false));
            for ($i = 1; $i -lt $BuildCount; $i++) {
                $this.Build.Add([BuildSegment]::new(($Script:Random.Next(0, 2) -eq 1)));
            }
        }
    }

    [string]ToTestData() {
        $Result = '';
        if ($null -ne $this.Prefix) { $Result = $this.Prefix }
        $Result += $this.Major;
        if ($null -ne $this.Minor) {
            $Result += '.' + $this.Minor;
            if ($null -ne $this.Patch) {
                $Result += '.' + $this.Patch;
                if ($null -ne $this.Revision) {
                    $Result += '.' + $this.Revision;
                    if ($null -ne $this.AdditionalNumerical) {
                        $this.AdditionalNumerical | ForEach-Object {
                            $Result += '.' + $_;
                        }
                    }
                }
            }
        }
        if ($null -ne $this.PreRelease) {
            if ($this.PreRelease[0].AltSeparator) {
                $Result += $this.PreRelease[0].Value;
            } else {
                $Result += '_' + $this.PreRelease[0].Value;
            }
            ($this.PreRelease | Select-Object -Skip 1) | ForEach-Object {
                if ($_.AltSeparator) {
                    $Result += '.' + $_.Value;
                } else {
                    $Result += '_' + $_.Value;
                }
            }
        }
        if ($null -ne $this.Build) {
            $this.Build | ForEach-Object {
                $Result += $_.Separator + $_.Value;
            }
        }
        $Result = "            Add(`"$($Result.Replace('\', '\\').Replace('"', '\"'))`", new JsonObject()";
        if ($null -eq $this.Prefix) {
            $Result += '.AddNullProperty(nameof(SwVersion.Prefix))';
        } else {
            $Result += ".AddProperty(nameof(SwVersion.Prefix), `"$($this.Prefix.Replace('\', '\\').Replace('"', '\"'))`")";
        }
        $Result += "`n                .AddProperty(nameof(SwVersion.Major), $($this.Major))";
        if ($null -ne $this.Minor) {
            $Result += ".AddProperty(nameof(SwVersion.Minor), $($this.Minor))";
            if ($null -ne $this.Patch) {
                $Result += ".AddProperty(nameof(SwVersion.Patch), $($this.Patch))";
                if ($null -ne $this.Revision) {
                    $Result += ".AddProperty(nameof(SwVersion.Revision), $($this.Revision))";
                    if ($null -ne $this.AdditionalNumerical) {
                        $Result += "`n                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical), $($this.AdditionalNumerical -join ', '))";
                    } else {
                        $Result += "`n                .AddNullProperty(nameof(SwVersion.AdditionalNumerical))";
                    }
                } else {
                    $Result += ".AddNullProperty(nameof(SwVersion.Revision))`n                .AddNullProperty(nameof(SwVersion.AdditionalNumerical))";
                }
            } else {
                $Result += ".AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))`n                .AddNullProperty(nameof(SwVersion.AdditionalNumerical))";
            }
        } else {
            $Result += ".AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))`n                .AddNullProperty(nameof(SwVersion.AdditionalNumerical))";
        }
        if ($null -ne $this.PreRelease) {
            $Result += @'

                .AddObjectArrayProperty(nameof(SwVersion.PreRelease)
'@
            $this.PreRelease | ForEach-Object {
                $Result += @'
,
                    new JsonObject().AddProperty(nameof(SwVersion.PreReleaseSegment.AltSeparator), 
'@
                if ($_.AltSeparator) {
                    $Result += 'true';
                } else {
                    $Result += 'false';
                }
                $Result += ").AddProperty(nameof(SwVersion.PreReleaseSegment.Value), `"$($_.Value.Replace('\', '\\').Replace('"', '\"'))`")";
            }
            $Result += ')';
        } else {
            $Result += ".AddNullProperty(nameof(SwVersion.PreRelease))";
        }
        if ($null -ne $this.Build) {
            $Result += @'

                .AddObjectArrayProperty(nameof(SwVersion.Build)
'@
            $this.Build | ForEach-Object {
                $Result += @"
,
                    new JsonObject().AddProperty(nameof(SwVersion.BuildSegment.Separator), "$($_.Separator)").AddProperty(nameof(SwVersion.BuildSegment.Value), "$($_.Value.Replace('\', '\\').Replace('"', '\"'))")
"@
            }
            $Result += ')';
        } else {
            $Result += ".AddNullProperty(nameof(SwVersion.Build))";
        }
        return $Result + '.ToJsonString());';
    }
}

Function Get-RandomString {
    Param(
        [Parameter(Mandatory = $true)]
        [int]$MinLength,
        [Parameter(Mandatory = $true)]
        [int]$MaxLength,
        [string]$Omit,
        [switch]$IncludeWhiteSpace
    )
    $Length = $MinLength;
    if ($MaxLength -gt $MinLength) { $Length = $Script:Random.Next($MinLength, $MaxLength + 1) }
    $Result = '';
    if ($Length -lt 1) { return $Result }
    if ($IncludeWhiteSpace.IsPresent) {
        if ($PSBoundParameters.ContainsKey('Omit')) {
            [System.Linq.Enumerable]::Range(0, $Length) | ForEach-Object {

            }
            for ($i = 0; $i -lt $Length; $i++) {
                $max = 128;
                if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                [char]$c = $Script:Random.Next(32, $max - 32);
                while ($Omit.Contains($c) -or [char]::IsControl($c) -or [char]::IsSurrogate($c)) {
                    $max = 128;
                    if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                    [char]$c = $Script:Random.Next(32, $max - 32);
                }
                $Result += $c;
            }
        } else {
            for ($i = 0; $i -lt $Length; $i++) {
                $max = 128;
                if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                [char]$c = $Script:Random.Next(32, $max - 32);
                while ([char]::IsControl($c) -or [char]::IsSurrogate($c)) {
                    $max = 128;
                    if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                    [char]$c = $Script:Random.Next(32, $max - 32);
                }
                $Result += $c;
            }
        }
    } else {
        if ($PSBoundParameters.ContainsKey('Omit')) {
            for ($i = 0; $i -lt $Length; $i++) {
                $max = 128;
                if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                [char]$c = $Script:Random.Next(32, $max - 32);
                while ($Omit.Contains($c) -or [char]::IsWhiteSpace($c) -or [char]::IsControl($c) -or [char]::IsSurrogate($c)) {
                    $max = 128;
                    if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                    [char]$c = $Script:Random.Next(32, $max - 32);
                }
                $Result += $c;
            }
        } else {
            for ($i = 0; $i -lt $Length; $i++) {
                $max = 128;
                if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                [char]$c = $Script:Random.Next(32, $max - 32);
                while ([char]::IsWhiteSpace($c) -or [char]::IsControl($c) -or [char]::IsSurrogate($c)) {
                    $max = 128;
                    if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                    [char]$c = $Script:Random.Next(32, $max - 32);
                }
                $Result += $c;
            }
        }
    }
    return $Result;
}

Function Get-RandomChars {
    [CmdletBinding(DefaultParameterSetName = 'Inclusive')]
    Param(
        [Parameter(Mandatory = $true)]
        [int]$MinCount,
        [Parameter(Mandatory = $true)]
        [int]$MaxCount,
        [Parameter(ParameterSetName = 'Inclusive')]
        [string]$Omit,
        [Parameter(ParameterSetName = 'Inclusive')]
        [switch]$IncludeWhiteSpace,
        [Parameter(ParameterSetName = 'Exclusive')]
        [string]$From
    )
    $Count = $MinCount;
    if ($MaxCount -gt $MinCount) { $Count = $Script:Random.Next($MinCount, $MaxCount + 1) }
    if ($PSCmdlet.ParameterSetName -eq 'Exclusive') {
        [System.Linq.Enumerable]::Range(0, $Count) | ForEach-Object {
            $From[$Script:Random.Next(0, $From.Length)] | Write-Output;
        }
    } else {
        if ($IncludeWhiteSpace.IsPresent) {
            if ($PSBoundParameters.ContainsKey('Omit')) {
                [System.Linq.Enumerable]::Range(0, $Count) | ForEach-Object {
                    $max = 128;
                    if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                    [char]$c = $Script:Random.Next(32, $max - 32);
                    while ($Omit.Contains($c) -or [char]::IsControl($c) -or [char]::IsSurrogate($c)) {
                        [char]$c = $Script:Random.Next(32, $max - 32);
                    }
                    $c | Write-Output;
                }
            } else {
                [System.Linq.Enumerable]::Range(0, $Count) | ForEach-Object {
                    $max = 128;
                    if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                    [char]$c = $Script:Random.Next(32, $max - 32);
                    while ([char]::IsControl($c) -or [char]::IsSurrogate($c)) {
                        [char]$c = $Script:Random.Next(32, $max - 32);
                    }
                    $c | Write-Output;
                }
            }
        } else {
            if ($PSBoundParameters.ContainsKey('Omit')) {
                [System.Linq.Enumerable]::Range(0, $Count) | ForEach-Object {
                    $max = 128;
                    if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                    [char]$c = $Script:Random.Next(32, $max - 32);
                    while ($Omit.Contains($c) -or [char]::IsWhiteSpace($c) -or [char]::IsControl($c) -or [char]::IsSurrogate($c)) {
                        [char]$c = $Script:Random.Next(32, $max - 32);
                    }
                    $c | Write-Output;
                }
            } else {
                [System.Linq.Enumerable]::Range(0, $Count) | ForEach-Object {
                    $max = 128;
                    if ($Random.Next(0, 4) -eq 4) { $max = 256 }
                    [char]$c = $Script:Random.Next(32, $max - 32);
                    while ([char]::IsWhiteSpace($c) -or [char]::IsControl($c) -or [char]::IsSurrogate($c)) {
                        [char]$c = $Script:Random.Next(32, $max - 32);
                    }
                    $c | Write-Output;
                }
            }
        }
    }
}

Function Get-RandomNumberChars {
    Param(
        [Parameter(Mandatory = $true)]
        [int]$MinCount,
        [Parameter(Mandatory = $true)]
        [int]$MaxCount
    )
    Get-RandomChars -MinCount $MinCount -MaxCount $MaxCount -From $Script:NumberChars;
}

$StringWriter = [System.IO.StringWriter]::new();
foreach ($HasPrefix in @($false, $true)) {
    foreach ($HasBuild in @($false, $true)) {
        foreach ($HasPreRelease in @($false, $true)) {
            foreach ($VersionNumberCount in [System.Linq.Enumerable]::Range(1, 8)) {
                $StringWriter.Write('            Add("');
                $Pfx = $null;
                if ($HasPrefix) {
                    $Pfx = (-join ([System.Linq.Enumerable]::Range(0, $Script:Random.Next(1, 4)) | ForEach-Object {
                        switch ($Script:Random.Next(0, 7)) {
                            0 {
                                '+' | Write-Output;
                                break;
                            }
                            1 {
                                '-' | Write-Output;
                                break;
                            }
                        }
                        Get-RandomChars -MinCount 1 -MaxCount 8 -Omit '+-0123456789';
                    })).Replace('\', '\\').Replace('"', '\"');
                    $StringWriter.Write($Pfx);
                }
                $Major = -join (Get-RandomNumberChars -MinCount 1 -MaxCount 4);
                if ($Script:Random.Next(0, 3) -eq 1) { $Major = "-$Major" }
                $StringWriter.Write($Major);
                $Minor = $null;
                $Patch = $null;
                $Rev = $null;
                $Xnum = $null;
                if ($VersionNumberCount -gt 1) {
                    $Minor = -join (Get-RandomNumberChars -MinCount 1 -MaxCount 4);
                    if ($Script:Random.Next(0, 3) -eq 1) { $Minor = "-$Minor" }
                    $StringWriter.Write('.');
                    $StringWriter.Write($Minor);
                    if ($VersionNumberCount -gt 2) {
                        $Patch = -join (Get-RandomNumberChars -MinCount 1 -MaxCount 4);
                        if ($Script:Random.Next(0, 3) -eq 1) { $Patch = "-$Patch" }
                        $StringWriter.Write('.');
                        $StringWriter.Write($Patch);
                        if ($VersionNumberCount -gt 3) {
                            $Rev = -join (Get-RandomNumberChars -MinCount 1 -MaxCount 4);
                            if ($Script:Random.Next(0, 3) -eq 1) { $Rev = "-$Rev" }
                            $StringWriter.Write('.');
                            $StringWriter.Write($Rev);
                            if ($VersionNumberCount -gt 4) {
                                $XNum = ([System.Linq.Enumerable]::Range(4, $VersionNumberCount) | ForEach-Object {
                                    $n = -join (Get-RandomNumberChars -MinCount 1 -MaxCount 4);
                                    if ($Script:Random.Next(0, 3) -eq 1) { "-$n" } else { $n }
                                }) -join '.';
                                $StringWriter.Write('.');
                                $StringWriter.Write($XNum);
                            }
                        }
                    }
                }
                $Delim = $null;
                $Pre = $null;
                if ($HasPreRelease) {
                    switch ($Script:Random.Next(0, 3)) {
                        1 {
                            $Delim = '.';
                            $Pre = (-join (@(Get-RandomChars -MinCount 1 -MaxCount 1 -Omit '+0123456789') + @(Get-RandomChars -MinCount 0 -MaxCount 7 -Omit '+'))).Replace('\', '\\').Replace('"', '\"');
                            $StringWriter.Write($Delim);
                            break;
                        }
                        2 {
                            $Delim = '-';
                            $Pre = (-join (Get-RandomChars -MinCount 0 -MaxCount 8 -Omit '+')).Replace('\', '\\').Replace('"', '\"');
                            $StringWriter.Write($Delim);
                            break;
                        }
                        default {
                            $Pre = (-join (@(Get-RandomChars -MinCount 1 -MaxCount 1 -Omit '+.-0123456789') + @(Get-RandomChars -MinCount 0 -MaxCount 7 -Omit '+'))).Replace('\', '\\').Replace('"', '\"');
                            break;
                        }
                    }
                    $StringWriter.Write($Pre);
                }
                $Build = $null;
                if ($HasBuild) {
                    $Build = (-join (Get-RandomChars -MinCount 0 -MaxCount 8)).Replace('\', '\\').Replace('"', '\"');
                    $StringWriter.Write('+');
                    $StringWriter.Write($Build);
                }
                $StringWriter.Write('", /*pfx*/');
                if ($HasPrefix) {
                    $StringWriter.Write('"');
                    $StringWriter.Write($Pfx);
                    $StringWriter.WriteLine('",');
                    $StringWriter.Write('                /*major*/"');
                } else {
                    $StringWriter.Write('null, /*major*/"');
                }
                $StringWriter.Write($Major);
                $StringWriter.Write('", /*minor*/');
                if ($VersionNumberCount -gt 1) {
                    $StringWriter.Write('"');
                    $StringWriter.Write($Minor);
                    $StringWriter.Write('", /*patch*/');
                    if ($VersionNumberCount -gt 2) {
                        $StringWriter.Write('"');
                        $StringWriter.Write($Patch);
                        $StringWriter.Write('", /*rev*/');
                        if ($VersionNumberCount -gt 3) {
                            $StringWriter.Write('"');
                            $StringWriter.Write($Rev);
                            $StringWriter.Write('", /*xnum*/');
                            if ($VersionNumberCount -gt 4) {
                                $StringWriter.Write('"');
                                $StringWriter.Write($Xnum);
                                $StringWriter.WriteLine('",');
                            } else {
                                $StringWriter.WriteLine('null,');
                            }
                        } else {
                            $StringWriter.WriteLine('null, /*xnum*/null,');
                        }
                    } else {
                        $StringWriter.WriteLine('null, /*rev*/null, /*xnum*/null,');
                    }
                } else {
                    if ($HasPreRelease -or $HasBuild) {
                        $StringWriter.WriteLine('null, /*patch*/null, /*rev*/null, /*xnum*/null,');
                    } else {
                        $StringWriter.Write('null, /*patch*/null, /*rev*/null, /*xnum*/null, ');
                    }
                }
                if ($HasPreRelease) {
                    if ($null -eq $Delim) {
                        $StringWriter.Write('                /*delim*/null, /*pre*/"');
                    } else {
                        $StringWriter.Write("                /*delim*/'");
                        $StringWriter.Write($Delim);
                        $StringWriter.Write("', /*pre*/`"");
                    }
                    $StringWriter.Write($Pre);
                    $StringWriter.WriteLine('",');
                } else {
                    if ($HasBuild) {
                        $StringWriter.WriteLine('                /*delim*/null, /*pre*/null,')
                    } else {
                        $StringWriter.Write('                /*delim*/null, /*pre*/null, ')
                    }
                }
                if ($HasBuild) {
                    $StringWriter.Write('                /*build*/"');
                    $StringWriter.Write($Build);
                    $StringWriter.WriteLine('");');
                } else {
                    $StringWriter.WriteLine('                /*build*/null);');
                }
            }
        }
    }
}
#>
<#
$TestDataText = [System.Linq.Enumerable]::Range(1, 8) | ForEach-Object {
    $NumberCount = $_;
    [System.Linq.Enumerable]::Range(0, 3) | ForEach-Object {
        $PreReleaseCount = $_;
        [System.Linq.Enumerable]::Range(0, 3) | ForEach-Object {
            [SwVersion]::new($false, $NumberCount, $false, $PreReleaseCount, $_);
            [SwVersion]::new($true, $NumberCount, $false, $PreReleaseCount, $_);
            [SwVersion]::new($false, $NumberCount, $true, $PreReleaseCount, $_);
            [SwVersion]::new($true, $NumberCount, $true, $PreReleaseCount, $_);
        }
    }
} | ForEach-Object { $_.ToTestData() } | Out-String;
#>
<#
for ($NumberCount = 1; $NumberCount -lt 8; $NumberCount++) {
    for ($PreReleaseCount = 1; $PreReleaseCount -lt 3; $PreReleaseCount++) {
        for ($BuildCount = 1; $BuildCount -lt 3; $BuildCount++) {
            
        }
    }
}
for ($r = 0; $r -lt 50; $r++) {
    $VersionString = '';
    if ($Script:Random.Next(0, 2) -eq 1) {
        $VersionString = Get-RandomString -MinLength 1 -MaxLength 8 -Omit '.+-0123456789';
    }
    $VersionPrefix = $VersionString;
    $Major = Get-RandomNumber -MinLength 1 -MaxLength 4;
    if ($Script:Random.Next(0, 2) -eq 1) { $Major *= -1 }
    $VersionString += $Major;
    $Minor = $Patch = $Revision = -1;
    $Count = $Script:Random.Next(1, 5);
    if ($Count -gt 1) {
        $Minor = Get-RandomNumber -MinLength 1 -MaxLength 4;
        $VersionString += '.' + $Minor;
        if ($Count -gt 2) {
            $Patch = Get-RandomNumber -MinLength 1 -MaxLength 4;
            $VersionString += '.' + $Patch;
            if ($Count -gt 3) {
                $Revision = Get-RandomNumber -MinLength 1 -MaxLength 4;
                $VersionString += '.' + $Revision;
            }
        }
    }
    $AdditionalNumerical = @();
    if ($Count -eq 3 -and $Script:Random.Next(0, 2) -eq 1) {
        $Count = $Script:Random.Next(1, 5);
        for ($i = 0; $i -lt $Count; $i++) {
            $Value = Get-RandomNumber -MinLength 1 -MaxLength 4;
            $AdditionalNumerical += $Value;
            $VersionString += '.' + $Value;
        }
    }

    $PreRelease = @();
    if ($Script:Random.Next(0, 2) -eq 1) {
        $Text = Get-RandomString -MinLength 1 -MaxLength 8 -Omit '+.-';
        if ($Script:Random.Next(0, 2) -eq 1) { $Text = '-' + $Text }
        $PreRelease += $Text.Replace('\', '\\').Replace('"', '\"');
        $VersionString += $Text;
        $Count = $Script:Random.Next(0, 4);
        for ($i = 0; $i -lt $Count; $i++) {
            if ($Script:Random.Next(0, 2) -eq 1) {
                $Text = '.' + (Get-RandomString -MinLength 1 -MaxLength 8 -Omit '+.-');
            } else {
                $Text = '-' + (Get-RandomString -MinLength 1 -MaxLength 8 -Omit '+.-');
            }
            $PreRelease += $Text.Replace('\', '\\').Replace('"', '\"');
            $VersionString += $Text;
        }
    }

    $Build = @();
    if ($Script:Random.Next(0, 2) -eq 1) {
        $Text = '+' + (Get-RandomString -MinLength 1 -MaxLength 8 -Omit '+.-');
        $Build += $Text.Replace('\', '\\').Replace('"', '\"');
        $VersionString += $Text;
        $Count = $Script:Random.Next(0, 4);
        for ($i = 0; $i -lt $Count; $i++) {
            if ($Script:Random.Next(0, 2) -eq 1) {
                if ($Script:Random.Next(0, 2) -eq 1) {
                    $Text += '.' + (Get-RandomString -MinLength 1 -MaxLength 8 -Omit '+.-');
                } else {
                    $Text += '-' + (Get-RandomString -MinLength 1 -MaxLength 8 -Omit '+.-');
                }
            } else {
                $Text += '+' + (Get-RandomString -MinLength 1 -MaxLength 8 -Omit '+.-');
            }
            $Build += $Text.Replace('\', '\\').Replace('"', '\"');
            $VersionString += $Text;
        }
    }
    $TestDataText += @"

            Add("$($VersionString.Replace('\', '\\').Replace('"', '\"'))", new JsonObject()
"@
    if ($VersionPrefix.Length -gt 0) {
        $TestDataText += ".AddProperty(nameof(SwVersion.Prefix), `"$($VersionPrefix.Replace('\', '\\').Replace('"', '\"'))`")"
    } else {
        $TestDataText += '.AddNullProperty(nameof(SwVersion.Prefix))'
    }
    $TestDataText += @"

                .AddProperty(nameof(SwVersion.Major), $Major)
"@
    if ($Minor -lt 0) {
        $TestDataText += '.AddNullProperty(nameof(SwVersion.Minor)).AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))';
    } else {
        $TestDataText += ".AddProperty(nameof(SwVersion.Minor), $Minor)";
        if ($Patch -lt 0) {
            $TestDataText += '.AddNullProperty(nameof(SwVersion.Patch)).AddNullProperty(nameof(SwVersion.Revision))';
        } else {
            $TestDataText += ".AddProperty(nameof(SwVersion.Patch), $Patch)";
            if ($Revision -lt 0) {
                $TestDataText += '.AddNullProperty(nameof(SwVersion.Revision))';
            } else {
                $TestDataText += ".AddProperty(nameof(SwVersion.Revision), $Revision)";
            }
        }
    }
    $TestDataText += @"

                .AddIntArrayProperty(nameof(SwVersion.AdditionalNumerical)
"@
    if ($AdditionalNumerical.Count -gt 0) {
        $TestDataText += ", $($AdditionalNumerical -join ', '))";
    } else {
        $TestDataText += ')';
    }
    if ($PreRelease.Count -gt 0) {
        $TestDataText += ".AddStringArrayProperty(nameof(SwVersion.PreRelease), `"$($PreRelease -join '", "')`")"
    } else {
        $TestDataText += '.AddNullProperty(nameof(SwVersion.PreRelease))'
    }
    if ($Build.Count -gt 0) {
        $TestDataText += ".AddStringArrayProperty(nameof(SwVersion.Build), `"$($Build -join '", "')`").ToJsonString());"
    } else {
        $TestDataText += '.AddNullProperty(nameof(SwVersion.Build)).ToJsonString());'
    }
}
#>
$BasePath = $PSScriptRoot | Join-Path -ChildPath 'Output';
if (-not ($BasePath | Test-Path)) { (New-Item -Path $PSScriptRoot -ItemType Directory -Name 'Output' -ErrorAction Stop) | Out-Null; }
[System.IO.File]::WriteAllText(($BasePath | Join-Path -ChildPath 'TestData.txt'), $StringWriter.ToString(), [System.Text.UTF8Encoding]::new($false, $false));
