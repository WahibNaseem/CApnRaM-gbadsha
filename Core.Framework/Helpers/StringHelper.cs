using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Core.Helpers
{
    public class StringHelper
    {
       
        // check if msg is arabic or not.
        public  bool IsArabic(string msg)
        {
            char[] chars = msg.ToCharArray();
            foreach (char c in chars)
            {
                int charvalue = (int)c;
                if (charvalue > 255)
                {
                    return true;
                }
            }
            return false;
        }

        public  string[] SplitByCharCount(string input, int charCount)
        {
            int arrayLength = input.Length / charCount;
            int reminder = input.Length % charCount;
            if (reminder > 0)
                arrayLength++;
            string[] arr = new string[arrayLength];
            int startIndex = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (startIndex + charCount > input.Length)
                    arr[i] = input.Substring(startIndex);
                else
                    arr[i] = input.Substring(startIndex, charCount);
                startIndex += charCount;
            }
            return arr;
        }

        // Convert to a format accepted by kannel ex: 12%57%78% for arabic
        public string EncodeForSending(string msg)
        {
            string MessageURL = "";
            if (IsArabic(msg))
            {
                msg = HexEncode(msg);
                for (int i = 0; i < msg.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        MessageURL += "%" + msg[i];
                    }
                    else
                    {
                        MessageURL += msg[i];
                    }
                }
            }
            else
            {
                //MessageURL = msg.Replace(" ", "+");
                MessageURL = HttpUtility.UrlEncode(msg);
            }
            return MessageURL;
        }

        public  string AddPercentage(string msg)
        {
            string MessageURL = "";
            for (int i = 0; i < msg.Length; i++)
            {
                if (i % 2 == 0)
                {
                    MessageURL += "%" + msg[i];
                }
                else
                {
                    MessageURL += msg[i];
                }
            }
            return MessageURL;
        }

        // Convert arabic to UCS16 so u can send it to getway
        public  string HexEncode(string s)
        {
            string result = "";
            for (int i = 0; i < s.Length; i++)
            {

                result = result + string.Format("{0:X4}", (Char.ConvertToUtf32(s, i)));
            }
            return result;
        }
        
        //This method has bug, For example: in case the user sent "0601" which is Hex value but the user meant it to be english. you will never know it he meant arabic or english.
        public bool IsArabicUnicode(string hexString)
        {
            bool isArabic = false;

            string[] arabicValues ={
            "0600","0601","0602","0603","0604","0605","0606","0607","0608","0609","060A","060B","060C","060D","060E","060F","0610","0611","0612","0613","0614","0615","0616","0617","0618","0619","061A","061B","061C","061D","061E","061F","0620","0621",
            "0622","0623","0624","0625","0626","0627","0628","0629","062A","062B","062C","062D","062E","062F","0630","0631","0632","0633","0634","0635","0636","0637","0638","0639","063A","063B","063C","063D","063E","063F","0640","0641","0642","0643","0644","0645","0646",
            "0647","0648","0649","064A","064B","064C","064D","064E","064F","0650","0651","0652","0653","0654","0655","0656","0657","0658","0659","065A","065B","065C","065D","065E","065F","0660","0661","0662","0663","0664","0665","0666","0667","0668","0669","066A",
            "066B","066C","066D","066E","066F","0670","0671","0672","0673","0674","0675","0676","0677","0678","0679","067A","067B","067C","067D","067E","067F"};

            string[] arrString = SplitByCharCount(hexString, 4);

            foreach (string stringItem in arrString)
            {
                isArabic = arabicValues.Where(s => s.Equals(stringItem)).Count() > 0 ? true : false;

                if (isArabic)
                    return true;
            }

            return isArabic;
        }
        // Convert Hexa to Arabic
        public  string HexDecode(string s)
        {
            string temp;
            string result = "";
            for (int i = 0; i < s.Length; i = i + 4)
            {
                try
                {
                    temp = s.Substring(i, 4);
                }
                catch (Exception)
                {
                    return result;
                }

                try
                {
                    result = result + Char.ConvertFromUtf32(int.Parse(temp, System.Globalization.NumberStyles.AllowHexSpecifier));
                }
                catch (Exception)
                {
                    return result;
                }
            }
            return result;
        }

        public  string ConvertToEnglishNumbers(string scratchcode)
        {
            return (scratchcode.Replace("٠", "0").Replace("١", "1").Replace("٢", "2").Replace("٣", "3").Replace("٤", "4").Replace("٥", "5").
                Replace("٦", "6").Replace("٧", "7").Replace("٨", "8").Replace("٩", "9"));
        }

        public  string ConvertToArabicNumbers(string text)
        {
            return text.Replace("١", "1").Replace("٢", "2").Replace("٣", "3").Replace("٤", "4").Replace("٥", "5").Replace("٦", "6").Replace("٧", "7").Replace("٨", "8").Replace("٩", "9").Replace("٠", "0");
        }
    }
}
