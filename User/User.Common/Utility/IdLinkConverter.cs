using System.Text;

namespace User.Common.Utility
{
    public class IdLinkConverter
    {
        public string GuidToIdLink(Guid id)
        {
            var idLink = id.ToString();
            StringBuilder sb = new();

            for (var i = 0; i < idLink.Length; i++)
            {
                if (idLink[i] == '-')
                {
                    sb.Append(GenerateRandomString());
                }
                else
                {
                    sb.Append(idLink[i]);
                }
            }

            return sb.ToString();
        }

        public Guid IdLinkToGuid(string idLink)
        {
            StringBuilder sb = new();

            sb.Append(idLink.AsSpan(0,8));
            sb.Append('-');

            int index = Convert.ToInt32(idLink[8]) - 40;

            int length = 4;
            int i = 0;
            while (i < 4)
            {
                sb.Append(idLink.AsSpan(index, length));
                index += length;
                if (i < 3)
                {
                    sb.Append('-');
                    index += Convert.ToInt32(idLink[index]) - 48;
                }

                i++;
                if (i == 3) length = 12;
            }

            return new Guid(sb.ToString());
        }

        private string GenerateRandomString()
        {
            Random rand = new();
            var charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var length = rand.Next(0,4);
            StringBuilder sb = new();
            for (var i = 0; i < length; i++)
            {
                if (sb.Length == 0)
                {
                    sb.Append((length + 1).ToString());
                }
                
                sb.Append(charset[rand.Next(charset.Length)]);
            }
            if (sb.Length == 0)
                {
                    sb.Append(1.ToString());
                }

            return sb.ToString();
        }
    }
}