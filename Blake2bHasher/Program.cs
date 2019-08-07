using EncodingHandler;
using System;
using System.Configuration;

namespace Blake2bHasher
{
    class Program
    {
        static void Main(string[] args)
        {

            #region Read Data from Config File

            string encodedString = ConfigurationManager.AppSettings[ConfigKeys.EncodedStringKey];
            if (String.IsNullOrWhiteSpace(encodedString))
            {
                Console.WriteLine($"Missing {ConfigKeys.EncodedStringKey} value in ConfigFile. Current Value is: {encodedString}. Press any Key to Exit.");
                Console.ReadKey();
                return;
            }

            string tagIds = ConfigurationManager.AppSettings[ConfigKeys.TagsIdKey];
            if (String.IsNullOrWhiteSpace(tagIds))
            {
                Console.WriteLine($"Missing {ConfigKeys.TagsIdKey} value in ConfigFile. Current Value is: {tagIds}. Press any Key to Exit.");
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
                string hash = BerTlvLogic.GetBase64UrlHashFromEncodedStringForGivenTags(requestedTags, encodedString);
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
