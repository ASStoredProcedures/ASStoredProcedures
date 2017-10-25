---
title: StrToSet
version: "1.2"
permissions: S
class: StrToSet
functions:
    - KeysStrToSet
    - CompositeKeysStrToSet
---
The functions in this class are designed to allow the StrToSet function to receive shorter strings by passing in just a list of keys, not the entire member unique name.

**KeysStrToSet(_string Hierarchy_, _string KeysCommaSeparated_)**

The following two functions return the same set:

```raw
StrToSet("{ [Product].[Product].&[1],[Product].[Product].&[2] }")
ASSP.KeysStrToSet("[Product].[Product]", "1,2")
```

**CompositeKeysStrToSet(_string Hierarchy_, _string KeysCommaSeparated_)**

The following two functions return the same set:

```raw
StrToSet("{ [Geography].[City].&[Austin]&[TX],[Geography].[City].&[Baltimore]&[MD] }")
ASSP.CompositeKeysStrToSet("[Geography].[City]", "&[Austin]&[TX],&[Baltimore]&[MD]")
```


Being able to pass in more concise representations of sets can be useful when writing Reporting Services reports. By default, the parameter value is the full unique name (i.e. ```[Geography].[City].&[Austin]&[TX]```). For large multi-select parameters, that set can be quite large.

**StrToSet in Reporting Services**
The standard way of parameterizing an MDX query in Reporting Services would look like:

```raw
StrToSet(@Product)
```

The query to populate that Product parameter would look like:

```raw
with member [Measures].[ParameterValue] as
 [Product].[Product].CurrentMember.UniqueName
```

**KeysStrToSet in Reporting Services**
Parameterizing an MDX query in Reporting Services using KeysStrToSet would look like:

```raw
ASSP.KeysStrToSet("[Product].[Product]", @Product)
```

The query to populate that Product parameter would look like:

```raw
with member [Measures].[ParameterValue] as
 [Product].[Product].CurrentMember.Member_Key
```

KeysStrToSet will not work if the members have composite keys, or if the members have keys which are strings that may have a comma in them, or if the member is the all member or the unknown member, or if member keys are not unique across all levels of a user-defined hierarchy, or if the members have a nullable key (because the unique name for a member with a null key looks like ```[Dimension].[Attribute].&```).


**CompositeKeysStrToSet in Reporting Services**
Parameterizing an MDX query in Reporting Services using CompositeKeysStrToSet would look like:

```raw
ASSP.CompositeKeysStrToSet("[Geography].[City]", @City)
```

The query to populate that City parameter would look like:

```raw
with member [Measures].[ParameterValue] as
 Mid([Geography].[City].CurrentMember.UniqueName, Len([Geography].[City].UniqueName)+2)
```

CompositeKeysStrToSet will work with any member keys and should be used under the scenarios outlined above when KeysStrToSet will not work.

For an example of a working report using KeysStrToSet and CompositeKeysStrToSet, see "KeysStrToSet Example.rdl" in the report project that is distributed with the source code.

For a more complete discussion of the scenarios in which this sproc can be helpful, please read this [blog post](http://www.artisconsulting.com/blogs/greggalloway/Lists/Posts/Post.aspx?ID=12).