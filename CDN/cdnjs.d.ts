/**
 * Error response object.
 * @interface IErrorResponse
 */
declare interface IErrorResponse {
    /**
     * This will always be set to true to indicate that an error has occurred.
     * @type {true}
     * @memberof IErrorResponse
     */
    error: true;

    /**
     * The status code for the error, which should be identical to the HTTP response status code.
     * @type {number}
     * @memberof IErrorResponse
     */
    status: number;

    /**
     * A message explaining what error has occurred.
     * @type {string}
     * @memberof IErrorResponse
     */
    message: string;
}

/**
 * Response element from the /libraries endpoint.
 * @interface ILibararyListItem
 */
declare interface ILibararyListItem {
    /**
     * This will be the full name of the library, as stored on cdnjs.
     * @type {string}
     * @memberof ILibararyListItem
     */
    name: string;

    /**
     * This will be the URL of the default file on the latest version of the library.
     * 
     * It is important to note that this URL is based on the latest version number of the library and the default file name configured, there is no validation in place to ensure that this URL will actually serve a valid asset.
     * @type {string}
     * @memberof ILibararyListItem
     */
    latest: string;
}

/**
 * Response from the /libraries/:library/:version endpoint
 * @interface ILibararyByVersion
 */
declare interface ILibararyByVersion {
    /**
     * This will be the full name of the library, as stored on cdnjs.
     * @type {string}
     * @memberof ILibararyByVersion
     */
    name: string;

    /**
     * The version of the library that has been requested. This should match the version requested in the URL.
     * @type {string}
     * @memberof ILibararyByVersion
     */
    version: string;

    /**
     * The files available for this version of the library on the cdnjs CDN.
     *
     * This array of files is filtered by our CDN whitelist, so all these files will be available for use on our CDN.
     * @type {string[]}
     * @memberof ILibararyByVersion
     */
    files: string[];

    /**
     * All the files that cdnjs has for this version of the library, irrespective of if they will be available on the CDN.
     * 
     * This array of files ignores the whitelist filter, which means some files may not be available on the CDN if their extensions aren't whitelisted.
     * @type {string[]}
     * @memberof ILibararyByVersion
     */
    rawFiles: string[];

    /**
     * This object will contain a key for each file that cdnjs could calculate an SRI hash for, with the value being the SRI hash.
     * @type {{ [key: string]: string; }}
     */
    sri: { [key: string]: string; };
}

/**
 * Response from the /libraries/:library endpoint.
 * @interface ILibararyByName
 */
declare interface ILibararyByName {
    /**
     * This will be the full name of the library, as stored on cdnjs.
     * @type {string}
     * @memberof ILibararyByName
     */
    name: string;

    /**
     * This will be the URL of the default file on the latest version of the library.
     * 
     * It is important to note that this URL is based on the latest version number of the library and the default file name configured, there is no validation in place to ensure that this URL will actually serve a valid asset.
     * @type {string}
     * @memberof ILibararyByName
     */
    latest: string;

    /**
     * The SRI hash value for the file provided in the latest property, if it exists and is valid (i.e. if the SRI hash could be calculated for it).
     * @type {string}
     * @memberof ILibararyByName
     */
    sri: string;

    /**
     * This will be the name of the default file for the library. If not defined in the package's JSON file, this will be an empty string
     * 
     * There is no validation that this file actually exists in each version of the library.
     * @type {string}
     * @memberof ILibararyByName
     */
    filename: string;

    /**
     * The attributed author for the library, as defined in the cdnjs package JSON file for this library.
     * @type {(string | null | undefined)}
     * @memberof ILibararyByName
     */
    author?: string | null;

    /**
     * The repository for the library, if known, in standard repository format.
     * @type {({ type: string; url: string; } | null)}
     * @memberof ILibararyByName
     */
    authors?: {
        /**
         * The name of a co-author.
         * @type {string}
         */
        name: string;
        /**
         * The url for author.
         * @type {string}
         */
        url: string;
    }[];

