using System.Collections.Generic;

namespace PurpleMylkLibrary.BehaviorTrees
{
         /// <summary>
         /// The Taskstatus is needed to evaluate each task. It will act as a return type.
         /// Options: Success,Failure,Waiting.
         /// </summary>
        public enum Taskstatus
        {
            /// <summary>
            /// The condition is met/The task is executed.
            /// </summary>
            Success,
            /// <summary>
            /// The condition isn't met/The task isn't executed.
            /// </summary>
            Failure,
            /// <summary>
            /// The condition/task isn't evaluated yet.
            /// </summary>
            Waiting
        }

        /// <summary>
        /// Is the base class all tasks derrive from.
        /// </summary>
        public abstract class Task
        {
            public Task() { }

            protected Taskstatus currentTaskstatus;
            /// <summary>
            /// Displays the current state of evaluation. 
            /// Either Success,Failure,Waiting.
            /// </summary>
            public Taskstatus CurrentTaskstatus { get { return currentTaskstatus; } }

            /// <summary>
            /// Delegate that returns the status of the task.
            /// </summary>
            public delegate Taskstatus Taskreturn();

            /// <summary>
            /// method for evaluation the desired set of conditions/tasks.
            /// </summary>

            public abstract Taskstatus Evaluate();
        }

        /// <summary>
        /// The action is the leaf/end Task of our Behavior Tree, it is
		/// the element that actually executes a given behavior.
		/// this script is used as a generic which we will then pass a delegate.
		/// Important for making this approach work is the action delegate.
		/// You can implement whatever logic you want, however your method has to
        ///return a Taskstatus in order to work.
        /// </summary>
        public class Action : Task
        {
            //Method signature for the action
            public delegate Taskstatus ActionTaskDelegate();
            //Delegate that is called to evaluate this task
            private ActionTaskDelegate action;
            //Action Constructor
            public Action(ActionTaskDelegate _action)
            {
                action = _action;
            }

            //Switch statement for evaluating the current tasks status
            public override Taskstatus Evaluate()
            {
                switch (action())
                {
                    case Taskstatus.Success:
                        currentTaskstatus = Taskstatus.Success;
                        return CurrentTaskstatus;
                    case Taskstatus.Failure:
                        currentTaskstatus = Taskstatus.Failure;
                        return CurrentTaskstatus;
                    case Taskstatus.Waiting:
                        currentTaskstatus = Taskstatus.Waiting;
                        return CurrentTaskstatus;
                    default:
                        currentTaskstatus = Taskstatus.Failure;
                        return CurrentTaskstatus;
                }
            }
        }
        
        /// <summary>
        /// An Inverter is a Decorator Task and therefore can only have 
		/// one child.The Inverter works like the ! Operator.It turns
		/// the result into its opposite if its either Success or Failure.
        ///
        /// However, Waiting will be handled regularly.
        /// </summary>
        public class Inverter : Task
        {
            //Childtask and its getter
            protected Task childTask;
            public Task ChildTask { get { return childTask; } }

            //Inverter constructor
            public Inverter(Task _childTask)
            {
                childTask = _childTask;
            }

            //Inverts the result
            public override Taskstatus Evaluate()
            {
                switch (childTask.Evaluate())
                {
                    case Taskstatus.Failure:
                        currentTaskstatus = Taskstatus.Success;
                        return CurrentTaskstatus;
                    case Taskstatus.Success:
                        currentTaskstatus = Taskstatus.Failure;
                        return CurrentTaskstatus;
                    case Taskstatus.Waiting:
                        currentTaskstatus = Taskstatus.Waiting;
                        return CurrentTaskstatus;
                }
                currentTaskstatus = Taskstatus.Success;
                return currentTaskstatus;
            }
        }
		 
        /// <summary>
        /// A Limiter is a Decorator Task and therefore can only have
	    /// one child. The Limiter evaluates its child only for a set 
	    /// amount of calls. Once that amount is reached, it will never
	    /// evaluate again and always return Failure for its child
		/// from that point on
        /// </summary>
        public class Limiter : Task
        {
            //Limiter maxiumum value
            protected int maxCalls;
            //Call amount
            protected int callCount;

            //Childtask and its getter
            protected Task childTask;
            public Task ChildTask { get { return childTask; } }

            //Limiter Constructor
            public Limiter(Task _childTask, int _maxCalls)
            {
                childTask = _childTask;
                maxCalls = _maxCalls;
            }

