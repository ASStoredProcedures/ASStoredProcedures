---
title: AsymmetricSet
version: "1.0"
permissions: S
class: AsymmetricSet
comments: true
functions:
    - AsymmetricSet
---

The AsymmetricSet function provides a way of filtering using a logical OR. When a tuple is specified in the WHERE clause it is essentially a logical AND. 

For example the tuple of 

```raw
{ [Gender].[Male],[MaritalStatus].[Married] }
```

filters the results down to people that are Male **and** Married. If you want to filter for results from people that are either Male **or** Married, then the long hand way of doing this is to specify a filter set of 

```raw
{ 
    ([Gender].[Male],[Marital Status].[All Marital Status])
    ,([Gender].[All Gender], [Marital Status].[Married])
}
```

The AsymmetricSet function will build this filter set for you based on the parameters that are passed in.

eg. 

```raw
AsymmetricSet([Gender].[Male],[Marital Status].[Married])
```

AsymmetricSet has two different signatures:

**AsymmetricSet(_Member1_, _Member2_, ... _Member8_)**

You can pass between 2 to 8 members to the function and it will return an Asymmetric set. 


> There is no specific reason behind the upper limit of eight members other than the fact that Analysis Services 2005 does not support parameter arrays so each additional parameter need to be coded as an additional function overload. If you need to pass in more than eight members it is easy enough to alter the source and add function overloads with the required number of parameters.



**AsymmetricSet(_tuple_)**

This function will take a tuple and convert it to an asymmetric set. This syntax is not restricted to any particular number of members, but, being a tuple you cannot have more than one member from any one hierarchy.