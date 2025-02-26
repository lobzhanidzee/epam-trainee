namespace Tpl;

public static class StudentLogic
{
    public static Task TaskCreated()
    {
        Task newTask = new Task(() => { });
        return newTask;
    }

    public static Task WaitingForActivation()
    {
        Task task = Foo(10);
        return task;
    }

    public static Task WaitingToRun()
    {
        Task newTask = new Task(() => { });
        newTask.Start();
        return newTask;
    }

    public static Task Running()
    {
        Task newTask = new Task(() => { Thread.Sleep(3000); });
        newTask.Start();
        while (newTask.Status != TaskStatus.Running)
        {
            Thread.Sleep(10);
        }

        return newTask;
    }

    public static Task RanToCompletion()
    {
        Task newTask = new Task(() => { });
        newTask.Start();
        newTask.Wait();

        return newTask;
    }

    public static Task WaitingForChildrenToComplete()
    {
        Task parent = Task.Factory.StartNew(
            () =>
        {
            for (int i = 0; i < 10; i++)
            {
                int taskNo = i;
                _ = Task.Factory.StartNew(
                    (x) =>
                {
                    Thread.SpinWait(5000000);
                }, taskNo,
                    CancellationToken.None,
                    TaskCreationOptions.AttachedToParent,
                    TaskScheduler.Current);
            }
        }, CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Current);

        while (parent.Status != TaskStatus.WaitingForChildrenToComplete)
        {
            Thread.Sleep(10);
        }

        return parent;
    }

    public static Task IsCompleted()
    {
        Task task = Task.Factory.StartNew(
           () => { },
           CancellationToken.None,
           TaskCreationOptions.None,
           TaskScheduler.Current);

        task.Wait();
        return task;
    }

    public static Task IsCancelled()
    {
        var tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;

        Task task = Task.Run(
            () =>
        {
            ct.ThrowIfCancellationRequested();
        }, tokenSource2.Token);

        try
        {
            tokenSource2.Cancel();
            task.Wait();
        }
        catch (AggregateException ae)
        {
            ae.Handle(e => e is TaskCanceledException);
        }
        finally
        {
            tokenSource2.Dispose();
        }

        return task;
    }

    public static Task IsFaulted()
    {
        Task task = Task.Run(() =>
        {
            throw new InvalidOperationException("This task is faulted.");
        });

        try
        {
            task.Wait();
        }
        catch (AggregateException ae)
        {
            ae.Handle(e => e is InvalidOperationException);
        }

        return task;
    }

    public static List<int> ForceParallelismPlinq()
    {
        var testList = Enumerable.Range(1, 300).ToList();
        List<int> newList = testList.AsParallel().Select(x => x * 2).ToList();
        return newList;
    }

    private static async Task<string> Foo(int seconds)
    {
        return await Task.Run(() =>
        {
            for (int i = 0; i < seconds; i++)
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }

            return "Foo Completed";
        });
    }
}
