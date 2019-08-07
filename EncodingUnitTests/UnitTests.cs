using System;
using System.Collections.Generic;
using EncodingHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            string[] requestedTags = { "0x01", "0x03", "0x04", "0x05", "0x06", "0x0a","0xaa" };
            List<string> extractedValues = BerTlvLogic.ExtractSortedRequestedTagsFromString(requestedTags, originalEncoded);
            Assert.AreEqual(6, extractedValues.Count);
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
        public void CorruptedString()
        {
            string originalEncoded = @"ThisIsClearlyNotBerTlvEncoded";
            string[] requestedTags = { };
            List<string> extractedValues = BerTlvLogic.ExtractSortedRequestedTagsFromString(requestedTags, originalEncoded);
            Assert.AreEqual(0, extractedValues.Count);
        }

        #endregion
    }
}
