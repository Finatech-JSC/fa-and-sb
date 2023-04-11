namespace MicroBase.NoDependencyService
{
    public interface IRandomService
    {
        string GenerateRandomCharacter(int len, 
            bool isNumberOnly = false, 
            bool isUpperLetter = false, 
            bool isLowerLetter = false);
    }

    public class RandomService : IRandomService
    {
        public string GenerateRandomCharacter(int len,
            bool isNumberOnly = false,
            bool isUpperLetter = false,
            bool isLowerLetter = false)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (isNumberOnly)
            {
                chars = "1234567890";
            }

            if (isUpperLetter)
            {
                chars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }

            if (isLowerLetter)
            {
                chars = "abcdefghijklmnopqrstuvwxyz1234567890";
            }

            var arrs = new char[len];
            var rd = new Random();
            for (int i = 0; i < arrs.Length; i++)
            {
                arrs[i] = chars[rd.Next(chars.Length)];
            }

            return new string(arrs);
        }
    }
}