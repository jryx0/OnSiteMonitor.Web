using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;

namespace WFM.JW.HB.Console
{
    public static class GenericConverter
    {
        public static bool CanConvert(string value, Type type)
        {
            if (string.IsNullOrEmpty(value) || type == null)
                return false;

            System.ComponentModel.TypeConverter conv = System.ComponentModel.TypeDescriptor.GetConverter(type);
            if (conv.CanConvertFrom(typeof(string)))
            {
                try
                {
                    conv.ConvertFrom(value);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }
    }
}
