---
title: StringFilters
version:
permissions:
class: StringFilters
functions:
  - IsLike
  - Like
  - RegExFilter
---

There are 3 functions in the StringFilters class

**RegExFilter(_Set_, _RegExPattern_, _Expression_, _\[ caseSensitive]_ )**

This function returns a set of the members from the _Set_ parameter than match the _RegExPattern_ parameter when it is evaluated against the _Expression_ parameter. The _RegExPattern_ parameter can be any valid regular expression. The _Expression_ parameter provides the flexibility that the function can be run against member names, keys, captions or any expression that returns a string result.


**Like(_Set_, _LikePattern_, _Expression_, _\[caseSensitive]_ )**

This function uses the same syntax as the T-SQL _LIKE_ operator, using the following wildcard characters in its patterns:

* % - matches 0 or more of any character
* _ - matches a single character


It returns a set of the members from the _Set_ parameter than match the _LikePattern_ parameter when it is evaluated against the _Expression_ parameter.



eg

```raw
  // Returns all customers whose name contains a single initial followed by a full stop

  Like([Customer].[Customer].Members

      ,"% _. %"

      ,[Customer].[Customer].CurrentMember.Name)
```


**IsLike(_StringToMatch_, _RegExPattern_, _\[caseSensitive]_ )**

Returns true if the first parameter matches the "Like" pattern that is provided as the second parameter. This variation of the function would most likely be used with functions like IIF() and FILTER().


> **NOTE:** There appears to be an issue with running the string filters code under SP1 which manifests itself by throwing relatively frequent exceptions. Unfortunately, at the present time it is not  advisable to use this code in a production environment.



> **UPDATE:** _(14 Nov 2006)_ It looks like the issue noted about may have been corrected in build 2154 (see [http://support.microsoft.com/kb/918753/](http://support.microsoft.com/kb/918753/)) and as such should be included in SP2 when it is released.