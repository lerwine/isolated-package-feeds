DROP TABLE IF EXISTS "FileLogs";
DROP TABLE IF EXISTS "RemoteFiles";
DROP TABLE IF EXISTS "LocalFiles";
DROP TABLE IF EXISTS "VersionLogs";
DROP TABLE IF EXISTS "RemoteVersions";
DROP TABLE IF EXISTS "LocalVersions";
DROP TABLE IF EXISTS "LibraryLogs";
DROP TABLE IF EXISTS "RemoteLibraries";
DROP TABLE IF EXISTS "LocalLibraries";
DROP TABLE IF EXISTS "RemoteServices";

CREATE TABLE IF NOT EXISTS "RemoteServices" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Name" NVARCHAR(1024) NOT NULL CHECK(length(trim("Name"))=length("Name") AND length("Name")>0) COLLATE NOCASE,
    "Priority" UNSIGNED SMALLINT NOT NULL DEFAULT 65535,
    "Description" TEXT NOT NULL CHECK(length(trim("Description"))=length("Description")),
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    CONSTRAINT "PK_RemoteServices" PRIMARY KEY("Id"),
    CONSTRAINT "UK_RemoteService_Name" UNIQUE("Name"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE INDEX "IDX_RemoteServices_Priority" ON "RemoteServices" ("Priority");
CREATE INDEX "IDX_RemoteServices_Name" ON "RemoteServices" ("Name" COLLATE NOCASE);

CREATE TABLE IF NOT EXISTS "LocalLibraries" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Name" NVARCHAR(1024) NOT NULL CHECK(length(trim("Name"))=length("Name") AND length("Name")>0) COLLATE NOCASE,
    "Description" TEXT NOT NULL CHECK(length(trim("Description"))=length("Description")),
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    CONSTRAINT "PK_LocalLibraries" PRIMARY KEY("Id"),
    CONSTRAINT "UK_LocalLibrary_Name" UNIQUE("Name"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE INDEX "IDX_LocalLibraries_Name" ON "LocalLibraries" ("Name" COLLATE NOCASE);

CREATE TABLE IF NOT EXISTS "RemoteLibraries" (
    "LocalId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_RemoteLibrary_LocalLibrary" REFERENCES "LocalLibraries"("Id") ON DELETE RESTRICT COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_RemoteLibrary_RemoteService" REFERENCES "RemoteServices"("Id") ON DELETE RESTRICT COLLATE NOCASE,
    "Priority" UNSIGNED SMALLINT DEFAULT NULL,
    "Description" TEXT NOT NULL CHECK(length(trim("Description"))=length("Description")),
    "ProviderData" TEXT DEFAULT NULL,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    CONSTRAINT "PK_RemoteLibraries" PRIMARY KEY("LocalId", "RemoteServiceId"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE TABLE IF NOT EXISTS "LibraryLogs" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Message" TEXT NOT NULL CHECK(length(trim("Message"))=length("Message")),
    "Action" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    "EventId" INT DEFAULT NULL,
    "Url" NVARCHAR(4096) DEFAULT NULL,
    "ProviderData" TEXT DEFAULT NULL,
    "Timestamp" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    CONSTRAINT "PK_LibraryLogs" PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "LocalVersions" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Version" NVARCHAR(1024) NOT NULL CHECK(length(trim("Version"))=length("Version") AND length("Version")>0) COLLATE NOCASE,
    "Order" UNSIGNED SMALLINT NOT NULL DEFAULT 65535,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "LibraryId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_LocalVersion_LocalLibrary" REFERENCES "LocalLibraries"("Id") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT "PK_LocalVersions" PRIMARY KEY("Id"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE INDEX "IDX_LocalVersions_Version" ON "LocalVersions" ("Version" COLLATE NOCASE);
CREATE INDEX "IDX_LocalVersions_Order" ON "LocalVersions" ("Order");

CREATE TABLE IF NOT EXISTS "RemoteVersions" (
    "LocalId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_RemoteVersion_LocalVersion" REFERENCES "LocalVersions"("Id") ON DELETE RESTRICT COLLATE NOCASE,
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Priority" UNSIGNED SMALLINT DEFAULT NULL,
    "ProviderData" TEXT DEFAULT NULL,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    CONSTRAINT "PK_RemoteVersions" PRIMARY KEY("LocalId", "LibraryId", "RemoteServiceId"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE TABLE IF NOT EXISTS "VersionLogs" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Message" TEXT NOT NULL CHECK(length(trim("Message"))=length("Message")),
    "Action" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    "EventId" INT DEFAULT NULL,
    "Url" NVARCHAR(4096) DEFAULT NULL,
    "ProviderData" TEXT DEFAULT NULL,
    "Timestamp" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "VersionId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    CONSTRAINT "PK_VersionLogs" PRIMARY KEY("Id")
);

CREATE TABLE IF NOT EXISTS "LocalFiles" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Name" NVARCHAR(1024) NOT NULL CHECK(length(trim("Name"))=length("Name") AND length("Name")>0) COLLATE NOCASE,
    "SRI" NVARCHAR(256) NOT NULL CHECK(length(trim("SRI"))=length("SRI") AND length("SRI")>0) COLLATE NOCASE,
    "Order" UNSIGNED SMALLINT NOT NULL DEFAULT 65535,
    "ContentType" NVARCHAR(512) NOT NULL CHECK(length(trim("ContentType"))=length("ContentType") AND length("SRI")>0) COLLATE NOCASE,
    "Encoding" NVARCHAR(32) NOT NULL CHECK(length(trim("Encoding"))=length("Encoding")) COLLATE NOCASE,
    "Data" BLOB NOT NULL,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "VersionId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_LocalFile_LocalVersion" REFERENCES "LocalVersions"("Id") ON DELETE RESTRICT COLLATE NOCASE,
    CONSTRAINT "PK_LocalFiles" PRIMARY KEY("Id"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE INDEX "IDX_LocalFiles_Name" ON "LocalFiles" ("Name" COLLATE NOCASE);
CREATE INDEX "IDX_LocalFiles_Order" ON "LocalFiles" ("Order");

CREATE TABLE IF NOT EXISTS "RemoteFiles" (
    "LocalId" UNIQUEIDENTIFIER NOT NULL CONSTRAINT "FK_RemoteFile_LocalFile" REFERENCES "LocalFiles"("Id") ON DELETE RESTRICT COLLATE NOCASE,
    "VersionId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Encoding" NVARCHAR(32) DEFAULT NULL CHECK("Encoding" IS NULL OR (length(trim("Encoding"))=length("Encoding"))) COLLATE NOCASE,
    "SRI" NVARCHAR(256) DEFAULT NULL CHECK("SRI" IS NULL OR (length(trim("SRI"))=length("SRI") AND length("SRI")>0)) COLLATE NOCASE,
    "Data" BLOB DEFAULT NULL,
    "Priority" UNSIGNED SMALLINT DEFAULT NULL,
    "ProviderData" TEXT DEFAULT NULL,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    CONSTRAINT "PK_RemoteFiles" PRIMARY KEY("LocalId", "VersionId", "LibraryId", "RemoteServiceId"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE TABLE IF NOT EXISTS "FileLogs" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Message" TEXT NOT NULL CHECK(length(trim("Message"))=length("Message")),
    "Action" UNSIGNED TINYINT NOT NULL DEFAULT 0,
    "EventId" INT DEFAULT NULL,
    "Url" NVARCHAR(4096) DEFAULT NULL,
    "ProviderData" TEXT DEFAULT NULL,
    "Timestamp" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "FileId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "VersionId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    CONSTRAINT "PK_FileLogs" PRIMARY KEY("Id")
);