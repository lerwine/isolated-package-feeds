$LocalRepoPath = $PSScriptRoot | Join-Path -ChildPath 'ExampleLocalNuGetRepo';
$UpstreamRepoPath = $PSScriptRoot | Join-Path -ChildPath 'ExampleUpstreamNuGetRepo';
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