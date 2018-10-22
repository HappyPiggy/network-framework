using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class UIWidget : UIPanel
{
    public override UITypeDef UIType { get { return UITypeDef.Widget; } }


}