using EncodingHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blake2bHasher
{
    class Program
    {
        static void Main(string[] args)
        {

            #region Read Data from Config File
            string encodedStringKey = "EncodedString";
            string tagsIdKey = "TagIds";
            string encodedString = ConfigurationManager.AppSettings[encodedStringKey];
            if (String.IsNullOrWhiteSpace(encodedString))
            {
                Console.WriteLine($"Missing {encodedStringKey} value in ConfigFile. Current Value is: {encodedString}. Press any Key to Exit.");
                Console.ReadKey();
                return;
            }

            string tagIds = ConfigurationManager.AppSettings[tagsIdKey];
            if (String.IsNullOrWhiteSpace(tagIds))
            {
                Console.WriteLine($"Missing {tagsIdKey} value in ConfigFile. Current Value is: {tagIds}. Press any Key to Exit.");
                Console.ReadKey();
                return;
            }

            string[] requestedTags = tagIds.Split(new char[] { ',' });
            if (requestedTags == null || requestedTags.Length < 1)
            {
                Console.WriteLine($"No tags to extract. TagIds obtained from Config file are: {tagIds}. Press any Key to Exit.");
                Console.ReadKey();
                return;
            }

            #endregion

            #region Get hash from values extracted

            try
            {
                string hash = BerTlvLogic.GetBase64HashFromEncodedStringForGivenTags(requestedTags, encodedString);
                Console.WriteLine($"Extracted hash: {hash} from Encoded String: {encodedString} for tags: {String.Join(",", requestedTags)}. Press any Key to Exit.");
                Console.ReadKey();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown when trying to Convert String: {encodedString} for tags: {String.Join(",", requestedTags)}. Exception is: {ex.Message}. Press any Key to Exit.");
                Console.ReadKey();
                return;
            }

            #endregion
        }
    }
}
