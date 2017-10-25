---
title: Downloads
layout: page
---

{% for release in  site.github.releases %} 
{% if release.draft != true and release.prerelease != true %}
- **{{ release.name }}**
    - {% for asset in release.assets %}[{{asset.name}}]({{ asset.browser_download_url }}) \| Size: {% include filesize.html number=asset.size %} \| Date: {% if asset.created_at  %}{{ asset.created_at | date_to_string }} {% else %} N/A {% endif %}{% endfor %}
    {% endif %}
{% endfor %}

> Prior versions can be found on the old codeplex site at [http://asstoredprocedures.codeplex.com](http://asstoredprocedures.codeplex.com)