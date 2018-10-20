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
    }

}