            public override Taskstatus Evaluate()
            {
                if (callCount < maxCalls)
                {
                    switch (childTask.Evaluate())
                    {
                        case Taskstatus.Failure:
                            currentTaskstatus = Taskstatus.Failure;
                            ++callCount;
                            return CurrentTaskstatus;
                        case Taskstatus.Success:
                            currentTaskstatus = Taskstatus.Success;
                            ++callCount;
                            return CurrentTaskstatus;
                        case Taskstatus.Waiting:
                            currentTaskstatus = Taskstatus.Waiting;
                            ++callCount;
                            return CurrentTaskstatus;
                    }
                    currentTaskstatus = Taskstatus.Success;
                    return CurrentTaskstatus;
                }
                else
                {
                    currentTaskstatus = Taskstatus.Failure;
                    return CurrentTaskstatus;
                }
            }
        }

        /// <summary>
        ///  A repeater is a Decorator task and therefore can only have one child.
		/// The repeater evaluates its child unitl it either returns Failure or Success.
        /// </summary>
        public class Repeater : Task
        {
            //Child task and its getter
            protected Task childtask;
            public Task ChildTask { get { return childtask; } }

            public Repeater(Task _childTask)
            {
                childtask = _childTask;
            }

            public override Taskstatus Evaluate()
            {
                if (currentTaskstatus != Taskstatus.Success && currentTaskstatus != Taskstatus.Failure)
                {
                    switch (childtask.Evaluate())
                    {
                        case Taskstatus.Failure:
                            currentTaskstatus = Taskstatus.Failure;
                            return CurrentTaskstatus;
                        case Taskstatus.Success:
                            currentTaskstatus = Taskstatus.Success;
                            return CurrentTaskstatus;
                        case Taskstatus.Waiting:
                            currentTaskstatus = Taskstatus.Waiting;
                            return CurrentTaskstatus;
                        default:
                            currentTaskstatus = Taskstatus.Waiting;
                            return CurrentTaskstatus;
                    }
                }
                else
                {
                    return currentTaskstatus;
                }
            }
        }

        /// <summary>
        /// The Selector is a composite Task and therefore can possible have one or 
		/// more children, thats why we use a List to hold the Children.

        /// During the evaluation we run through all of its children and evaluate
        /// each one individually.
        /// </summary>
        public class Selector : Task
        {
            //Selectors childTasks and its Getter
            protected List<Task> childTasks = new List<Task>();
            public List<Task> ChildTasks { get { return childTasks; } }

            public Selector(List<Task> _childTasks)
            {
                childTasks = _childTasks;
            }

            //If any of the children returns success we will report it upwards
            //else we will report failure instead.
            public override Taskstatus Evaluate()
            {
                for (int i = 0; i < childTasks.Count; ++i)
                {
                    Task currentTask = childTasks[i];
                    switch (currentTask.Evaluate())
                    {
                        case Taskstatus.Failure:
                            continue;
                        case Taskstatus.Success:
                            currentTaskstatus = Taskstatus.Success;
                            return CurrentTaskstatus;
                        case Taskstatus.Waiting:
                            currentTaskstatus = Taskstatus.Waiting;
                            return currentTaskstatus;
                        default:
                            continue;
                    }
                }
                currentTaskstatus = Taskstatus.Failure;
                return CurrentTaskstatus;
            }
        }

        /// <summary>
        /// The Sequence is a composite Task and therefore can possibly have one or more
		/// childtasks, thats why we use a List to hold all the children.
        ///
        /// During the evaluation we picture all of the children as Success, only if
		/// one of them evaluate to failure the entire sequence fails.
        /// </summary>
        public class Sequence : Task
        {
            protected List<Task> childTasks;
            public List<Task> ChildTasks { get { return childTasks; } }

            public Sequence(List<Task> _childTasks)
            {
                childTasks = _childTasks;
            }

            public override Taskstatus Evaluate()
            {
                bool taskRunning = false;
                for (int i = 0; i < childTasks.Count; ++i)
                {
                    Task currentTask = childTasks[i];
                    switch (currentTask.Evaluate())
                    {
                        case Taskstatus.Failure:
                            currentTaskstatus = Taskstatus.Failure;
                            return CurrentTaskstatus;
                        case Taskstatus.Success:
                            continue;
                        case Taskstatus.Waiting:
                            taskRunning = true;
                            continue;
                        default:
                            currentTaskstatus = Taskstatus.Success;
                            return CurrentTaskstatus;
                    }
                }

                if (taskRunning)
                {
                    currentTaskstatus = Taskstatus.Waiting;
                    return CurrentTaskstatus;
                }
                else
                {
                    currentTaskstatus = Taskstatus.Success;
                    return CurrentTaskstatus;
                }
            }
        }
}

