﻿$EntityMap = @{
    38 = 'amp';
    60 = 'lt';
    62 = 'gt';
    160 = 'nbsp';
    161 = 'iexcl';
    162 = 'cent';
    163 = 'pound';
    164 = 'curren';
    165 = 'yen';
    166 = 'brvbar';
    167 = 'sect';
    168 = 'uml';
    169 = 'copy';
    170 = 'ordf';
    171 = 'laquo';
    172 = 'not';
    173 = 'shy';
    174 = 'reg';
    175 = 'macr';
    176 = 'deg';
    177 = 'plusmn';
    178 = 'sup2';
    179 = 'sup3';
    180 = 'acute';
    181 = 'micro';
    182 = 'para';
    184 = 'cedil';
    185 = 'sup1';
    186 = 'ordm';
    187 = 'raquo';
    188 = 'frac14';
    189 = 'frac12';
    190 = 'frac34';
    191 = 'iquest';
    215 = 'times';
    247 = 'divide';
    8704 = 'forall';
    8706 = 'part';
    8707 = 'exist';
    8709 = 'empty';
    8711 = 'nabla';
    8712 = 'isin';
    8713 = 'notin';
    8715 = 'ni';
    8719 = 'prod';
    8721 = 'sum';
    8722 = 'minus';
    8727 = 'lowast';
    8730 = 'radic';
    8733 = 'prop';
    8734 = 'infin';
    8736 = 'ang';
    8743 = 'and';
    8744 = 'or';
    8745 = 'cap';
    8746 = 'cup';
    8747 = 'int';
    8756 = 'there4';
    8764 = 'sim';
    8773 = 'cong';
    8776 = 'asymp';
    8800 = 'ne';
    8801 = 'equiv';
    8804 = 'le';
    8805 = 'ge';
    8834 = 'sub';
    8835 = 'sup';
    8836 = 'nsub';
    8838 = 'sube';
    8839 = 'supe';
    8853 = 'oplus';
    8855 = 'otimes';
    8869 = 'perp';
    8901 = 'sdot';
    913 = 'Alpha';
    914 = 'Beta';
    915 = 'Gamma';
    916 = 'Delta';
    917 = 'Epsilon';
    918 = 'Zeta';
    919 = 'Eta';
    920 = 'Theta';
    921 = 'Iota';
    922 = 'Kappa';
    923 = 'Lambda';
    924 = 'Mu';
    925 = 'Nu';
    926 = 'Xi';
    927 = 'Omicron';
    928 = 'Pi';
    929 = 'Rho';
    931 = 'Sigma';
    932 = 'Tau';
    933 = 'Upsilon';
    934 = 'Phi';
    935 = 'Chi';
    936 = 'Psi';
    937 = 'Omega';
    945 = 'alpha';
    946 = 'beta';
    947 = 'gamma';
    948 = 'delta';
    949 = 'epsilon';
    950 = 'zeta';
    951 = 'eta';
    952 = 'theta';
    953 = 'iota';
    954 = 'kappa';
    955 = 'lambda';
    956 = 'mu';
    957 = 'nu';
    958 = 'xi';
    959 = 'omicron';
    960 = 'pi';
    961 = 'rho';
    962 = 'sigmaf';
    963 = 'sigma';
    964 = 'tau';
    965 = 'upsilon';
    966 = 'phi';
    967 = 'chi';
    968 = 'psi';
    969 = 'omega';
    977 = 'thetasym';
    978 = 'upsih';
    982 = 'piv';
    338 = 'OElig';
    339 = 'oelig';
    352 = 'Scaron';
    353 = 'scaron';
    376 = 'Yuml';
    402 = 'fnof';
    710 = 'circ';
    732 = 'tilde';
    8194 = 'ensp';
    8195 = 'emsp';
    8201 = 'thinsp';
    8204 = 'zwnj';
    8205 = 'zwj';
    8206 = 'lrm';
    8207 = 'rlm';
    8211 = 'ndash';
    8212 = 'mdash';
    8216 = 'lsquo';
    8217 = 'rsquo';
    8218 = 'sbquo';
    8220 = 'ldquo';
    8221 = 'rdquo';
    8222 = 'bdquo';
    8224 = 'dagger';
    8225 = 'Dagger';
    8226 = 'bull';
    8230 = 'hellip';
    8240 = 'permil';
    8242 = 'prime';
    8243 = 'Prime';
    8249 = 'lsaquo';
    8250 = 'rsaquo';
    8254 = 'oline';
    8364 = 'euro';
    8482 = 'trade';
    8592 = 'larr';
    8593 = 'uarr';
    8594 = 'rarr';
    8595 = 'darr';
    8596 = 'harr';
    8629 = 'crarr';
    8968 = 'lceil';
    8969 = 'rceil';
    8970 = 'lfloor';
    8971 = 'rfloor';
    9674 = 'loz';
    9824 = 'spades';
    9827 = 'clubs';
    9829 = 'hearts';
    9830 = 'diams';
}
[Xml]$CharacterTypes = '<html><head><title>Character Types</title></head></html>';
$Body = $CharacterTypes.DocumentElement.AppendChild($CharacterTypes.CreateElement('body'));
$Ul = $Body.AppendChild($CharacterTypes.CreateElement('ul'));
$Ul.Attributes.Append($CharacterTypes.CreateAttribute('id')).Value = "top";
$Keys = @('Control', 'Digit', 'Lower', 'Upper', 'Punctuation', 'Number', 'Separator', 'Symbol', 'WhiteSpace', 'LowSurrogate', 'HighSurrogate', 'Other');
$CharTypeTables = @{
    Control = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    Digit = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    Lower = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    Upper = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    Punctuation = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    Number = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    Separator = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    Symbol = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    WhiteSpace = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    LowSurrogate = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    HighSurrogate = $Body.AppendChild($CharacterTypes.CreateElement('table'));
    Other = $Body.AppendChild($CharacterTypes.CreateElement('table'));
};
$Keys | ForEach-Object {
    $A = $Ul.AppendChild($CharacterTypes.CreateElement('li')).AppendChild($CharacterTypes.CreateElement('a'));
    $A.Attributes.Append($CharacterTypes.CreateAttribute('href')).Value = "#$_";
    $A.InnerText = $_;
    $Table = $CharTypeTables[$_];
    $A = $Body.InsertAfter($CharacterTypes.CreateElement('a'), $Table);
    $A.Attributes.Append($CharacterTypes.CreateAttribute('href')).Value = '#top';
    $A.InnerText = '(top)';
    $Table.Attributes.Append($CharacterTypes.CreateAttribute('id')).Value = $_;
    $Table.AppendChild($CharacterTypes.CreateElement('caption')).InnerText = "$_ Characters";
    $Tbody = $Table.AppendChild($CharacterTypes.CreateElement('thead'));
    $Tr = $Tbody.AppendChild($CharacterTypes.CreateElement('tr'));
    $Tr.AppendChild($CharacterTypes.CreateElement('th')).InnerText = 'Char';
    $Tr.AppendChild($CharacterTypes.CreateElement('th')).InnerText = 'Category';
    $Tr.AppendChild($CharacterTypes.CreateElement('th')).InnerText = 'Code';
    $Tr.AppendChild($CharacterTypes.CreateElement('th')).InnerText = 'Value';
    $Tr.AppendChild($CharacterTypes.CreateElement('th')).InnerText = 'Entity';
    $Tr.AppendChild($CharacterTypes.CreateElement('th')).InnerText = 'Other Types';
    $CharTypeTables[$_] = $Table.AppendChild($CharacterTypes.CreateElement('tbody'));
}
$Dictionary = [System.Collections.Generic.Dictionary[System.Globalization.UnicodeCategory, [System.Collections.ObjectModel.Collection[char]]]]::new();
$end = [int][char]::MaxValue;
for ($code = 0; $code -lt 10000; $code++) {
    [char]$c = $code;
    [System.Collections.ObjectModel.Collection[char]]$Collection = $null;
    $u = [char]::GetUnicodeCategory($c);
    if (-not $Dictionary.TryGetValue($u, [ref]$Collection)) {
        $Collection = [System.Collections.ObjectModel.Collection[char]]::new();
        $Dictionary.Add($u, $Collection);
    }
    $HasType = $false;
    $Collection.Add($c);
    $Keys = @();
    if ([char]::IsControl($c)) { $Keys += 'Control' }
    if ([char]::IsDigit($c)) { $Keys += 'Digit' }
    if ([char]::IsLetter($c)) {
        if ([char]::IsLower($c)) {
            $Keys += 'Lower';
        } else {
            $Keys += 'Upper';
        }
    }
    if ([char]::IsNumber($c)) { $Keys += 'Number' }
    if ([char]::IsPunctuation($c)) { $Keys += 'Punctuation' }
    if ([char]::IsSeparator($c)) { $Keys += 'Separator' }
    if ([char]::IsSymbol($c)) { $Keys += 'Symbol' }
    if ([char]::IsWhiteSpace($c)) { $Keys += 'WhiteSpace' }
    if ([char]::IsSurrogate($c)) {
        if ([char]::IsLowSurrogate($c)) {
            $Keys += 'LowSurrogate';
        } else {
            $Keys += 'HighSurrogate';
        }
    }
    if ($Keys.Count -eq 0) { $Keys += 'Other' }
    $Keys | ForEach-Object {
        $TBody = $CharTypeTables[$_];
        $Tr = $Tbody.AppendChild($CharacterTypes.CreateElement('tr'));

        if (-not ([char]::IsControl($c) -or [char]::IsSurrogate($c) -or [char]::IsWhiteSpace($c))) {
            $Tr.AppendChild($CharacterTypes.CreateElement('td')).InnerText = "$c";
        } else {
            $Tr.AppendChild($CharacterTypes.CreateElement('td')).InnerText = " ";
        }
        $Tr.AppendChild($CharacterTypes.CreateElement('td')).InnerText = $u.ToString('F');
        $Tr.AppendChild($CharacterTypes.CreateElement('td')).InnerText = $code.ToString();
        $nv = [char]::GetNumericValue($c);
        if ($nv -ne -1) {
            $Tr.AppendChild($CharacterTypes.CreateElement('th')).InnerText = $nv.ToString();
        } else {
            $Tr.AppendChild($CharacterTypes.CreateElement('th')).InnerText = ' ';
        }
        $e = $EntityMap[$code];
        if ($null -ne $e) {
            $Tr.AppendChild($CharacterTypes.CreateElement('td')).InnerText = "&$e;";
        } else {
            if ([char]::IsControl($c) -or [char]::IsSurrogate($c) -or [char]::IsWhiteSpace($c)) {
                $Tr.AppendChild($CharacterTypes.CreateElement('td')).InnerText = "&#$code;";
            } else {
                $Tr.AppendChild($CharacterTypes.CreateElement('td')).InnerText = " ";
            }
        }
        if ($Keys.Count -gt 1) {
            $id = $_;
            $Ul = $Tr.AppendChild($CharacterTypes.CreateElement('td')).AppendChild($CharacterTypes.CreateElement('ul'));
            $Keys | Where-Object { $_ -ne $id } | ForEach-Object {
                $A = $Ul.AppendChild($CharacterTypes.CreateElement('li')).AppendChild($CharacterTypes.CreateElement('a'));
                $A.Attributes.Append($CharacterTypes.CreateAttribute('href')).Value = "#$_";
                $A.InnerText = $_;
            }
        } else {
            $Tr.AppendChild($CharacterTypes.CreateElement('td')).InnerText = ' ';
        }
    }
}

