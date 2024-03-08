using System.Text;

namespace Northwind.Features.AdditionalSources.PhoneticSearch
{
    public static class Soundex
    {
        public static string Compute(string data)
        {
            StringBuilder result = new StringBuilder();

            if (data != null && data.Length > 0)
            {
                string previousCode = "", currentCode = "", currentLetter = "";
                result.Append(data[0]); // keep initial char

                for (int i = 0; i < data.Length; i++) //start at 0 in order to correctly encode "Pf..."
                {
                    currentLetter = data[i].ToString().ToLower();
                    currentCode = "";

                    if ("bfpv".Contains(currentLetter))
                        currentCode = "1";
                    else if ("cgjkqsxz".Contains(currentLetter))
                        currentCode = "2";
                    else if ("dt".Contains(currentLetter))
                        currentCode = "3";
                    else if (currentLetter == "l")
                        currentCode = "4";
                    else if ("mn".Contains(currentLetter))
                        currentCode = "5";
                    else if (currentLetter == "r")
                        currentCode = "6";

                    if (currentCode != previousCode && i > 0) // do not add first code to result string
                        result.Append(currentCode);

                    if (result.Length == 4) break;

                    previousCode = currentCode; // always retain previous code, even empty
                }
            }
            if (result.Length < 4)
                result.Append(new string('0', 4 - result.Length));

            return result.ToString().ToUpper();
        }
    }
}
