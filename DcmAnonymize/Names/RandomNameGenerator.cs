using System;
using System.IO;

namespace DcmAnonymize.Names
{
    public class RandomNameGenerator
    {
        private readonly Lazy<string[]> _firstNames = new Lazy<string[]>(() => File.ReadAllLines("Names/first-names.txt"));
        private readonly Lazy<string[]> _lastNames = new Lazy<string[]>(() => File.ReadAllLines("Names/last-names.txt"));
        private readonly Random _random = new Random();

        public RandomName GenerateRandomName()
        {
            var firstNames = _firstNames.Value;
            var lastNames = _lastNames.Value;

            var firstName = firstNames[_random.Next(0, firstNames.Length - 1)];
            var lastName = lastNames[_random.Next(0, lastNames.Length - 1)];

            return new RandomName {FirstName = firstName, LastName = lastName};
        }
    }
}