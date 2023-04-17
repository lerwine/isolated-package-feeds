namespace CdnGetter;

public partial class Url
{
    /*
URI         = scheme ":" hier-part [ "?" query ] [ "#" fragment ]

authority   = [ userinfo "@" ] host [ ":" port ]
   userinfo    = *( unreserved / pct-encoded / sub-delims / ":" )
   host        = IP-literal / IPv4address / reg-name
       IPv6address =                            6( h16 ":" ) ls32
                   /                       "::" 5( h16 ":" ) ls32
                   / [               h16 ] "::" 4( h16 ":" ) ls32
                   / [ *1( h16 ":" ) h16 ] "::" 3( h16 ":" ) ls32
                   / [ *2( h16 ":" ) h16 ] "::" 2( h16 ":" ) ls32
                   / [ *3( h16 ":" ) h16 ] "::"    h16 ":"   ls32
                   / [ *4( h16 ":" ) h16 ] "::"              ls32
                   / [ *5( h16 ":" ) h16 ] "::"              h16
                   / [ *6( h16 ":" ) h16 ] "::"
           ls32        = ( h16 ":" h16 ) / IPv4address
                       ; least-significant 32 bits of address
           h16         = 1*4HEXDIG
                       ; 16 bits of address represented in hexadecimal
       IPvFuture   = "v" 1*HEXDIG "." 1*( unreserved / sub-delims / ":" )
       IP-literal  = "[" ( IPv6address / IPvFuture  ) "]"
       IPv4address = dec-octet "." dec-octet "." dec-octet "." dec-octet
           dec-octet   = DIGIT                 ; 0-9
                       / %x31-39 DIGIT         ; 10-99
                       / "1" 2DIGIT            ; 100-199
                       / "2" %x30-34 DIGIT     ; 200-249
                       / "25" %x30-35          ; 250-255
   port        = *DIGIT
       reg-name    = *( unreserved / pct-encoded / sub-delims )

hier-part   = "//" authority path-abempty
           / path-absolute
           / path-rootless
           / path-empty

path          = path-abempty    ; begins with "/" or is empty
               / path-absolute   ; begins with "/" but not "//"
               / path-noscheme   ; begins with a non-colon segment
               / path-rootless   ; begins with a segment
               / path-empty      ; zero characters

   path-abempty  = *( "/" segment )
   path-absolute = "/" [ segment-nz *( "/" segment ) ]
   path-noscheme = segment-nz-nc *( "/" segment )
   path-rootless = segment-nz *( "/" segment )
   path-empty    = 0<pchar>
       segment       = *pchar
       segment-nz    = 1*pchar
       segment-nz-nc = 1*( unreserved / pct-encoded / sub-delims / "@" )
                       ; non-zero-length segment without any colon ":"
           pchar         = unreserved / pct-encoded / sub-delims / ":" / "@"

query       = *( pchar / "/" / "?" )
fragment    = *( pchar / "/" / "?" )
*/

    public class UserInfo
    {
        private string _userName;
        
        /// <summary>
        /// The user name.
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => _userName = value ?? "";
        }

        public string? Password { get; set; }   

        public UserInfo(string userName, string? password = null)
        {
            _userName = userName ?? "";
            Password = password;    
        }
    }
}
