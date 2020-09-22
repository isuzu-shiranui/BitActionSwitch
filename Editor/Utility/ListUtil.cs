using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BitActionSwitch.Editor.Utility
{
    public static class ListUtil
    {
        public static T Peek<T>(this IList<T> self)
        {
            return self[0];
        }

        public static T Pop<T>(this IList<T> self)
        {
            var result = self[0];
            self.RemoveAt(0);
            return result;
        }

        public static void Push<T>(this IList<T> self, T item)
        {
            self.Insert(0, item);
        }

        public static T PullAt<T>(this IList<T> self, int index)
        {
            var result = self[index];
            self.RemoveAt(index);
            return result;
        }
        
        public static void PushAt<T>(this IList<T> self, T item, int index)
        {
            if(index > self.Count) self.Add(item);
            else self.Insert(index, item);
        }

        public static void ShiftElement<T>(this IList<T> self, int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex || 0 > oldIndex || oldIndex >= self.Count || 0 > newIndex ||
                newIndex >= self.Count) return;
            int i;
            var tmp = self[oldIndex];
            if (oldIndex < newIndex)
            {
                for (i = oldIndex; i < newIndex; i++)
                {
                    self[i] = self[i + 1];
                }
            }
            else
            {
                for (i = oldIndex; i > newIndex; i--)
                {
                    self[i] = self[i - 1];
                }
            }
            self[newIndex] = tmp;
        }
    }
}