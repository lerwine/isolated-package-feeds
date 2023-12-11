(@((Get-ChildItem -Path $PSScriptRoot -Filter '*.nuspec') | ForEach-Object {
    [xml]$Xml = (Get-Content -LiteralPath $_.FullName);
    $nsmgr = [System.Xml.XmlNamespaceManager]::new($Xml.NameTable);
    $nsmgr.AddNamespace("n", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");
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
}) | ConvertTo-Json -Depth 5) | Out-File ($PSScriptRoot | Join-Path -ChildPath 'NuspecToJson.json') -Force;
