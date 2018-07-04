# Behavior-Tree-Library-for-Unity
I worked out a little library for you to easily implement Behaviour Trees into your Unity Projects. You’ll find a Link to the GitHub Repository at the very end of this page. The Library is fully commented but I decided to post the documentation and explanation of Behaviour Trees here as it might be helpful for some people (You can thank me later!).
Documentation:
What is a Behavior Tree?
Behavior Trees are branching, hierarchical systems of different tasks that share a common parent, which is called the root. Picture a tree with the roots, the stem and all its branches that eventually have leaves attached to them (so poetic)!
Now, tasks can represent tests or actual behaviours. If you have been working with state machines before you will probably know that they follow transition rules to change between states – f.e. if the target is close enough -> attack. A state machine has to test all of the possible conditions for all states it could possibly go to from whatever state it is currently in, which is a pretty hefty workload if your project is of at least average size, not to mention that it gets crumby really easily.

A Behavior Tree works differently in a way where its flow is defined by every single task’s order within the entire tree hierarchy. They are executed starting from their root task, continuing through each child, which, in turns, runs through each of its children until a condition is met or the action task/leaf task is reached. Sounds much more pleasing, doesn’t it?

There are different types of tasks, which will be evaluated in a different way and also differ in many other aspects, however, all of them have one thing in common. They all have to return a TaskStatus which can be Success, Failure or Waiting.
I think Success and Failure are pretty much self-explanatory, so let’s focus on Waiting. Behavior Trees can get really complex depending on the size and ramification of a project, therefore a lot of their implementations are asynchronous and this is why we need a waiting state. Imagine a tree with 100 Tasks of different intensities, some of them may even need a couple of frames for evaluation, if they wouldn’t run asynchronous, this would result in massive freezes if we would have to wait for each task to be completely executed and return either Success or Failure.Makes sense, right?
Don’t worry if you still feel stranded at this point. I will be explaining the different types of tasks up next – this will definitely help you grasp the concept.

Task Types:
Composite Tasks:
Composite tasks can have more than one child and their state is based purely on the evaluation of its children – as long as the evaluation of its children is still running, it will be in WAITING state. These tasks can be split up into different types as well, Sequences and Selectors, those separations are made upon the way their children get evaluated.

Sequences run from left to right. A Sequence only returns SUCCESS if all of its children have been evaluated as SUCCESS.

Selectors return SUCCESS if at least one of its children returns SUCCESS. The only way a selector returns FAILURE is if all of its children returned FAILURE.

Decorator Tasks:
A Decorator task has only one child. It may seem strange to rely on a child if there is only one of them but the decorator is special in a way that it takes the state returned by its child and evaluates it based on its own parameters, it can specify for example how and how often its child gets. Just as composite Tasks, decorators can be split up into different kinds based on their functionality.

Here are the most common ones:

The repeater will repeat the evaluation of a child until it returns either SUCCESS or FAILURE.
The inverter will inverse the return of its child.
The limiter will limit the number of times a task will be evaluated to avoid a weird looking behaviour loop. It is different from the repeater in a way that it will only try to do something for a certain amount of times before moving on to trying something else.

Like is said, the possibilities are endless and you surely can combine different functionalities as well, although I would advise you to not go overboard with this as things will get unorganized if you do so.

Action Tasks:
Action Tasks or leaf Tasks are the outermost tasks of your Behaviour Tree. They will carry your executed agent behaviours like walking, running, shooting, fighting and so on.

Implementing the Library:
Implementing a Library into Unity is fairly easy. All you have to do are these easy steps:

1.) Download my GitHub Repository.
2.) Open the desired Unity project and create a folder Plugins
3.) Import PurpleMylkLibrary.DLL and PurpleMylkLibrary.XML into the Plugins folder.
