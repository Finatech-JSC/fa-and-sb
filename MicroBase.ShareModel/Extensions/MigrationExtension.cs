using System;
using System.IO;

namespace MicroBase.Share.Extensions
{
    public class MigrationExtension
    {
        public static string ReadSql(Type migrationType, string sqlFileName)
        {
            var assembly = migrationType.Assembly;
            string resourceName = $"{migrationType.Namespace}.{sqlFileName}";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Unable to find the SQL file from an embedded resource", resourceName);
                }

                using (var reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    return content;
                }
            }
        }
    }
}
