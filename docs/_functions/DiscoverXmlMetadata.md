---
title: DiscoverXmlMetaData
version: "1.1"
permissions: U
class: XmlaDiscover
functions:
  - DiscoverXmlMetaData
  - DiscoverXmlMetaDataFull
---

**DiscoverXmlMetaData(path _,whereClause, restrictions_)**
This function executes a ```"DISCOVER_XML_METADATA"``` command, which returns a hierarchical resultset and then "flattens" that resultset and returns it as a datatable which can be displayed by SSMS or used in a Reporting Services report.

Unfortunately standard Xpath style queries did not prove rich enough to return data in it's full context. That is you could return a list of all the dimensions in every cube on a given server, but you could not return a column containing the cube and database to which they belonged. So a hybrid Xpath syntax was developed which allows you to specify a list of fields to return at each level.

The easiest way to explain this is with a few examples

In order to return return a list of all the dimensions in every cube in the current database you would issue the following command.

```raw
CALL ASSP.discoverXmlMetaData("\Database\Cubes\Cube\Dimensions\Dimension");
```

But this returns the list without any context, If you want to report the CubeName and LastProcessed date you would put a vertical bar ```(|)``` after the Cube element in the path and then put a comma separated list of fields that you want returned

```raw
CALL ASSP.discoverXmlMetaData("\Database\Cubes\Cube|Name,LastProcessed\Dimensions\Dimension");
```

Note: it is not entirely true that the DiscoverXmlMetadata() command returns data without context - one of the features of this function is that it automatically returns the "ID" field of any ancestor elements in the resultset.

If you want to see what sort of paths are available the easiest way is to run the following command in an XMLA query window. You basically need to skip the xsd:schema> element and look at what is returned under the <row><xars:METADATA> elements.

```xml
<Discover xmlns="urn:schemas-microsoft-com:xml-analysis">
  <RequestType>DISCOVER_XML_METADATA</RequestType>
  <Restrictions />
  <Properties />
</Discover>
```

The path parameter actually matches from the leaf level up, so you could do a command like

```raw
CALL ASSP.discoverXmlMetaData("Dimension");
```

But this would produce a bit of a mess as it would return both cube dimensions and database dimensions and other dimension references from all over the xml resultset. The function is built in such a way that it build the columns for the resultset based off the first populated node in that matches the path so you will end up with a lot of blank columns.

But what this does mean is that you can specify just the minimal path that will unquiely identify the elements you are interested in.

eg

```raw
CALL ASSP.discoverXmlMetaData("Cube\Dimensions\Dimension");
```

returns the same result as

```raw
CALL ASSP.discoverXmlMetaData("\Database\Cubes\Cube\Dimensions\Dimension");
```

It should be noted that because this is base on Xml, the path is case sensitive and "dimension" is not the same as "Dimension".

If you want to filter down to just a subset of the results you can pass in the sort of restrictions that you would normally put in a SQL WHERE clause. For example the following query will return all of the role playing time dimensions in the Adventure Works database

```raw
 CALL ASSP.discoverXmlMetaData("Cube\Dimensions\Dimension", "DimensionID='Dim Time'");
```

Role information is not exposed by any of the other schema rowsets. The following commands will extract information about roles, dimension permissions, attribute permissions and cube permissions and would allow for the complete documentation of the security settings.

```raw
CALL ASSP.DiscoverXmlMetadata("\Database\Roles\Role");

CALL ASSP.DiscoverXmlMetadata("\Database\Dimensions\Dimension\DimensionPermissions\DimensionPermission");

CALL ASSP.DiscoverXmlMetadata("\Database\Dimensions\Dimension\DimensionPermissions\DimensionPermission\AttributePermissions\AttributePermission")

CALL ASSP.DiscoverXmlMetadata("\Database\Cubes\Cube\CubePermissions\CubePermission");

CALL ASSP.DiscoverXmlMetadata("\Database\Cubes\Cube\CubePermissions\CubePermission\DimensionPermissions\DimensionPermission");

CALL ASSP.DiscoverXmlMetadata("\Database\Cubes\Cube\CubePermissions\CubePermission\DimensionPermissions\DimensionPermission\AttributePermissions\AttributePermission");
```

An example which retrieves the list of measures is:

```raw
CALL ASSP.discoverXmlMetaData("\Database\Cubes\Cube\MeasureGroups\MeasureGroup|Name\Measures\Measure");
```

An example which retrieves the DMV column upon which each measure is based is:

```raw
CALL ASSP.discoverXmlMetaData("\Database\Cubes\Cube\MeasureGroups\MeasureGroup|Name\Measures\Measure|Name\Source\Source");
```

**DiscoverXmlMetaDataFull(path _, whereClause, restrictions_)**
The DiscoverXmlMetadata() function automatically injects a <DatabaseID> restriction into the xmla command using the database specified in the current connection as it was considered to be the most likely use case. If however you want to report on server properties, or do execute a command over every database on the server you can use the DiscoverXmlMetaDataFull() function which has the same syntax as the DiscoverXmlMetaData() function, but it does not insert any implied restrictions.

eg

```raw
CALL ASSP.DiscoverXmlMetadataFull("\Databases\Database", "State<>'Processed'" ,"<ObjectExpansion>ExpandObject</ObjectExpansion>")

CALL ASSP.DiscoverXmlMetadataFull("\Server\ServerProperties\ServerProperty", "" ,"<ObjectExpansion>ObjectProperties</ObjectExpansion>")
```