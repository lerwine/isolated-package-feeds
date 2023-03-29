namespace CdnGetter;

public class SqlDefinitions
{
    public const string COLLATION_NOCASE = "NOCASE";
    public const string DEFAULT_SQL_NOW = "(datetime('now','localtime'))";
    public const int MAX_LENGTH_Url = 4096;
    public const int MAX_LENGTH_FileName = 256;
    public const int MAXLENGTH_SRI = 256;
    public const int MAXLENGTH_ContentType = 512;
    public const int MAXLENGTH_Encoding = 32;
    public const byte DEFAULT_VALUE_Action = 0;
    
    public static string SqlUniqueIdentifier(string colName, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" UNIQUEIDENTIFIER DEFAULT NULL COLLATE NOCASE";
        return $"\"{colName}\" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE";
    }
    
    public static string SqlReferenceColumn(string targetEntity, string targetColName, string referencedEntity, string refColName, string referencedTable, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{targetColName}\" UNIQUEIDENTIFIER DEFAULT NULL CONSTRAINT \"FK_{targetEntity}_{referencedEntity}\" REFERENCES \"{referencedTable}\"(\"{refColName}\") ON DELETE RESTRICT COLLATE NOCASE";
        return $"\"{targetColName}\" UNIQUEIDENTIFIER NOT NULL CONSTRAINT \"FK_{targetEntity}_{referencedEntity}\" REFERENCES \"{referencedTable}\"(\"{refColName}\") ON DELETE RESTRICT COLLATE NOCASE";
    }
    
    public static string SqlSmallUInt(string colName, uint defaultValue) => $"\"{colName}\" UNSIGNED SMALLINT NOT NULL DEFAULT {defaultValue}";
    
    public static string SqlSmallUInt(string colName, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" UNSIGNED SMALLINT DEFAULT NULL";
        return $"\"{colName}\" UNSIGNED SMALLINT NOT NULL";
    }
    
    public static string VarCharTrimmedNotEmptyNoCase(string colName, int maxLength, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" NVARCHAR({maxLength}) DEFAULT NULL CHECK(\"{colName}\" IS NULL OR (length(trim(\"{colName}\"))=length(\"{colName}\") AND length(\"{colName}\")>0)) COLLATE NOCASE";
        return $"\"{colName}\" NVARCHAR({maxLength}) NOT NULL CHECK(length(trim(\"{colName}\"))=length(\"{colName}\") AND length(\"{colName}\")>0) COLLATE NOCASE";
    }

    public static string VarCharTrimmedNoCase(string colName, int maxLength, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" NVARCHAR({maxLength}) DEFAULT NULL CHECK(\"{colName}\" IS NULL OR length(trim(\"{colName}\"))=length(\"{colName}\")) COLLATE NOCASE";
        return $"\"{colName}\" NVARCHAR({maxLength}) NOT NULL CHECK(length(trim(\"{colName}\"))=length(\"{colName}\")) COLLATE NOCASE";
    }

    public static string VarCharTrimmedNotEmpty(string colName, int maxLength, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" NVARCHAR({maxLength}) DEFAULT NULL CHECK(\"{colName}\" IS NULL OR (length(trim(\"{colName}\"))=length(\"{colName}\") AND length(\"{colName}\")>0))";
        return $"\"{colName}\" NVARCHAR({maxLength}) NOT NULL CHECK(length(trim(\"{colName}\"))=length(\"{colName}\") AND length(\"{colName}\")>0)";
    }

    public static string VarCharTrimmed(string colName, int maxLength, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" NVARCHAR({maxLength}) DEFAULT NULL CHECK(\"{colName}\" IS NULL OR length(trim(\"{colName}\"))=length(\"{colName}\"))";
        return $"\"{colName}\" NVARCHAR({maxLength}) NOT NULL CHECK(length(trim(\"{colName}\"))=length(\"{colName}\"))";
    }

    public static string VarChar(string colName, int maxLength, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" NVARCHAR({maxLength}) DEFAULT NULL";
        return $"\"{colName}\" NVARCHAR({maxLength}) NOT NULL";
    }

    public static string SqlTextTrimmedNotEmpty(string colName, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" TEXT DEFAULT NULL CHECK(\"{colName}\" IS NULL OR (length(trim(\"{colName}\"))=length(\"{colName}\") AND length(\"{colName}\")>0))";
        return $"\"{colName}\" TEXT NOT NULL CHECK(length(trim(\"{colName}\"))=length(\"{colName}\") AND length(\"{colName}\")>0)";
    }

    public static string SqlTextTrimmed(string colName, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" TEXT DEFAULT NULL CHECK(\"{colName}\" IS NULL OR length(trim(\"{colName}\"))=length(\"{colName}\"))";
        return $"\"{colName}\" TEXT NOT NULL CHECK(length(trim(\"{colName}\"))=length(\"{colName}\"))";
    }

    public static string SqlTextNotEmpty(string colName, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" TEXT DEFAULT NULL CHECK(\"{colName}\" IS NULL OR length(\"{colName}\")>0)";
        return $"\"{colName}\" TEXT NOT NULL CHECK(length(\"{colName}\")>0)";
    }

    public static string SqlText(string colName, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" TEXT DEFAULT NULL";
        return $"\"{colName}\" TEXT NOT NULL";
    }

    public static string SqlDateTime(string colName, bool allowNull = false)
    {
        if (allowNull)
            return $"\"{colName}\" DATETIME DEFAULT NULL";
        return $"\"{colName}\" DATETIME NOT NULL DEFAULT (datetime('now','localtime'))";
    }

    public static string SqlPkConstraint(string tableName, string colName, params string[] compoundColNames)
    {
        if (compoundColNames is null || $"CONSTRAINT \"PK_{tableName}\" PRIMARY KEY(\"{colName}\")".Length == 0)
            return $"CONSTRAINT \"PK_{tableName}\" PRIMARY KEY(\"{colName}\")";
        string n = string.Join("\", \"", compoundColNames);
        return $"CONSTRAINT \"PK_{tableName}\" PRIMARY KEY(\"{colName}\", \"{n}\")";
    }

    public static string SqlUniqueConstraint(string entityName, string colName) => $"CONSTRAINT \"UK_{entityName}_{colName}\" UNIQUE(\"{colName}\")";

    public static string SqlCompoundUniqueConstraint(string entityName, string indexName, string colName1, string colName2, params string[] compoundColNames)
    {
        if (compoundColNames is null || compoundColNames.Length == 0)
            return $"CONSTRAINT \"UK_{entityName}_{indexName}\" UNIQUE(\"{colName1}\", \"{colName2}\")";
        string n = string.Join("\", \"", compoundColNames);
        return $"CONSTRAINT \"UK_{entityName}_{indexName}\" UNIQUE(\"{colName1}\", \"{colName2}\", \"{n}\")";
    }

    public static string SqlIndex(string tableName, string colName, bool noCase = false)
    {
        if (noCase)
            return $"CREATE INDEX \"IDX_{tableName}_{colName}\" ON \"{tableName}\" (\"{colName}\" COLLATE NOCASE)";
        return $"CREATE INDEX \"IDX_{tableName}_{colName}\" ON \"{tableName}\" (\"{colName}\")";
    }
}