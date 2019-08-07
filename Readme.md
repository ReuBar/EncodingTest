Encoding Test

This solves Exercise 3 for the Core Engineers. 

Considering how simple the solution is, it was decided to keep it simple, without abusing design patterns
or over-architecting or over-engineering the solution. For this reason the main methods are in a simple
Class Library, which are then called from a simple Console Application.

The solution is made up of three Projects, all using .Net Framework 4.7.2 

	Blake2bHasher is a Windows Console application which is meant to be a standalone application to run the Implementation.
	It contains a config file which contains two tags. These can be modified to deal with different inputs.
		EncodedString contains the Encoded string to start from, which is BER TLV Encoded
		TagIds is a comma separated list of hex values to specify which tags to extract

	EncodingHandler is a Class Library which contains the Business Logic layer. It exposes all methods needs,
	plus a main one GetBase64HashFromEncodedStringForGivenTags which returns the Hashed value.
		It makes use of the following two NuGet libraries:
			https://github.com/kspearrin/BerTlv.NET  License: Free to Use
			This is used to parse the BerTlv String

			https://github.com/saucecontrol/Blake2Fast License: MIT
			This is used to Hash the code created.

			Microsoft.AspNetCore.WebUtilities
			This is used to Base64URLEncode the resulting byte[]

		In an actual production environment due diligence would have been performed to ensure
		that the libraries being used are stable, safe, and supported.
		These were chosen for this exercise because they seemed popular enough and gave correct results.

	EncodingUnitTests contains all Unit Tests implemented using the default MSTest Framework.
	Given more time metrics such as speed metrics and RAM usage would have been recorded


	
REASONING:

The solution makes use of Buffering for the Hashing. This is to ensure that if big data is provided, 
it will not run out of memory or slow down other processes running. The values of Digest Length and
Buffer Size can be changed easily as exposed parameters.

The Using keywork was used whenever a Stream was handled, to ensure proper disposal of IDisposable resources.

The name of the company was left out of the solution, to avoid other candidates finding ready-solutions on Github.


ASSUMPTIONS: 

If Tags are missing, it will safely ignore them as opposed to throwing an Exception.
This can be easily changed if the requirements are otherwise.

The Digest Length chosen for the Blake2b algorithm was 32, since this gave the same result as expected.


TODO:

Given more time:
	A better structure for the code would have been used
	A logging framework such as Log4Net would have been used
	I would have created a proper branch and worked on it, and merged at the end, rather than working on Main
	Extension methods would have been used for methods such as converting hex to int.
	LINQ could have been used for handling of data, but for readability this was not done to avoid having massive one-liners.

