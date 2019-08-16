using Newtonsoft.Json;
using System;
using System.Web;

namespace MCCommon
{
    public sealed class ForgeAppConfiguration : ForgeApp
    {
        [JsonIgnore]
        public string Host { get; internal set; }

        [JsonProperty]
        public Guid Account { get; set; }

        [JsonProperty]
        public Guid Project { get; set; }

        [JsonIgnore]
        public Uri BasePath => new Uri($"https://{Host}/");

        [JsonIgnore]
        public Uri ModelDerivativeBasePath => new Uri($"https://{Host}/modelderivative/v2/designdata/");

        [JsonIgnore]
        public Uri IssueManagementBasePath
        {
            get
            {
                switch (Environment)
                {
                    case ForgeEnvironment.Development:
                        return new Uri($"https://{Host}/issues-dev/v1/containers/");

                    case ForgeEnvironment.Staging:
                        return new Uri($"https://{Host}/issues-stg/v1/containers/");

                    default:
                        return new Uri($"https://{Host}/issues/v1/containers/");
                }
            }
        }

        [JsonIgnore]
        public Uri NucleusLegacyBasePath => new Uri($"https://{Host}/bim360/aggregate/");

        [JsonIgnore]
        public Uri NucleusModelSetBasePath => new Uri($"https://{Host}/bim360/modelset/");

        [JsonIgnore]
        public Uri NucleusClashBasePath => new Uri($"https://{Host}/bim360/clash/");

        [JsonIgnore]
        public Uri AuthorizeUrl => new Uri($"https://{Host}/authentication/v1/authorize?response_type=code&client_id={ClientId}&redirect_uri={UrlSafeCallbackUrl}&scope=data:read%20data:write");

        [JsonIgnore]
        public string UrlSafeCallbackUrl => HttpUtility.UrlEncode(this.CallbackUrl);

        [JsonProperty]
        public string ForgeBimHubId => $"b.{Account}";

        [JsonProperty]
        public string ForgeBimProjectId => $"b.{Project}";

        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static ForgeAppConfiguration Current { get; set; }
    }
}
