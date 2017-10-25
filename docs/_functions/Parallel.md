---
title: Parallel
version: "1.1"
permission: S
class: Parallel
functions:
    - ParallelGenerate
    - ParallelUnion
---
This class contains two functions, ParallelUnion and ParallelGenerate which allow two or more set operations to be executed in parallel, in order to improve query performance for calculation-heavy queries on multi-processor servers. Here's an example of how to use ParallelUnion:


```raw

select measures.amount

on 0,

bottomcount(

ASSP.ParallelUnion(

"bottomcount(

{[Scenario].[Scenario].&[1]}

*

[Account].[Account].[Account].members

*

[Date].[Date].[Date].members

*

[Department].[Departments](Departments).[Department Level 02].members

, 10,[Measures].[Amount]"

,

"bottomcount(

{[Scenario].[Scenario].&[2]}

*

[Account].[Account].[Account].members

*

[Date].[Date].[Date].members

*

[Department].[Departments].[Department Level 02].members

, 10,[Measures].[Amount]"

), 10, [Measures].[Amount])

on 1

from

[Adventure Works]

```

Here's an example of ParallelGenerate:

```raw

select measures.amount

on 0,

bottomcount(

ASSP.ParallelGenerate(

{[Scenario].[Scenario].&[1],[Scenario].[Scenario].&[2] },

"bottomcount(

{[Scenario].[Scenario].currentmember}

*

[Account].[Account].[Account].members

*

[Date].[Date].[Date].members

*

[Department].[Departments].[Department Level 02].members

, 10,[Measures].[Amount])"

), 10, [Measures].[Amount])

on 1

from

[Adventure Works]

```