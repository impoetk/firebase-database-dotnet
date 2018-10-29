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
        [JsonProperty("localId")]
        public string LocalId { get; set; }
        [JsonProperty("idToken")]
        public string FirebaseToken { get; set; }
    }

    public class FirebaseResetPassword
    {
        public static async Task<FirebaseResetPassword> ResetPasswordAsync(string email, string k)
        {
            var cl = new HttpClient();
            var req = new StringContent(string.Format("{{\"requestType\":\"{0}\",\"email\":\"{1}\",\"returnSecureToken\":true}}", new object[2]
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

    public class FirebaseAddUser
    {
        public static async Task<FirebaseAddUser> AddUserAsync(string user, string password, string k)
        {
            var cl = new HttpClient();
            var req = new StringContent(string.Format("{{\"email\":\"{0}\",\"password\":\"{1}\",\"returnSecureToken\":true}}", new object[2]
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

        [JsonProperty("error")]
        public FirebaseError FirebaseError = new FirebaseError();

        [JsonProperty("localid")]
        public string LocalId { get; set; }
    }

    public class FirebaseError
    {
        [JsonProperty("code")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return "Error " + ErrorCode + ": " + ErrorMessage;
        }
    }
}
