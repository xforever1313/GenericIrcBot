﻿Url Bot
==============

About
======
When someone posts a URL in the chat, this will get the HTML from the URL and then read the title tag inside.  It then prints the title tag to the chat.

Files more than 1MB are ignored.

Anything without a title tag is ignored.

Titles that are longer than 400 characters are stripped to 400 characters.

Configuration
=====

There is no configuration.

Installing
======

Url Bot comes with Chaskis by default. To enable, open PluginConfig.xml, and add the following line inside of &lt;pluginconfig&gt;

### Windows: ###

```XML
<assembly path="C:\Program Files\Chaskis\Plugins\UrlBot\UrlBot.dll" />
```

### Linux: ###

```XML
<assembly path="/usr/lib/Chaskis/Plugins/UrlBot/UrlBot.dll" />
```

Sample Output:
======

```
[12:13.21] <xforever1313> https://www.google.com/
[12:13.21] <SethTestBot> Title: Google
[12:13.46] <xforever1313> https://github.com/xforever1313/Chaskis
[12:13.47] <SethTestBot> Title: GitHub - xforever1313/Chaskis: A generic framework for making IRC Bots.
```

Credits
--------

 * ### HtmlAgilityPack ###
    * **License:** https://github.com/zzzprojects/html-agility-pack/blob/master/LICENSE
    * **Website:** http://html-agility-pack.net/
