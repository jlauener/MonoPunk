# MonoPunk

MonoPunk is a cross-platform 2D game engine built on top of MonoGame. It aims to be an easy to use yet powerful engine for gamejams, prototyping and shippdable games. It has been heavily tested on Windows and MacOS but all platforms supported by MonoGame (Linux, Android, iOS, ...) should work as well.

The engine is heavily inspired by [HaxePunk](https://haxepunk.com/), an engine heavily inspired by [FlashPunk](http://useflashpunk.net/).

Features:
* __Entity__: Scene/Entity/Component system
* __Debug__: Debug view that shows hitboxes and can track variables (press Oem Tilde to show)
* __Input__: Virtual input to keyboard/gamepad.
* __Animator__: Tile based animations with different mode (e.g. Loop, OneShot, ...)
* __Tilemap__: Efficient tilemap rendering
* __Tweening__: Tweening based on [Glide](https://bitbucket.org/jacobalbano/glide)
* __Collision__: Pixel perfect collision based on hitbox, grid (for tilemap) or pixel mask
* __TilEd__: TilEd map import based on [MonoGameExtended](https://github.com/craftworkgames/MonoGame.Extended)
* __BitmapFont__: Bitmap font import based on [MonoGameExtended](https://github.com/craftworkgames/MonoGame.Extended)
* __Utilities__: Vector2 extensions, float based math, logging, ...

The engine is provided as is, without documentation/support. The best place to start would be the MonoPunk.TestBed project which shows some features of the engine and can be used as a starting project.
