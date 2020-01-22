# Hash Value Format Validation Performance

In this repository the performance of various methods is measured of checking if a string contains a valid hash value.

## Measurements

### What is a valid hash value?

A valid hash value is described by the following regular expressions ([PCRE syntax](https://www.pcre.org/current/doc/html/pcre2syntax.html)):
- **MD5:** `\A[[:xdigit:]]{32}\z`
- **SHA1:** `\A[[:xdigit:]]{40}\z`
- **SHA256:** `\A[[:xdigit:]]{64}\z`
- **SHA384:** `\A[[:xdigit:]]{96}\z`
- **SHA512:** `\A[[:xdigit:]]{128}\z`

**Examples**

- **MD5:** c3f7276866858ec107f633b511d691ba
- **SHA1:** 1e9224399b8cbb3fa02f12808b96bbb90cb5e0a3
- **SHA256:** a88a31c27a8de03b866f529696354ad5a42a6e65ca9c5514c2790822492e6410
- **SHA384:** d58fb0ad67ec3330e7a17d12a59b4944d91f9b9d48d9fbe053b61944378cc1c6217c07f8aa49c6c85bac2a33d8ce025b
- **SHA512:** 016c082c2213d7b22e0c0722ae70e2835fb3c700bd136a08fe8f39ef6baa0f3dfb8ac147b86bae52ae93e34b544f3a345efe7b4f2b59ffa5e78fb5956e399624

### Which methods are used?

`HashValueLength` is an integer that describes the length of a valid hash value string.  
`ValidationPattern` is a `System.Text.RegularExpressions.Regex` instance that uses one of the above described regex patterns.
`allowedChars` is a constant that stores the string `"0123456789ABCDEFabcdef"`

#### Validate with Regex

```C#
public bool ValidateWithRegex(string hash)
{
    return hash != null && ValidationPattern.IsMatch(hash);
}
```

#### Validate with LINQ (Language Integrated Query)

```C#
public bool ValidateWithLinq(string hash)
{
    return hash != null && hash.Length == HashValueLength && hash.All(c => allowedChars.Contains(c));
}
```
#### Validate with for loop over all characters (Version 1)

```C#
public bool ValidateWithForLoop1(string hash)
{
    if (hash == null || hash.Length != HashValueLength)
        return false;

    char c;

    for (int i = 0; i < hash.Length; ++i)
    {
        c = hash[i];
        
        if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
            return false;
    }

    return true;
}
```

#### Validate with for loop over all characters (Version 2)

```C#
public bool ValidateWithForLoop2(string hash)
{
    if (hash == null || hash.Length != HashValueLength)
        return false;

    char c;
    char[] charArr = hash.ToCharArray();

    for (int i = 0; i < hash.Length; ++i)
    {
        c = charArr[i];
        
        if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
            return false;
    }

    return true;
}
```

#### Validate with a bidirectional for loop over all characters

```C#
public bool ValidateWithBidirectionalForLoop(string hash)
{
    if (hash == null || hash.Length != HashValueLength)
        return false;

    char c;
    char[] charArr = hash.ToCharArray();
    int end = charArr.Length / 2;

    for (int i = 0; i < end;)
    {
        c = charArr[i++];
        
        if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
            return false;

        c = charArr[^i];

        if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
            return false;
    }

    return true;
}
```

#### Validate with foreach loop over all characters (Version 1)

```C#
public bool ValidateWithForeachLoop1(string hash)
{
    if (hash == null || hash.Length != HashValueLength)
        return false;

    foreach (char c in hash)
    {
        if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
            return false;
    }

    return true;
}
```

#### Validate with foreach loop over all characters (Version 2)

```C#
public bool ValidateWithForeachLoop2(string hash)
{
    if (hash == null || hash.Length != HashValueLength)
        return false;

    foreach (char c in hash.ToCharArray())
    {
        if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
            return false;
    }

    return true;
}
```

### Results

Note: Each sample exists twice; one in uppercase and one in lowercase.

#### Sample Size: 100.000

##### MD5

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    00   |      099     |        992239 |
| ValidateWithLinq                 |    00   |      124     |       1244031 |
| ValidateWithForLoop1             |    00   |      064     |        648103 |
| ValidateWithForLoop2             |    00   |      058     |        588684 |
| ValidateWithBidirectionalForLoop |    00   |      061     |        612690 |
| ValidateWithForeachLoop1         |    00   |      063     |        634852 |
| ValidateWithForeachLoop2         |    00   |      052     |        528246 |

##### SHA1

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    00   |      122     |       1223646 |
| ValidateWithLinq                 |    00   |      146     |       1467781 |
| ValidateWithForLoop1             |    00   |      076     |        762538 |
| ValidateWithForLoop2             |    00   |      068     |        681149 |
| ValidateWithBidirectionalForLoop |    00   |      069     |        699009 |
| ValidateWithForeachLoop1         |    00   |      073     |        738280 |
| ValidateWithForeachLoop2         |    00   |      061     |        613838 |

##### SHA256

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    00   |      189     |       1598398 |
| ValidateWithLinq                 |    00   |      224     |       2242073 |
| ValidateWithForLoop1             |    00   |      110     |       1103268 |
| ValidateWithForLoop2             |    00   |      099     |        999415 |
| ValidateWithBidirectionalForLoop |    00   |      102     |       1029035 |
| ValidateWithForeachLoop1         |    00   |      106     |       1068247 |
| ValidateWithForeachLoop2         |    00   |      089     |        896481 |

##### SHA384

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    00   |      226     |       2266556 |
| ValidateWithLinq                 |    00   |      343     |       3430587 |
| ValidateWithForLoop1             |    00   |      156     |       1561352 |
| ValidateWithForLoop2             |    00   |      143     |       1435330 |
| ValidateWithBidirectionalForLoop |    00   |      146     |       1469542 |
| ValidateWithForeachLoop1         |    00   |      151     |       1514509 |
| ValidateWithForeachLoop2         |    00   |      129     |       1292137 |

##### SHA512

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    00   |      294     |       2944697 |
| ValidateWithLinq                 |    00   |      424     |       4245834 |
| ValidateWithForLoop1             |    00   |      201     |       2011717 |
| ValidateWithForLoop2             |    00   |      184     |       1841056 |
| ValidateWithBidirectionalForLoop |    00   |      192     |       1926557 |
| ValidateWithForeachLoop1         |    00   |      193     |       1936772 |
| ValidateWithForeachLoop2         |    00   |      164     |       1640374 |

#### Sample Size: 1.000.000

##### MD5

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    01   |      028     |      10287786 |
| ValidateWithLinq                 |    01   |      200     |      12009297 |
| ValidateWithForLoop1             |    00   |      644     |       6442640 |
| ValidateWithForLoop2             |    00   |      603     |       6030988 |
| ValidateWithBidirectionalForLoop |    00   |      586     |       5861373 |
| ValidateWithForeachLoop1         |    00   |      632     |       6325330 |
| ValidateWithForeachLoop2         |    00   |      525     |       5253087 |

##### SHA1

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    01   |      028     |      11398438 |
| ValidateWithLinq                 |    01   |      200     |      14437431 |
| ValidateWithForLoop1             |    00   |      761     |       7611447 |
| ValidateWithForLoop2             |    00   |      688     |       6885528 |
| ValidateWithBidirectionalForLoop |    00   |      690     |       6908000 |
| ValidateWithForeachLoop1         |    00   |      746     |       7468917 |
| ValidateWithForeachLoop2         |    00   |      613     |       6132205 |

##### SHA256

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    01   |      639     |      16390632 |
| ValidateWithLinq                 |    02   |      191     |      21918665 |
| ValidateWithForLoop1             |    01   |      110     |      11107600 |
| ValidateWithForLoop2             |    01   |      047     |      10473113 |
| ValidateWithBidirectionalForLoop |    01   |      015     |      10150580 |
| ValidateWithForeachLoop1         |    01   |      081     |      10813794 |
| ValidateWithForeachLoop2         |    00   |      898     |       8982058 |

##### SHA384

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    02   |      315     |      23158235 |
| ValidateWithLinq                 |    03   |      193     |      31937078 |
| ValidateWithForLoop1             |    01   |      552     |      15523011 |
| ValidateWithForLoop2             |    01   |      510     |      15104776 |
| ValidateWithBidirectionalForLoop |    01   |      600     |      16009009 |
| ValidateWithForeachLoop1         |    01   |      687     |      16870519 |
| ValidateWithForeachLoop2         |    01   |      353     |      13535071 |

##### SHA512

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    03   |      013     |      30135931 |
| ValidateWithLinq                 |    04   |      217     |      42172467 |
| ValidateWithForLoop1             |    02   |      035     |      20358748 |
| ValidateWithForLoop2             |    01   |      904     |      19046150 |
| ValidateWithBidirectionalForLoop |    01   |      889     |      18894773 |
| ValidateWithForeachLoop1         |    01   |      978     |      19787607 |
| ValidateWithForeachLoop2         |    01   |      646     |      16463711 |

#### Sample Size: 10.000.000

##### MD5

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    10   |      096     |     100962319 |
| ValidateWithLinq                 |    12   |      227     |     122275006 |
| ValidateWithForLoop1             |    06   |      520     |      65206552 |
| ValidateWithForLoop2             |    08   |      865     |      88651065 |
| ValidateWithBidirectionalForLoop |    06   |      193     |      61936386 |
| ValidateWithForeachLoop1         |    06   |      300     |      63001769 |
| ValidateWithForeachLoop2         |    05   |      269     |      52692391 |

##### SHA1

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    11   |      434     |     114346239 |
| ValidateWithLinq                 |    14   |      822     |     148227702 |
| ValidateWithForLoop1             |    07   |      680     |      76808934 |
| ValidateWithForLoop2             |    06   |      963     |      69631127 |
| ValidateWithBidirectionalForLoop |    07   |      289     |      72899504 |
| ValidateWithForeachLoop1         |    07   |      668     |      76687708 |
| ValidateWithForeachLoop2         |    06   |      198     |      61984647 |

##### SHA256

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    16   |      243     |     162430691 |
| ValidateWithLinq                 |    24   |      071     |     240710290 |
| ValidateWithForLoop1             |    11   |      257     |     112575687 |
| ValidateWithForLoop2             |    10   |      855     |     108555899 |
| ValidateWithBidirectionalForLoop |    10   |      560     |     105609528 |
| ValidateWithForeachLoop1         |    10   |      893     |     108934085 |
| ValidateWithForeachLoop2         |    08   |      915     |      89151628 |

##### SHA384

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    22   |      589     |     225897909 |
| ValidateWithLinq                 |    34   |      699     |     346990746 |
| ValidateWithForLoop1             |    15   |      696     |     156961042 |
| ValidateWithForLoop2             |    15   |      209     |     152093268 |
| ValidateWithBidirectionalForLoop |    15   |      278     |     152783572 |
| ValidateWithForeachLoop1         |    15   |      165     |     151650110 |
| ValidateWithForeachLoop2         |    15   |      437     |     154378758 |

##### SHA512

| Method used                      | Seconds | Milliseconds | Ticks (total) |
|----------------------------------|:-------:|:------------:|--------------:|
| ValidateWithRegex                |    30   |      134     |     301345397 |
| ValidateWithLinq                 |    43   |      975     |     439758632 |
| ValidateWithForLoop1             |    20   |      933     |     209336038 |
| ValidateWithForLoop2             |    22   |      311     |     223118960 |
| ValidateWithBidirectionalForLoop |    19   |      350     |     193500891 |
| ValidateWithForeachLoop1         |    20   |      235     |     202359275 |
| ValidateWithForeachLoop2         |    16   |      659     |     166597822 |

### Conclusion

* `ValidateWithForeachLoop2` is the clear winner (what realy surprised me)!
* `ValidateWithLinq` is the clear looser!

## Remark

The performance was just tested for valid hash values, because it almost never occures that an value
stores an invalid hash value. In my case validation is used just to ensure that data is not corrupted or malformed.

Maybe some of the osther methods performs better if the portion of invalid samples is higher; although I don't believe that.

## License

(MIT License)

Copyright (c) 2020 Dominik Viererbe

Permission is hereby granted, free of charge, to any person obtaining  
a copy of this software and associated documentation files (the  
"Software"), to deal in the Software without restriction, including  
without limitation the rights to use, copy, modify, merge, publish,  
distribute, sublicense, and/or sell copies of the Software, and to  
permit persons to whom the Software is furnished to do so, subject to  
the following conditions:  

The above copyright notice and this permission notice shall be  
included in all copies or substantial portions of the Software.  

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,  
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF  
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND  
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE  
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION  
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION  
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
