using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tome.Utilities
{
    public class InputSantiziation
    {

        public static Boolean CheckLength(String inputString,int length)
        {
            if (inputString.Length < length)
                return false;
            return true;
        }

        public static Boolean ContainsRiskText(String inputString)
        {
            return inputString.Any(character => !Char.IsLetterOrDigit(character));
        }

        public static Boolean IsValidInput(String inputString, int length)
        {
            return CheckLength(inputString, length) && ContainsRiskText(inputString);
        }


    }
}