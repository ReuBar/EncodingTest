using SauceControl.Blake2Fast;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EncodingHandler
{
    public static class BerTlvLogic
    {
        public static string GetBase64HashFromEncodedStringForGivenTags(string[] tags, string encodedString)
        {
            List<string> sortedValues = ExtractSortedRequestedTagsFromString(tags, encodedString);
            string concatenatedString = CreateFullstopSeparatedString(sortedValues);

            string toReturn;
            using (Stream temp = new MemoryStream(Encoding.UTF8.GetBytes(concatenatedString)))
            {
                toReturn =  BerTlvLogic.ComputeBase64Blake2bHashInBuffers(temp);
            }

            return toReturn;

        }

        /// <summary>
        /// Wrap Hexademical to Integer conversion into a well-named method
        /// </summary>
        /// <param name="hex">Hexadecimal value to convert, in format 0x** </param>
        /// <returns></returns>
        public static int HexadecimalToInteger(string hex)
        {
            return Convert.ToInt32(hex.Trim(), 16);
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


        /// <summary>
        /// Since data provided might be massive, it is not recommended to read it all into memory at the same time.
        /// For this reason we use Streams and only hash it in chunks
        /// </summary>
        /// <param name="data">Stream of data to be hashed</param>
        /// <param name="digestLength">Optional parameter for digest Length to be used when Hashing. Defaults to 32</param>
        /// <param name="bufferToDigestRatio">Optional parameter to define buffer size as a ratio to the Digest Length. Defaults to 128
        /// The bigger the bufferToDigestRatio, the more you are sacrificing memory use to gain speed.</param>
        /// <returns>Returns Base64 Encoded String with hashed value</returns>
        public static string ComputeBase64Blake2bHashInBuffers(Stream data, int digestLength = 32, int bufferToDigestRatio = 128)
        {
            var hasher = Blake2b.CreateIncrementalHasher(digestLength);
            var buffer = ArrayPool<byte>.Shared.Rent(digestLength);

            int bytesRead;
            while ((bytesRead = data.Read(buffer, 0, buffer.Length)) > 0)
                hasher.Update(new Span<byte>(buffer, 0, bytesRead));

            ArrayPool<byte>.Shared.Return(buffer);

            return Convert.ToBase64String(hasher.Finish());

        }
    }
}
