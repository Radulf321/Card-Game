using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public static class AsyncHelper
{
    public static Task<T> HandleToTask<T>(AsyncOperationHandle<T> handle)
    {
        TaskCompletionSource<T> result = new TaskCompletionSource<T>();
        handle.Completed += op =>
            {
                result.SetResult(op.Result);
            };

        return result.Task;
    }
}