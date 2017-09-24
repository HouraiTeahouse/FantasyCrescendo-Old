using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    public static class AsyncOperationExtensions {

        public static ITask<T> ToTask<T>(this T operation) where T : AsyncOperation {
            var  task = new Task<T>();
            operation.completed += (_) => task.Resolve(operation);
            return task;
        }

    }

}
