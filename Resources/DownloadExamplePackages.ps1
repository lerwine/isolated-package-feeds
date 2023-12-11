[Uri]$BaseUri = 'https://www.nuget.org/api/v2/package/';
# System.Text.Json/8.0.0-preview.7.23375.6
$LocalRepoPath = $PSScriptRoot | Join-Path -ChildPath 'ExampleLocalNuGetRepo';
$UpstreamRepoPath = $PSScriptRoot | Join-Path -ChildPath 'ExampleUpstreamNuGetRepo';
$AllPackages = @((Get-Content -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'DownloadedNuGetPackages.json')) | ConvertFrom-Json);
$AllPackages | ForEach-Object {
    $ID = $_.ID;
    $_.Versions | ForEach-Object {
        $Name = $ID;
        if ($null -ne $_.Version) { $Name = "$Name.$($_.Version)" }
        $Path = $UpstreamRepoPath | Join-Path -ChildPath "$Name.nupkg";
        if ($Path | Test-Path -PathType Leaf) {
            Write-Information -MessageData "$Name already exists in upstream repo";
        } else {
            $UriLeaf = $ID;
            if ($null -ne $_.Version) { $UriLeaf = "$UriLeaf/$($_.Version)" }
            $Uri = [Uri]::new($BaseUri, "$UriLeaf");
            # TODO: Download
        }
        # if ($_.ExistsLocally) {
        #    # TODO: Copy to local
        # }
    }
}