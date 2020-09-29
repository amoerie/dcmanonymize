using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DcmAnonymize.Names
{
    public class RandomNameGenerator
    {
        private readonly Lazy<string[]> _firstNames = new Lazy<string[]>(() => ReadEmbeddedResource("DcmAnonymize.Names.first-names.txt").ToArray());
        private readonly Lazy<string[]> _lastNames = new Lazy<string[]>(() => ReadEmbeddedResource("DcmAnonymize.Names.last-names.txt").ToArray());
        private readonly Random _random = new Random();

        public RandomName GenerateRandomName()
        {
            var firstNames = _firstNames.Value;
            var lastNames = _lastNames.Value;

            var firstName = firstNames[_random.Next(0, firstNames.Length - 1)];
            var lastName = lastNames[_random.Next(0, lastNames.Length - 1)];

            return new RandomName {FirstName = firstName, LastName = lastName};
        }

        private static IEnumerable<string> ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName);
            
            if (stream == null)
                yield break;

            using var reader = new StreamReader(stream, Encoding.UTF8);
            
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}