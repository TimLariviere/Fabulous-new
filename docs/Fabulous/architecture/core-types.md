# Core types

To get the best performance possible, Fabulous needs to use specialized and optimized data structures to minimize the impact on computation speed and memory usage.

Those specialized data structures are known as the core types and we will explain them here.

## General concepts

### Optimizing for memory usage (aka. avoiding garbage collection)
Garbage collection is a huge issue on mobile devices, especially on low-end Android devices with low memory and slow CPU. In Fabulous v1, we were instantiating a lot of objects everytime a new message was received, it was almost always triggering a garbage collection.  

On some devices, this garbage collection would pause the app for a few hundred milliseconds which would result in a poor user experience.

To avoid this, we now favor structs over classes when possible (and when it makes sense).

### Optimizing for computation speed
The primary goal of Fabulous is to reconcile two UI states almost instantaneously.

This means we need to optimize the most common hot paths in the code for speed.

Structs are stored on the stack locally to the function we're currently executing, and not on the heap that is handled by the garbage collector. Once the function is done, the structs are automatically destroyed without having to pause the app.

This comes with its own set of challenges though. We have to be careful to not copy structs all the time from function to function, and also make sure the size of each struct stays below a maximum size (we can't store too many data in a struct).

This limitation on the size of a struct enables .NET to store those structs inside the L1 / L2 caches of the CPU, which yield faster reading speed than RAM.

## Types

### Widget and attributes
The way Fabulous represents the UI tree is with `Widget` and `Attributes`.

A `Widget` represents a UI element such as a label, a button, an text field, a list, etc.

Those `Widgets` will have `Attributes` to declare how they should render.  
Such attributes can be the text of a label, the click handler of a button, the items in a list or the color of the background.

We can categorize those attributes into 3 sets:
- Scalar values: string, bool, Color, etc.
- Widget values: of type `Widget`. Represent a parent -> child relationship
- WidgetCollection values: of type `Widget[]`. Represent a parent -> children relationship

TODO: Why we choose the Attribute model? (maybe in a page about AttributeDefinition)

The reason we went with multiple `WidgetAttributes` and `WidgetCollectionAttributes` instead of a single "parent -> child", "parent -> children" model is because frameworks like Xamarin.Forms can take multiple collections of UI elements under one single node. (e.g. `Xamarin.Forms.ViewCell` takes both a `View` (single child) and `GestureRecognizers` (multiple children)).

### Diff changes

### Program

### Runners and ViewAdapters

- Attributes
	- Scalar
	- Widget
	- WidgetCollection
- Widget
- WidgetDiff
	- ScalarChange
	- WidgetChange
	- WidgetCollectionItemChange
- ScalarAttributeComparison
- ViewTreeContext
- Program
- IViewNode
- IRunner
- IViewAdapter