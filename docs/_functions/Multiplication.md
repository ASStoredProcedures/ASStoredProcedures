---
title: Multiply
version: "1.0"
permissions: S
class: Multiplication
functions:
    - Multiply
---

Multiply() multiplies the value returned by an MDX expression for each member of a set. This is useful for calculations such as compound growths, and (limited) testing shows that using this function is much faster than implementing the same calculation in pure MDX using recursion or the approach using log functions descibed here [http://geekswithblogs.net/darrengosbell/archive/2006/07/18/85539.aspx](http://geekswithblogs.net/darrengosbell/archive/2006/07/18/85539.aspx)