Legend for terms
Entity : A self contained object in the game
system : self contained code that performs 1 action
action : a single behavior for code to execute, physically or data
descriptor : a word used the define the name of a class, method, or variable
easing : a function that smooths movement from one action to another
Screen : the frame that is rendered to the monitor
(Game) Scene: the currently active unity scene
Event : when an action in the game scene occurs
Job : an action that occurs asynchronously from the main thread

Overhead items
 - Coordinator - manages performance of multiple sequences for smooth transitions
 - Sequence - performs a single sequence of actions with timing, easing, and index control
 - Event System - defines when code executes based on fulfilled conditions
 - Handler - takes input from another system and determines actions, calling on other systems to process and respond
 - Commander - performs actions using an above view of an entire entity
 - Domain - a large-scale system managing terrain objects and their code
 - Manager - a script controlling many controllers
 - Driver - a bridge between two separate systems
 - Controller - a general system containing operations for an entity
 - System - AVOID (use specific descriptors instead)

subsystem names
- Player - usually defined as the player entity in its entirety.
- User - code that directly involves the user.
- Input - code that handles conversion of input into useable data for other systems to refer to.
- Component - an individual part of system to be combined with multiple other components to create a unified system.
- Definition - a structure for variables or methods, such as a struct, class, enum, datatype, or scriptable object.
- Pool - a system that controls when objects are to be used within a system
- Correction/Error - Code that defines behavior for when code malfunctions, either as a full error or unanticipated behavior.

Interaction between objects
- Detection - code that defines how an entity will interact with another entity, but does not have actions embedded into it. Usually is used to determine actions before a collision occurs.
- Collision - code that is ran when collisions occur between entities. usually calls into a handler to determine what actions can be taken.

Data Manipulation
 - Formatter - code that takes raw data directly from other systems and converts it into standardized formats for use in another system.
	 - Alteration - a converter that shifts a value to a different value using an algorithm.
	 - replacement - a converter that replaces a value with another one using either a custom preference or determined value.
 - Converter - Similar to a formatter, it is code that takes one datatype and converts it into another datatype or type organization.
 - Record - code that takes formatted code and documents it in a useable manner.

Rendering
- Render - Code that affects how frames are rendered to the screen. can include items rendered to other objects within the camera view that are on screen.
- Display - code that determines what information is shown onto the screen and how it is formatted. usually works with a formatter or renderer.
- terrain - code that modifies a terrain of a game scene, usually works with a renderer and is managed by a domain
- Mesh - code that affects how/what the mesh of an object is rendered(into), or if it is not rendered at all
- Shader - code or systems that modify the display of vertexes, planes, or shader code
- Color - 