using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    public static class AsyncOperationExtensions {

        /// <summary>
        /// Converts an existing AsyncOperation into a ITask.
        /// Can be used to simplify working with async Unity3D APIs.
        /// </summary>
        /// <param name="operation">the async operation</param>
        /// <returns>A task that will be resolved when the operation is complete.</returns>
        public static ITask<T> ToTask<T>(this T operation) where T : AsyncOperation {
            var  task = new Task<T>();
            operation.completed += (_) => task.Resolve(operation);
            return task;
        }

    }

}
