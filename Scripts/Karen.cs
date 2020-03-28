using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Karen : MonoBehaviour
{
    bool executingBehavior = false;
    Task currentTask;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!executingBehavior)
            {
                executingBehavior = true;
                currentTask = BuildTask();

                EventBus.StartListening(currentTask.TaskFinished, OnTaskFinished);
                currentTask.run();
            }
        }
    }

    void OnTaskFinished()
    {
        EventBus.StopListening(currentTask.TaskFinished, OnTaskFinished);
        executingBehavior = false;
    }

    Task BuildTask()
    {
        Renderer renderer = GetComponent<Renderer>();
        List<Task> taskList = new List<Task>();

        Task wait = new Wait(1f);
        Task turnRed = new TurnRed(renderer);
        Task turnYellow = new TurnYellow(renderer);
        Task turnBlue = new TurnBlue(renderer);

        taskList.Add(turnRed);
        taskList.Add(wait);
        taskList.Add(turnYellow);
        taskList.Add(wait);
        taskList.Add(turnBlue);

        Sequence sequence = new Sequence(taskList);

        taskList = new List<Task>();

        taskList.Add(sequence);

        Selector selector = new Selector(taskList);

        return selector;
    }
}
