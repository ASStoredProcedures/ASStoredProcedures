---
title: ASSP - Analysis Services Stored Procedure Project
---
The Analysis Services Stored Procedure project is a set of sample stored procedures for Analysis Services 2005, 2008, 2008 R2, 2012, and 2014. These samples have been written in C# and sample MDX queries are included that demonstrate the use of the procedures against the Adventure Works DW sample database. It was developed by a group of community volunteers with 2 main aims in mind:
- To provide a set of useful extensions to Analysis Services 2005 and higher.
- To provide a variety of example source code for people looking to write their own stored procedures.

The project currently contains the following samples:

{% assign sorted_functions = site.functions | where_exp: 'item', 'item.class != null' | sort: 'class' %}

| Class | [Permissions](#Permissions) | [Version](#Version) | Functions |
|-------|------------------------------|----------------------|-----------|
{% for func in sorted_functions %}|[{{func.class}}]({{func.url}})|{{func.permissions}} | {{func.version}}|{% if func.functions %}{{func.functions | join: ', '}} {% else %}{{func.title}}{% endif %}|
{% endfor %}

<a name="Permissions"></a> 
### Permissions:
Denotes the minimum required permissions level for the function in the class to work. If you deploy the assembly with a lower permission level than that required by a given function, that function will simply throw a security exception when you attempt to execute it, all the other functions will execute normally. 

- S - Safe
- U - Unrestricted

<a name="Version"></a>
### Version:
Denotes the initial release version for the class.

### Installation
For details on how to install the compiled release see the [Installation Instructions](Installation-Instructions)

### Additional Resources
For a list of other resources and links see:

* [Additional Resources](Additional-Resources)

<div class="result">result goes here</div>

