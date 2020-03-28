using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task
{
    public abstract void run();
    public bool succeeded;

    protected int eventId;
    const string EVENT_NAME_PREFIX = "FinishedTask";

    public string TaskFinished
    {
        get
        {
            return EVENT_NAME_PREFIX + eventId;
        }
    }
    public Task()
    {
        eventId = EventBus.GetEventID();
    }
}

public class TurnRed : Task
{
    Renderer color;

    public TurnRed(Renderer renderer)
    {
        color = renderer;
    }

    public override void run()
    {
        color.material.color = new Color(255, 0, 0);
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class TurnYellow : Task
{
    Renderer color;

    public TurnYellow(Renderer renderer)
    {
        color = renderer;
    }

    public override void run()
    {
        color.material.color = new Color(255, 128, 0);
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class TurnBlue : Task
{
    Renderer color;

    public TurnBlue(Renderer renderer)
    {
        color = renderer;
    }

    public override void run()
    {
        color.material.color = new Color(0, 0, 255);
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class Wait : Task
{
    float waitTime;

    public Wait(float time)
    {
        waitTime = time;
    }

    public override void run()
    {
        succeeded = true;
        EventBus.ScheduleTrigger(TaskFinished, waitTime);
    }
}

public class Sequence : Task
{
    List<Task> children;
    Task currentTask;
    int currentTaskIndex = 0;

    public Sequence(List<Task> taskList)
    {
        children = taskList;
    }

    public override void run()
    {
        currentTask = children[currentTaskIndex];
        EventBus.StartListening(currentTask.TaskFinished, OnChildTaskFinished);
        currentTask.run();
    }

    void OnChildTaskFinished()
    {
        if (currentTask.succeeded)
        {
            EventBus.StopListening(currentTask.TaskFinished, OnChildTaskFinished);
            currentTaskIndex++;
            if (currentTaskIndex < children.Count)
            {
                this.run();
            }
            else
            {
                succeeded = true;
                EventBus.TriggerEvent(TaskFinished);
            }

        }
        else
        {
            succeeded = false;
            EventBus.TriggerEvent(TaskFinished);
        }
    }
}

public class Selector : Task
{
    List<Task> children;
    Task currentTask;
    int currentTaskIndex = 0;

    public Selector(List<Task> taskList)
    {
        children = taskList;
    }

    public override void run()
    {
        currentTask = children[currentTaskIndex];
        EventBus.StartListening(currentTask.TaskFinished, OnChildTaskFinished);
        currentTask.run();
    }

    void OnChildTaskFinished()
    {
        if (currentTask.succeeded)
        {
            succeeded = true;
            EventBus.TriggerEvent(TaskFinished);
        }
        else
        {
            EventBus.StopListening(currentTask.TaskFinished, OnChildTaskFinished);
            currentTaskIndex++;
            if (currentTaskIndex < children.Count)
            {
                this.run();
            }
            else
            {
                succeeded = false;
                EventBus.TriggerEvent(TaskFinished);
            }
        }
    }
}




