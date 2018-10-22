using Cindi.DotNetCore.BotExtensions.Exceptions;
using Cindi.DotNetCore.BotExtensions.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions
{
    public static class BotUtility
    {
        public static CommonData GetData(List<CommonData> data, string keyName)
        {
            var result = data.Where(d => d.Id.ToLower() == keyName.ToLower()).ToList();

            if(result.Count() == 0)
            {
                throw new MissingInputException("Missing " + keyName);
            }
            else if(result.Count() > 1)
            {
                throw new DuplicateInputException();
            }
            else
            {
                return result.First();
            }
        }

        /// <summary>
        /// Used for cloning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }


        public static string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;


        }
    }

}
