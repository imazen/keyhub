Flags: hidden

# Roadmap

This is a preliminary, private roadmap.

## 3.X

#### March 1st, 2013

* Publish beta KeyHub website for private testing, with persistent DB, proper backups, security, and all SKUs loaded.
* Release new imageresizer website
* Release ImageResizer 3.3.3

#### March 14th, 2013

* Publish public KeyHub website, connect e-junkie, with all new purchasers using it.
* Release ImageResizer V3.4 with passive licensing, connected to KeyHub
* Add DRM-free build branch

#### March 21st, 2013

* Implement ffmpeg thumbnailing

#### April 1st, 2013

* Release ImageResizer 3.4.1, with licesing improvements
* Import all existing 2.X and 3.X licenses into KeyHub
* Email all existing users that they need to log in, get an application ID, and enter it into Web.config
* KeyHub must now provide private downloads for Elite/SupportContract/DRM-Free users

#### April 15th, 2013 

* Release ImageResizer 3.5 with mild licensing enforcement (e-mail notification)
* Begin V4 development

#### April 30th, 2013

* Release ImageResizer 3.5.3 with full licensing enforcement (watermarking)
* Announce V4 development, offer V4 SKU for purchase, but specify only V3 is supported 

Launching V4 without license keys, then adding them later, would cause unnaceptable problems for users - this needs to be solved in V3, and included in V4.

## 4.0 - Alpha release May 5th 2013

* Require .NET 3.5 instead of .NET 2, so extension methods and MVC can be supported in Core.
* Switch to a 'hints'-based plugin lookup model, so we can support .NET 4.X lazy initialization better.
* Migrate all API calls to use the `Instructions` class instead of `ResizeSettings`. This will eliminate the need for consumers to reference System.Drawing, and fix a number of long-standing limitations.
* Modify ImageJob so it can track source and final image dimensions; break compatibility with ImageBuilder subclasses
* Provide simplified virtual file API.
* Enhance InterceptModule extensibility, so security and rules can be easily applied to resized and non-resized images and files
* Add metadata API stubs

## 4.X

* Add complete metadata support
* Add ImageRules plugin
* Integrate FluentExtensions and add MVC URL and HTML helpers
* Integrate ResponsivePresets 

## 5.0 - 2014?

* Require .NET 4.5
* Use .NET 4 & WebActivator so the HttpModule doesn't have to be registered in Web.config anymore.
* Support async/await

## Backlog

* Support for ActionResults (although they can never be as fast as the HttpModule itself)
* Make extensible system for determining if the background color needs to be applied
* ImageResizer.Util Assembly Attribute classes will be moved to ImageResizer.Configuration
* No APIs will directly reference System.Drawing. A wrapper class will permit System.Drawing, WIC, WPF, FreeImage, or byte array images instead.
* ImageResizer.Encoding namespace will move to ImageResizer.Plugins.Encoding or ImageResizer.Configuration.Encoding
* All ImageResizer.Util classes will be reorganized or moved elsewhere
* ImageResizer.Configuration.Config will move to ImageResizer.ImageConfig
