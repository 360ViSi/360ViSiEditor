using System.Collections.Generic;
using UnityEngine;

/*

Is now functional, BUT has some major issues still.

- Since the nodes are recreated the selected isnt reselected when undoing -> undo doesn't 
        seemingly happen, but it does
- Not every action is set to create a new state
- Sometimes the states clump up (redoing multiple undos), not clear why

*/
public class UndoRedoHandler : MonoBehaviour
{
    public static UndoRedoHandler instance;
    [SerializeField] int undos;
    [SerializeField] int redos;
    [SerializeField] StructureManager structureManager;
    [SerializeField] int undoCount;
    [SerializeField] DropOutList<VideoJSONWrapper> undoStates;
    [SerializeField] DropOutList<VideoJSONWrapper> redoStates;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (this != instance)
            Destroy(gameObject);

        undoStates = new DropOutList<VideoJSONWrapper>(undoCount, true);
        redoStates = new DropOutList<VideoJSONWrapper>(undoCount, false);
    }
    private void Update()
    {
        undos = undoStates.Count;
        redos = redoStates.Count;

#if !UNITY_EDITOR
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            return;
#endif

        if (Input.GetKeyDown(KeyCode.Z))
            Undo();
        else if (Input.GetKeyDown(KeyCode.Y))
            Redo();
    }
    public void SaveState()
    {
        undoStates.Push(structureManager.CreateSaveState());
        redoStates.Clear();
    }

    void Undo()
    {
        if (undoStates.Count == 0) return;

        var loadState = undoStates.Pop();
        var newState = structureManager.CreateSaveState();
        if (redoStates.Last == null || newState != redoStates.Last)
            redoStates.Push(newState);

        structureManager.LoadSaveState(loadState);
        NodeInspector.instance.RefreshSelection();
    }
    void Redo()
    {
        if (redoStates.Count == 0) return;

        var state = redoStates.Pop();

        undoStates.Push(structureManager.CreateSaveState(), false);
        structureManager.LoadSaveState(state);
        NodeInspector.instance.RefreshSelection();
    }
 
}

[System.Serializable]
public class DropOutList<T>
{
    private List<T> items;
    private int capacity = 0;
    bool useDoubleProtection;
    bool removeDouble; //if pushing then undo wants to jump over last one, because that is the state where we are currently
    public DropOutList(int capacity, bool useDoubleProtection = true)
    {
        items = new List<T>();
        this.capacity = capacity;
        this.useDoubleProtection = useDoubleProtection;
    }

    public void Push(T item, bool removeDouble = true)
    {
        items.Add(item);
        this.removeDouble = removeDouble;
        if (items.Count <= capacity) return;

        for (int i = 0; i < items.Count - 1; i++)
            items[i] = items[i + 1];

        items.RemoveAt(items.Count - 1);
    }

    public T Pop()
    {
        if (items.Count == 0) return default;
        var result = items[items.Count - 1];

        //moves over the current save state when undoing, if it was just created
        if (useDoubleProtection && removeDouble && items.Count > 1)
        {
            result = items[items.Count - 2];
            items.RemoveAt(items.Count - 2);
        }

        items.RemoveAt(items.Count - 1);
        removeDouble = false;
        if (useDoubleProtection)
            Push(result);
        return result;
    }

    public T Last
    {
        get
        {
            if (items.Count > 0)
                return items[items.Count - 1];
            else return default;
        }
    }

    public int Count => items.Count;
    public void Clear() => items.Clear();
}