[Xml]$UnicodeCategories = '<html><head><title>Unicode Categories</title></head></html>';
$Body = $UnicodeCategories.DocumentElement.AppendChild($UnicodeCategories.CreateElement('body'));
$Ul = $Body.AppendChild($UnicodeCategories.CreateElement('ul'));
$Ul.Attributes.Append($UnicodeCategories.CreateAttribute('id')).Value = 'top';
foreach ($u in [Enum]::GetValues([System.Globalization.UnicodeCategory])) {
    if ($Dictionary.ContainsKey($u)) {
        $A = $Ul.AppendChild($UnicodeCategories.CreateElement('li')).AppendChild($UnicodeCategories.CreateElement('a'));
        $A.Attributes.Append($UnicodeCategories.CreateAttribute('href')).Value = "#$($u.ToString('F'))";
        $A.InnerText = $u.ToString('F');
    }
}
foreach ($u in [Enum]::GetValues([System.Globalization.UnicodeCategory])) {
    if ($Dictionary.TryGetValue($u, [ref]$Collection)) {
        $Table = $Body.AppendChild($UnicodeCategories.CreateElement('table'));
        $A = $Body.AppendChild($UnicodeCategories.CreateElement('a'));
        $A.Attributes.Append($UnicodeCategories.CreateAttribute('href')).Value = '#top';
        $A.InnerText = '[top]';
        $Table.Attributes.Append($UnicodeCategories.CreateAttribute('id')).Value = $u.ToString('F');
        $Table.AppendChild($UnicodeCategories.CreateElement('caption')).InnerText = $u.ToString('F');
        $Tbody = $Table.AppendChild($UnicodeCategories.CreateElement('thead'));
        $Tr = $Tbody.AppendChild($UnicodeCategories.CreateElement('tr'));
        $Tr.AppendChild($UnicodeCategories.CreateElement('th')).InnerText = 'Char';
        $Tr.AppendChild($UnicodeCategories.CreateElement('th')).InnerText = 'Code';
        $Tr.AppendChild($UnicodeCategories.CreateElement('th')).InnerText = 'Value';
        $Tr.AppendChild($UnicodeCategories.CreateElement('th')).InnerText = 'Entity';
        $Tr.AppendChild($UnicodeCategories.CreateElement('th')).InnerText = 'Type';
        $Tbody = $Table.AppendChild($UnicodeCategories.CreateElement('tbody'));
        foreach ($c in $Collection) {
            [int]$code = $c;
            $Tr = $Tbody.AppendChild($UnicodeCategories.CreateElement('tr'));
            if (-not ([char]::IsControl($c) -or [char]::IsSurrogate($c) -or [char]::IsWhiteSpace($c))) {
                $Tr.AppendChild($UnicodeCategories.CreateElement('td')).InnerText = "$c";
            } else {
                $Tr.AppendChild($UnicodeCategories.CreateElement('td')).InnerText = " ";
            }
            $Tr.AppendChild($UnicodeCategories.CreateElement('td')).InnerText = $code.ToString();
            $nv = [char]::GetNumericValue($c);
            if ($nv -ne -1) {
                $Tr.AppendChild($UnicodeCategories.CreateElement('th')).InnerText = $nv.ToString();
            } else {
                $Tr.AppendChild($UnicodeCategories.CreateElement('th')).InnerText = ' ';
            }
            $e = $EntityMap[$code];
            if ($null -ne $e) {
                $Tr.AppendChild($UnicodeCategories.CreateElement('td')).InnerText = "&$e;";
            } else {
                if ([char]::IsControl($c) -or [char]::IsSurrogate($c) -or [char]::IsWhiteSpace($c)) {
                    $Tr.AppendChild($UnicodeCategories.CreateElement('td')).InnerText = "&#$code;";
                } else {
                    $Tr.AppendChild($UnicodeCategories.CreateElement('td')).InnerText = " ";
                }
            }
            $Types = @();
            if ([char]::IsControl($c)) { $Types += 'Control' }
            if ([char]::IsDigit($c)) { $Types += 'Digit' }
            if ([char]::IsLetter($c)) {
                if ([char]::IsLower($c)) {
                    $Types += "Lower Case";
                } else {
                    $Types += "Upper Case";
                }
            }
            if ([char]::IsNumber($c)) { $Types += 'Number' }
            if ([char]::IsPunctuation($c)) { $Types += 'Punctuation' }
            if ([char]::IsSeparator($c)) { $Types += 'Separator' }
            if ([char]::IsSymbol($c)) { $Types += 'Symbol' }
            if ([char]::IsWhiteSpace($c)) { $Types += 'WhiteSpace' }
            if ([char]::IsSurrogate($c)) {
                if ([char]::IsLowSurrogate($c)) {
                    $Types += "Low Surrogate";
                } else {
                    $Types += "High Surrogate";
                }
            }
            if ($Types.Count -gt 0) {
                $Tr.AppendChild($UnicodeCategories.CreateElement('td')).InnerText = $Types -join ', ';
            } else {
                $Tr.AppendChild($UnicodeCategories.CreateElement('td')).InnerText = " ";
            }
        }
    }
}
$BasePath = $PSScriptRoot | Join-Path -ChildPath 'Output';
if (-not ($BasePath | Test-Path)) { (New-Item -Path $PSScriptRoot -ItemType Directory -Name 'Output' -ErrorAction Stop) | Out-Null; }
$Writer = [System.Xml.XmlWriter]::Create(($BasePath | Join-Path -ChildPath 'UnicodeCategories.html'), ([System.Xml.XmlWriterSettings]@{
    Indent = $true;
    Encoding = [System.Text.UTF8Encoding]::new($false, $true);
}));
try {
    $UnicodeCategories.WriteTo($Writer);
    $Writer.Flush();
} finally { $Writer.Close() }
$Writer = [System.Xml.XmlWriter]::Create(($BasePath | Join-Path -ChildPath 'CharacterTypes.html'), ([System.Xml.XmlWriterSettings]@{
    Indent = $true;
    Encoding = [System.Text.UTF8Encoding]::new($false, $true);
}));
try {
    $CharacterTypes.WriteTo($Writer);
    $Writer.Flush();
} finally { $Writer.Close() }
