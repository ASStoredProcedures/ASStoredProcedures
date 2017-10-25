---
title: ListFunctions
version: "1.1"
permissions: U
class: ListFunctions
functions:
    - AssemblyVersion
    - ListFunctions
---
This stored procedure will list all of the available functions in .Net based assemblies that are stored either on the server or in a specified database. If the function is called with no parameters it will return the functions available in all the assemblies located at the server level.

eg.  

```raw
CALL ListFunctions()
```

If this stored procedure is called with the name of a specific database, it will return the functions available for all the .Net assemblies located in that specific database.

eg.  

```raw
CALL ListFunctions("Adventure Works DW")
```

This sample has a number of interesting features which set it apart from the other samples currently available.

* It is currently the only sample for a stored procedure that uses the CALL syntax.
* It demonstrates returning a DataTable from a stored procedure.
* It shows how to "re-assemble" an assembly using AMO

This function does not work with any of the system assemblies as none of these assemblies contain "main" files which can be re-assembled and reflected against. The functions in these files should all be well documented in Books online anyway.

Also, this class contains an AssemblyVersion sproc useful for detecting easily what version of ASSP is deployed. This AssemblyVersion sproc is new with release 1.3.5:

```raw
CALL ASSP.AssemblyVersion()
```

> **Note:** This sample requires being deployed with unrestricted security priviledges in order for it to work.

