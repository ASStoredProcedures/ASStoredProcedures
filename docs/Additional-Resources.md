---
title: Additional Resources
---
#### Additional Resources

For instructions on creating and registering Analysis Services stored procedures, see the Books Online topic [Creating Stored Procedures](http://msdn2.microsoft.com/en-us/library/ms175340.aspx).

For information on using stored procedures in conjunction with association rules data mining to perform social network analysis, see the [whitepaper](http://blogs.msdn.com/johnchancock/archive/2007/02/25/social-relationship-analysis-with-data-mining.aspx) and download the [code](http://www.johnchancock.net/downloads/Social Relationships and Data Mining.zip) by John C. Hancock.

For several other data mining related stored procs, see Jamie MacLennan's [blog](http://blogs.msdn.com/jamiemac/default.aspx) and the [sqlserverdatamining.com](http://www.sqlserverdatamining.com) site:
* [Here](http://blogs.msdn.com/jamiemac/archive/2007/03/02/tree-utilities-in-analysis-services-stored-procedures.aspx) is one that's useful for finding the shortest or longest path to a leaf node in a decision tree.
* [Here](http://blogs.msdn.com/jamiemac/archive/2006/03/30/565142.aspx) is one that builds the DMX CREATE MINING MODEL statement for an existing model.
* [Here](http://www.sqlserverdatamining.com/ssdm/Default.aspx?tabid=61&Id=8) is one that calculates the goodness of fit of a regression model.
* [Here](http://www.sqlserverdatamining.com/ssdm/Default.aspx?tabid=61&Id=12) is one that calculates covariance/correlation matrixes.
* [Here](http://blogs.msdn.com/jamiemac/archive/2008/02/03/dmx-queries-the-datasource-hole.aspx) is one that creates a data source object.
* [Here](http://www.sqlserverdatamining.com/ssdm/Default.aspx?tabid=102&Id=41) is one that extracts information from a logistic regression model.

For an example of how to use a config file to store settings for Analysis Services stored procs, see a [blog entry](http://prologika.com/CS/blogs/blog/archive/2007/01/18/dealing-with-udm-configuration-settings.aspx) by Teo Lachev.

For some performance tips and other sproc best practices, see a [blog entry](http://sqljunkies.com/WebLog/mosha/archive/2007/04/19/stored_procs_best_practices.aspx) by Mosha Pasumansky.

For information about debugging sprocs, see a [blog entry](http://geekswithblogs.net/darrengosbell/archive/2007/05/22/Debugging-SSAS-.Net-Stored-Procedures.aspx) by Darren Gosbell.

For some examples of returning images from a sproc, see Hilmar Buchta's blog [here](http://ms-olap.blogspot.com/2008/07/returning-image-from-ssas-stored.html), [here](http://ms-olap.blogspot.com/2008/07/winloss-chart-as-dynamic-image-created.html), and [here](http://ms-olap.blogspot.com/2008/07/some-more-samples-for-data-aware-images.html). He also has a post about [quantiles](http://ms-olap.blogspot.com/2008/07/some-more-thoughts-on-quantiles.html) and sproc performance.

For an example of using C# to deploy an assembly to Analysis Services using the management API, see this [forum thread](http://social.msdn.microsoft.com/Forums/en-US/sqlanalysisservices/thread/cf24c471-2efd-40f6-92df-06b18ffcaa2e). That thread also shows how to deploy a third-party DLL in addition to your assembly.

For an example of a stored procedure which allows you to retrieve members with identical names, see this [blog post](http://ms-olap.blogspot.com/2009/05/accessing-duplicate-members-in.html) by Hilmar Buchta.

If your sproc opens a connection to Analysis Services, be sure to review the [Don't Deadlock Yourself](../Don't-Deadlock-Yourself) article.

For an example of a stored procedure which merges two datasets into one dataset, see this blog post by François Jehl in [French](http://fjehl.blogspot.com/2009/11/ssas-loperateur-union-manquant-en-dmx.html) and [translated into English](http://translate.google.com/translate?js=y&prev=_t&hl=en&ie=UTF-8&u=http%3A%2F%2Ffjehl.blogspot.com%2F2009%2F11%2Fssas-loperateur-union-manquant-en-dmx.html&sl=fr&tl=en)

For a list of best practices on when (and when not) to use stored procedures, see [this](http://bi-logger.blogspot.com/2010/10/analysis-services-stored-procedure-best.html) blog post by Philip Stephenson based upon Darren Gosbell's list.

For an example sproc which allows a user to initiate cube processing from an Excel action, see this blog post by [Gerhard Brueckl](http://gbrueckl.wordpress.com/2013/10/03/trigger-cube-processing-from-excel-using-cube-actions/).

For an example sproc to detect which roles the current user is in, see this [blog post](http://blogs.msdn.com/b/psssql/archive/2014/07/30/the-additive-design-of-ssas-role-security.aspx) by Jon Burchel.

An example use of ASSP to [improve performance of drillthrough](http://blogs.msdn.com/b/sql_server_team/archive/2015/02/10/pdw-and-ssas-rolap-challenges.aspx) in ROLAP mode against PDW.


_**Inclusion in ASSP:** If you would like to see any of the above included in ASSP, please add a new feature request to the Issue Tracker tab or vote for an existing feature request._


**Commercial SSAS Sprocs**

IT-Workplace has a product called [Intelligencia Extensions for SSAS](http://www.it-workplace.com/_layouts/CustomerSitePages/IX.aspx) which contains a sproc that draws a sparkline image.