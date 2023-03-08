using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CdnSync.SettingsSections;

public class CdnSyncSettings
{
    public string? dbFile;

    public CdnJsSettings? cdnJs;

    public string GetDbFile() { return dbFile.ToTrimmedOrDefaultIfEmpty(CdnSyncDb.DEFAULT_DbFile); }
}