class NuGetPackage {
    [bool]$ExistsLocally;
    [System.IO.FileInfo]$LocalArchive;
    [System.IO.FileInfo]$UpstreamArchive;
    [System.Collections.Generic.Dictionary[string,System.Collections.ObjectModel.Collection[Dependency]]]$Dependencies = [System.Collections.Generic.Dictionary[string,System.Collections.ObjectModel.Collection[Dependency]]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
    NuGetPackage([bool]$ExistsLocally) { $this.ExistsLocally = $ExistsLocally }
    static [void] ImportFromJson([PSObject]$JsonObj, [System.Collections.Generic.Dictionary[string,System.Collections.Generic.Dictionary[string,NuGetPackage]]]$ById) {
        [System.Collections.Generic.Dictionary[string,NuGetPackage]]$ByVersion = $null;
        if (-not $ById.TryGetValue($JsonObj.ID, [ref]$ByVersion)) {
            $ByVersion = [System.Collections.Generic.Dictionary[string,NuGetPackage]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
            $ById.Add($JsonObj.ID, $ByVersion);
        }
        $JsonObj.Versions | ForEach-Object {
            [NuGetPackage]$NuGetPackage = $null;
            if (-not $ByVersion.TryGetValue($_.Version, [ref]$NuGetPackage)) {
                $NuGetPackage = [NuGetPackage]::new(($_.ExistsLocally -eq $true));
                $ByVersion.Add($_.Version, $NuGetPackage);
            }
            if ($null -ne $_.DependencyGroups) {
                $_.DependencyGroups | ForEach-Object {
                    [System.Collections.ObjectModel.Collection[Dependency]]$Dependencies = $null;
                    if ($NuGetPackage.Dependencies.TryGetValue($_.TargetFramework, [ref]$Dependencies)) {
                        $_.Dependencies | ForEach-Object {
                            if ([string]::IsNullOrWhiteSpace($_.Version)) {
                                foreach ($obj in $Dependencies) {
                                    if ($null -eq $obj.Version -and $obj.ID -ieq $d.ID) {
                                        $d = $obj;
                                        break;
                                    }
                                }
                            } else {
                                foreach ($obj in $Dependencies) {
                                    if ($null -ne $obj.Version -and $obj.ID -ieq $d.ID -and $obj.Version -ieq $d.Version) {
                                        $d = $obj;
                                        break;
                                    }
                                }
                            }
                            if ($null -eq $d) {
                                $d = [Dependency]::new($_.ID, $_.Version);
                                if ($null -ne $_.Exclude) {
                                    $_.Exclude | ForEach-Object { $d.Exclude.Add($_) }
                                }
                                $Dependencies.Add($d);
                            } else {
                                if ($null -ne $_.Exclude) {
                                    $_.Exclude | ForEach-Object {
                                        if (-not $d.Exclude.Contains($_)) { $d.Exclude.Add($_) }
                                    }
                                }
                            }
                        }
                    } else {
                        $Dependencies = [System.Collections.ObjectModel.Collection[Dependency]]::new();
                        $_.Dependencies | ForEach-Object {
                            $d = [Dependency]::new($_.ID, $_.Version);
                            if ($null -ne $_.Exclude) {
                                $_.Exclude | ForEach-Object { $d.Exclude.Add($_) }
                            }
                            $Dependencies.Add($d);
                        }
                        $NuGetPackage.Dependencies.Add($_.TargetFramework, $Dependencies);
                    }
                }
            }
        }
    }
}
class Dependency {
    [string]$ID;
    [string]$Version = '';
    Dependency([string]$ID, [string]$Version) {
        $this.ID = $ID;
        if ($null -eq $Version) { $this.Version = '' } else { $this.Version = $Version }
    }
    [System.Collections.Generic.HashSet[string]]$Exclude = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
}

Function Read-NuSpec {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [System.IO.FileInfo]$Archive,
        
        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string,System.Collections.Generic.Dictionary[string,NuGetPackage]]]$ById,

