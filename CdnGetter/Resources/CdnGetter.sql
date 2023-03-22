CREATE TABLE "RemoteServices" (
	"Id"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"Name"	NVARCHAR(1024) NOT NULL CHECK(length(trim("Name"))=length("Name") AND length("Name")>0) UNIQUE COLLATE NOCASE,
	"Priority"	UNSIGNED SMALLINT NOT NULL DEFAULT 65535,
	"Description"	TEXT NOT NULL CHECK(length(trim("Description"))=length("Description")),
	"CreatedOn"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
	"ModifiedOn"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
	PRIMARY KEY("Id"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE INDEX "IDX_RemoteServices_Priority" ON "RemoteServices" ("Priority" ASC);

CREATE UNIQUE INDEX "IDX_RemoteServices_Name" ON "RemoteServices" ("Name" COLLATE NOCASE ASC);

CREATE TABLE "LocalLibraries" (
	"Id"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"Name"	NVARCHAR(1024) NOT NULL CHECK(length(trim("Name"))=length("Name") AND length("Name")>0) UNIQUE COLLATE NOCASE,
	"Description"	TEXT NOT NULL CHECK(length(trim("Description"))=length("Description")),
	"CreatedOn"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
	"ModifiedOn"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
	PRIMARY KEY("Id"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE UNIQUE INDEX "IDX_LocalLibraries_Name" ON "LocalLibraries" ("Name" COLLATE NOCASE ASC);

CREATE TABLE "RemoteLibraries" (
	"LocalId"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"RemoteServiceId"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"Priority"	UNSIGNED SMALLINT DEFAULT NULL,
	"Description"	TEXT NOT NULL DEFAULT '',
	"ProviderData"	TEXT DEFAULT NULL,
	"CreatedOn"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
	"ModifiedOn"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
	PRIMARY KEY("LocalId","RemoteServiceId"),
	FOREIGN KEY("RemoteServiceId") REFERENCES "RemoteServices"("Id") ON DELETE RESTRICT,
	FOREIGN KEY("LocalId") REFERENCES "LocalLibraries"("Id") ON DELETE RESTRICT,
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE TABLE "LibraryLogs" (
	"Id"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"Message"	TEXT NOT NULL DEFAULT '' CHECK(length(trim("Message"))=length("Message")),
	"Action"	UNSIGNED TINYINT NOT NULL DEFAULT 0,
	"EventId"	INTEGER DEFAULT NULL,
	"Url"	NVARCHAR(4096) DEFAULT NULL,
	"ProviderData"	TEXT DEFAULT NULL,
	"Timestamp"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
	"LibraryId"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"RemoteServiceId"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	FOREIGN KEY("LibraryId","RemoteServiceId") REFERENCES "RemoteLibraries"("LocalId","RemoteServiceId") ON DELETE RESTRICT,
	PRIMARY KEY("Id")
);

CREATE INDEX "IDX_LibraryLogs_Timestamp" ON "LibraryLogs" ("Timestamp"	DESC);

CREATE TABLE "LocalVersions" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Version" NVARCHAR(1024) NOT NULL CHECK(length(trim("Version"))=length("Version") AND length("Version")>0) COLLATE NOCASE,
    "Order" UNSIGNED SMALLINT NOT NULL DEFAULT 65535,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	FOREIGN KEY("LibraryId") REFERENCES "LocalLibraries"("Id") ON DELETE RESTRICT,
    PRIMARY KEY("Id"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE INDEX "IDX_LocalVersions_Version" ON "LocalVersions" ("Version" COLLATE NOCASE DESC);

CREATE INDEX "IDX_LocalVersions_Order" ON "LocalVersions" ("Order" ASC);

CREATE TABLE "RemoteVersions" (
    "LocalId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Priority" UNSIGNED SMALLINT DEFAULT NULL,
    "ProviderData" TEXT DEFAULT NULL,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    FOREIGN KEY("LocalId") REFERENCES "LocalVersions"("Id") ON DELETE RESTRICT,
	FOREIGN KEY("LibraryId","RemoteServiceId") REFERENCES "RemoteLibraries"("LocalId","RemoteServiceId") ON DELETE RESTRICT,
    PRIMARY KEY("LocalId", "LibraryId", "RemoteServiceId"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE TABLE "VersionLogs" (
	"Id"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"Message"	TEXT NOT NULL DEFAULT '' CHECK(length(trim("Message"))=length("Message")),
	"Action"	UNSIGNED TINYINT NOT NULL DEFAULT 0,
	"EventId"	INTEGER DEFAULT NULL,
	"Url"	NVARCHAR(4096) DEFAULT NULL,
	"ProviderData"	TEXT DEFAULT NULL,
	"Timestamp"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "VersionId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	FOREIGN KEY("VersionId","LibraryId","RemoteServiceId") REFERENCES "RemoteVersions"("LocalId","LibraryId","RemoteServiceId") ON DELETE RESTRICT,
	PRIMARY KEY("Id")
);

CREATE INDEX "IDX_VersionLogs_Timestamp" ON "VersionLogs" ("Timestamp"	DESC);

CREATE TABLE "LocalFiles" (
    "Id" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "Name" NVARCHAR(1024) NOT NULL CHECK(length(trim("Name"))=length("Name") AND length("Name")>0) COLLATE NOCASE,
    "SRI" NVARCHAR(256) NOT NULL CHECK(length(trim("SRI"))=length("SRI") AND length("SRI")>0) COLLATE NOCASE,
    "Order" UNSIGNED SMALLINT NOT NULL DEFAULT 65535,
    "ContentType" NVARCHAR(512) NOT NULL CHECK(length(trim("ContentType"))=length("ContentType") AND length("ContentType")>0),
    "Encoding" NVARCHAR(32) NOT NULL CHECK(length(trim("Encoding"))=length("Encoding")),
    "Data" BLOB NOT NULL,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "VersionId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	FOREIGN KEY("VersionId") REFERENCES "LocalVersions"("Id") ON DELETE RESTRICT,
    PRIMARY KEY("Id"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE INDEX "IDX_LocalFiles_Name" ON "LocalFiles" ("Name" COLLATE NOCASE ASC);

CREATE INDEX "IDX_LocalFiles_Order" ON "LocalFiles" ("Order" ASC);

CREATE TABLE "RemoteFiles" (
	"LocalId"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"VersionId"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"LibraryId"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"RemoteServiceId"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"Encoding"	NVARCHAR(32) DEFAULT NULL CHECK("Encoding" IS NULL OR (length(trim("Encoding"))=length("Encoding"))),
	"SRI"	NVARCHAR(256) DEFAULT NULL CHECK("SRI" IS NULL OR (length(trim("SRI"))=length("SRI") AND length("SRI")>0)) COLLATE NOCASE,
	"Data"	BLOB DEFAULT NULL,
	"Priority"	UNSIGNED SMALLINT DEFAULT NULL,
	"ProviderData"	TEXT DEFAULT NULL,
    "CreatedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "ModifiedOn" DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
	FOREIGN KEY("LocalId") REFERENCES "LocalFiles"("Id") ON DELETE RESTRICT,
	PRIMARY KEY("LocalId","VersionId","LibraryId","RemoteServiceId"),
	FOREIGN KEY("VersionId","LibraryId","RemoteServiceId") REFERENCES "RemoteVersions"("LocalId","LibraryId","RemoteServiceId"),
    CHECK("CreatedOn"<="ModifiedOn")
);

CREATE TABLE "FileLogs" (
	"Id"	UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	"Message"	TEXT NOT NULL DEFAULT '' CHECK(length(trim("Message"))=length("Message")),
	"Action"	UNSIGNED TINYINT NOT NULL DEFAULT 0,
	"EventId"	INTEGER DEFAULT NULL,
	"Url"	NVARCHAR(4096) DEFAULT NULL,
	"ProviderData"	TEXT DEFAULT NULL,
	"Timestamp"	DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
    "FileId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "VersionId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "LibraryId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
    "RemoteServiceId" UNIQUEIDENTIFIER NOT NULL COLLATE NOCASE,
	FOREIGN KEY("FileId","VersionId","LibraryId","RemoteServiceId") REFERENCES "RemoteFiles"("LocalId","VersionId","LibraryId","RemoteServiceId") ON DELETE RESTRICT,
	PRIMARY KEY("Id")
);

CREATE INDEX "IDX_FileLogs_Timestamp" ON "FileLogs" ("Timestamp"	DESC);