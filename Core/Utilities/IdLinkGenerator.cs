namespace Core.Utilities
{
    public class IdLinkConverter
    {
        public static string GenerateRandomString()
        {
            var unreservedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.~";
            var random = new Random();
            return new string(Enumerable.Repeat(unreservedChars, 20)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}