﻿using System.Collections.Generic;

public abstract class BehaviorComposite : BehaviorComponent {

    public BehaviorComponent[] childBehaviors;
	
    public BehaviorComposite(string name, BehaviorComponent[] childBehaviors) : base(name)
    {
        this.childBehaviors = childBehaviors;
    }

}
