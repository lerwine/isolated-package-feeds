$Response = Invoke-WebRequest -Uri 'https://api.cdnjs.com/libraries?fields=description' -Headers @{ Accept = 'text/json' } -Method Get;
$Response.Content | Out-File -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'CdnJsLibraries.json') -Force;
$Libraries = $Response.Content | ConvertFrom-Json;
$Count = $Libraries.results.Count;
$Index = 0;
$v = $null
$Libraries.results | ForEach-Object {
    $UriBuilder = [System.UriBuilder]::new("https://api.cdnjs.com");
    $UriBuilder.Path = "/libraries/$($_.name)";
    $UriBuilder.Query = 'fields=name,sri,description,versions';
    Write-Progress -Activity 'Downloading Content' -Status 'CDNJS' -CurrentOperation $UriBuilder.Uri.AbsoluteUri -PercentComplete ($Index * 100 / $Count);
    $Index++;
    $Response = Invoke-WebRequest -Uri $UriBuilder.Uri.AbsoluteUri -Headers @{ Accept = 'text/json' } -Method Get;
    $v = $Response.Content | ConvertFrom-Json;
    if (@($v.versions | Where-Object { $_ -inotmatch '^[a-z]*\d+(\.\d+[a-z]*(\.\d+[a-z]*(\.\d+[a-z]*)?)?)?(\.?[a-z]+)?([-+][^-+]+)*$' }).Count -gt 0) {
        $Response.Content | Out-File -LiteralPath ($PSScriptRoot | Join-Path -ChildPath "$($_.name).json") -Force;
    }
}