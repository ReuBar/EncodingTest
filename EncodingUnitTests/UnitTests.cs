using EncodingHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncodingUnitTests
{
    [TestClass]
    public class UnitTests
    {

        #region HexConversion

        [TestMethod]
        public void Convert1()
        {
            Assert.AreEqual(1, BerTlvLogic.HexadecimalToInteger("0x01"));
        }

        [TestMethod]
        public void Convert3()
        {
            Assert.AreEqual(3, BerTlvLogic.HexadecimalToInteger("0x03"));
        }

        [TestMethod]
        public void Convert4()
        {
            Assert.AreEqual(4, BerTlvLogic.HexadecimalToInteger("0x04"));
        }

        [TestMethod]
        public void Convert5()
        {
            Assert.AreEqual(5, BerTlvLogic.HexadecimalToInteger("0x05"));
        }

        [TestMethod]
        public void Convert6()
        {
            Assert.AreEqual(6, BerTlvLogic.HexadecimalToInteger("0x06"));
        }

        [TestMethod]
        public void Convert10()
        {
            Assert.AreEqual(10, BerTlvLogic.HexadecimalToInteger("0x0a"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConvertEmpty()
        {
            Assert.AreEqual(10, BerTlvLogic.HexadecimalToInteger(""));
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertNotAHex()
        {
            Assert.AreEqual(10, BerTlvLogic.HexadecimalToInteger("NotAHex"));
        }

        #endregion

        #region BerValueExtraction

        [TestMethod]
        public void ExtractCorrectTagNumber()
        {
            string originalEncoded = @"0812561ae6bd42cfb7bee1311a29893508e71c340912e4614f3d9e6c61dccdd5af228051eb4373170A058394b17ac301086ef72ac5367180eb051040d4241e426bbde12bca805f665227e8060234ec07042859ffbc0202b4ed0304bdadb4e00402de60";
            string[] requestedTags = { "0x01", "0x03", "0x04", "0x05", "0x06", "0x0a" };
            List<string> extractedValues = BerTlvLogic.ExtractSortedRequestedTagsFromString(requestedTags, originalEncoded);
            Assert.AreEqual(6, extractedValues.Count);
        }

        [TestMethod]
        public void MissingTagsAreSafelyIgnored()
        {
            string originalEncoded = @"0812561ae6bd42cfb7bee1311a29893508e71c340912e4614f3d9e6c61dccdd5af228051eb4373170A058394b17ac301086ef72ac5367180eb051040d4241e426bbde12bca805f665227e8060234ec07042859ffbc0202b4ed0304bdadb4e00402de60";
            string[] requestedTags = { "0x01", "0x03", "0x04", "0x05", "0x06", "0x0a", "0xaa" };
            List<string> extractedValues = BerTlvLogic.ExtractSortedRequestedTagsFromString(requestedTags, originalEncoded);
            Assert.AreEqual(6, extractedValues.Count);
        }

        [TestMethod]
        public void AllTagsAreMissing()
        {
            string originalEncoded = @"0812561ae6bd42cfb7bee1311a29893508e71c340912e4614f3d9e6c61dccdd5af228051eb4373170A058394b17ac301086ef72ac5367180eb051040d4241e426bbde12bca805f665227e8060234ec07042859ffbc0202b4ed0304bdadb4e00402de60";
            string[] requestedTags = { "0xaa", "0xba" };
            List<string> extractedValues = BerTlvLogic.ExtractSortedRequestedTagsFromString(requestedTags, originalEncoded);
            Assert.AreEqual(0, extractedValues.Count);
        }

        [TestMethod]
        public void NoTagsReturnsEmptyList()
        {
            string originalEncoded = @"0812561ae6bd42cfb7bee1311a29893508e71c340912e4614f3d9e6c61dccdd5af228051eb4373170A058394b17ac301086ef72ac5367180eb051040d4241e426bbde12bca805f665227e8060234ec07042859ffbc0202b4ed0304bdadb4e00402de60";
            string[] requestedTags = { };
            List<string> extractedValues = BerTlvLogic.ExtractSortedRequestedTagsFromString(requestedTags, originalEncoded);
            Assert.AreEqual(0, extractedValues.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ExtractTagsFromCorruptedString()
        {
            string originalEncoded = @"ThisIsClearlyNotBerTlvEncoded";
            string[] requestedTags = { "0x01", "0x03", "0x04", "0x05", "0x06", "0x0a", "0xaa" };
            List<string> extractedValues = BerTlvLogic.ExtractSortedRequestedTagsFromString(requestedTags, originalEncoded);
            Assert.AreEqual(0, extractedValues.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ExtractNullTags()
        {
            string originalEncoded = @"ThisIsClearlyNotBerTlvEncoded";
            string[] requestedTags = null;
            List<string> extractedValues = BerTlvLogic.ExtractSortedRequestedTagsFromString(requestedTags, originalEncoded);
            Assert.AreEqual(0, extractedValues.Count);
        }

        #endregion

        #region StringConcatenation

        [TestMethod]
        public void ConcatenateNullList()
        {
            Assert.AreEqual(String.Empty, BerTlvLogic.CreateFullstopSeparatedString(null));
        }

        [TestMethod]
        public void ConcatenateEmptyList()
        {
            Assert.AreEqual(String.Empty, BerTlvLogic.CreateFullstopSeparatedString(new List<string>()));
        }


        [TestMethod]
        public void ConcatenatePopulatedList()
        {
            Assert.AreEqual("6EF72AC5367180EB.BDADB4E0.DE60.40D4241E426BBDE12BCA805F665227E8.34EC.8394B17AC3",
                BerTlvLogic.CreateFullstopSeparatedString(new List<string>
            {
                "6EF72AC5367180EB",
                "BDADB4E0",
                "DE60",
                "40D4241E426BBDE12BCA805F665227E8",
                "34EC",
                "8394B17AC3"
            }));
        }

        #endregion

        #region HashingOfStreams

        [TestMethod]
        public void HashDefaultValuesStream()
        {
            string startString = "6EF72AC5367180EB.BDADB4E0.DE60.40D4241E426BBDE12BCA805F665227E8.34EC.8394B17AC3";
            using (Stream temp = new MemoryStream(Encoding.UTF8.GetBytes(startString)))
            {
               Assert.AreEqual(@"EQ9q1Hjyr/pfSBj1/M2vqAMFH+MYhP2zwhgYfC8u8+g=", BerTlvLogic.ComputeBase64Blake2bHashInBuffers(temp));
            }           
        }

        [TestMethod]
        public void HashStreamWithLargeBuffer()
        {
            string startString = "6EF72AC5367180EB.BDADB4E0.DE60.40D4241E426BBDE12BCA805F665227E8.34EC.8394B17AC3";
            using (Stream temp = new MemoryStream(Encoding.UTF8.GetBytes(startString)))
            {
                Assert.AreEqual(@"EQ9q1Hjyr/pfSBj1/M2vqAMFH+MYhP2zwhgYfC8u8+g=", BerTlvLogic.ComputeBase64Blake2bHashInBuffers(temp,32,1280));
            }
        }

        [TestMethod]
        public void HashStreamWithSmallBuffer()
        {
            string startString = "6EF72AC5367180EB.BDADB4E0.DE60.40D4241E426BBDE12BCA805F665227E8.34EC.8394B17AC3";
            using (Stream temp = new MemoryStream(Encoding.UTF8.GetBytes(startString)))
            {
                Assert.AreEqual(@"EQ9q1Hjyr/pfSBj1/M2vqAMFH+MYhP2zwhgYfC8u8+g=", BerTlvLogic.ComputeBase64Blake2bHashInBuffers(temp, 32, 1));
            }
        }

        [TestMethod]
        public void HashStreamWithEmptyString()
        {
            string startString = "";
            using (Stream temp = new MemoryStream(Encoding.UTF8.GetBytes(startString)))
            {
                Assert.AreEqual(@"DldRwCblQ7Loqy6wYJnaodHl30d3j3eH+qtFzfEv46g=", BerTlvLogic.ComputeBase64Blake2bHashInBuffers(temp));
            }
        }

        #endregion
    }
}
