namespace QCVault.Utilities
{
    public static class Util
    {
        public static string Capitalize(this string input)
        {
            // eww
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
    }
}
