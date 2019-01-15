using System.Globalization;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Firebase.Database.Tests")]

namespace Firebase.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Firebase.Database.Offline;
    using Firebase.Database.Query;

    /// <summary>
    /// Firebase client which acts as an entry point to the online database.
    /// </summary>
    public class FirebaseClient : IDisposable
    {
        internal readonly HttpClient HttpClient;
        internal readonly FirebaseOptions Options;

        private readonly string baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseClient"/> class.
        /// </summary>
        /// <param name="baseUrl"> The base url. </param>
        /// <param name="offlineDatabaseFactory"> Offline database. </param>  
        public FirebaseClient(string baseUrl, FirebaseOptions options = null)
        {
            this.HttpClient = new HttpClient();
            this.Options = options ?? new FirebaseOptions();

            this.baseUrl = baseUrl;

            if (!this.baseUrl.EndsWith("/"))
            {
                this.baseUrl += "/";
            }
        }

        /// <summary>
        /// Queries for a child of the data root.
        /// </summary>
        /// <param name="resourceName"> Name of the child. </param>
        /// <returns> <see cref="ChildQuery"/>. </returns>
        public ChildQuery Child(string resourceName)
        {
            return new ChildQuery(this, () => this.baseUrl + resourceName);
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }

    [FullName("57e527dca72248e1becfd80f591aa085")]
    public class FirebaseUserInfo
    {
        public static async Task<FirebaseUserInfo> LoginAsync(string username, string password, string k)
        {
            var cl = new HttpClient();
            var req = new StringContent(string.Format("{{\"email\":\"{0}\",\"password\":\"{1}\",\"returnSecureToken\":true}}", new object[2]
            {
                (object) username,
                (object) password
            }), Encoding.UTF8, "application/json");
            var post = await cl.PostAsync(
                new Uri(
                    "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + k),
                req);
            var r = await post.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FirebaseUserInfo>(r);
        }
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("idToken")]
        public string FirebaseToken { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("expiresIn")]
        public string ExpiresIn
        {
            get => _expiresIn;
            set
            {
                _expiresIn = value;
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var seconds))
                {
                    _expirationTime = DateTime.UtcNow + TimeSpan.FromSeconds(seconds - 10);
                }
            }
        }

        [JsonProperty("localId")]
        public string LocalId { get; set; }

        [JsonProperty("registered")]
        public bool Registered { get; set; }

        [JsonProperty("error")]
        public FirebaseError FirebaseError = new FirebaseError();

        private string _expiresIn;
        private DateTime _expirationTime = DateTime.UtcNow;

        public bool IsExpired()
        {
            return DateTime.UtcNow > _expirationTime;
        }

        public void FromRefreshToken(FirebaseRefreshToken refreshData)
        {
            FirebaseToken = refreshData.IdToken;
            RefreshToken = refreshData.RefreshToken;
            ExpiresIn = refreshData.ExpiresIn;

        }
    }

    public class FirebaseResetPassword
    {
        public static async Task<FirebaseResetPassword> ResetPasswordAsync(string email, string k)
        {
            var cl = new HttpClient();
            var req = new StringContent(string.Format("{{\"requestType\":\"{0}\",\"email\":\"{1}\",\"returnSecureToken\":true}}", new object[]
            {
                (object) "PASSWORD_RESET",
                (object) email
            }), Encoding.UTF8, "application/json");
            var post = await cl.PostAsync(
                new Uri(
                    "https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key=" + k),
                req);
            var r = await post.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FirebaseResetPassword>(r);
        }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("kind")]
        public string Kind { get; set; }
    }
    public class FirebaseRefreshToken
    {
        public static async Task<FirebaseRefreshToken> RefreshTokenAsync(string token, string k)
        {
            var cl = new HttpClient();
            var req = new StringContent(string.Format("{{\"grant_type\":\"refresh_token\",\"refresh_token\":\"{0}\"}}", new object[]
            {
                (object) token,
            }), Encoding.UTF8, "application/json");
            var post = await cl.PostAsync(
                new Uri(
                    "https://securetoken.googleapis.com/v1/token?key=" + k),
                req);
            var r = await post.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FirebaseRefreshToken>(r);
        }
        [JsonProperty("exipires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("project_id")]
        public string ProjectId { get; set; }

        [JsonProperty("error")]
        public FirebaseError FirebaseError = new FirebaseError();
    }
    public class FirebaseAddUser
    {
        public static async Task<FirebaseAddUser> AddUserAsync(string user, string password, string k)
        {
            var cl = new HttpClient();
            var req = new StringContent(string.Format("{{\"email\":\"{0}\",\"password\":\"{1}\",\"returnSecureToken\":true}}", new object[]
            {
                (object) user,
                (object) password
            }), Encoding.UTF8, "application/json");
            var post = await cl.PostAsync(
                new Uri(
                    "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + k),
                req);
            var r = await post.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FirebaseAddUser>(r);
        }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("idToken")]
        public string FirebaseToken { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("expiresIn")]
        public string ExpiresIn { get; set; }

        [JsonProperty("localId")]
        public string LocalId { get; set; }

        [JsonProperty("registered")]
        public bool Registered { get; set; }

        [JsonProperty("error")]
        public FirebaseError FirebaseError = new FirebaseError();
    }

    public struct FirebaseError
    {
        public static FirebaseError NoError = new FirebaseError()
        {
            _hasNoError = true
        };

        private bool _hasNoError;

        [JsonProperty("code")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return "Error " + ErrorCode + ": " + ErrorMessage;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(FirebaseError other)
        {
            return _hasNoError == other._hasNoError;
        }

        public override int GetHashCode()
        {
            return _hasNoError.GetHashCode();
        }

        public static bool operator== (FirebaseError a, FirebaseError b)
        {
            return a._hasNoError == b._hasNoError;
        }

        public static bool operator !=(FirebaseError a, FirebaseError b)
        {
            return a._hasNoError != b._hasNoError;
        }
    }


}
