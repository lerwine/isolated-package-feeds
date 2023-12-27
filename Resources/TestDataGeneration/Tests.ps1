
if ($null -eq (Get-Module -Name 'Pester')) { Import-Module -Name 'Pester' }

BeforeAll {
    if ($null -ne (Get-Module -Name 'TestDataGeneration')) { Remove-Module -Name 'TestDataGeneration' }
    Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath 'TestDataGeneration.psd1') -ErrorAction Stop;
}

Describe 'Convert-RangeValuesToTuple' {
    Context 'Values by parameter' {
        Context 'Single Value' {
            It 'Given value of 0, it returns { Item1: 0, Item2: 0 }' {
                $Tuple = Convert-RangeValuesToTuple -Values 0;
                $Tuple.Item1 | Should -Be 0;
                $Tuple.Item2 | Should -Be 0;
            }
            It 'Given value of [int]::MinValue, it returns { Item1: [int]::MinValue, Item2: [int]::MinValue }' {
                $Tuple = Convert-RangeValuesToTuple -Values [int]::MinValue;
                $Tuple.Item1 | Should -Be [int]::MinValue;
                $Tuple.Item2 | Should -Be [int]::MinValue;
            }
            It 'Given value of 2, it returns { Item1: 2, Item2: 2 }' {
                $Tuple = Convert-RangeValuesToTuple -Values 2;
                $Tuple.Item1 | Should -Be 2;
                $Tuple.Item2 | Should -Be 2;
            }
        }
        Context 'Two Values' {
            It 'Given values of (4, 6), it returns { Item1: 4, Item2: 6 }' {
                $Tuple = Convert-RangeValuesToTuple -Values 4, 6;
                $Tuple.Item1 | Should -Be 4;
                $Tuple.Item2 | Should -Be 6;
            }
            It 'Given values of (-1, 7), it returns { Item1: -1, Item2: 7 }' {
                $Tuple = Convert-RangeValuesToTuple -Values -1, 7;
                $Tuple.Item1 | Should -Be -1;
                $Tuple.Item2 | Should -Be 7;
            }
        }
        # Context 'More Than Two Values' {
        # }
    }
    Context 'Values by pipeline' {
        Context 'Single Value' {
            It 'Given value of 0, it returns { Item1: 0, Item2: 0 }' {
                $Tuple = 0 | Convert-RangeValuesToTuple;
                $Tuple.Item1 | Should -Be 0;
                $Tuple.Item2 | Should -Be 0;
            }
            It 'Given value of [int]::MinValue, it returns { Item1: [int]::MinValue, Item2: [int]::MinValue }' {
                $Tuple = [int]::MinValue | Convert-RangeValuesToTuple;
                $Tuple.Item1 | Should -Be [int]::MinValue;
                $Tuple.Item2 | Should -Be [int]::MinValue;
            }
            It 'Given value of 3, it returns { Item1: 3, Item2: 3 }' {
                $Tuple = 3 | Convert-RangeValuesToTuple;
                $Tuple.Item1 | Should -Be 3;
                $Tuple.Item2 | Should -Be 3;
            }
        }
        Context 'Two Values' {
            It 'Given values of (3, 5), it returns { Item1: 3, Item2: 5 }' {
                $Tuple = (3, 5) | Convert-RangeValuesToTuple;
                $Tuple.Item1 | Should -Be 3;
                $Tuple.Item2 | Should -Be 5;
            }
            It 'Given values of (-32768, 65536), it returns { Item1: -32768, Item2: 65536 }' {
                $Tuple = (-32768, 65536) | Convert-RangeValuesToTuple;
                $Tuple.Item1 | Should -Be -32768;
                $Tuple.Item2 | Should -Be 65536;
            }
            It 'Given values of (-32768, 65536), it returns { Item1: -32768, Item2: 65536 }' {
                $Tuple = (-32768, 65536) | Convert-RangeValuesToTuple;
                $Tuple.Item1 | Should -Be -32768;
                $Tuple.Item2 | Should -Be 65536;
            }
        }
        # Context 'More Than Two Values' {
        # }
    }
}

Describe 'Get-RandomCharacterSource' {
    Context 'Single Include' {
        It 'Given "LettersAndDigits", it returns source where [char]::IsLetterOrDigit returns true for all' {
            [int]$e = [char]::MaxValue;
            $ExpectedLength = 0;
            for ($i = 0; $i -le $e; $i++) {
                [char]$c = $i;
                if ([char]::IsLetterOrDigit($c)) { $ExpectedLength++ }
            }
            $Source = Get-RandomCharacterSource -Include LettersAndDigits;
            $Source.Length | Should -Be $ExpectedLength;
            $Length = @($Source.Values | Where-Object { [char]::IsLetterOrDigit($_) }).Count;
            $Length | Should -Be $ExpectedLength;
        }
    }
    Context 'Combined Include' {
        It 'Given "Letters" and "Digits", it returns source where [char]::IsLetterOrDigit returns true for all' {
            [int]$e = [char]::MaxValue;
            $ExpectedLength = 0;
            for ($i = 0; $i -le $e; $i++) {
                [char]$c = $i;
                if ([char]::IsLetterOrDigit($c)) { $ExpectedLength++ }
            }
            $Source = Get-RandomCharacterSource -Include Letters, Digits;
            $Source.Length | Should -Be $ExpectedLength;
            $Length = @($Source.Values | Where-Object { [char]::IsLetterOrDigit($_) }).Count;
            $Length | Should -Be $ExpectedLength;
        }
    }
    Context 'Redundant Include' {
        It 'Given "LettersAndDigits" and "Digits", it returns source where [char]::IsLetterOrDigit returns true for all' {
            [int]$e = [char]::MaxValue;
            $ExpectedLength = 0;
            for ($i = 0; $i -le $e; $i++) {
                [char]$c = $i;
                if ([char]::IsLetterOrDigit($c)) { $ExpectedLength++ }
            }
            $Source = Get-RandomCharacterSource -Include Letters, Digits;
            $Source.Length | Should -Be $ExpectedLength;
            $Length = @($Source.Values | Where-Object { [char]::IsLetterOrDigit($_) }).Count;
            $Length | Should -Be $ExpectedLength;
        }
    }
    Context 'Overlapping Include' {
        It 'Given "LettersAndDigits" and "Numbers", it returns source where [char]::IsLetter or [char]::IsNumber returns true for all' {
            [int]$e = [char]::MaxValue;
            $ExpectedLength = 0;
            for ($i = 0; $i -le $e; $i++) {
                [char]$c = $i;
                if ([char]::IsLetter($c) -or [char]::IsNumber($c)) { $ExpectedLength++ }
            }
            $Source = Get-RandomCharacterSource -Include Letters, Digits;
            $Source.Length | Should -Be $ExpectedLength;
            $Length = @($Source.Values | Where-Object { [char]::IsLetter($_) -or [char]::IsNumber($_) }).Count;
            $Length | Should -Be $ExpectedLength;
        }
    }
}