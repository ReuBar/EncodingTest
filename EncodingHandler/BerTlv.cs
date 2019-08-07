using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodingHandler
{
    public static class BerTlvLogic
    {
        /// <summary>
        /// Wrap Hexademical to Integer conversion into a well-named method
        /// </summary>
        /// <param name="hex">Hexadecimal value to convert, in format 0x** </param>
        /// <returns></returns>
        public static int HexadecimalToInteger(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        /// <summary>
        /// Given a list of tags to extract, and an encoded String, return a sorted List<string> of values,
        /// where values are sorted by the tag ID. Throws FormatException if encodedString is corrupted.
        /// It will ignore any missing Tags, as opposed to throwing an exception.
        /// </summary>
        /// <param name="tags">Array of tags to extract</param>
        /// <param name="encodedString">Encoded string to extract values from</param>
        /// <returns>Sorted List of strings with extracted values </returns>
        public static List<string> ExtractSortedRequestedTagsFromString(string[] tags, string encodedString)
        {
            var allBerValues = BerTlv.Tlv.ParseTlv(encodedString);

            List<int> sortedIds = tags.Select(f => HexadecimalToInteger(f)).ToList();
            sortedIds.Sort();

            List<string> requestedTagsToReturn = new List<string>();
            foreach (int tagId in sortedIds)
            {
                if (allBerValues.Any(f => f.Tag == tagId))
                {
                    requestedTagsToReturn.Add(allBerValues.First(g => g.Tag == tagId).HexValue);
                }
            }
            return requestedTagsToReturn;

        }

        /// <summary>
        /// Taking a sorted List of String values, concatenate them using a separator.
        /// The separator is optional, and defaults to a full stop (.)
        /// </summary>
        /// <param name="sortedValues">A List of Sorted String Values to concatenate</param>
        /// <param name="separator">An optional string that is used to separate the string values provided</param>
        /// <returns>One string made up of all values provided, in order, separated by the provided separator. 
        /// If this is not provided, a full-stop is used.</returns>
        public static string CreateFullstopSeparatedString(List<string> sortedValues, string separator = ".")
        {
            if (sortedValues == null)
                return String.Empty;
            return String.Join(".", sortedValues);
        }
    }
}
