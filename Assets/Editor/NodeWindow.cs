using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class NodeWindow
{

    public List<Handle> InHandles = new List<Handle>();
    public List<Handle> OutHandles = new List<Handle>();

    private Rect _windowRect;

    private PcgWindow window;

    public AstroidGenerator.AstroidSettings.Action action { get; private set; }

    public NodeWindow(PcgWindow window)
    {
        this.window = window;
    }

    public Rect WindowRect
    {
        get { return new Rect(pos.x,pos.y,_windowRect.width,_windowRect.height); }
        set
        {
            _windowRect = value;
            pos = new Vector2(value.x, value.y);
           
        }
    }

    

    public Vector2 pos
    {
        get { return action.windowEditor.windowPos; }
        set { action.windowEditor.windowPos = value; }
        
    }

    public NodeWindow(Rect r, AstroidGenerator.AstroidSettings.Action action)
    {

        AstroidGenerator.AstroidSettings.Action.Method method = action.method;
        this.action = action;
        WindowRect = r;
        RegenerateWindow(method);
    }

    public void Render(PcgWindow window)
    {
        for (int i = 0; i < InHandles.Count; i++)
        {
            InHandles[i].Render(new Vector2(-15,(i+1)/(InHandles.Count+1)), window);
        }

        for (int i = 0; i < OutHandles.Count; i++)
        {
            OutHandles[i].Render(new Vector2(_windowRect.width, (i + 1) / (OutHandles.Count + 1)), window);
        }
    }

    public void Save()
    {
        action.windowEditor.inList.Clear();

        foreach (Handle h in InHandles)
        {
            if (h.Other != null)
                action.windowEditor.inList.Add(window.AstroidObject.actions.IndexOf(h.Other.MyWindow.action));
        }

        action.windowEditor.outList.Clear();

        foreach (Handle h in OutHandles)
        {
            if (h.Other != null)
                action.windowEditor.outList.Add(window.AstroidObject.actions.IndexOf(h.Other.MyWindow.action));
        }
        
    }

    private void RegenerateWindow(AstroidGenerator.AstroidSettings.Action.Method method)
    {
        InHandles.Clear();
        OutHandles.Clear();
        switch (method)
        {
            case AstroidGenerator.AstroidSettings.Action.Method.CellularAutomata:
                InHandles.Add(new Handle(this));
                InHandles.Add(new Handle(this));
                OutHandles.Add(new Handle(this));
                break;
            case AstroidGenerator.AstroidSettings.Action.Method.PerlinNoise:
                InHandles.Add(new Handle(this));
                OutHandles.Add(new Handle(this));
                break;
            case AstroidGenerator.AstroidSettings.Action.Method.Invert:
                InHandles.Add(new Handle(this));
                OutHandles.Add(new Handle(this));
                break;
            case AstroidGenerator.AstroidSettings.Action.Method.MapEdgeCleaning:
                InHandles.Add(new Handle(this));
                OutHandles.Add(new Handle(this));
                break;
            case AstroidGenerator.AstroidSettings.Action.Method.Noise:
                //InHandles.Add(new Handle(this));
               Debug.Log("TEST");
                OutHandles.Add(new Handle(this));
                break;
        }
    }

}
