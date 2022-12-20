# [![Made in Ukraine](https://img.shields.io/badge/made_in-ukraine-ffd700.svg?labelColor=0057b7&style=for-the-badge)](https://stand-with-ukraine.pp.ua) [Stand with the people of Ukraine: How to Help](https://stand-with-ukraine.pp.ua)

<img src="https://yevhencherkes.gallerycdn.vsassets.io/extensions/yevhencherkes/svgforuwpextension/1.5.5/1562857891500/Microsoft.VisualStudio.Services.Icons.Default" width="100" height="100" />

# SVG for UWP converter

[![Marketplace](https://img.shields.io/visual-studio-marketplace/v/YevhenCherkes.svgforuwpextension.svg?label=VS%20marketplace&style=for-the-badge)](https://marketplace.visualstudio.com/items?itemName=YevhenCherkes.svgforuwpextension)
[![Downloads](https://img.shields.io/visual-studio-marketplace/d/YevhenCherkes.svgforuwpextension?label=VS%20downloads&style=for-the-badge)](https://marketplace.visualstudio.com/items?itemName=YevhenCherkes.svgforuwpextension)

# **Inlines css styles to the SVG elements to make them compatible with the UWP SvgImageSource**

The Image control supports SVG files beginning from Windows 10 Creators Update (version 1703, build 15063).

It's just a partial support of SVG Ver 1.1 specification (see  [SVG Support](https://msdn.microsoft.com/en-us/library/windows/desktop/mt790715%28v=vs.85%29.aspx))

And it doesn't support css-like styles. If you used Adobe Illustrator for export image, you can play with export settings as described here:
 [SVG images shown blacked out in UWP application
](https://stackoverflow.com/questions/47557428/svg-images-shown-blacked-out-in-uwp-application)

If you don't have an ability to use Adobe Illustrator - you may use this tool to fix the showing issues.

**After installation you can use this extension via:**
- View -> Other Windows -> "**SVG for UWP Converter**".
- From the context menu of the Solution Explorer on the SVG file -> "**Inline SVG Styles**".


## Sample

**Let's use test SVG file for image source:**

`<Image Source="Assets/test.svg"/>`

**Content of the file:**

![image](https://user-images.githubusercontent.com/13467759/205915784-4fa64675-9680-4204-8d12-26c1c85bb5ca.png)

**Result should be:**

![image](https://user-images.githubusercontent.com/13467759/205915869-60f8019b-ca6e-48b3-8010-dd60ac2f0bbf.png)

**But styles aren't applied correctly:**

![image](https://user-images.githubusercontent.com/13467759/205915908-eb57fa19-a073-427c-ba5f-f46bf5f87e25.png)

**Let's update test.svg from the context menu:**

![image](https://user-images.githubusercontent.com/13467759/205915999-43aa46d1-b2f3-4b87-9056-9330fedf0a7c.png)

Then rebuild the project and everything looks fine!

This util also supports group operations via tool window:

![image](https://user-images.githubusercontent.com/13467759/208649275-f5afd1cc-1225-4a70-8c17-fc426d115af7.png)

**Privacy Notice:** No personal data is collected at all.

This tool has been working well for my own personal needs, but outside that its future depends on your feedback. Feel free to [open an issue](https://github.com/ycherkes/SVG-for-UWP/issues).

[![PayPal](https://img.shields.io/badge/Donate-PayPal-ffd700.svg?labelColor=0057b7&style=for-the-badge)](https://www.paypal.com/donate/?business=KXGF7CMW8Y8WJ&no_recurring=0&item_name=Help+SVG+Adapter+become+better%21)


**External links:**

- [Imported SVG shows black fill instead of colour](http://help.videoscribe.co/support/discussions/topics/1000060941/page/2)

- [PreMailer.Net](https://github.com/milkshakesoftware/PreMailer.Net)

- [UWP SvgImageSource Class](https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.imaging.svgimagesource)

- [SVG Support](https://msdn.microsoft.com/en-us/library/windows/desktop/mt790715%28v=vs.85%29.aspx)

[Previous releases](https://github.com/ycherkes/SVG-for-UWP/releases/tag/1.5.5-Visual-Studio-2017-2019)
