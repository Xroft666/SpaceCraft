using UnityEngine;
using System.Collections;

public class Handle
{

    public NodeWindow MyWindow;

    public Handle Other;


    public Handle(NodeWindow w)
    {
        MyWindow = w;
    }

    public void Render(Vector2 pos, PcgWindow window)
    {
     /*
        //TODO dont relay on button press
        if (GUI.Button(new Rect(pos.x+MyWindow.pos.x, pos.y+MyWindow.pos.y+200, 15, 15), ""))
        {
            if (window.SelectedHandle == null)
            {
                window.SelectedHandle = this;
            }
            else
            {
                Other = window.SelectedHandle;
                window.SelectedHandle = null;
                Debug.Log("Connected Handles");
            }
            
            /*
            if (Input.GetMouseButtonDown(0))
            {
                window.SelectedHandle = this;
                Debug.Log("Connected Handles");
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Other = window.SelectedHandle;
                
            }*/
       // }
    }
	
}