        [switch]$IsLocal
    )

    $TempDir = [System.IO.Path]::GetTempPath() | Join-Path -ChildPath ([Guid]::NewGuid().ToString('n'));
    (New-Item -Path $TempDir -ItemType Directory -Force) | Out-Null;
    try {
        $ZipFileName = $TempDir | Join-Path -ChildPath "$($Archive.BaseName).zip";
        Copy-Item -LiteralPath $Archive.FullName -Destination $ZipFileName -Force;
        $ContentsDir = $TempDir | Join-Path -ChildPath 'Contents';
        (New-Item -Path $ContentsDir -ItemType Directory -Force) | Out-Null;
        (Expand-Archive -LiteralPath $ZipFileName -DestinationPath $ContentsDir -Force) | Out-Null;
        (Get-ChildItem -LiteralPath $ContentsDir -Filter '*.nuspec') | ForEach-Object {
            [xml]$Xml = (Get-Content -LiteralPath $_.FullName);
            $nsmgr = [System.Xml.XmlNamespaceManager]::new($Xml.NameTable);
            $nsmgr.AddNamespace("n", $Xml.DocumentElement.NamespaceURI);
            $MetaData = $Xml.DocumentElement.SelectSingleNode('n:metadata', $nsmgr);
            if ($null -ne $MetaData) {
                $ID = '' + $MetaData.id;
                if ([string]::IsNullOrWhiteSpace($ID)) {
                    Write-Warning -Message "Empty ID for $($Archive.FullName)";
                } else {
                    if ($ID.Contains(' ')) {
                        Write-Warning -Message "Invalid ID for $($Archive.FullName)";
                    }
                }

                $Version = '' + $MetaData.version;
                [System.Collections.Generic.Dictionary[string,NuGetPackage]]$ByVersion = $null;
                if (-not $ById.TryGetValue($ID, [ref]$ByVersion)) {
                    $ByVersion = [System.Collections.Generic.Dictionary[string,NuGetPackage]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
                    $ById.Add($ID, $ByVersion);
                }
                [NuGetPackage]$NuGetPackage = $null;
                if ($ByVersion.TryGetValue($Version, [ref]$NuGetPackage)) {
                    if ($IsLocal.IsPresent) { $NuGetPackage.ExistsLocally = $true; }
                } else {
                    $NuGetPackage = [NuGetPackage]::new($IsLocal.IsPresent);
                    $ByVersion.Add($Version, $NuGetPackage);
                }
                if ($IsLocal.IsPresent) {
                    $NuGetPackage.LocalArchive = $Archive;
                } else {
                    $NuGetPackage.UpstreamArchive = $Archive;
                }
                @($MetaData.SelectNodes('n:dependencies/n:group', $nsmgr)) | ForEach-Object {
                    $TargetFramework = '' + $_.targetFramework;
                    [System.Collections.ObjectModel.Collection[Dependency]]$Dependencies = $null;
                    if (-not $NuGetPackage.Dependencies.TryGetValue($TargetFramework, [ref]$Dependencies)) {
                        $Dependencies = [System.Collections.ObjectModel.Collection[Dependency]]::new();
                        $NuGetPackage.Dependencies.Add($TargetFramework, $Dependencies);
                    }
                    @($_.SelectNodes('n:dependency', $nsmgr)) | ForEach-Object {
                        $ID = '' + $_.id;
                        $Version = '' + $_.version;
                        $Exclude = '' + $_.exclude;
                        [Dependency]$dep = $null;
                        if ([string]::IsNullOrWhiteSpace($Version)) { $Version = '' }
                        foreach ($d in $Dependencies) {
                            if ($d.ID -ieq $ID -and $d.Version -ieq $Version) {
                                $dep = $d;
                                break;
                            }
                        }
                        if ($null -eq $d) {
                            $d = [Dependency]::new($_.ID, $_.Version);
                            $Dependencies.Add($d);
                            if (-not [string]::IsNullOrWhiteSpace($Exclude)) {
                                $Exclude.Split(',') | ForEach-Object { $d.Exclude.Add($_) | Out-Null }
                            }
                        } else {
                            if (-not [string]::IsNullOrWhiteSpace($Exclude)) {
                                $Exclude.Split(',') | ForEach-Object {
                                    if (-not $d.Exclude.Contains($_)) { $d.Exclude.Add($_) | Out-Null }
                                }
                            }
                        }
                    }
                }
            }
        }
    } finally {
        if ($TempDir | Test-Path) {
            Remove-Item -LiteralPath $TempDir -Recurse -Force -ErrorAction Continue;
        }
    }
}