    /**
     * The latest version of the library that is available on cdnjs.
     * @type {string}
     * @memberof ILibararyByName
     */
    version: string;

    /**
     * The description of the library if it has been provided in the cdnjs package JSON file.
     * @type {string}
     * @memberof ILibararyByName
     */
    description: string;

    /**
     * A link to the homepage of the package, if one is defined in the cdnjs package JSON file. Normally, this is either the package repository or the package website.
     * 
     * If the library has no defined homepage, this property will be omitted (unless requested via fields explicitly, then it will be null).
     * @type {(string | undefined)}
     * @memberof ILibararyByName
     */
    homepage?: string;

    /**
     * An array of keywords provided in the cdnjs package JSON for the library.
     * @type {string[] | null}
     * @memberof ILibararyByName
     */
    keywords: string[] | null;

    /**
     * The repository for the library, if known, in standard repository format.
     * @type {({ type: string; url: string; } | null)}
     * @memberof ILibararyByName
     */
    repository:	{
        /**
         * The type of repository for the library, normally git.
         * @type {string}
         */
        type: string;
        /**
         * The url for repository associated with the library, if provided in the library's cdnjs package JSON file.
         * @type {string}
         */
        url: string;
    } | null;

    /**
     * The license defined for the library on cdnjs, as a string. If the library has a custom license, it may not be shown here.
     * 
     * If the library has no defined licenses in its cdnjs package JSON file, this property may be omitted (if explicitly requested via fields, it will be null).
     * @type {(string | undefined)}
     * @memberof ILibararyByName
     */
    license?: string;

    /**
     * The auto-update configuration for the library, from the library's package JSON file on cdnjs.
     * 
     * If the library has no auto-update configuration, this property will not be present (unless explicitly requested with fields, then it will be null).
     * @type {({ type?: string; source?: string; target: string; } | undefined)}
     * @memberof ILibararyByName
     */
    autoupdate?: {
        /**
         * The source type of auto-update begin used for the library, either git or npm.
         * 
         * This may be replaced with the source property in some responses.
         * @type {(string | undefined)}
         */
        type?: string;
        
        /**
         * This property is the same as type but may be present in some responses instead of type.
         * @type {(string | undefined)}
         */
        source?: string;

        //The target for the auto-update configuration. If git, this should be a valid git URL, if npm this will be a package name from the NPM package repository.
        target: string;
    }

    /**
     * An array containing all the versions of the library available on cdnjs.
     * 
     * These may not be valid semver.
     * @type {string[]}
     * @memberof ILibararyByName
     */
    versions: string[];


    /**
     * An array containing an object for the latest version of the library on cdnjs. The array is empty if the library has no known versions.
     * 
     * Older versions of the library are no longer included in this array. See deprecation notice.
     * @type {{ version: string; files: string[]; rawFiles: string[]; sri: { [key: string]: string; }; }[]}
     * @memberof ILibararyByName
     */
    assets: {
        /**
         * The version identifier for this version of the library on cdnjs.
         *
         * This may not be valid semver.
         * @type {string}
         */
        version: string;

        /**
         * The files available for this version of the library on the cdnjs CDN.
         *
         * This array of files is filtered by our CDN whitelist, so all these files will be available for use on our CDN.
         * @type {string[]}
         */
        files: string[];

        /**
         * All the files that cdnjs has for this version of the library, irrespective of if they will be available on the CDN.
         *
         * This array of files ignores the whitelist filter, which means some files may not be available on the CDN if their extensions aren't whitelisted.
         * @type {string[]}
         */
        rawFiles: string[];

        /**
         * This object will contain a key for each file that cdnjs could calculate an SRI hash for, with the value being the SRI hash.
         * @type {{ [key: string]: string; }}
         */
        sri: { [key: string]: string; };
    }[];
}