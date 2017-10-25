---
title: LinkMember
version: "1.1"
permissions: S
class: LinkMember
functions:
  - HierarchyLinkMember
  - LevelLinkMember
---


The built-in LinkMember function may not behave as you are expecting. For instance, the following query returns ```[Ship Date].[Calendar].[H1 CY 2004]``` not ```[Ship Date].[Calendar].[January 2004]```:



```raw

select {} on 0,

LinkMember([Date].[Calendar].[January 2004],[Ship Date].[Calendar]) on 1

from [Adventure Works]

```

The stored procedures in the LinkMember class provide an alternative to this behavior. The following query returns ```[Ship Date].[Calendar].[January 2004]``` as you were expecting:

```raw

select {} on 0,

ASSP.HierarchyLinkMember([Date].[Calendar].[January 2004],"[Ship Date].[Calendar]") on 1

from [Adventure Works]

```

The signatures for these functions are as follows:

**HierarchyLinkMember(_Member_, _HierarchyUniqueNameString_)**

This function returns the member with keys and level matching the member passed in as the first parameter. It searches within the hierarchy specified in the second parameter.


**LevelLinkMember(_Member_, _LevelUniqueNameString_)**

This function returns the member with keys matching the member passed in as the first parameter. It searches within the level specified in the second parameter.


> **General Note:** The signature of the stored proc asks for the hierarchy unique name as a string. This signature allows the function to run 50 times faster than if a Hierarchy object were used as the parameter.

It should be noted that this could be accomplished with plain MDX. If you know that a month has been selected, the MDX could use the attribute hierarchy and read as follows:

```raw

select {} on 0,

LinkMember([Date].[Calendar].[January 2004],[Ship Date].[Month Name]) on 1

from [Adventure Works]

```

However, if the level (quarter, month, etc.) is now known ahead of time, the MDX must be more complex:

```raw

select {} on 0,

case

 when [Date].[Calendar].CurrentMember.Level is [Date].[Calendar].[Calendar Year]

  then LinkMember([Date].[Calendar].CurrentMember,[Ship Date].[Calendar Year])

 when [Date].[Calendar].CurrentMember.Level is [Date].[Calendar].[Calendar Semester]

  then LinkMember([Date].[Calendar].CurrentMember,[Ship Date].[Calendar Semester])

 when [Date].[Calendar].CurrentMember.Level is [Date].[Calendar].[Calendar Quarter]

  then LinkMember([Date].[Calendar].CurrentMember,[Ship Date].[Calendar Quarter])

 when [Date].[Calendar].CurrentMember.Level is [Date].[Calendar].[Month]

  then LinkMember([Date].[Calendar].CurrentMember,[Ship Date].[Month Name])

 when [Date].[Calendar].CurrentMember.Level is [Date].[Calendar].[Date]

  then LinkMember([Date].[Calendar].CurrentMember,[Ship Date].[Date])

end

on 1

from [Adventure Works]

where ([Date].[Calendar].[January 2004])

```