---
title: XmlaDiscover
version: "1.1"
permissions: U
class: XmlaDiscover
---
This class has a number of functions that have all evolved around sending XML/A Discover and Execute commands to the server.

* [Discover](../Discover) - allows for the display of metadata using Discover XML/A commands
* [DiscoverXmlMetadata](../DiscoverXmlMetadata) - extracts a tabular dataset from the {{DISCOVER_XML_METADATA}} request
* [ClearCache](../ClearCache) - clears the cache, useful for performance testing
* [Cancel](../Cancel) - cancels sessions, connections and SPIDs
* [DMV](DMV) - allows for the execution of DMV select statements similar to what can be executed against SSAS 2008
* [DiscoverSingleValue](../DiscoverSingleValue) - returns the value from one column of Discover, DiscoverXmlMetadata, or DMV functions
* [ForEach](../ForEach) - the ForEachMeasureGroup and ForEachPartition functions loop across each object, run a command, then union the resultsets together

* [Discover Reports](../Discover-Reports) - SSRS Reports using the above functions