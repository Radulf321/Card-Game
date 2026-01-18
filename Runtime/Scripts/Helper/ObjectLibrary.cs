using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public abstract class ObjectLibrary<T> where T : Clonable<T>
{
    private readonly Dictionary<string, T> objects;

    public ObjectLibrary()
    {
        this.objects = new Dictionary<string, T>();
    }

    public ObjectLibrary(string resourceFolder)
    {
        Dictionary<string, T> objects = new Dictionary<string, T>();
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>(resourceFolder);
        foreach (TextAsset jsonFile in jsonFiles)
        {
            JObject jsonObject = JObject.Parse(jsonFile.text);
            string objectName = jsonObject["id"].ToString();
            T obj = CreateObjectFromJson(jsonObject);
            objects[objectName] = obj;
        }
        this.objects = objects;
    }

    public T GetObject(string objectID)
    {
        if (objects.ContainsKey(objectID))
        {
            return objects[objectID].Clone();
        }
        else
        {
            throw new System.Exception("Object not found in library: " + objectID);
        }
    }

    public List<T> GetAllObjects()
    {
        List<T> clonedObjects = new List<T>();
        foreach (T obj in objects.Values)
        {
            clonedObjects.Add(obj.Clone());
        }
        return clonedObjects;
    }

    protected abstract T CreateObjectFromJson(JObject jsonObject);
}