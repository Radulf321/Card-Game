using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public interface IViewUpdater
{
    abstract public void updateView();
}

public static class ViewUpdaterExtensions
{
    public static void updateChildrenViews<SELF, CHILD, DATA>(
        this SELF viewUpdater,
        List<DATA> rawData,
        Func<DATA, GameObject> instantiateNewObject,
        Func<CHILD, DATA?> getRawData,
        Func<int, CHILD>? getChildComponent = null
    )
    where SELF : MonoBehaviour
    where CHILD : MonoBehaviour, IViewUpdater
    where DATA: class
    {
        int dataIndex = 0;
        int childrenIndex = 0;
        if (getChildComponent == null) {
            getChildComponent = (childrenIndex) => viewUpdater.transform.GetChild(childrenIndex).GetComponent<CHILD>();
        }
        while ((dataIndex < rawData.Count) || (childrenIndex < viewUpdater.transform.childCount)) {
            if (childrenIndex >= viewUpdater.transform.childCount) {
                // Instantiate the prefab
                GameObject childrenInstance = instantiateNewObject(rawData[dataIndex]);
                childrenInstance.transform.SetParent(viewUpdater.transform, false);
                dataIndex++;
                childrenIndex++;
            }
            else if ((dataIndex >= rawData.Count) || (rawData[dataIndex] != getRawData(getChildComponent(childrenIndex)))) {
                UnityEngine.Object.Destroy(viewUpdater.transform.GetChild(childrenIndex).gameObject);
                childrenIndex++;
            } else {
                getChildComponent(childrenIndex).updateView();
                dataIndex++;
                childrenIndex++;
            }
        }
    }
}