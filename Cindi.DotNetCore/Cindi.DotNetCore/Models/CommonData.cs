using Cindi.DotNetCore.BotExtensions.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Models
{
    public class CommonData
    {
        /// <summary>
        /// Int is defined as a integer in c#
        /// Decimal is defined as a decimal type in c#
        /// </summary>
        public enum InputDataType {
            Int,
            String,
            Bool,
            Object,
            ErrorMessage,
            Decimal
        }

        public CommonData()
        { }

        public CommonData(string id, int type, string value)
        {
            this.Id = id;
            this.Type = type;

            try
            {
                switch (type)
                {
                    case (int)InputDataType.Bool:
                        var result = Convert.ToBoolean(value);
                        break;
                    case (int)InputDataType.String:
                        break;
                    case (int)InputDataType.Int:
                        var intConversion = int.Parse(value);
                        break;
                    case (int)InputDataType.Object:
                        var jsonConversion = JsonConvert.DeserializeObject(value);
                        break;
                }
                Value = value;
            }
            catch (Exception e)
            {
                throw new InvalidInputValueException(e.Message);
            }
        }

        public string Id { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
    }
}
