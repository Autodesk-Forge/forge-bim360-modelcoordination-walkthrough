using System;

namespace MCCommon
{
    public static class ForgeAppConfigurationExtensions
    {
        public static Uri ModelDerivativePath(this ForgeAppConfiguration config, string path) => new Uri(config.ModelDerivativeBasePath, path);

        public static Uri IssueManagementPath(this ForgeAppConfiguration config, string path) => new Uri(config.IssueManagementBasePath, path);
    }
}
