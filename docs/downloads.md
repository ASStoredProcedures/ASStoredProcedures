---
title: Downloads
layout: page
---

> The full list of releases can be found at the  [github releases page](https://github.com/ASStoredProcedures/ASStoredProcedures/releases).
> 
> Please follow the [installation instructions](Installation-Instructions)

{% for release in  site.github.releases %} 
{% if release.draft != true and release.prerelease != true %}
- **{{ release.name }}**
    {% for asset in release.assets %} - [{{asset.name}}]({{ asset.browser_download_url }}) \| Size: {% include filesize.html number=asset.size %} \| Date: {% if asset.created_at  %}{{ asset.created_at | date_to_string }} {% else %} N/A {% endif %}

    {% endfor %}
    {% endif %}
{% endfor %}