$JsonPath = $PSScriptRoot | Join-Path -ChildPath 'DownloadedNuGetPackages.json';
$PackagesById = [System.Collections.Generic.Dictionary[string,System.Collections.Generic.Dictionary[string,NuGetPackage]]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
if ($JsonPath | Test-Path) {
    $Text = (Get-Content -LiteralPath $JsonPath | Out-String).Trim();
    if ($Text.Length -gt 0) {
        ($Text | ConvertFrom-Json) | ForEach-Object { [NuGetPackage]::ImportFromJson($_, $PackagesById) }
    }
}

$Path = $PSScriptRoot | Join-Path -ChildPath 'ExampleLocalNuGetRepo';
$LocalRepoItems = @($Path | Get-ChildItem -Filter '*.nupkg');
$Path = $PSScriptRoot | Join-Path -ChildPath 'ExampleUpstreamNuGetRepo';
$UpstreamRepoItems = @($Path | Get-ChildItem -Filter '*.nupkg');
$LocalCount = $LocalRepoItems.Count;
[double]$TotalCount = $LocalCount + $UpstreamRepoItems.Count;
for ($i = 0; $i -lt $LocalCount; $i++) {
    $FileInfo = $LocalRepoItems[$i];
    Write-Progress -Activity 'Reading repository folders' -Status 'Example Local' -CurrentOperation $FileInfo.Name -PercentComplete ([int]((([double]$i) / $TotalCount) * 100.0));
    Read-NuSpec -Archive $FileInfo -ById $PackagesById -IsLocal;
}
for ($i = 0; $i -lt $UpstreamRepoItems.Count; $i++) {
    $FileInfo = $UpstreamRepoItems[$i];
    Write-Progress -Activity 'Reading repository folders' -Status 'Example Upstream' -CurrentOperation $FileInfo.Name -PercentComplete ([int]((([double]$i + $LocalCount) / $TotalCount) * 100.0));
    Read-NuSpec -Archive $FileInfo -ById $PackagesById;
}
Write-Progress -Activity 'Reading repository folders' -Status 'Finished' -PercentComplete 100 -Completed;

($PackagesById.Keys | Sort-Object | ForEach-Object {
    $ByVersion = $PackagesById[$_];
    [PSCustomObject]@{
        ID = $_;
        Versions = ([PSCustomObject[]]@($ByVersion.Keys | Sort-Object | ForEach-Object {
            $NuGetPackage = $ByVersion[$_];
            $PackageItem = $null;
            if ($_.Length -eq 0) {
                $PackageItem = [PSCustomObject]@{
                    ExistsLocally = $NuGetPackage.ExistsLocally;
                };
            } else {
                $PackageItem = [PSCustomObject]@{
                    Version = $_;
                    ExistsLocally = $NuGetPackage.ExistsLocally;
                };
            }
            if ($NuGetPackage.Dependencies.Count -gt 0) {
                $PackageItem | Add-Member -MemberType NoteProperty -Name 'DependencyGroups' -Value ([PSCustomObject[]]@(
                    ($NuGetPackage.Dependencies.Keys | Sort-Object | ForEach-Object {
                        $Grp = [PSCustomObject]@{ TargetFramework = $_ };
                        $dg = $NuGetPackage.Dependencies[$_];
                        if ($dg.Count -gt 0) {
                            $Grp | Add-Member -MemberType NoteProperty -Name 'Dependencies' -Value ([PSCustomObject[]]@(
                                ($dg | Sort-Object -Property 'ID') | ForEach-Object {
                                    $obj = [PSCustomObject]@{ ID = $_.ID};
                                    if ($_.Version.Length -gt 0) { $obj | Add-Member -MemberType NoteProperty -Name 'Version' -Value $_.Version }
                                    if ($_.Exclude.Count -gt 0) {
                                        $obj | Add-Member -MemberType NoteProperty -Name 'Exclude' -Value ([string[]]@($_.Exclude)) -PassThru;
                                    } else {
                                        $obj | Write-Output;
                                    }
                                }
                            )) -PassThru;
                        } else {
                            $Grp | Write-Output;
                        }
                    })
                ));
                
                #$PackageItem | Add-Member -MemberType NoteProperty -Name 'Dependencies' -Value ([PSCustomObject[]]@($NuGetPackage.Dependencies));
            }
            if ($null -ne $NuGetPackage.LocalArchive) {
                $PackageItem | Add-Member -MemberType NoteProperty -Name 'FileName' -Value $NuGetPackage.LocalArchive.Name -PassThru;
            } else {
                if ($null -ne $NuGetPackage.UpstreamArchive) {
                    $PackageItem | Add-Member -MemberType NoteProperty -Name 'FileName' -Value $NuGetPackage.UpstreamArchive.Name -PassThru;
                } else {
                    $PackageItem | Write-Output;
                }
            }
        }));
    };
} | ConvertTo-Json -Depth 7) | Out-File -LiteralPath $JsonPath -Encoding utf8;

$MissingPackages = [System.Collections.Generic.Dictionary[string,System.Collections.Generic.Dictionary[string,bool]]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
$PackagesById.Keys | Sort-Object | ForEach-Object {
    $ID = $_;
    $ByVersion.Keys | Sort-Object | ForEach-Object {
        $Version = $_;
        [NuGetPackage]$NuGetPackage = $ByVersion[$_];
        if ($null -eq $NuGetPackage.UpstreamArchive) {
            if ($null -eq $NuGetPackage.LocalArchive) {
                [System.Collections.Generic.Dictionary[string,bool]]$Dict = $null;
                if (-not $MissingPackages.TryGetValue($ID, [ref]$Dict)) {
                    $Dict = [System.Collections.Generic.Dictionary[string,bool]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
                    $MissingPackages.Add($ID, $Dict);
                }
                $Dict.Add($Version, $NuGetPackage.ExistsLocally);
            } else {
                $NuGetPackage.UpstreamArchive = [System.IO.FileInfo]::new(($UpstreamRepoPath | Join-Path -ChildPath $NuGetPackage.UpstreamArchive.Name));
                if (-not $NuGetPackage.UpstreamArchive.Exists) {
                    Write-Warning -Message "Copying $($NuGetPackage.LocalArchive.Name) to upstream example repository";
                    Copy-Item -LiteralPath $NuGetPackage.LocalArchive.FullName -Destination $NuGetPackage.UpstreamArchive.FullName -Force;
                    $NuGetPackage.UpstreamArchive.Refresh();
                }
            }
        } else {
            if ($NuGetPackage.ExistsLocally) {
                $NuGetPackage.LocalArchive = [System.IO.FileInfo]::new(($LocalRepoPath | Join-Path -ChildPath $NuGetPackage.UpstreamArchive.Name));
                if (-not $NuGetPackage.LocalArchive.Exists) {
                    Write-Warning -Message "Copying $($NuGetPackage.UpstreamArchive.Name) to local example repository";
                    Copy-Item -LiteralPath $NuGetPackage.UpstreamArchive.FullName -Destination $NuGetPackage.LocalArchive.FullName -Force;
                    $NuGetPackage.LocalArchive.Refresh();
                }
            }
        }
    }
}
$PackagesById.Keys | Sort-Object | ForEach-Object {
    $ID = $_;
    $ByVersion.Keys | Sort-Object | ForEach-Object {
        [NuGetPackage]$NuGetPackage = $ByVersion[$_];
        if ($NuGetPackage.Dependencies.Count -gt 0) {
            $NuGetPackage.Dependencies.Keys | ForEach-Object {
                [System.Collections.ObjectModel.Collection[Dependency]]$Dependencies = $NuGetPackage.Dependencies[$_];
                if ($Dependencies.Count -gt 0) {
                    $Dependencies | ForEach-Object {
                        $ID = $_.ID;
                        $Version = $_.Version;
                        [System.Collections.Generic.Dictionary[string,NuGetPackage]]$Dict = $null;
                        if ($PackagesById.TryGetValue($_.ID, [ref]$Dict)) {
                            # TODO: Validate dependencies
                        } else {
                            [System.Collections.Generic.Dictionary[string,bool]]$m = $null;
                            if ($MissingPackages.TryGetValue($ID, [ref]$m)) {
                                if ($NuGetPackage.ExistsLocally) {
                                    if ($m.ContainsKey($Version)) { $m[$Version] = $true } else { $m.Add($Version, $true) }
                                } else {
                                    if (-not $m.ContainsKey($Version)) { $m.Add($Version, $false) }
                                }
                            } else {
                                $m = [System.Collections.Generic.Dictionary[string,bool]]::new([System.StringComparer]::InvariantCultureIgnoreCase);
                                $MissingPackages.Add($ID, $m);
                                $m.Add($Version, $NuGetPackage.ExistsLocally);
                            }
                        }
                    }
                }
            }
        }
    }
}
foreach ($ID in $MissingPackages.Keys) {
    $Dict = $MissingPackages[$ID];
    foreach ($Version in $Dict.Keys) {
        if ($Dict[$Version]) {
            Write-Warning -Message "$ID $Version is missing from local and upstream example repositories";
        } else {
            Write-Warning -Message "$ID $Version is missing from upstream example repository";
        }
    }
}
<#
$TempPathRoot = [System.IO.Path]::GetTempPath();
($LocalRepoPath | Get-ChildItem -Filter '*.nupkg') | ForEach-Object {
    $SourceName = $_.Name;
    $TempDir = $TempPathRoot | Join-Path -ChildPath ([Guid]::NewGuid('n'));
    (New-Item -Path $TempDir -ItemType Directory -Force) | Out-Null;
    try {
        Expand-Archive -LiteralPath $_.FullName -DestinationPath $TempDir;
        (Get-ChildItem -LiteralPath $TempDir -Filter '*.nuspec') | ForEach-Object {
            [xml]$Xml = (Get-Content -LiteralPath $_.FullName);
            $nsmgr = [System.Xml.XmlNamespaceManager]::new($Xml.NameTable);
            $nsmgr.AddNamespace("n", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");
            $ID = '' + $Xml.DocumentElement.metadata.id;
            $Version = '' + $Xml.DocumentElement.metadata.version;
            [PSCustomObject]@{
                ID = '' + $Xml.DocumentElement.metadata.id;
                Version = '' + $Xml.DocumentElement.metadata.version;
                ExistsLocally = $false;
                DependencyGroups = ([PSCustomObject[]]@(@($Xml.DocumentElement.metadata.dependencies.SelectNodes('n:group', $nsmgr)) | ForEach-Object {
                    [PSCustomObject]@{
                        TargetFramework = '' + $_.targetFramework;
                        Dependencies = ([PSCustomObject[]]@(@($_.SelectNodes('n:dependency', $nsmgr)) | ForEach-Object {
                            $Version = '' + $_.version;
                            $Exclude = '' + $_.exclude;
                            $Item = [PSCustomObject]@{ ID = '' + $_.id };
                            if (-not [string]::IsNullOrWhiteSpace($Version)) { $Item | Add-Member -MemberType NoteProperty -Name 'Version' -Value $Version }
                            if (-not [string]::IsNullOrWhiteSpace($Exclude)) { $Item | Add-Member -MemberType NoteProperty -Name 'Exclude' -Value ([string[]]@($Exclude.Split(','))) }
                            $Item | Write-Output;
                        }));
                    };
                }));
            };
        }
    } finally {
        if ($TempDir | Test-Path) {
            Remove-Item -$TempDir -Recurse -Force -ErrorAction Continue;
        }
    }
    $_.Name
}
<#
$AllPackages = @((Get-Content -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'DownloadedNuGetPackages.json')) | ConvertFrom-Json);
@($AllPackages | ForEach-Object {
    $id = $_.ID.Trim();
    $_.Versions | Where-Object { -not $_.ExistsLocally } | ForEach-Object {
        if ($null -eq $_.Version) { "$id.nupkg" } else { "$id.$($_.Version.Trim()).nupkg" }
        $_.DependencyGroups | ForEach-Object {
            $_.Dependencies | ForEach-Object {
                if ($null -eq $_.Version) { "$($_.ID.Trim()).nupkg" } else { "$($_.ID.Trim()).$($_.Version.Trim()).nupkg" }
            }
        }
    }
} | Select-Object -Unique) | ForEach-Object {
    if (-not (($UpstreamRepoPath | Join-Path -ChildPath $_) | Test-Path -PathType Leaf)) {
        "Missing from Upstream: $_" | Write-Warning;
    }
}
@($AllPackages | ForEach-Object {
    $id = $_.ID.trim();
    $_.Versions | Where-Object { $_.ExistsLocally } | ForEach-Object {
        if ($null -eq $_.Version) { "$id.nupkg" } else { "$id.$($_.Version.Trim()).nupkg" }
        $_.DependencyGroups | ForEach-Object {
            $_.Dependencies | ForEach-Object {
                if ($null -eq $_.Version) { "$($_.ID.Trim()).nupkg" } else { "$($_.ID.Trim()).$($_.Version.Trim()).nupkg" }
            }
        }
    }
} | Select-Object -Unique) | ForEach-Object {
    if (($LocalRepoPath | Join-Path -ChildPath $_) | Test-Path -PathType Leaf) {
        if (-not (($UpstreamRepoPath | Join-Path -ChildPath $_) | Test-Path -PathType Leaf)) {
            "Missing from Upstream: $_" | Write-Warning;
        }
    } else {
        if (($UpstreamRepoPath | Join-Path -ChildPath $_) | Test-Path -PathType Leaf) {
            "Missing from Local: $_" | Write-Warning;
        } else {
            "Missing from Both: $_" | Write-Warning;
        }
    }
}
#>