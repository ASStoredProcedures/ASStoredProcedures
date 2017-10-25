---
title: FindCurrentMember
version: "1.0"
permissions: S
class: FindCurrenMembers
functions:
    - FindCurrentMember
    - FindCurrentTuple
---

### FindCurrentMember() and FindCurrentMemberVerbose()

The FindCurrentMember and FindCurrentMemberVerbose functions are simple examples of how you can use the Context class. They show you what the currentmember is on every attribute hierarchy on every dimension - useful when you are trying to understand how selecting one member on an attribute hierarchy affects the currentmember on other attribute hierarchies on an Analysis Services 2005 dimension.

### FindCurrentTuple()

FindCurrentTuple(). This does pretty much the same as the other functions in the class except it generates a tuple in the form of a string and it ignores a hierarchy when the currentmember is the same as the default member, making the output much more readable. 



> **LIMITATIONS:** _it doesn’t work with sets in the WHERE clause, although it could be modified to do so; it won’t work with subselects or calculated members defined outside the MDX Script_