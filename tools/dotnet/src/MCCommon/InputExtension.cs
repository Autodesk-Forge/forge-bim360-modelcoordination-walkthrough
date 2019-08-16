namespace MCCommon
{
    internal static class InputExtension
    {
        public static T As<T>(this object value) => value != null && value is T ? (T)value : default;

        public static string AsText(this object value) => value.As<string>();

        public static bool AsBoolean(this object value) => value.As<bool>();

        public static int AsInt(this object value) => value.As<int>();
    }
}
