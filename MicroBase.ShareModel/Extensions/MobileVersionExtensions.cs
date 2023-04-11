namespace MicroBase.Share.Extensions
{
    public static class MobileVersionExtensions
    {
        public static int GetAppVersionFromString(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return 0;
            }

            double major = 0,
                minor = 0,
                patch = 0;

            var versions = version.Split(".");
            if (versions.Length >= 1)
            {
                major = GetDivide10FromInt(versions[0]?.ToInt(0)) * 100000;
            }

            if (versions.Length >= 2)
            {
                minor = GetDivide10FromInt(versions[1]?.ToInt(0)) * 1000;
            }

            if (versions.Length >= 3)
            {
                patch = GetDivide10FromInt(versions[2]?.ToInt(0)) * 10;
            }

            return (int)(major + minor + patch);
        }

        private static double GetDivide10FromInt(int? number = 0)
        {
            if (number == null)
            {
                return 0;
            }

            return (double)number / 10;
        }
    }
}