using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CdnSync.CdnJs
{

    public class ErrorResponse : CdnJsObjectResponse
    {
        public const string PROPERTYNAME_error = "error";
        public const string PROPERTYNAME_status = "status";
        public const string PROPERTYNAME_message = "message";
        public const string PROPERTYNAME_rawData = "rawData";

        public int Status
        {
            get => EnsurePropertyValue(PROPERTYNAME_status, () => (int)HttpStatusCode.NoContent);
            set => SetPropertyValue(PROPERTYNAME_status, value);
        }

        public string Message
        {
            get => EnsurePropertyValue(PROPERTYNAME_message, () => ToStatusMessage(Status));
            set => SetPropertyValue(PROPERTYNAME_message, value.ToWsNormalizedOrDefaultIfEmpty(() => ToStatusMessage(Status)));
        }

        public ErrorResponse() : this(HttpStatusCode.NoContent) { }

        public ErrorResponse(HttpStatusCode statusCode, string? message = null) : base(new())
        {
            SetPropertyValue(PROPERTYNAME_error, true);
            int status = (int)statusCode;
            SetPropertyValue(PROPERTYNAME_status, status);
            SetPropertyValue(PROPERTYNAME_message, message.ToWsNormalizedOrDefaultIfEmpty(() => ToStatusMessage(status)));
        }

        private ErrorResponse(JsonObject rawJson) : base(rawJson) { }

        public static TernaryOption<T, ErrorResponse> Create<T>(string? jsonString, Func<JsonNode, T?> load)
            where T : CdnJsObjectResponse
        {
            if (jsonString is null || (jsonString = jsonString.ToTrimmedOrNullIfEmpty()) is null)
                return TernaryOption<T, ErrorResponse>.AsAlternate(new());
            JsonNode? node;
            try { node = JsonNode.Parse(jsonString); }
            catch { node = null; }
            ErrorResponse er;
            if (node is null)
            {
                (er = new(HttpStatusCode.ExpectationFailed, "Failed to parse JSON response.")).SetPropertyValue(PROPERTYNAME_rawData, jsonString);
                return TernaryOption<T, ErrorResponse>.AsAlternate(er);
            }
            T? response = load(node);
            if (response is not null) return new(response);
            (er = new(HttpStatusCode.ExpectationFailed, "Unexpected JSON response.")).SetPropertyValue(PROPERTYNAME_rawData, jsonString);
            return TernaryOption<T, ErrorResponse>.AsAlternate(er);
        }

        public static string ToStatusMessage(int statusCode) => statusCode switch
        {
            100 => "Continue",
            101 => "Switching Protocols",
            102 => "Processing",
            103 => "Early Hints",

            200 => "OK",
            201 => "Created",
            202 => "Accepted",
            203 => "Non-Authoritative Information",
            204 => "No Content",
            205 => "Reset Content",
            206 => "Partial Content",
            207 => "Multi-Status",
            208 => "Already Reported",
            226 => "IM Used",

            300 => "Multiple Choices",
            301 => "Moved Permanently",
            302 => "Found",
            303 => "See Other",
            304 => "Not Modified",
            305 => "Use Proxy",
            307 => "Temporary Redirect",
            308 => "Permanent Redirect",

            400 => "Bad Request",
            401 => "Unauthorized",
            402 => "Payment Required",
            403 => "Forbidden",
            404 => "Not Found",
            405 => "Method Not Allowed",
            406 => "Not Acceptable",
            407 => "Proxy Authentication Required",
            408 => "Request Timeout",
            409 => "Conflict",
            410 => "Gone",
            411 => "Length Required",
            412 => "Precondition Failed",
            413 => "Request Entity Too Large",
            414 => "Request-Uri Too Long",
            415 => "Unsupported Media Type",
            416 => "Requested Range Not Satisfiable",
            417 => "Expectation Failed",
            421 => "Misdirected Request",
            422 => "Unprocessable Entity",
            423 => "Locked",
            424 => "Failed Dependency",
            426 => "Upgrade Required", // RFC 2817
            428 => "Precondition Required",
            429 => "Too Many Requests",
            431 => "Request Header Fields Too Large",
            451 => "Unavailable For Legal Reasons",

            500 => "Internal Server Error",
            501 => "Not Implemented",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            504 => "Gateway Timeout",
            505 => "Http Version Not Supported",
            506 => "Variant Also Negotiates",
            507 => "Insufficient Storage",
            508 => "Loop Detected",
            510 => "Not Extended",
            511 => "Network Authentication Required",
            _ => $"Status Code {statusCode}"
        };
    }
}