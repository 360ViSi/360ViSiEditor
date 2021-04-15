using System.Collections.Generic;
using UnityEngine;
public class UndoRedoHandler : MonoBehaviour
{
    public static UndoRedoHandler instance;
    [SerializeField] StructureManager structureManager;
    [SerializeField] int undoCount;

    DropOutList<VideoJSONWrapper> saveStates = new DropOutList<VideoJSONWrapper>(10);
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (this != instance)
            Destroy(gameObject);

        saveStates = new DropOutList<VideoJSONWrapper>(undoCount);
    }
    private void Update()
    {

#if !UNITY_EDITOR
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            return;
#endif

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
        }
    }
    public void SaveState()
    {
        saveStates.Push(structureManager.CreateSaveState());
    }

    void Undo()
    {
        if (saveStates.Count() > 0)
            structureManager.LoadSaveState(saveStates.Pop());
    }
    void Redo()
    {

    }

}

public class DropOutList<T>
{
    private List<T> items;
    private int capacity = 0;
    bool removeDouble; //if pushing then undo wants to jump over last one, because that is the state where we are currently
    public DropOutList(int capacity)
    {
        items = new List<T>();
        this.capacity = capacity;
    }

    public void Push(T item)
    {
        items.Add(item);
        removeDouble = true;
        if (items.Count <= capacity) return;

        for (int i = 0; i < items.Count - 1; i++)
            items[i] = items[i + 1];

        items.RemoveAt(items.Count - 1);
    }

    public int Count() => items.Count;

    public T Pop()
    {
        if (items.Count == 0) return default;

        var result = items[items.Count - 1];
 
        if (removeDouble && items.Count > 1)
        {
            result = items[items.Count - 2];
            items.RemoveAt(items.Count - 2);
        }
 
        items.RemoveAt(items.Count - 1);
        removeDouble = false;
        Push(result);
        return result;
    }
}