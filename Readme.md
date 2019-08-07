# Encoding Test #

This solves Exercise 3 for the Core Engineers. 

Considering how simple the solution is, it was decided to keep it elegant, without abusing design patterns
or over-architecting or over-engineering the solution. For this reason the main methods are in a simple
Class Library, which are then called from a simple Console Application.

The solution is made up of three Projects, all using .Net Framework 4.7.2 

* EncodingUnitTests contains all Unit Tests implemented using the default MSTest Framework.
	* Given more time metrics such as speed metrics and RAM usage would have been recorded

* Blake2bHasher is a Windows Console application which is meant to be a standalone application to run the Implementation.
	* It contains a config file which contains two tags. These can be modified to deal with different inputs.
		* EncodedString contains the Encoded string to start from, which is BER TLV Encoded
		* TagIds is a comma separated list of hex values to specify which tags to extract

* EncodingHandler is a Class Library which contains the Business Logic layer. It exposes all methods needed,
plus a main one GetBase64HashFromEncodedStringForGivenTags which returns the Hashed value.
	* It makes use of the following three NuGet libraries:

Nuget Library | URL | License | Reason
------------- | ------------- | ------------- | -------------
BerTlv.NET  | https://github.com/kspearrin/BerTlv.NET | Free to use | This is used to parse the BerTlv String
Blake2Fast  | https://github.com/saucecontrol/Blake2Fast | MIT | This is used to Hash the code created.
Microsoft.AspNetCore.WebUtilities | https://www.nuget.org/packages/Microsoft.AspNetCore.WebUtilities/ | Apache License 2 | This is used to Base64URLEncode the resulting byte[]

 ___In an actual production environment due diligence would have been performed to ensure that the libraries being used are stable, safe, and supported. These were chosen for this exercise because they seemed popular enough and gave correct results.___



	
## REASONING: ##

* The solution makes use of Buffering for the Hashing. This is to ensure that if big data is provided, 
it will not run out of memory or slow down other processes running. The values of Digest Length and
Buffer Size can be changed easily as exposed parameters.

* The Using keywork was used whenever a Stream was handled, to ensure proper disposal of IDisposable resources.

* The name of the company was left out of the solution, to avoid other candidates finding ready-solutions on Github.


## ASSUMPTIONS: ##

* If Tags are missing, it will safely ignore them as opposed to throwing an Exception.
This can be easily changed if the requirements are otherwise.

* The Digest Length chosen for the Blake2b algorithm was 32, since this gave the same result as expected.


## TODO: ## 

Given more time:
	
* A better structure for the code would have been used
* A logging framework such as Log4Net would have been used
* I would have created a proper branch and worked on it, and merged at the end, rather than working on Main.
* Extension methods would have been used for methods such as converting hex to int.
* LINQ could have been used for handling of data, but for readability this was not done to avoid having massive one-liners.
* Performance metrics would have been gathered and recorded over time, to ensure no performance degradation happens with subsequent commits.
* I would have exposed DigestLength and BufferSize all the way to the config file to make them more configurable. 

