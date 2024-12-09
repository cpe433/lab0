This is the sourcecode for Lab 0 of CPE433, Computer Engineering, Chiang Mai University.

Software Requirement:
- .Net Core 8.0

Build Instruction:
You should be able to build using command line (i.e., dotnet build) or through your IDE without any extra library

Lab Instruction:
Current code will load only the webpage of the given URL, make it "recursively" load all webpages from the link of the original page.

Extra score:
Can you improve Main method in Program class? for example, those parameters should not be hard-coded.


# Lab 0: Crawler
### Papop Khemngoen 660615033

**Context:**

We're going to develop a simple [Web Crawler](https://en.wikipedia.org/wiki/Web_crawler) using C#.

**Requirement:**

Implement recursive operation inside the source code. Currently, it will download only a single page, make it follows the links inside webpage to download linked page.
## Changes made by me
### 1. Infinite Recursion Prevention
![A snap from my code](https://i.postimg.cc/xT2p9Wfr/image.png)
The code is a base case for recursive function to prevent infinite recursion. It stops the crawling when the specified recursion depth (level) reaches zero, preventing the crawler from endlessly following links.

### 2. Recursive GetPage Operation
![Recursive GetPage Snippet](https://i.postimg.cc/bvpqb6H4/image.png)
This line of code added will look up for links, and recursively access them and decrement from each call by 1, and with the code above, the crawler will stop when recursion level reaches 0.

### 3. Program Class Improvement
![Program Class Snippet](https://i.postimg.cc/kGQmBKR7/image.png)
Changed to `async Task Main` for proper async handling, then with using await with GetPage, we can now omit .Wait() function, and lastly, Path.GetTempPath() will ensure a reliable temporary folder

