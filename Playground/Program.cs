
/* Just a playground for me to play around. */

using System;
using Shared.Helpers;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Localisation.Initialize();
            var data = Localisation.GetData("1.1.1.1");
            Console.WriteLine(Localisation.StringToCountryId(data.Country.IsoCode));
            Console.ReadLine();
        }
    }
}
