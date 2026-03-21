namespace URLShortener.Helpers
{
    public static class ShortCodeGenerator
    {
        private static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly Random random = new Random();

        public static string GenerateShortCode(int length = 6)
        {
            var shortCode = new char[length];
            for (int i = 0; i < length; i++)
            {
                shortCode[i] = chars[random.Next(chars.Length)];
            }
            return new string(shortCode);
        }
    }
}