using UnityEngine;
using System.Collections;

public class BaseInputNode : BaseNode
{

    public virtual string getResult()
    {
        return "None";
    }

    public override void DrawCurves()
    {

    }
}
